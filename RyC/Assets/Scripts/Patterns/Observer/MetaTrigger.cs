using UnityEngine;

public class MetaTrigger1 : MonoBehaviour
{
  private bool raceFinished = false;

  private void OnTriggerEnter(Collider other)
  {
    if (raceFinished) return;

    CarController car = other.GetComponent<CarController>();

    if (car != null)
    {
      raceFinished = true;

      if (QuizManager1.Instance != null)
      {
        // ELIMINADO: QuizManager1.Instance.ResetActiveQuestions(); 
        // No reseteamos nada todavía para no romperle el juego al que viene atrás.

        QuizManager1.Instance.FinishRace(car.playerIndex);
      }
    }
  }
}