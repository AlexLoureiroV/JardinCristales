# Jardín de Cristales

Práctica de programación y Git (módulo 5.2.8, Tema 2: Gestión de Componentes y Datos en Unity): un cuidador planta cristales aleatorios en un jardín, camina hasta ellos para recogerlos, y todo el estado del juego se guarda en JSON al cerrar y se reconstruye al abrir.

Versión de Unity: 6000.3.16f1 (URP). Escena: `Assets/Scenes/Garden.unity`.

### Enunciado generado: "Jardín de Cristales" 🌱

Quiero que me propongas un ejercicio para Unity 3D, para un curso de videojuegos,
como si fuera el alumno. Sencillo y original, debe ser minimalista y específico para el
tema donde hemos tratado **arrays, listas, persistencia de datos, GetComponent e
Instantiate**. No añadir otras mecánicas, como mucho inputs.

---

### Contexto del juego

Eres un cuidador de un jardín mágico de cristales. En un jardín (plano de 10x10
metros) puedes **plantar** cristales que brotan en posiciones aleatorias, cada uno con
propiedades distintas (color, altura y rareza). También puedes **recoger** los
cristales que están a tu alcance, guardándolos en un inventario.

El jardín debe **recordar su estado**: cuando la escena se cierra, todo lo plantado se
guarda en un archivo JSON; al volver a abrirla, el jardín se reconstruye tal y como
estaba, incluido el inventario de cristales recogidos.


## Cómo se juega

- Pulsa **Espacio** para plantar cristales: cada uno brota en una posición aleatoria del jardín con un prefab, color, altura y rareza aleatorios (Común, Poco común, Raro o Épico).
- Muévete por el jardín con **WASD / flechas** (cámara fija en tercera persona).
- Acércate a un cristal y pulsa **E** para recogerlo: se lee su componente con `GetComponent`, pasa al inventario y la UI se actualiza al momento.
- El jardín tiene un límite de 15 cristales plantados.
- Al cerrar la partida (`OnDestroy`) todo el estado se guarda en `Assets/dataFinal.json`; al abrir de nuevo (`Start`), el jardín se reconstruye tal cual y el inventario recogido se restaura.

Pantalla: contador de cristales en el jardín (arriba a la izquierda), inventario con total y desglose por rareza (arriba a la derecha) y panel de ayuda con los controles (abajo a la izquierda, se oculta con H).

## Controles

| Tecla | Acción |
|---|---|
| W A S D / ← ↑ ↓ → | Mover al jugador por el jardín |
| Espacio | Plantar un cristal aleatorio |
| E | Recoger el cristal más cercano al jugador (a menos de 2 m) |
| H | Mostrar / ocultar la guía de controles |
| X | Vaciar el inventario (los cristales recogidos se pierden) |


## Qué se practica

| Concepto | Dónde |
|---|---|
| Arrays | Configuración de cristales: `possibleColors`, `possibleHeights`, `rarityNames` (GardenManager) |
| Listas | `crystalPrefabs`, `plantedCrystals` (jardín) e `inventory` (inventario) (GardenManager) |
| `Instantiate` | `CreateObject`: los cristales se generan siempre desde prefabs, nunca desde la escena |
| `GetComponent` | Lectura del componente `Crystal` al recoger (CollectNearestCrystal) y al configurar el clon en `CreateObject` |
| Persistencia de datos | `Save` / `SaveFile` / `LoadData` con `JsonUtility`: guardar en `OnDestroy`, cargar en `Start` (`dataFinal.json`) |
| `CharacterController` + Input | Movimiento WASD del jugador (PlayerController) |
| UI (`UnityEngine.UI.Text`) | Contador de jardín, inventario por rareza (`BuildInventoryText`) y panel de controles ocultable (`ToggleControlsPanel`) |

## Guiones

Todos en `Assets/Scripts/`, con el esquema de regiones del curso (Properties, Fields, Unity Callbacks, Public Methods, Private Methods).

| Guion | Función |
|---|---|
| GardenManager | Controlador: plantar, recoger, inventario, persistencia JSON, textos de UI y panel de ayuda |
| Crystal | Componente de cada cristal instanciado (prefab, color, altura, rareza) que se lee con `GetComponent`; anima la flotación y el giro del cristal |
| CrystalData | Estructura serializable para el JSON: prefab, posición, color, altura, rareza e `inInventory` |
| PlayerController | Movimiento del jugador con `CharacterController` (cámara fija, sin rotar al jugador) |

## Posibles ampliaciones

- Sonido al plantar y al recoger cristales.
- Plantar donde señala el jugador con un raycast desde la cámara (clic sobre el terreno).
- Inventario visual con iconos de los cristales recogidos.
- Seeds reproducibles: guardar la semilla de generación para reconstruir jardines idénticos.
- Contrarreloj: recoger X cristales de rareza Épica antes de que acabe el día.
