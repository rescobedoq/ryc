using UnityEngine;

public class PortalAnswerTrigger1 : MonoBehaviour
{
  [Tooltip("Debe coincidir con el ID del PreportalTrigger anterior")]
  public int portalId = 1; // Identificador del portal

  public int answerIndex; // 0 para izquierda, 1 para derecha

  // Variable para controlar el tiempo y evitar dobles activaciones
  private float lastTriggerTime = -1f;
  private float triggerCooldown = 1.0f;

  private void OnTriggerEnter(Collider other)
  {
    if (Time.time - lastTriggerTime < triggerCooldown) return;

    CarController car = other.GetComponentInParent<CarController>();

    if (car != null)
    {
      lastTriggerTime = Time.time;

      if (QuizManager1.Instance != null)
      {
        // Pasamos también el portalId
        QuizManager1.Instance.SubmitAnswer(car.playerIndex, answerIndex, portalId);
      }
    }
  }
}