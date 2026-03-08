# Script para actualizar URLs de imágenes en DataSeedService.cs
# Agrega parámetros de optimización a todas las URLs de Unsplash

$filePath = "backend/Infrastructure/Services/DataSeedService.cs"
$content = Get-Content $filePath -Raw

# Patrón para encontrar URLs de Unsplash sin parámetros
$pattern = 'ImageUrl = "https://images\.unsplash\.com/photo-([a-f0-9\-]+)",'

# Reemplazo con parámetros optimizados
$replacement = 'ImageUrl = "https://images.unsplash.com/photo-$1?w=800&h=600&fit=crop&auto=format&q=80",'

# Aplicar el reemplazo
$updatedContent = $content -replace $pattern, $replacement

# Guardar el archivo actualizado
Set-Content -Path $filePath -Value $updatedContent

Write-Host "✅ URLs de imágenes actualizadas exitosamente!" -ForegroundColor Green
Write-Host "📊 Se agregaron parámetros de optimización a todas las URLs de Unsplash" -ForegroundColor Cyan
