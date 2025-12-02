using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class PortalQuestionDisplay1 : MonoBehaviour, IQuizObserver
{
  [Tooltip("Debe coincidir con el ID del PreportalTrigger cercano")]
  public int myPortalId = 1;
  private TextMeshPro tmp;

  private void Awake()
  {
    tmp = GetComponent<TextMeshPro>();
  }

  private void Start()
  {
    // Nos suscribimos para escuchar cuando llegue una pregunta
    if (QuizManager1.Instance != null)
    {
      QuizManager1.Instance.AddObserver(this);
    }
  }

  private void OnDestroy()
  {
    if (QuizManager1.Instance != null)
    {
      QuizManager1.Instance.RemoveObserver(this);
    }
  }

  // IQuizObserver Implementation

  public void OnQuestionLoaded(string questionText, string[] answers, int portalId)
  {
    if (portalId == myPortalId)
    {
      if (GetComponent<TextMeshPro>() != null)
        GetComponent<TextMeshPro>().text = questionText;
    }
  }

  public void OnAnswerCorrect(PlayerIndex player, int portalId)
  {
  }

  public void OnAnswerWrong(PlayerIndex player, int portalId)
  {
  }

  public void OnQuizFinished()
  {
    Clear();
  }

  public void OnRaceFinished(PlayerIndex winner) { }

  private void Clear()
  {
    if (tmp != null) tmp.text = "";
  }
}