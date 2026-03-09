#!/usr/bin/env bash
# Quick compile check for all Unity C# assemblies.
# Run from the repo root: bash test-compile.sh

set -e

echo "=== Compiling Assembly-CSharp ==="
dotnet build "Assembly-CSharp.csproj" --nologo -v quiet

echo "=== Compiling Assembly-CSharp-Editor ==="
dotnet build "Assembly-CSharp-Editor.csproj" --nologo -v quiet

echo "=== All assemblies compiled successfully ==="
