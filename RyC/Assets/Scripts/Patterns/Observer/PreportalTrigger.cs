using UnityEngine;

public class PreportalTrigger1 : MonoBehaviour
{
  [Tooltip("Ponle 1 al primer portal, 2 al segundo, etc.")]
  public int portalId = 1;

  private void OnTriggerEnter(Collider other)
  {
    Debug.Log($"[PreportalTrigger1] Algo entró en el trigger: {other.name} (Tag: {other.tag})");

    if (other.CompareTag("Player"))
    {
      Debug.Log("[PreportalTrigger1] ¡Es el Player! Llamando a QuizManager1...");

      if (QuizManager1.Instance != null)
      {
        QuizManager1.Instance.LoadRandomQuestion(portalId);
      }
      else
      {
        Debug.LogError("[PreportalTrigger1] ERROR: QuizManager1.Instance es NULL. ¿Está el QuizManager en la escena?");
      }
    }
  }
}