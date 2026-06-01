# Proyecto: Transformación Estilo DOOM (1993)

## Project Overview
- **Game Title:** Retro Doom Perspective Lab
- **High-Level Concept:** Transformación de una escena 3D de bosque en una experiencia FPS retro donde los modelos 3D parecen sprites pixelados.
- **Players:** Single Player (FPS)
- **Inspiration:** DOOM (1993), Heretic.
- **Tone / Art Direction:** Pixelado, colores vibrantes pero limitados, alta velocidad.
- **Target Platform:** PC
- **Render Pipeline:** URP (PC_RPAsset activo)

## Game Mechanics
### Core Gameplay Loop
- Exploración del laberinto generado.
- Observación de la perspectiva y proyección de objetos 3D bajo restricciones de baja resolución.

### Controls and Input Methods
- **WASD / Flechas:** Movimiento y rotación.
- **E / Espacio:** Interacción (opcional).
- **Restricción:** El eje vertical de la cámara estará bloqueado o limitado para simular la proyección original de Doom.

## UI
- **HUD:** Superposición simple de "Puntos de Vida" o una mira (crosshair) de baja resolución en el centro.

## Key Asset & Context
- **Scripts:** 
    - `DoomPlayerController.cs`: Control de movimiento y rotación horizontal.
    - `PixelationFeature.cs` / `PixelationPass.cs`: Renderer Feature para URP que maneja la pixelación por shader.
- **Shaders:**
    - `RetroPixel.shader`: Shader de pantalla completa para downsampling y posterización de color.
- **Existentes:** 
    - `FirstPersonProxy`: Se usará como base para el CharacterController.
    - `FirstPersonCamera`: Se configurará con Cinemachine.

## Implementation Steps

### 1. Sistema de Entrada y Controlador
1. **Configurar CharacterController:** Añadir componente `CharacterController` a `FirstPersonProxy`.
2. **Implementar `DoomPlayerController.cs`:** 
    - Manejar entrada WASD.
    - Aplicar rotación horizontal suave.
    - Bloquear rotación vertical en la cámara Cinemachine (o limitarla).
    - Asignar a `FirstPersonProxy`.

### 2. Cámara Cinemachine
1. **Configurar `FirstPersonCamera`:** 
    - Asegurar que `CinemachineCamera` sigue a `FirstPersonProxy`.
    - Ajustar `Lens Settings`: FOV a 90, Near Clip a 0.01.
    - Configurar `CinemachinePOV` solo para el eje horizontal si se elige el modo clásico.

### 3. Efecto Visual de Pixelación y Color (Renderer Feature)
1. **Crear Shader `RetroPixel.shader`:** Shader que reduce la resolución de la textura de entrada.
2. **Crear Shader `DoomColorLimiter.shader`:** Shader que maneja la profundidad de color, el dithering y el **oscurecimiento por profundidad** (Depth Dimming).
3. **Configurar Renderer Features:** Inyectar los pases en el `PC_Renderer` para lograr el look de 320x240 con paleta limitada.
4. **Ajuste Atmosférico:** Configurar niebla negra y ambiente oscuro para resaltar las luces emissivas.

### 4. Iluminación y Materiales
1. **Ajustar Luces:** Configurar la luz direccional para sombras duras (Hard Shadows) sin suavizado.
2. **Post-Processing Volume:** 
    - Añadir `Color Adjustments`: Subir contraste, bajar ligeramente la saturación.
    - Tonemapping: Modo "Neutral" para colores más planos.

## Verification & Testing
1. **Prueba de Movimiento:** Verificar que el jugador se desplaza correctamente por el NavMesh/suelo sin colisiones extrañas.
2. **Validación Visual:** Comprobar que los modelos de animales (Ciervo/Tigre) mantienen su forma 3D al girar pero se ven compuestos por "bloques" de píxeles.
3. **Perspectiva:** Verificar que al acercarse a los objetos, los píxeles se mantienen estables en pantalla (espacio de pantalla vs espacio de objeto).
