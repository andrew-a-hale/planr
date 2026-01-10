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
    "title": "Quarterly Performance",
    "seriesNames": ["Q1 2024", "Q2 2024", "Q3 2024"],
    "containerMaxWidth": 1200
  },
  "items": [
    {
      "category": "Revenue",
      "seriesValues": {
        "Q1 2024": -1,
        "Q2 2024": 2,
        "Q3 2024": null
      }
    },
    {
      "category": "Growth",
      "seriesValues": {
        "Q1 2024": -1,
        "Q2 2024": 3,
        "Q3 2024": null
      }
    },
    {
      "category": "Retention",
      "seriesValues": {
        "Q1 2024": 0,
        "Q2 2024": 3,
        "Q3 2024": null
      }
    },
    {
      "category": "Satisfaction",
      "seriesValues": {
        "Q1 2024": 1,
        "Q2 2024": 2,
        "Q3 2024": null
      }
    }
  ]
}
```

### Features
- **Flexible series**: Add or remove series by editing the `seriesNames` array in config (minimum 2 required)
- **Custom names**: Series names can be any text (e.g., "Q1 2024", "Product A", etc.)
- **Automatic colors**: Colors are automatically assigned from a palette of 10 colors, cycling if more series are added
- **Value range**: Values are clamped between -1 (center) and 4 (outer edge)
- **Null handling**: All values for a series must be provided, or all must be null (to hide a series)
