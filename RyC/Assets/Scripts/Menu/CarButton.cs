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

  public CarData carData;
  public bool isSelected = false;
  public static System.Action<CarButton> OnCarSelected;  // Pasa el botón entero

  public void Initialize(CarData data)
  {
    carData = data;
    carImage.sprite = data.carImage;
    carNameText.text = data.carName;
    statsDisplay.UpdateStats(data);
    highlightImage.color = Color.clear;  // Inicial no selected
    UpdateVisual();  // Llama para asegurar estado inicial
  }

  public void OnClick()
  {
    Debug.Log($"Clic en {carData.carName} - Selected: {isSelected} - Color: {highlightImage.color}");

    // Setear como seleccionado (sin toggle – siempre selecciona este)
    isSelected = true;
    UpdateVisual();  // Actualiza visual inmediato

    OnCarSelected?.Invoke(this);  // Dispara evento (MenuManager manejará deselección de otros)
  }

  // Método público para deseleccionar (llamado por MenuManager)
  public void Deselect()
  {
    isSelected = false;
    UpdateVisual();
  }

  // Centraliza lógica visual (color + scale)
  private void UpdateVisual()
  {
    if (highlightImage != null)
    {
      highlightImage.color = isSelected ? new Color(0f, 1f, 0f, 0.3f) : new Color(0f, 0f, 0f, 0f);  // Verde si selected
    }

    transform.localScale = isSelected ? Vector3.one * 1.05f : Vector3.one;  // Escala si selected
  }
}