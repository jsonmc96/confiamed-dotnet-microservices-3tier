# Prueba Técnica: CONFIAMED - Asignación de Ítems de Trabajo

Esta es la solución a la prueba técnica de Backend (.NET) para CONFIAMED. Consiste en una arquitectura orientada a microservicios utilizando **.NET 10** y **C#**.

## Arquitectura

La solución está construida siguiendo un enfoque de Microservicios con una **Arquitectura en 3 Capas** tradicional para cada servicio (Presentación, Lógica de Negocio y Acceso a Datos), tal como se requirió en el documento de especificaciones.

El proyecto se divide en dos microservicios principales:

1. **ItemsDeTrabajo.API**: Encargado de la gestión de ítems y la ejecución del algoritmo principal de asignación.
2. **GestionDeUsuario.API**: (Simulado) Encargado de administrar los usuarios del sistema.

## Algoritmo de Asignación

El núcleo del sistema incluye un algoritmo que evalúa la carga de trabajo de los usuarios disponibles basándose en las siguientes reglas de negocio:
- **REGLA 1 - URGENCIA**: Prioriza la asignación basándose en ítems próximos a vencer (≤ 2 días).
- **REGLA 2 - RELEVANCIA**: Balancea la carga considerando la cantidad de ítems "relevantes" que tiene cada usuario.
- **REGLA 3 - DISTRIBUCIÓN EQUITATIVA**: Asigna el ítem al usuario con menor carga total si no aplican las reglas anteriores.

## Tecnologías Utilizadas
- **.NET 10** (C# 14)
- **Scalar UI** (Reemplazo moderno de Swagger para documentación de OpenAPI nativa)
- **Inyección de Dependencias** nativa de ASP.NET Core
- Patrón **Repository** para la abstracción del acceso a datos.

## Ejecución del Proyecto

1. Clonar el repositorio.
2. Abrir la solución `Confiamed.sln` en Visual Studio o VS Code.
3. Establecer ambos proyectos (`ItemsDeTrabajo.API` y `GestionDeUsuario.API`) como proyectos de inicio múltiple, o ejecutarlos individualmente mediante la CLI:

```bash
cd ItemsDeTrabajo.API
dotnet run
```

4. El navegador se abrirá automáticamente en `/scalar/v1`, mostrando la documentación interactiva y permitiendo simular el algoritmo de asignación de carga de trabajo.
