using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Azure.Core;

namespace PosterAgent;

/// <summary>
/// The poster agent's tool. It generates an image through Foundry's image API and saves it locally.
/// </summary>
public sealed class PosterGenerator(
    HttpClient httpClient,
    TokenCredential credential,
    Uri projectEndpoint,
    string imageDeploymentName,
    string baseUrl)
{
    private const string OutputDirectory = "posters";
    private static readonly Uri ImageGenerationScope = new("https://ai.azure.com/.default");

    [Description("Creates a poster from an English visual description. Returns the image URL.")]
    public async Task<string> GeneratePoster(
        [Description("An English visual description of the image: style, colors, and composition. No text.")] string description)
    {
        var token = await credential.GetTokenAsync(
            new TokenRequestContext([ImageGenerationScope.AbsoluteUri]),
            CancellationToken.None);
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(projectEndpoint, "/openai/v1/images/generations"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        request.Content = JsonContent.Create(new
        {
            model = imageDeploymentName,
            prompt = description,
            n = 1,
            size = "1024x1024",
            quality = "low",
            output_format = "png",
        });

        using var response = await httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Console.Error.WriteLine(
                $"Foundry image generation failed with HTTP {(int)response.StatusCode}: {responseBody}");
            throw new HttpRequestException(
                $"Foundry image generation failed with HTTP {(int)response.StatusCode} ({response.ReasonPhrase}): {responseBody}",
                inner: null,
                response.StatusCode);
        }

        using var document = JsonDocument.Parse(responseBody);
        if (!document.RootElement.TryGetProperty("data", out var data) ||
            data.ValueKind != JsonValueKind.Array ||
            data.GetArrayLength() == 0 ||
            !data[0].TryGetProperty("b64_json", out var encodedImage) ||
            encodedImage.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException("Foundry image generation returned no encoded image.");
        }

        Console.Error.WriteLine($"Foundry image API returned {encodedImage.GetString()!.Length} base64 characters.");
        Directory.CreateDirectory(OutputDirectory);
        var fileName = $"poster-{DateTime.Now:HHmmss}.png";
        await File.WriteAllBytesAsync(
            Path.Combine(OutputDirectory, fileName),
            Convert.FromBase64String(encodedImage.GetString()!));

        return $"{baseUrl}/posters/{fileName}";
    }

    // GetFileName prevents requests such as ../../anything.
    public static IResult Serve(string fileName)
    {
        var path = Path.GetFullPath(Path.Combine(OutputDirectory, Path.GetFileName(fileName)));
        return File.Exists(path) ? Results.File(path, "image/png") : Results.NotFound();
    }
}
