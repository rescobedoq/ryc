using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameMode { SinglePlayer, MultiPlayer }

public class MenuManager : MonoBehaviour
{
  [Header("Menus")]
  public GameObject singlePlayerMenu;    // Panel con 1 ScrollView
  public GameObject multiPlayerMenu;     // Panel con 2 ScrollViews
  public Button singleModeButton;        // Botón "1 Jugador"
  public Button multiModeButton;         // "2 Jugadores"
  public Button playButton;              // "Jugar" (común)

  [Header("Car Selection")]
  public List<CarData> availableCars = new List<CarData>();  // Arrastra tus SO assets
  public Transform singleScrollContent;  // Content del ScrollView para 1P
  public Transform multiScrollContent1;  // Para P1 en 2P
  public Transform multiScrollContent2;  // Para P2 en 2P
  public GameObject carButtonPrefab;

  private GameMode currentMode;
  private CarData selectedCar1, selectedCar2;  // Opcional para 2P

  private void Start()
  {
    singleModeButton.onClick.AddListener(() => SelectMode(GameMode.SinglePlayer));
    multiModeButton.onClick.AddListener(() => SelectMode(GameMode.MultiPlayer));
    playButton.onClick.AddListener(LoadGameScene);

    // Inicial: Oculta submenús
    singlePlayerMenu.SetActive(false);
    multiPlayerMenu.SetActive(false);

    // Suscribe a selección
    CarButton.OnCarSelected += OnCarSelected;
  }

  private void SelectMode(GameMode mode)
  {
    currentMode = mode;
    singlePlayerMenu.SetActive(mode == GameMode.SinglePlayer);
    multiPlayerMenu.SetActive(mode == GameMode.MultiPlayer);

    // Limpia selecciones previas
    selectedCar1 = null; selectedCar2 = null;
    ClearScrollContents();

    // Popula ScrollViews con botones
    PopulateScrollView(singleScrollContent, mode == GameMode.SinglePlayer ? 1 : 0);
    PopulateScrollView(multiScrollContent1, mode == GameMode.MultiPlayer ? 1 : 0);
    PopulateScrollView(multiScrollContent2, mode == GameMode.MultiPlayer ? 1 : 0);
  }

  private void PopulateScrollView(Transform content, int playerCount)
  {
    if (playerCount == 0) return;

    foreach (var car in availableCars)
    {
      var button = Instantiate(carButtonPrefab, content).GetComponent<CarButton>();
      button.Initialize(car);
    }
  }

  private void ClearScrollContents()
  {
    // Borra botones viejos
    foreach (Transform child in singleScrollContent) Destroy(child.gameObject);
    foreach (Transform child in multiScrollContent1) Destroy(child.gameObject);
    foreach (Transform child in multiScrollContent2) Destroy(child.gameObject);
  }

//   private void OnCarSelected(CarData car)
//   {
//     if (currentMode == GameMode.SinglePlayer)
//       selectedCar1 = car;
//     else if (/* Detecta cuál ScrollView, e.g., via tag o parámetro */)
//       selectedCar1 = car;  // Para P1
//     else
//       selectedCar2 = car;  // Para P2

//     // Habilita "Jugar" solo si hay selección
//     playButton.interactable = (currentMode == GameMode.SinglePlayer && selectedCar1 != null) ||
//                              (currentMode == GameMode.MultiPlayer && selectedCar1 != null && selectedCar2 != null);
//   }

  private void OnCarSelected(CarButton selectedButton)
  {
    CarData car = selectedButton.carData;  // Extrae el data del botón
  
    if (currentMode == GameMode.SinglePlayer)
    {
      selectedCar1 = car;
    }
    else  // MultiPlayer: Detecta por parent
    {
      if (selectedButton.transform.IsChildOf(multiScrollContent1))
      {
        selectedCar1 = car;  // P1 seleccionó
      }
      else if (selectedButton.transform.IsChildOf(multiScrollContent2))
      {
        selectedCar2 = car;  // P2 seleccionó
      }
    }
  
    // Habilita "Jugar" solo si hay selección completa
    playButton.interactable = (currentMode == GameMode.SinglePlayer && selectedCar1 != null) ||
                             (currentMode == GameMode.MultiPlayer && selectedCar1 != null && selectedCar2 != null);
  }

  private void LoadGameScene()
  {
    // Guarda en PlayerPrefs
    PlayerPrefs.SetInt("GameMode", (int)currentMode);
    if (selectedCar1 != null) PlayerPrefs.SetString("Car1Name", selectedCar1.carName);
    if (selectedCar2 != null) PlayerPrefs.SetString("Car2Name", selectedCar2.carName);
    PlayerPrefs.Save();

    SceneManager.LoadScene("GameScene");  // Tu escena de juego
  }
}