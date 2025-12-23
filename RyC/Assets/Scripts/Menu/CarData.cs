using UnityEngine;

[CreateAssetMenu(fileName = "New Car Data", menuName = "Car Data")]
public class CarData : ScriptableObject
{
  public string carName = "Default Car";
  public string prefabName = "CarPrefab";
  public Sprite carImage;  // Imagen del carro para el botón

  [Header("Attributes (0-100 for bars)")]
  public float baseSpeed = 50f;      // Normalizado 0-100
  public float baseAcceleration = 50f;
  public float steeringForce = 50f;
  public float brakeForce = 50f;
  public float maxSpeed = 50f;

  // Método para mapear a valores reales de CarController (e.g., 50 → 1000 accel)
  public float GetRealAcceleration() => baseAcceleration * 20f;  // Ej: 50=1000, 100=2000
  public float GetRealMaxSpeed() => maxSpeed * 1.5f;             // Ej: 50=75, 100=150
  // Agrega más getters si necesitas.
}