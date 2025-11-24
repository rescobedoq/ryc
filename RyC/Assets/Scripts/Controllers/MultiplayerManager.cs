using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
  [Header("Setup")]
  [SerializeField] private GameObject carPrefab;
  [SerializeField] private Transform spawnPoint1;  // Posición P1 (izquierda)
  [SerializeField] private Transform spawnPoint2;  // Posición P2 (derecha)

  private void Start()
  {
    // Spawn P1
    var car1 = Instantiate(carPrefab, spawnPoint1.position, spawnPoint1.rotation);
    var ctrl1 = car1.GetComponent<CarController>();
    ctrl1.SetPlayerIndex(PlayerIndex.One);

    // Spawn P2
    var car2 = Instantiate(carPrefab, spawnPoint2.position, spawnPoint2.rotation);
    var ctrl2 = car2.GetComponent<CarController>();
    ctrl2.SetPlayerIndex(PlayerIndex.Two);

    // Desactiva cámara principal si existe
    var mainCam = Camera.main;
    if (mainCam != null) mainCam.enabled = false;

    Debug.Log("¡Multijugador iniciado! P1: WASD+Espacio | P2: Flechas+Enter");
  }
}