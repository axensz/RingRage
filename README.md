# **RingRage - Juego de Boxeo**

**RingRage** es un juego de boxeo 1v1, donde los jugadores pueden elegir personajes, enfrentar desafíos en un torneo y mejorar su rendimiento a través de entrenamientos personalizados. El objetivo es escalar en los torneos y convertirse en el campeón supremo.

## **Características**

- **Torneo 1v1:** Enfrenta a tu personaje contra oponentes en un torneo desafiante.
- **Entrenamiento Personalizado:** Mejora tu personaje a través de entrenamientos que analizan tus debilidades.
- **Modos de Juego:**
  - **Torneo:** Enfréntate a diferentes rivales hasta llegar a la final.
  - **Combate rápido:** Combate rápido contra un oponente en cualquier momento.
  - **Ajustes de sonido y música.**

## **Flujo del Juego**

1. **Pantalla de Inicio**
   - Jugar
   - Créditos
   - Ajustes
   - Salir

2. **Seleccionar Modo de Juego**
   - Torneo
   - Combate rápido

3. **Pantalla de Selección de Personaje**
   - Escoge un personaje con habilidades y estilos únicos.

4. **Seleccionar Dificultad**
   - Fácil
   - Medio
   - Difícil

5. **Combate 1v1**
   - HUD con barras de salud, energía y botones de control (golpe, esquivar).

6. **Resultado del Combate**
   - Ganas → Pasas al siguiente nivel.
   - Pierdes → Opción de reintentar o salir.

7. **Pantalla Final del Torneo**
   - Recompensas, mejoras y desbloqueo de nuevos niveles de dificultad.

## **Tecnologías Usadas**

- **Motor de Juego:** Unity
- **Lenguaje de Programación:** C#
- **Plataforma:** Escritorio

## **Estructura del Proyecto**

```bash
RingRage/
│
├── Assets/
│   └── Sprites/          # Gráficos de los personajes y escenarios
│   └── Sounds/           # Archivos de sonido y música
│
├── Scripts/
│   └── GameController.cs # Lógica principal del juego
│   └── Character.cs      # Control de los personajes y sus stats
│   └── AudioManager.cs   # Control de música y sonidos
│   └── UIController.cs   # Interfaz de usuario
│
├── Scenes/
│   └── MainMenu.unity    # Escena de menú principal
│   └── GameScene.unity   # Escena de combate
│   └── EndScene.unity    # Escena de resultado
│
└── README.md             
