using System.ComponentModel;
using OpenAI.Images;

namespace PosterAgent;

/// <summary>
/// The poster agent's tool. It calls the image model and saves the PNG in the posters/ directory.
/// Locally, the agent serves the image at /posters/{fileName} so it can be opened in a browser.
/// </summary>
public sealed class PosterGenerator(ImageClient imageClient, string baseUrl)
{
    private const string OutputDirectory = "posters";

    [Description("Creates a poster from an English visual description. Returns the image URL.")]
    public async Task<string> GeneratePoster(
        [Description("An English visual description of the image: style, colors, and composition. No text.")] string description)
    {
        GeneratedImage image = await imageClient.GenerateImageAsync(description, new ImageGenerationOptions
        {
            Size = GeneratedImageSize.W1024xH1024,
        });

        Directory.CreateDirectory(OutputDirectory);
        var fileName = $"poster-{DateTime.Now:HHmmss}.png";
        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, fileName), image.ImageBytes.ToArray());

        return $"{baseUrl}/posters/{fileName}";
    }

    // GetFileName prevents requests such as ../../anything.
    public static IResult Serve(string fileName)
    {
        var path = Path.GetFullPath(Path.Combine(OutputDirectory, Path.GetFileName(fileName)));
        return File.Exists(path) ? Results.File(path, "image/png") : Results.NotFound();
    }
}
