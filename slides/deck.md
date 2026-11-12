---
marp: true
theme: default
paginate: true
size: 16:9
lang: es
title: "De código a Foundry: infraestructura de agentes con Aspire"
description: "Global AI Night 2026: desarrollo local e infraestructura con Aspire, de dos agentes .NET a Foundry."
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
    height: 420px;
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
  section.problem .scatter span:nth-child(1) { top: 4%; left: 2%; transform: rotate(-6deg); }
  section.problem .scatter span:nth-child(2) { top: 1%; left: 28%; transform: rotate(5deg); }
  section.problem .scatter span:nth-child(3) { top: 7%; left: 59%; transform: rotate(-4deg); }
  section.problem .scatter span:nth-child(4) { top: 3%; left: 84%; transform: rotate(6deg); }
  section.problem .scatter span:nth-child(5) { top: 27%; left: 5%; transform: rotate(-5deg); }
  section.problem .scatter span:nth-child(6) { top: 24%; left: 27%; transform: rotate(6deg); }
  section.problem .scatter span:nth-child(7) { top: 26%; left: 49%; transform: rotate(-6deg); }
  section.problem .scatter span:nth-child(8) { top: 25%; left: 73%; transform: rotate(3deg); }
  section.problem .scatter span:nth-child(9) { top: 48%; left: 2%; transform: rotate(4deg); }
  section.problem .scatter span:nth-child(10) { top: 46%; left: 24%; transform: rotate(-5deg); }
  section.problem .scatter span:nth-child(11) { top: 49%; left: 53%; transform: rotate(2deg); }
  section.problem .scatter span:nth-child(12) { top: 47%; left: 78%; transform: rotate(-3deg); }
  section.problem .scatter span:nth-child(13) { top: 72%; left: 5%; transform: rotate(5deg); }
  section.problem .scatter span:nth-child(14) { top: 70%; left: 22%; transform: rotate(-4deg); }
  section.problem .scatter span:nth-child(15) { top: 73%; left: 41%; transform: rotate(4deg); }
  section.problem .scatter span:nth-child(16) { top: 69%; left: 70%; transform: rotate(-5deg); }
  section.problem .scatter span:nth-child(17) { top: 91%; left: 17%; transform: rotate(-3deg); }
  section.problem .scatter span:nth-child(18) { top: 91%; left: 63%; transform: rotate(3deg); }
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
Global AI Night 2026 · 12 de noviembre · en línea

<!--
TIEMPO: 0:00 – 0:30

Abrir con una promesa: desarrollar y depurar en nuestro equipo usando recursos reales de Azure,
y después desplegar los mismos agentes en Foundry. Aspire conecta código e infraestructura.
-->

---

<!-- _class: bio -->

## Soy Emi

<div class="bio-layout">
  <div class="bio-copy">
    <p><strong>Cloud Solutions Team Lead</strong><br>Intelequia Technologies</p>
    <p>x5 <strong>Microsoft MVP</strong><br>Developer Technologies </p>
    <p><strong>Marido</strong> y <strong>papá</strong> de las más hermosas.</p>
    <p><strong>Uruguayo</strong>, muy canario.</p>
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

<!-- _class: lead -->
<!-- _paginate: false -->

# ¿Qué componentes forman una aplicación?

<!--
TIEMPO: 1:30 – 1:40

Plantear la pregunta y dejar unos segundos de silencio. La siguiente slide ofrece una respuesta visual.
-->

---

<!-- _class: problem -->

<div class="scatter">
  <span>App en local</span>
  <span>Modelos en el portal</span>
  <span>URLs y endpoints</span>
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
  <span>Puertos</span>
  <span>Credenciales</span>
  <span>Entornos</span>
  <span>Conexiones</span>
  <span>Health checks</span>
</div>

<!--
TIEMPO: 1:40 – 2:30

Presentar la nube como respuesta. Dejar que reconozcan su día a día y preguntar: «¿Os suenan todas estas cajas?».
Contar el recorrido: elegir suscripción y región, encontrar una URL, configurar credenciales,
arrancar la app, averiguar por qué no conecta y repetir parte del trabajo al desplegar.
No leer todas las cajas ni resolverlas todavía.
-->

---

<!-- _class: visual -->

## Crear servicios era fácil. Conectarlos, no.

<p class="visual-intro">Puertos, URLs, variables y orden de arranque: todo se coordinaba a mano.</p>

<div class="visual-layout">
  <div class="visual-copy">
    <p><strong>Crear:</strong> levantar cada proceso por separado.</p>
    <p><strong>Conectar:</strong> copiar puertos y endpoints a la configuración.</p>
    <p><strong>Arrancar:</strong> coordinar el orden y las dependencias.</p>
    <p><strong>Depurar:</strong> buscar errores entre procesos y logs.</p>
    <p><strong>Repetir:</strong> volver a alinear todo al cambiar de entorno.</p>
  </div>
  <div class="diagram">
    <div class="diagram-node primary">Aplicación</div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-pair">
      <div class="diagram-node">Puertos<br><small>¿dónde escucha?</small></div>
      <div class="diagram-node cyan">URLs<br><small>¿a qué endpoint?</small></div>
    </div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-node orange">Configuración dispersa</div>
    <div class="diagram-note">.env · scripts · Docker · portal · README</div>
  </div>
</div>

<p class="metric">Si una pieza cambiaba, tocaba perseguir el mapa de nuevo.</p>

<!--
TIEMPO: 2:30 – 3:00

Contar la rutina: arrancar servicios por separado, asignar puertos, copiar URLs y endpoints
a la configuración, y repetir el montaje al cambiar de entorno. El mapa vivía repartido entre
.env, scripts, Docker, el portal y la memoria del equipo.
Cerrar: «El reto no era crear cada servicio, sino coordinar el conjunto». Pasar a Aspire como
una forma de describir ese sistema desde un solo lugar.
-->

---

<!-- _class: visual -->

## Una aplicación completa. Un mismo flujo de desarrollo. El mismo punto de partida.

<p class="visual-intro">Aspire coordina servicios locales y recursos de Azure desde el AppHost.</p>

<div class="visual-layout">
  <div class="visual-copy">
    <p><strong>Desarrolla:</strong> el playground y los agentes corren como procesos locales.</p>
    <p><strong>Orquesta:</strong> el AppHost declara cómo se conectan y de qué dependen.</p>
    <p><strong>Observa:</strong> el dashboard reúne estado, URLs, logs y trazas.</p>
  </div>
  <div class="diagram">
    <div class="diagram-node primary">AppHost · Aspire<br><small>define y coordina el sistema</small></div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-pair">
      <div class="diagram-node">En local<br><small>playground · agentes</small></div>
      <div class="diagram-node cyan">En Azure<br><small>Foundry · proyecto · modelos</small></div>
    </div>
    <div class="diagram-note">Aspire conecta los procesos locales con sus dependencias en Azure.</div>
  </div>
</div>

<p class="metric">Desarrollar en local. Aspire orquesta el sistema completo.</p>

<!--
TIEMPO: 3:00 – 4:00

La demo se centra en el ciclo de desarrollo y la orquestación con Aspire, no en el framework
interno de los agentes. El AppHost describe los procesos locales, sus dependencias y los recursos
de Foundry en Azure. Con `aspire run`, el playground y los agentes se ejecutan en local; el
dashboard centraliza URLs, estado, logs y trazas. Después revisaremos el Bicep generado y
desplegaremos la infraestructura: en este ejemplo, Aspire no aloja los agentes en Azure.
-->

---

<!-- _class: visual -->

## Aspire

<p class="visual-intro">Describe la aplicación y sus dependencias; trabaja con ellas desde un mismo sitio.</p>

<div class="visual-layout">
  <div class="visual-copy">
    <p><strong>AppHost.cs</strong> declara recursos, dependencias y referencias.</p>
    <p><code>WithReference</code> suministra endpoints y configuración.</p>
    <p><code>WaitFor</code> espera a las dependencias antes de arrancar.</p>
    <p><strong>Dashboard:</strong> URLs, estado, logs y trazas para depurar.</p>
  </div>
  <div class="diagram">
    <div class="diagram-node primary">AppHost.cs<br><small>modelo de la aplicación</small></div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-pair">
      <div class="diagram-node">Azure<br><small>Foundry · proyecto · modelos</small></div>
      <div class="diagram-node cyan">Tu equipo<br><small>agentes · playground</small></div>
    </div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-node">Dashboard<br><small>estado · logs · trazas</small></div>
  </div>
</div>

<!--
TIEMPO: 4:00 – 5:30

Aspire no es el runtime del agente: describe qué recursos hay y qué necesita cada servicio.
En desarrollo, conviven procesos locales y recursos reales de Azure.
Las referencias suministran endpoints y nombres de despliegue como configuración;
no hay que copiarlos a mano entre el portal y cada servicio. El dashboard permite abrir URLs
y seguir una petición sin ir saltando entre consolas.
-->

---

<!-- _class: visual -->

## Microsoft Foundry

<p class="visual-intro">La plataforma de Azure donde desplegamos y consumimos los modelos.</p>

<div class="visual-layout">
  <div class="visual-copy">
    <p><strong>Proyecto:</strong> agrupa los modelos y expone una API.</p>
    <p><strong>Despliegues independientes:</strong> uno para texto y otro para imágenes, cada uno con su modelo y versión.</p>
    <p><strong>Los agentes</strong> llaman a esa API desde tu equipo.</p>
    <p><strong>Preparación:</strong> región, cuota y permisos de la suscripción.</p>
    <p><strong>Identidad y roles:</strong> acceso sin claves compartidas.</p>
  </div>
  <div class="diagram">
    <div class="diagram-node primary">project-globalainight</div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-pair">
      <div class="diagram-node">chat-globalainight<br><small>texto</small></div>
      <div class="diagram-node orange">image-globalainight<br><small>generación de imagen</small></div>
    </div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-node cyan">Responses API<br><small>la llaman los agentes</small></div>
    <div class="diagram-note">Entra ID · sin API keys</div>
  </div>
</div>

<!--
TIEMPO: 5:30 – 7:00

Foundry es la parte de IA de esta infraestructura, no una explicación aparte del flujo.
Aquí lo usamos solo para los modelos: los agentes son procesos normales que llaman a la
Responses API del proyecto. La autenticación va con Entra ID (az login en local), sin repartir API keys.
-->

---

<!-- _class: visual -->

## Cada pieza en su sitio

<div class="visual-layout">
  <div class="visual-copy">
    <p><strong>AppHost:</strong> proyecto, modelos y conexiones declarados en código.</p>
    <p><strong>Agenda:</strong> recibe el proyecto y el modelo de chat.</p>
    <p><strong>Carteles:</strong> añade el modelo de imágenes.</p>
    <p><strong>Los agentes leen la configuración;</strong> no necesitan conocer Aspire.</p>
  </div>
  <div class="diagram">
    <div class="diagram-node primary">AppHost · Aspire</div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-node">Proyecto de Foundry · Azure</div>
    <div class="diagram-pair">
      <div class="diagram-node">Chat</div>
      <div class="diagram-node orange">Imágenes</div>
    </div>
    <div class="diagram-arrow">↓</div>
    <div class="diagram-pair">
      <div class="diagram-node cyan">Agenda<br><small>proyecto + chat</small></div>
      <div class="diagram-node orange">Carteles<br><small>proyecto + chat + imágenes</small></div>
    </div>
    <div class="diagram-note">Agentes en tu equipo · modelos en Foundry</div>
  </div>
</div>

<!--
TIEMPO: 7:00 – 8:30

Enseñar el AppHost una sola vez: recursos de Azure, referencias y WaitFor.
La agenda no recibe el modelo de imágenes. Los agentes solo leen variables de entorno:
el endpoint del proyecto y los nombres de despliegue de los modelos.
-->

---


<!-- _class: demo -->

## Demo 1 · Desarrollar en local

```bash
cd src
aspire run
```

**Agentes en tu equipo · modelos en Azure**

Dashboard → `playground-globalainight` → pruebas y trazas

<!--
TIEMPO: 9:00 – 20:00

Antes de la charla, deja Foundry y los modelos provisionados, inicia sesión con az login y
ten el dashboard listo. En directo:
1. Distinguir procesos locales y recursos de Azure en Resources.
2. Abrir la URL del playground desde el dashboard y ver la configuración que recibe un agente.
3. Probar una recomendación y pedir un cartel; abrir la imagen generada.
4. Seguir la petición en las trazas, desde el agente hasta el modelo y la herramienta.

El foco es el ciclo de desarrollo y depuración, no la calidad del cartel ni el prompt.

Si la generación tarda, abrir Traces mientras espera. Si falla, reproducir el vídeo de respaldo.
-->

---

<!-- _class: demo -->

## Demo 2 · Revisar la infraestructura

```bash
aspire publish -o infra
```

**Bicep generado:** Foundry · proyecto · modelos

<!--
TIEMPO: 20:00 – 26:00

Volver a las declaraciones del AppHost y abrir el Bicep generado: cuenta de Foundry
(disableLocalAuth), proyecto y despliegues de modelos. Los agentes y el playground solo existen
en aspire run, así que no aparecen aquí. Conectar lo que usamos en local con la infraestructura que se revisa
junto al código. Elegir un fichero durante el ensayo; no explicar cada propiedad.
Si vamos tarde, contar el resultado sin recorrerlo.
-->

---

<!-- _class: demo -->

## Demo 3 · Desplegar

```bash
aspire deploy
```

**El mismo AppHost → Foundry · proyecto · modelos en Azure**

<!--
TIEMPO: 26:00 – 32:00

Breve. Lanzar el deploy (por ejemplo, contra otro grupo de recursos) y enseñar el progreso:
crea la misma cuenta de Foundry, el proyecto y los modelos que usamos en local.
Los agentes no se despliegan: siguen siendo procesos que llaman a la API del proyecto.
Cómo alojarlos (App Service, Container Apps…) es otra decisión, fuera de esta charla.

Si tarda, no esperar: enseñar el grupo de recursos ya desplegado en el portal.
-->

---

## Del desarrollo al despliegue

- **En local:** URLs, configuración y trazas desde Aspire.
- **En el repositorio:** Foundry, proyecto y modelos junto al código.
- **En Azure:** la misma infraestructura con `aspire deploy`, sin clics en el portal.

<!--
TIEMPO: 32:00 – 35:00

Volver a la pregunta de las cajas: no desaparecen la suscripción, los permisos ni los modelos;
lo que cambia es cómo los conectamos y cómo trabajamos con ellos.
Aspire une el ciclo de desarrollo con la infraestructura y Foundry aporta los modelos.
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
