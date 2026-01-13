#!/bin/bash

# Configuration
PROJECT_PATH="src/Planr.Wasm"
OUTPUT_DIR="dist/portal"
BASE_HREF="/tools/planr/"

echo "🚀 Starting portal-optimized build..."

# 1. Clean and Publish
echo "📦 Publishing Blazor WebAssembly..."
dotnet publish "$PROJECT_PATH" -c Release -o "$OUTPUT_DIR" --nologo

# 2. Patch index.html
if [ -f "$OUTPUT_DIR/wwwroot/index.html" ]; then
    echo "🔧 Patching index.html with base href: $BASE_HREF"
    sed -i "s|<base href=\"/\" />|<base href=\"$BASE_HREF\" />|g" "$OUTPUT_DIR/wwwroot/index.html"
else
    echo "❌ Error: index.html not found in $OUTPUT_DIR/wwwroot/"
    exit 1
fi

# 3. Patch 404.html (for SPA routing fallbacks)
if [ -f "$OUTPUT_DIR/wwwroot/404.html" ]; then
    echo "🔧 Patching 404.html with base href: $BASE_HREF"
    sed -i "s|<base href=\"/\" />|<base href=\"$BASE_HREF\" />|g" "$OUTPUT_DIR/wwwroot/404.html"
fi

echo "✅ Build complete! Assets ready in: $OUTPUT_DIR/wwwroot"
