# Planr

A project planning toolkit running entirely in the browser using Blazor WebAssembly.

## Architecture

- **Planr.Core**: Shared library containing models, logic, and the Gantt renderer.
- **Planr.Wasm**: Client-side Blazor WebAssembly interface.

## Getting Started

Run the application locally:

```bash
dotnet run --project src/Planr.Wasm
```

Open your browser to the URL shown in the console (typically `http://localhost:5030` or similar).

### Features

- **Client-Side Execution**: No server required. All chart generation happens instantly in your browser.
- **Privacy Focused**: Your project data never leaves your device.
- **Static Hosting**: The app can be deployed to any static file host (GitHub Pages, Netlify, etc.).
- **Import/Export**: Save and load your project plans as JSON files locally.

## JSON Specification Format

```json
{
  "tasks": [
    {
      "project": "Project Alpha",
      "name": "Task 1",
      "start": "2026-01-01",
      "end": "2026-01-14",
      "priority": 1
    }
  ]
}
```

## Priorities

1. Critical (Red)
2. High (Orange)
3. Medium (Yellow)
4. Low (Blue)
5. Lowest (Green)
