using UnityEngine;
using UnityEngine.SceneManagement;  // Para SceneManager si usas

public class MultiplayerManager : MonoBehaviour
{
  [Header("Setup")]
  [SerializeField] private GameObject carPrefab;
  [SerializeField] private Transform spawnPoint1;  // Posición P1 (izquierda)
  [SerializeField] private Transform spawnPoint2;  // Posición P2 (derecha)

  private void Start()
  {
    GameMode mode = (GameMode)PlayerPrefs.GetInt("GameMode", (int)GameMode.SinglePlayer);
    string car1Name = PlayerPrefs.GetString("Car1Name", "Default");
    string car1PrefabName = PlayerPrefs.GetString("Car1Prefab", "CarPrefab");  // <-- NUEVO
    string car2Name = PlayerPrefs.GetString("Car2Name", "Default");
    string car2PrefabName = PlayerPrefs.GetString("Car2Prefab", "CarPrefab");  // <-- NUEVO
  
    if (mode == GameMode.SinglePlayer)
    {
      GameObject carPrefab1 = Resources.Load<GameObject>("Prefabs/Cars/" + car1PrefabName);
      if (carPrefab1 == null)
      {
        Debug.LogError($"Prefab no encontrado: Prefabs/Cars/{car1PrefabName}");
        return;
      }
        
      // Spawn solo P1, full screen
      var car1 = Instantiate(carPrefab1, spawnPoint1.position, spawnPoint1.rotation);
      var ctrl1 = car1.GetComponent<CarController>();
      if (ctrl1 != null)
      {
        ctrl1.SetPlayerIndex(PlayerIndex.One);
        ApplyCarData(ctrl1, car1Name);  // Stats de CarData
      }

      var mainCam = Camera.main;
      if (mainCam != null) mainCam.enabled = false;
    }
    else  // MultiPlayer
    {
      // Como antes: Spawn 2, split
      GameObject carPrefab1 = Resources.Load<GameObject>("Prefabs/Cars/" + car1PrefabName);
      GameObject carPrefab2 = Resources.Load<GameObject>("Prefabs/Cars/" + car2PrefabName);
      if (carPrefab1 == null || carPrefab2 == null)
      {
        Debug.LogError($"Prefab(s) no encontrados: {car1PrefabName}, {car2PrefabName}");
        return;
      }

      var car1 = Instantiate(carPrefab1, spawnPoint1.position, spawnPoint1.rotation);
      var ctrl1 = car1.GetComponent<CarController>();
      if (ctrl1 != null)
      {
        ctrl1.SetPlayerIndex(PlayerIndex.One);
        ApplyCarData(ctrl1, car1Name);
      }

      var car2 = Instantiate(carPrefab2, spawnPoint2.position, spawnPoint2.rotation);
      var ctrl2 = car2.GetComponent<CarController>();
      if (ctrl2 != null)
      {
        ctrl2.SetPlayerIndex(PlayerIndex.Two);
        ApplyCarData(ctrl2, car2Name);
      }
    }

    Debug.Log($"Spawned {car1Name} (prefab: {car1PrefabName}) y {car2Name} (prefab: {car2PrefabName}) en modo {mode}");
  }
  
  private void ApplyCarData(CarController ctrl, string carName)
  {
    // Busca el SO por nombre (o usa Resources.Load si los guardas)
    CarData data = Resources.Load<CarData>("Cars/" + carName);  // Carpeta Cars en Resources
    if (data == null) return;
  
    ctrl.BaseAcceleration = data.GetRealAcceleration();
    ctrl.MaxSpeed = data.GetRealMaxSpeed();
    ctrl.SteeringForce = data.steeringForce * 0.8f;  // Ejemplo mapeo
    ctrl.BrakeForce = data.brakeForce * 6f;
    // Agrega más.
  }
}