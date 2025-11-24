using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarButton : MonoBehaviour
{
  [Header("UI References")]
  public Image carImage;
  public TextMeshProUGUI carNameText;
  public Image highlightImage;  // Imagen de fondo para brillo (color verde al seleccionar)
  public CarStatsDisplay statsDisplay;

  private CarData carData;
  private bool isSelected = false;
  public static System.Action<CarData> OnCarSelected;  // Evento para notificar al manager

  public void Initialize(CarData data)
  {
    carData = data;
    carImage.sprite = data.carImage;
    carNameText.text = data.carName;
    statsDisplay.UpdateStats(data);
    highlightImage.color = Color.clear;  // Inicial no selected
  }

  public void OnClick()
  {
    // Toggle selección (o solo select si prefieres)
    isSelected = !isSelected;
    highlightImage.color = isSelected ? Color.green : Color.clear;  // Brillo verde
    transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;  // Escala

    if (isSelected) OnCarSelected?.Invoke(carData);  // Notifica
  }
}