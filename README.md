# TAYMOTIVE – Sistema de Gestión Fitness

## Descripción del proyecto

TAYMOTIVE es una aplicación de escritorio desarrollada en C# con Windows Forms y .NET 10, orientada a la gestión de usuarios, entrenadores, rutinas de ejercicio y seguimiento de actividades físicas.

El proyecto fue desarrollado para el curso de Técnica de Programación de la Universidad Politécnica Internacional.

**Desarrolladora:** Tariz Taylor Calero  
**Profesor:** Luis Felipe Mora Umaña

## Arquitectura del sistema

El proyecto utiliza la arquitectura MVC (Modelo–Vista–Controlador):

- **Modelo:** representa usuarios, entrenadores, rutinas, mediciones, visitas, objetivos e insignias.
- **Vista:** interfaces gráficas desarrolladas con Windows Forms.
- **Controlador:** gestiona las operaciones y validaciones del sistema.

La información se almacena localmente mediante archivos JSON.

## Funcionalidades principales

- Registro y administración de usuarios.
- Gestión de entrenadores.
- Asignación y consulta de rutinas.
- Registro de visitas y actividades.
- Seguimiento de mediciones físicas.
- Registro de objetivos personales.
- Seguimiento del progreso de rutinas.
- Gestión de insignias.
- Consultas y reportes.
- Autenticación de usuarios.

## Requisitos

- Windows 10 u 11.
- .NET 10 SDK.
- Visual Studio Code o Visual Studio.

## Instalación y ejecución

1. Clonar el repositorio:

   `git clone https://github.com/taytariz/TAYMOTIVE.git`

2. Abrir una terminal en la carpeta TAYMOTIVE.

3. Crear la carpeta `Files` si no existe.

4. Copiar los siete archivos `.json` de `SampleData` a `Files`.

5. Ejecutar:

   `dotnet restore Taymotive.sln`

   `dotnet run --project src/Taymotive/Taymotive.csproj`

**Importante:** ejecutar desde la carpeta raíz del repositorio para que la aplicación encuentre los archivos JSON.

## Credenciales de demostración

| Rol | Usuario | Contraseña |
|---|---|---|
| Administrador | admin_demo | TaymotiveDemo2026! |
| Usuario regular | usuario01 | TaymotiveDemo2026! |

Estas credenciales corresponden exclusivamente a datos ficticios de demostración.

## Datos de prueba

La carpeta `SampleData` contiene siete archivos JSON:

- `users.json`
- `trainers.json`
- `visits.json`
- `measurements.json`
- `badges.json`
- `fitnessgoals.json`
- `routine_progress.json`

Los datos incluyen 41 usuarios, 40 entrenadores y 200 rutinas asignadas.

## Pruebas y calidad

El proyecto utiliza xUnit para pruebas automatizadas y SonarAnalyzer para el análisis estático del código.

Para ejecutar las pruebas:

`dotnet test tests/TaymotiveTests/TaymotiveTests.csproj`

En la última verificación se aprobaron 11 pruebas automatizadas y la solución compiló correctamente.

## Repositorio

https://github.com/taytariz/TAYMOTIVE

## Conclusión
TAYMOTIVE permitió desarrollar una aplicación de escritorio para la gestión de actividades fitness utilizando C#, Windows Forms y .NET 10.

Durante el desarrollo se aplicó la arquitectura MVC para separar las responsabilidades del sistema, junto con principios de programación orientada a objetos, SOLID y Clean Code.

Se implementaron funcionalidades para administrar usuarios, entrenadores, rutinas, mediciones, visitas, objetivos, insignias y reportes. Asimismo, se incorporaron mecanismos de persistencia mediante archivos JSON, pruebas automatizadas con xUnit y análisis estático mediante SonarAnalyzer.

El proyecto permitió fortalecer los conocimientos de desarrollo de software, organización del código, validación de datos, control de versiones con Git y seguimiento de actividades mediante Jira.