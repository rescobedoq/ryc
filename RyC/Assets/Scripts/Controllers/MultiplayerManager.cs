using UnityEngine;
using UnityEngine.SceneManagement;  // Para SceneManager si usas

public class MultiplayerManager : MonoBehaviour
{
  [Header("Setup")]
  [SerializeField] private GameObject carPrefab;
  [SerializeField] private Transform spawnPoint1;  // Posición P1 (izquierda)
  [SerializeField] private Transform spawnPoint2;  // Posición P2 (derecha)

//   private void Start()
//   {
//     // Spawn P1
//     var car1 = Instantiate(carPrefab, spawnPoint1.position, spawnPoint1.rotation);
//     var ctrl1 = car1.GetComponent<CarController>();
//     ctrl1.SetPlayerIndex(PlayerIndex.One);

//     // Spawn P2
//     var car2 = Instantiate(carPrefab, spawnPoint2.position, spawnPoint2.rotation);
//     var ctrl2 = car2.GetComponent<CarController>();
//     ctrl2.SetPlayerIndex(PlayerIndex.Two);

//     // Desactiva cámara principal si existe
//     var mainCam = Camera.main;
//     if (mainCam != null) mainCam.enabled = false;

//     Debug.Log("¡Multijugador iniciado! P1: WASD+Espacio | P2: Flechas+Enter");
//   }
  private void Start()
  {
    GameMode mode = (GameMode)PlayerPrefs.GetInt("GameMode", (int)GameMode.SinglePlayer);
    string car1Name = PlayerPrefs.GetString("Car1Name", "Default");
    string car2Name = PlayerPrefs.GetString("Car2Name", "Default");
  
    if (mode == GameMode.SinglePlayer)
    {
      // Spawn solo P1, full screen
      var car1 = Instantiate(carPrefab, spawnPoint1.position, spawnPoint1.rotation);
      var ctrl1 = car1.GetComponent<CarController>();
      ctrl1.SetPlayerIndex(PlayerIndex.One);
      ApplyCarData(ctrl1, car1Name);  // Aplica atributos
  
      // Cámara full: En CameraController, si solo P1, set rect = new Rect(0,0,1,1);
      var camCtrl1 = car1.GetComponentInChildren<CameraController>();
      //cam1.rect = new Rect(0f, 0f, 1f, 1f);  // Full screen
      if (camCtrl1 != null) camCtrl1.cam.rect = new Rect(0f, 0f, 1f, 1f);
  
      // Desactiva Main Cam
      //var mainCam = Camera.main; if (mainCam) mainCam.enabled = false;
      var mainCam = Camera.main;
      if (mainCam != null) mainCam.enabled = false;
    }
    else  // MultiPlayer
    {
      // Como antes: Spawn 2, split
      var car1 = Instantiate(carPrefab, spawnPoint1.position, spawnPoint1.rotation);
      var ctrl1 = car1.GetComponent<CarController>();
      ctrl1.SetPlayerIndex(PlayerIndex.One);
      ApplyCarData(ctrl1, car1Name);
  
      var car2 = Instantiate(carPrefab, spawnPoint2.position, spawnPoint2.rotation);
      var ctrl2 = car2.GetComponent<CarController>();
      ctrl2.SetPlayerIndex(PlayerIndex.Two);
      ApplyCarData(ctrl2, car2Name);
  
      // Cámaras split: Ya lo hace CameraController via playerIndex
    }
  
    Debug.Log($"Modo: {mode} | Car1: {car1Name} | Car2: {car2Name}");
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