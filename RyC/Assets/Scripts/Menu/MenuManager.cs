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

  [Header("Play Buttons (Independientes)")]
  public Button singlePlayButton;        // "Jugar" en single menu
  public Button multiPlayButton;         // "Jugar" en multi menu

  [Header("Car Selection")]
  public List<CarData> availableCars = new List<CarData>();  // Arrastra tus SO assets
  public Transform singleScrollContent;  // Content del ScrollView para 1P
  public Transform multiScrollContent1;  // Para P1 en 2P
  public Transform multiScrollContent2;  // Para P2 en 2P
  public GameObject carButtonPrefab;

  private GameMode currentMode;
  private CarData selectedCar1, selectedCar2;  // Selecciones
  private CarButton selectedButton1, selectedButton2;

  private void Start()
  {
    // Suscribe botones de modo
    singleModeButton.onClick.AddListener(() => SelectMode(GameMode.SinglePlayer));
    multiModeButton.onClick.AddListener(() => SelectMode(GameMode.MultiPlayer));

    // Suscribe botones de jugar (comunes, pero independientes)
    if (singlePlayButton != null) singlePlayButton.onClick.AddListener(LoadGameScene);
    if (multiPlayButton != null) multiPlayButton.onClick.AddListener(LoadGameScene);

    // Inicial: Oculta submenús y desactiva botones de jugar
    singlePlayerMenu.SetActive(false);
    multiPlayerMenu.SetActive(false);
    if (singlePlayButton != null) singlePlayButton.interactable = false;
    if (multiPlayButton != null) multiPlayButton.interactable = false;

    // Suscribe a selección
    CarButton.OnCarSelected += OnCarSelected;
  }

  private void SelectMode(GameMode mode)
{
  currentMode = mode;
  singlePlayerMenu.SetActive(mode == GameMode.SinglePlayer);
  multiPlayerMenu.SetActive(mode == GameMode.MultiPlayer);

  // Limpia selecciones y BOTONES previos
  selectedCar1 = null;
  selectedCar2 = null;
  if (selectedButton1 != null) selectedButton1.Deselect(); selectedButton1 = null;  // <-- NUEVO
  if (selectedButton2 != null) selectedButton2.Deselect(); selectedButton2 = null;  // <-- NUEVO

  // Desactiva ambos botones de jugar al cambiar modo
  if (singlePlayButton != null) singlePlayButton.interactable = false;
  if (multiPlayButton != null) multiPlayButton.interactable = false;

  // Limpia y popula ScrollViews
  ClearScrollContents();
  PopulateScrollView(singleScrollContent, mode == GameMode.SinglePlayer ? 1 : 0);
  PopulateScrollView(multiScrollContent1, mode == GameMode.MultiPlayer ? 1 : 0);
  PopulateScrollView(multiScrollContent2, mode == GameMode.MultiPlayer ? 1 : 0);
}

  private void PopulateScrollView(Transform content, int playerCount)
  {
    if (playerCount == 0) return;

    foreach (var car in availableCars)
    {
      var buttonObj = Instantiate(carButtonPrefab, content);
      var button = buttonObj.GetComponent<CarButton>();
      if (button != null)
      {
        button.Initialize(car);
      }
      else
      {
        Debug.LogError("CarButton no encontrado en prefab instanciado!");
      }
    }
  }

  private void ClearScrollContents()
  {
    // Borra botones viejos (usa DestroyImmediate en Editor para limpieza rápida)
    foreach (Transform child in singleScrollContent) if (Application.isEditor) DestroyImmediate(child.gameObject); else Destroy(child.gameObject);
    foreach (Transform child in multiScrollContent1) if (Application.isEditor) DestroyImmediate(child.gameObject); else Destroy(child.gameObject);
    foreach (Transform child in multiScrollContent2) if (Application.isEditor) DestroyImmediate(child.gameObject); else Destroy(child.gameObject);
  }

  private void OnCarSelected(CarButton selectedButton)
  {
    CarData car = selectedButton.carData;
  
    if (currentMode == GameMode.SinglePlayer)
    {
      // Deselecciona anterior si hay
      if (selectedButton1 != null) selectedButton1.Deselect();
      selectedButton1 = selectedButton;  // Nuevo seleccionado
      selectedCar1 = car;
  
      // Habilita botón de single
      if (singlePlayButton != null) singlePlayButton.interactable = (selectedCar1 != null);
    }
    else  // MultiPlayer
    {
      if (selectedButton.transform.IsChildOf(multiScrollContent1))
      {
        // Deselecciona anterior P1
        if (selectedButton1 != null) selectedButton1.Deselect();
        selectedButton1 = selectedButton;
        selectedCar1 = car;
      }
      else if (selectedButton.transform.IsChildOf(multiScrollContent2))
      {
        // Deselecciona anterior P2
        if (selectedButton2 != null) selectedButton2.Deselect();
        selectedButton2 = selectedButton;
        selectedCar2 = car;
      }
  
      // Habilita multi solo si ambos
      if (multiPlayButton != null) multiPlayButton.interactable = (selectedCar1 != null && selectedCar2 != null);
    }
  
    Debug.Log($"Seleccionado: {car.carName} en modo {currentMode} - Solo este verde!");
  }
  
  // Helper: Deselecciona todos los botones en un Content (excepto el actual si pasas)
  private void DeselectAllButtonsInContent(Transform content, CarButton exceptThis = null)
  {
    foreach (Transform child in content)
    {
      var button = child.GetComponent<CarButton>();
      if (button != null && button != exceptThis)
      {
        button.isSelected = false;
        if (button.highlightImage != null)
        {
          button.highlightImage.color = Color.clear;  // Transparente
        }
        button.transform.localScale = Vector3.one;  // Escala normal
      }
    }
  }

  private void LoadGameScene()
  {
    // Guarda en PlayerPrefs (usa currentMode)
    PlayerPrefs.SetInt("GameMode", (int)currentMode);
    if (selectedCar1 != null) PlayerPrefs.SetString("Car1Name", selectedCar1.carName);
    if (selectedCar2 != null) PlayerPrefs.SetString("Car2Name", selectedCar2.carName);
    PlayerPrefs.Save();

    SceneManager.LoadScene("GameScene");  // Tu escena de juego
  }

  // Cleanup al destruir (opcional)
  private void OnDestroy()
  {
    CarButton.OnCarSelected -= OnCarSelected;
  }
}