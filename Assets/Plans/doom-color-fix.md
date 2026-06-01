# Proyecto: Transformación Estilo DOOM (1993) - Fase de Color y Atmósfera

## Project Overview
- **Objetivo:** Eliminar el aspecto "blanco/lavado" y añadir el contraste y la paleta vibrante de DOOM.

## Key Asset & Context
- **Shaders:** 
    - `DoomColorLimiter.shader`: Actualizado para incluir **Depth Dimming** (oscurecimiento por distancia).
- **Escena:**
    - **Iluminación:** Ajustar la luz ambiental y la niebla para crear profundidad.

## Implementation Steps

### 1. Actualización del Shader `DoomColorLimiter.shader`
- Añadir soporte para textura de profundidad mediante `_CameraDepthTexture`.
- Implementar lógica de oscurecimiento: los objetos a más de 30 unidades se verán negros.
- Añadir un multiplicador de **Contraste** de 1.5 para dar esa "fuerza" visual de los 90.
- Saturar los verdes y rojos para que resalten.

### 2. Configuración Atmosférica
- Cambiar el color del cielo/ambiente a tonos más oscuros (Casi negro o azul profundo).
- Activar la **Niebla (Fog)** en la escena con un color que combine con el oscurecimiento del shader.
- Ajustar la intensidad de la luz direccional (Sun) para que no sobre-exponga los píxeles.

### 3. Ajuste de Materiales
- Crear un material para el cielo (Skybox) que sea menos brillante o desactivarlo por un color sólido.

## Verification & Testing
1. **Contraste:** Verificar que los objetos cercanos se ven coloridos y los lejanos se desvanecen en la oscuridad.
2. **Color:** Los rojos (ojos del tigre), verdes (bosque) y marrones deben ser intensos y pixelados.
