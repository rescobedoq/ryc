using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarStatsDisplay : MonoBehaviour
{
  [Header("Stat Templates (Asigna en prefab)")]
  public StatBar[] statBars = new StatBar[5];  // Array de 5 barras

  [System.Serializable]
  public class StatBar
  {
    public string statName;       // Ej: "Max Speed"
    public TMP_Text nameText;
    public Image barImage;
    public TMP_Text valueText;
  }

  private CarData carData;

  public void UpdateStats(CarData data)
  {
    carData = data;
    for (int i = 0; i < statBars.Length; i++)
    {
      var stat = statBars[i];
      stat.nameText.text = stat.statName;

      float normalizedValue = GetNormalizedValue(i);  // 0-100
      stat.valueText.text = normalizedValue.ToString("F0");
      stat.barImage.fillAmount = normalizedValue / 100f;
    }
  }

  private float GetNormalizedValue(int index)
  {
    return index switch
    {
      0 => carData.baseSpeed,
      1 => carData.baseAcceleration,
      2 => carData.steeringForce,
      3 => carData.brakeForce,
      4 => carData.maxSpeed,
      _ => 0f
    };
  }
}