using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager1 : MonoBehaviour
{
  public static QuizManager1 Instance { get; private set; }

  [Header("Data")]
  [SerializeField] private QuestionBank1 questionBank; // Asigna tu ScriptableObject o referencia aquí

  private List<IQuizObserver> observers = new List<IQuizObserver>();
  // Key: ID del Portal (1, 2, etc.) -> Value: La pregunta activa en ese portal
  private Dictionary<int, Question> activeQuestions = new Dictionary<int, Question>();

  // Diccionario para contar cuántas veces han pedido pregunta en cada portal
  private Dictionary<int, int> portalRequestCounts = new Dictionary<int, int>();

  private void Start()
  {
    ResetActiveQuestions();
  }

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  // --- Gestión de Observadores (Patrón Observer) ---
  public void AddObserver(IQuizObserver observer)
  {
    if (!observers.Contains(observer))
      observers.Add(observer);
  }

  public void RemoveObserver(IQuizObserver observer)
  {
    if (observers.Contains(observer))
      observers.Remove(observer);
  }

  private void NotifyQuestionLoaded(Question q, int portalId)
  {
    foreach (var observer in observers)
      observer.OnQuestionLoaded(q.questionText, q.answers, portalId);
  }

  private void NotifyAnswerCorrect(PlayerIndex player, int portalId)
  {
    foreach (var observer in observers) observer.OnAnswerCorrect(player, portalId);
  }

  private void NotifyAnswerWrong(PlayerIndex player, int portalId)
  {
    foreach (var observer in observers) observer.OnAnswerWrong(player, portalId);
  }

  private void NotifyFinished()
  {
    foreach (var observer in observers)
      observer.OnQuizFinished();
  }

  // --- Lógica del Quiz ---
  public void LoadRandomQuestion(int portalId)
  {
      if (questionBank == null) return;

      // Inicializar contador si no existe
      if (!portalRequestCounts.ContainsKey(portalId))
      {
          portalRequestCounts[portalId] = 0;
      }

      // Aumentar contador de visitas
      portalRequestCounts[portalId]++;
      int currentCount = portalRequestCounts[portalId];

      // --- PARA SOLO 1 JUGADOR ---
      // Contamos cuántos CarControllers hay activos en la escena
      int playerCount = FindObjectsOfType<CarController>().Length;

      bool shouldGenerateNew = true;

      if (playerCount > 1)
      {
          // Lógica Multijugador: Solo generamos nueva si es impar (el líder de la vuelta)
          bool isLeaderOfLap = (currentCount % 2 != 0);
          shouldGenerateNew = isLeaderOfLap;
      }
      // Si playerCount == 1, shouldGenerateNew se queda en true siempre.

      if (!shouldGenerateNew && activeQuestions.ContainsKey(portalId))
      {
          // Es el segundo auto en multijugador, reutilizamos la pregunta
          Question existingQuestion = activeQuestions[portalId];
          NotifyQuestionLoaded(existingQuestion, portalId);
          return;
      }

      // Generamos nueva pregunta (Modo 1 jugador O Líder en multijugador)
      Question newQuestion = questionBank.GetRandomQuestion();

      if (activeQuestions.ContainsKey(portalId))
          activeQuestions[portalId] = newQuestion;
      else
          activeQuestions.Add(portalId, newQuestion);

      NotifyQuestionLoaded(newQuestion, portalId);
  }

  public void ResetActiveQuestions()
  {
    activeQuestions.Clear();
    portalRequestCounts.Clear();
    Debug.Log("[QuizManager1] Datos reseteados.");
  }

  // Método público llamado por MetaTrigger1
  public void FinishRace(PlayerIndex winner)
  {
    Debug.Log($"Carrera terminada. Ganador: {winner}");
    NotifyRaceFinished(winner);
  }
  private void NotifyRaceFinished(PlayerIndex winner)
  {
    foreach (var observer in observers)
      observer.OnRaceFinished(winner);
  }

  public void SubmitAnswer(PlayerIndex player, int answerIndex, int portalId)
  {
    // Verificamos si existe una pregunta activa para este portal
    if (!activeQuestions.ContainsKey(portalId))
    {
      Debug.LogWarning($"[QuizManager1] El jugador {player} pasó por el Portal {portalId}, pero no había pregunta activa allí.");
      return;
    }

    Question q = activeQuestions[portalId];

    if (q.correctAnswerIndex == answerIndex)
    {
      Debug.Log($"[QuizManager1] ¡Correcto en Portal {portalId}!");
      NotifyAnswerCorrect(player, portalId);
    }
    else
    {
      Debug.Log($"[QuizManager1] Incorrecto en Portal {portalId}.");
      NotifyAnswerWrong(player, portalId);
    }

    // Opcional: Limpiar la pregunta de este portal después de responder
    // activeQuestions.Remove(portalId); 
    // NotifyQuizFinished(); // Cuidado: esto podría borrar el HUD del otro jugador si no se filtra
  }
}
