using UnityEngine;

public class MetaTrigger1 : MonoBehaviour
{
  private bool raceFinished = false;

  private void OnTriggerEnter(Collider other)
  {
    var car = other.GetComponentInParent<CarController>();
    if (car == null) return;

    // PlayerIndex del coche
    PlayerIndex p = car.GetPlayerIndex();

    // Notificar paso por META + referencia al coche para escribir en HUD
    QuizManager1.Instance.NotifyMetaPassed(p, car);
  }
}