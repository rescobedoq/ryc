using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MetaLapNotifier : MonoBehaviour
{
    [Header("Número de vueltas de la carrera")]
    [SerializeField] private int totalLaps = 3;

    [Header("Solo mostrar para este jugador")]
    [SerializeField] private PlayerIndex playerToShow = PlayerIndex.One;

    private int lapsPassed = 0;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    // ← AGREGA ESTE MÉTODO PARA PROBAR AL INICIO
    private void Start()
    {
        if (LapBannerUI.Instance != null)
        {
            Debug.Log("✓ LapBannerUI encontrado");
            LapBannerUI.Instance.ShowLapMessage("Sistema de vueltas listo");
        }
        else
        {
            Debug.LogError("✗ LapBannerUI.Instance es NULL!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
            // Primero, detecta CUALQUIER cosa que entre
        Debug.Log($"[META] ¡ALGO ENTRÓ AL TRIGGER! Objeto: {other.name}, Tag: {other.tag}");
        
        var car = other.GetComponentInParent<CarController>();
        if (car == null)
        {
            Debug.LogWarning($"[META] No se encontró CarController en {other.name}");
            return;
        }
        Debug.Log($"[META] Trigger detectado: {other.name}"); // ← PASO 1

        Debug.Log($"[META] CarController encontrado. PlayerIndex: {car.GetPlayerIndex()}"); // ← PASO 3

        // Solo mostramos en la pantalla del jugador indicado (por defecto: Player 1)
        if (car.GetPlayerIndex() != playerToShow)
        {
            Debug.Log($"[META] Jugador ignorado (esperando: {playerToShow})"); // ← PASO 4
            return;
        }

        lapsPassed++;
        Debug.Log($"[META] ¡VUELTA {lapsPassed} DETECTADA!"); // ← PASO 5

        if (LapBannerUI.Instance == null)
        {
            Debug.LogError("[META] LapBannerUI.Instance es NULL en OnTriggerEnter"); // ← PASO 6
            return;
        }

        if (lapsPassed < totalLaps)
        {
            if (lapsPassed < totalLaps - 1)
            {
                LapBannerUI.Instance.ShowLapMessage($"Vuelta {lapsPassed}");
                Debug.Log($"[META] Mostrando: Vuelta {lapsPassed}"); // ← PASO 7
            }
            else
            {
                LapBannerUI.Instance.ShowLapMessage("Última vuelta");
                Debug.Log("[META] Mostrando: Última vuelta"); // ← PASO 8
            }
        }
        else
        {
            Debug.Log("[META] Carrera terminada"); // ← PASO 9
            // LapBannerUI.Instance.ShowLapMessage("Carrera terminada"); // Opcional
        }
    }
}