---
marp: true
theme: default
paginate: true
size: 16:9
lang: es
title: "De código a Foundry: infraestructura de agentes con Aspire"
description: "Global AI Night 2026: .NET, Aspire y Foundry, con una demo en vivo de dos agentes."
header: "**Global AI Night**  \n2026"
footer: "Emiliano Montesdeoca · Global AI Night 2026"
style: |
  @font-face {
    font-family: "Space Grotesk";
    src: url("assets/fonts/SpaceGrotesk-VariableFont_wght.ttf") format("truetype");
    font-style: normal;
    font-weight: 300 700;
  }
  section {
    box-sizing: border-box;
    padding: 88px 76px 72px;
    background: #F8F8F8;
    color: #001344;
    font-family: "Space Grotesk", "Segoe UI", sans-serif;
    font-size: 30px;
    line-height: 1.35;
  }
  section h1 {
    color: #001344;
    font-size: 2em;
    line-height: 1.12;
    letter-spacing: -0.025em;
  }
  section h2 {
    margin-bottom: 0.65em;
    border-left: 5px solid #00BCF2;
    padding-left: 0.35em;
    color: #001344;
    font-size: 1.5em;
    line-height: 1.15;
  }
  section strong, section a { color: #0078D7; }
  section code, section pre { font-family: "Cascadia Code", Consolas, monospace; }
  section code {
    color: #001344;
    background: #DBEFF9;
    padding: 0.08em 0.22em;
    border-radius: 4px;
  }
  section pre {
    color: #001344;
    background: #EAF6FC;
    border-left: 5px solid #00BCF2;
    padding: 0.6em 0.8em;
    font-size: 0.85em;
  }
  section pre code { background: transparent; padding: 0; }
  section table { width: 100%; font-size: 0.82em; }
  section th { background: #0078D7; color: #fff; }
  section header {
    top: 24px;
    right: 76px;
    left: auto;
    width: 220px;
    color: #0078D7;
    font-size: 13px;
    text-align: right;
  }
  section footer, section::after {
    color: #53647A;
    font-size: 14px;
  }
  section footer { left: 76px; bottom: 18px; }
  section::after { right: 76px; bottom: 18px; }
  section.lead, section.closing {
    border-top: 8px solid #D83B01;
    background: linear-gradient(135deg, #001344 0%, #0078D7 100%);
    color: #fff;
  }
  section.lead h1, section.lead h2, section.closing h1, section.closing h2 {
    border: 0;
    padding-left: 0;
    color: #fff;
  }
  section.lead strong, section.lead a, section.closing strong, section.closing a { color: #fff; }
  section.lead p, section.closing p { color: #DBEFF9; }
  section.lead header, section.lead footer, section.lead::after,
  section.closing header, section.closing footer, section.closing::after { color: #DBEFF9; }
  section.problem .scatter {
    position: relative;
    height: 360px;
    margin: 0.2em 0 0.35em;
  }
  section.problem .scatter span {
    position: absolute;
    display: block;
    padding: 0.35em 0.65em;
    border: 2px solid #0078D7;
    border-radius: 8px;
    background: #DBEFF9;
    color: #001344;
    font-size: 0.78em;
    font-weight: 700;
    white-space: nowrap;
  }
  section.problem .scatter span:nth-child(4n) { border-color: #D83B01; background: #FFF1E8; }
  section.problem .scatter span:nth-child(4n+2) { border-color: #00BCF2; }
  section.problem .scatter span:nth-child(1) { top: 4%; left: 2%; transform: rotate(-8deg); }
  section.problem .scatter span:nth-child(2) { top: 29%; left: 38%; transform: rotate(5deg); }
  section.problem .scatter span:nth-child(3) { top: 6%; left: 74%; transform: rotate(-4deg); }
  section.problem .scatter span:nth-child(4) { top: 46%; left: 86%; transform: rotate(7deg); }
  section.problem .scatter span:nth-child(5) { top: 53%; left: 7%; transform: rotate(-5deg); }
  section.problem .scatter span:nth-child(6) { top: 31%; left: 64%; transform: rotate(6deg); }
  section.problem .scatter span:nth-child(7) { top: 67%; left: 33%; transform: rotate(-6deg); }
  section.problem .scatter span:nth-child(8) { top: 71%; left: 70%; transform: rotate(3deg); }
  section.problem .scatter span:nth-child(9) { top: 19%; left: 20%; transform: rotate(4deg); }
  section.problem .scatter span:nth-child(10) { top: 3%; left: 44%; transform: rotate(-7deg); }
  section.problem .scatter span:nth-child(11) { top: 56%; left: 57%; transform: rotate(2deg); }
  section.problem .scatter span:nth-child(12) { top: 81%; left: 3%; transform: rotate(-3deg); }
  section.problem .scatter span:nth-child(13) { top: 83%; left: 51%; transform: rotate(5deg); }
  section.demo {
    border-top: 8px solid #D83B01;
    background: #001344;
    color: #fff;
  }
  section.demo h2 { color: #fff; }
  section.demo strong, section.demo header { color: #00BCF2; }
  section.demo footer, section.demo::after { color: #DBEFF9; }
  section.demo pre { font-size: 1.1em; }
  section.bio .bio-layout {
    display: grid;
    grid-template-columns: 1.15fr 0.85fr;
    align-items: center;
    gap: 1.2em;
  }
  section.bio .bio-copy p { margin: 0.55em 0; }
  section.bio .bio-badges {
    display: flex;
    align-items: center;
    gap: 0.35em;
    margin-top: 0.8em;
  }
  section.bio .bio-badges img {
    display: block;
    width: auto;
    height: 70px;
    object-fit: contain;
  }
  section.bio .bio-badges img.mvp { height: 48px; }
  section.bio .bio-photo {
    display: block;
    width: 100%;
    height: 390px;
    border-radius: 14px;
    object-fit: cover;
    object-position: center 28%;
  }
  section.visual .visual-layout {
    display: grid;
    grid-template-columns: minmax(0, 1.1fr) minmax(0, 0.9fr);
    align-items: center;
    gap: 1em;
  }
  section.visual h2 { margin-bottom: 0.35em; }
  section.visual .visual-intro {
    margin: -0.1em 0 0.55em;
    color: #53647A;
    font-size: 0.9em;
    font-weight: 600;
  }
  section.visual .visual-copy { font-size: 0.84em; line-height: 1.3; }
  section.visual .visual-copy p { margin: 0.45em 0; }
  section.visual .diagram {
    display: flex;
    flex-direction: column;
    gap: 0.22em;
    padding: 0.7em;
    border: 1px solid #B7DCEB;
    border-radius: 14px;
    background: #EAF6FC;
  }
  section.visual .diagram-node {
    padding: 0.35em 0.4em;
    border: 2px solid #0078D7;
    border-radius: 9px;
    background: #fff;
    color: #001344;
    font-size: 0.76em;
    font-weight: 700;
    line-height: 1.15;
    text-align: center;
  }
  section.visual .diagram-node.primary {
    border-color: #001344;
    background: #001344;
    color: #fff;
  }
  section.visual .diagram-node.cyan { border-color: #00BCF2; }
  section.visual .diagram-node.orange { border-color: #D83B01; }
  section.visual .diagram-node small {
    display: block;
    margin-top: 0.12em;
    font-size: 0.72em;
    font-weight: 400;
  }
  section.visual .diagram-pair {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 0.32em;
  }
  section.visual .diagram-arrow {
    color: #0078D7;
    font-size: 0.95em;
    line-height: 1;
    text-align: center;
  }
  section.visual .diagram-note {
    color: #53647A;
    font-size: 0.62em;
    text-align: center;
  }
  .small { color: #53647A; font-size: 0.75em; }
  .metric { color: #0078D7; font-size: 1.35em; font-weight: 700; }
  .intro { margin: -0.1em 0 0.75em; color: #53647A; font-size: 1.05em; font-weight: 600; }
---

<!-- _class: lead -->
<!-- _paginate: false -->

# De código a Foundry

## Infraestructura de agentes con Aspire

**Emiliano Montesdeoca del Puerto**<br>
Global AI Night 2026 · 12 de noviembre · online

<!--
TIEMPO: 0:00 – 0:30

Abrir con una promesa: en unos minutos explico el modelo; después veremos el recorrido funcionando.
-->

---

<!-- _class: bio -->

## Soy Emi

<div class="bio-layout">
  <div class="bio-copy">
    <p><strong>Cloud Solutions Team Lead</strong><br>Intelequia</p>
    <p><strong>Microsoft MVP</strong><br>Developer Technologies · desde 2022</p>
    <p>Uruguayo, muy canario.</p>
    <div class="bio-badges">
      <img src="assets/most-active-speaker-2025.svg" alt="Most Active Speaker 2025">
      <img src="assets/most-active-speaker-2024.svg" alt="Most Active Speaker 2024">
      <img src="assets/most-active-speaker-2023.svg" alt="Most Active Speaker 2023">
      <img class="mvp" src="assets/microsoft-mvp.png" alt="Microsoft MVP">
    </div>
  </div>
  <img class="bio-photo" src="assets/emiliano-avatar.jpg" alt="Foto de Emiliano Montesdeoca">
</div>

<!--
TIEMPO: 0:30 – 1:30

Presentación breve: qué haces, el trabajo con cloud e IA y los años como MVP.
-->

---

<!-- _class: problem -->

<div class="scatter">
  <span>App en local</span>
  <span>Modelos en el portal</span>
  <span>Endpoint</span>
  <span>API keys</span>
  <span>Permisos</span>
  <span>Docker</span>
  <span>Deploy</span>
  <span>Logs y trazas</span>
  <span>Suscripción</span>
  <span>Región y cuota</span>
  <span>Variables</span>
  <span>Registry</span>
  <span>Scripts</span>
</div>

**Cada pieza en un sitio. El pegamento vive en scripts, un README o en alguien.**

<!--
TIEMPO: 1:30 – 2:30

Pregunta quién ha tenido que perseguir endpoints, claves, permisos y despliegues para pasar
un agente de local a otro entorno. No explicar cada pieza: la imagen es el problema.
-->

---

## Lo que proponemos

<p class="intro">Un AppHost conecta tus agentes .NET con los modelos de Foundry.</p>

| .NET + Agent Framework | Aspire | Microsoft Foundry |
|---|---|---|
| Escribes los agentes | Describes recursos y conexiones | Despliegas modelos y alojas agentes |

<p class="metric">Un AppHost. El mismo código en local y en Foundry.</p>

<!--
TIEMPO: 2:30 – 3:30

Presentar el reparto en una frase: Agent Framework escribe los agentes, Aspire describe la
aplicación y Foundry ofrece modelos, identidad y hosting.
-->

---

<!-- _class: visual -->

## Aspire

<p class="visual-intro">El toolkit de .NET para construir y orquestar aplicaciones distribuidas.</p>

<div class="visual-layout">
  <div class="visual-copy">
    <p><strong>AppHost.cs</strong> declara recursos, dependencias y referencias.</p>
    <p><code>WithReference</code> conecta servicios; <code>WaitFor</code> ordena el arranque.</p>
    <p><strong>Dashboard:</strong> estado, variables, logs y trazas.</p>
    <p class="small"><code>run</code> local · <code>publish</code> Bicep · <code>deploy</code> agentes</p>
  </div>
  <div class="diagram">
    <div class="diagram-node primary">AppHost.cs<br><small>modelo de la aplicación</small></div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-pair">
      <div class="diagram-node">Recursos<br><small>Foundry · project · models</small></div>
      <div class="diagram-node cyan">Agentes<br><small>schedule · poster</small></div>
    </div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-node">Dashboard<br><small>health · logs · traces</small></div>
  </div>
</div>

<!--
TIEMPO: 3:30 – 5:00

Aspire no es otro portal ni el runtime del agente. Es el modelo de la aplicación:
qué recursos hay y qué necesita cada servicio. Enseñar cómo las referencias inyectan
endpoints/configuración y cómo el dashboard ayuda a depurar en local.
-->

---

<!-- _class: visual -->

## Microsoft Foundry

<p class="visual-intro">La plataforma de Azure para construir y operar soluciones de IA.</p>

<div class="visual-layout">
  <div class="visual-copy">
    <p><strong>Project</strong> agrupa los despliegues de modelos y los hosted agents.</p>
    <p><strong>Identity + roles:</strong> acceso sin claves compartidas.</p>
    <p><strong>Tracing:</strong> llamadas al modelo y a las herramientas.</p>
  </div>
  <div class="diagram">
    <div class="diagram-node primary">Foundry project</div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-pair">
      <div class="diagram-node">chat<br><small>texto</small></div>
      <div class="diagram-node orange">image<br><small>generación de imagen</small></div>
    </div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-node cyan">Hosted agents</div>
    <div class="diagram-note">identity · roles · tracing</div>
  </div>
</div>

<!--
TIEMPO: 5:00 – 6:30

En esta demo el proyecto contiene los dos despliegues. La identidad evita repartir API keys
y tracing permite seguir llamadas al modelo y a herramientas.
-->

---

<!-- _class: visual -->

## Cómo lo unimos

<div class="visual-layout">
  <div class="visual-copy">
    <p><strong>Un AppHost</strong> declara el proyecto, los modelos y las referencias.</p>
    <p><code>schedule</code> recibe <code>project + chat</code>.</p>
    <p><code>poster</code> recibe <code>project + chat + image</code>.</p>
    <p class="small"><code>aspire run</code> → local · <code>aspire deploy</code> → Foundry</p>
  </div>
  <div class="diagram">
    <div class="diagram-node primary">AppHost · Aspire</div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-node">Foundry project</div>
    <div class="diagram-pair">
      <div class="diagram-node">chat</div>
      <div class="diagram-node orange">image</div>
    </div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-pair">
      <div class="diagram-node cyan">schedule<br><small>project + chat</small></div>
      <div class="diagram-node orange">poster<br><small>project + chat + image</small></div>
    </div>
  </div>
</div>

<!--
TIEMPO: 6:30 – 8:00

Enseñar el AppHost una sola vez. La agenda no recibe el modelo de imágenes.
El código de los agentes no cambia: cambia dónde se ejecutan.
-->

---

## La demo: dos agentes

| Agente | Qué hace | Modelos y herramienta |
|---|---|---|
| `schedule` | Recomienda sesiones | `gpt-5-mini` · `SearchSessions` |
| `poster` | Crea un cartel | `gpt-5-mini` + `gpt-image-1-mini` · `GeneratePoster` |

<p class="small">La agenda es ficticia. El modelo de imágenes solo dibuja; el agente decide qué pedirle.</p>

<!--
TIEMPO: 8:00 – 9:00

Contexto mínimo para entender la demo: un agente de texto y otro que llama a una herramienta
de imágenes. No recorrer el código línea a línea.
-->

---

<!-- _class: demo -->

## Demo 1 · Local

```bash
cd src
aspire run
```

Dashboard → `schedule` y `poster` → mensajes y trazas

<!--
TIEMPO: 9:00 – 19:00

Antes de la charla, deja Foundry y los modelos provisionados, inicia sesión con az login y
ten el dashboard listo. En directo:
1. Enseñar los recursos.
2. Enviar una pregunta a schedule.
3. Pedir un cartel a poster y abrir el PNG.
4. Mostrar la traza invoke_agent → chat → execute_tool GeneratePoster.

Si la generación tarda, abrir Traces mientras espera. Si falla, reproducir el vídeo de respaldo.
-->

---

<!-- _class: demo -->

## Demo 2 · Infraestructura

```bash
aspire publish -o infra
```

**Bicep generado:** Foundry · proyecto · modelos · roles

<!--
TIEMPO: 19:00 – 22:00

Abrir la carpeta infra y mostrar el Bicep que genera Aspire. Elegir un fichero durante el
ensayo; no explicar cada propiedad. Si vamos tarde, contar el resultado sin recorrerlo.
-->

---

<!-- _class: demo -->

## Demo 3 · Hosted agents en Foundry

```bash
aspire deploy
```

**Build → registry → agentes alojados → Playground y tracing**

<!--
TIEMPO: 22:00 – 32:00

Lanzar el deploy. Mientras se construyen y suben las imágenes, explicar qué ocurre en cada paso.
En Foundry, abrir schedule en Playground y después las trazas.

Si tarda más de lo previsto, usar la versión ya desplegada o el vídeo de respaldo.
-->

---

## Qué cambia

- **Menos pasos manuales:** la infraestructura se revisa junto al código.
- **Sin claves compartidas:** acceso mediante identidad y roles.
- **El mismo agente:** local con Aspire, alojado en Foundry.

<!--
TIEMPO: 32:00 – 35:00

Cerrar el arco con la slide 3: ya no hay que pegar a mano las piezas para cada entorno.
Terminar el contenido en el minuto 35.
-->

---

<!-- _class: closing -->
<!-- _paginate: false -->

# Gracias

## ¿Preguntas?

**Emiliano Montesdeoca**<br>
linkedin.com/in/emimontesdeoca · @emimontesdeocaa

**Repo:** [github.com/emimontesdeoca/foundry-aspire-global-ai-night-2026](https://github.com/emimontesdeoca/foundry-aspire-global-ai-night-2026)

<!--
TIEMPO: 35:00 – 40:00

Dejar esta slide durante las preguntas.
-->
