using UnityEngine;

public class PreportalTrigger1 : MonoBehaviour
{
  [Tooltip("Ponle 1 al primer portal, 2 al segundo, etc.")]
  public int portalId = 1;

  private void OnTriggerEnter(Collider other)
  {
    var car = other.GetComponentInParent<CarController>();
    if (car == null) return;

    // 1) MARCAR que este jugador pasó por preportal
    QuizManager1.Instance.NotifyPreportalPassed(car.GetPlayerIndex());

    // 2) Cargar la pregunta (lo que ya hacías)
    QuizManager1.Instance.LoadRandomQuestion(portalId);
  }
}