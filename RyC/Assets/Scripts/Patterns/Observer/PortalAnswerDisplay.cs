using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class PortalAnswerDisplay1 : MonoBehaviour, IQuizObserver
{
  [Tooltip("Si es True, muestra la respuesta 0. Si es False, muestra la respuesta 1.")]
  public int myPortalId = 1;
  public bool isLeft = true;

  private TextMeshPro tmp;

  private void Awake()
  {
    tmp = GetComponent<TextMeshPro>();
  }

  private void Start()
  {
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
      // ... Tu lógica existente para mostrar respuesta izq/der ...
      int index = isLeft ? 0 : 1;
      if (answers != null && answers.Length > index)
        GetComponent<TextMeshPro>().text = answers[index];
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