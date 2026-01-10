# Planr

[Live Demo](https://andrew-a-hale.github.io/planr/)

A project planning toolkit running entirely in the browser using Blazor WebAssembly.

## Architecture

- **Planr.Core**: Shared library containing models, logic, and the Gantt renderer.
- **Planr.Wasm**: Client-side Blazor WebAssembly interface.

## Getting Started

Run the application locally:

```bash
dotnet run --project src/Planr.Wasm
```

Open your browser to the URL shown in the console (typically `http://localhost:5251` or similar).

### Features

- **Client-Side Execution**: No server required. All chart generation happens in your browser.
- **Privacy Focused**: Your project data never leaves your device.
- **Import/Export**: Save and load your project plans as JSON files locally.

## Gantt Chart

### JSON Specification Format

```json
{
  "config": {
    "title": "Project Plan"
    "labelWidth": 150,
    "showLegend": true,
    "containerMaxWidth": 1200,
  },
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

### Priorities

1. Critical (Red)
2. High (Orange)
3. Medium (Yellow)
4. Low (Blue)
5. Lowest (Green)

## Effort-Impact Chart

### JSON Specification Format

```json
{
  "config": {
    "title": "Prioritisation"
    "containerMaxWidth": 1200,
  },
  "items": [
    {
      "category": "Project Alpha",
      "ref": 1,
      "name": "Fix Bug",
      "impact": 1,
      "effort": 1
    },
    {
      "category": "Project Beta",
      "ref": 2,
      "name": "Optimisation",
      "impact": 2,
      "effort": 2
    }
  ]
}
```

## Spider Chart

### JSON Specification Format

```json
{
  "config": {
    "title": "Spider Chart",
    "containerMaxWidth": 1200
  },
  "items": [
    {
      "category": "Dimension 1",
      "first": -1,
      "second": 2,
      "third": null
    },
    {
      "category": "Dimension 2",
      "first": -1,
      "second": 3,
      "third": null
    },
    {
      "category": "Dimension 3",
      "first": 0,
      "second": 3,
      "third": null
    }
  ]
}
```
