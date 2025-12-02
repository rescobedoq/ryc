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

  // LÓGICA DE VUELTAS 
  [Header("Vueltas")]
  [SerializeField] private int totalLapsToWin = 3;

  // Vueltas válidas por jugador (PlayerIndex.One, PlayerIndex.Two, etc.)
  private Dictionary<PlayerIndex, int> lapsByPlayer = new Dictionary<PlayerIndex, int>();

  // Marca si el jugador pasó por algún preportal en la vuelta actual
  private HashSet<PlayerIndex> preportalPassedThisLap = new HashSet<PlayerIndex>();

  // Cuando alguien llega a totalLapsToWin, se cierra la carrera
  private bool raceFinished = false;

  private void EnsurePlayerInit(PlayerIndex p)
  {
    if (!lapsByPlayer.ContainsKey(p))
      lapsByPlayer[p] = 0;
  }

  private void Start()
  {
    ResetActiveQuestions();
  }

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  //                GESTIÓN DE OBSERVADORES

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
    foreach (var observer in observers) 
      observer.OnAnswerCorrect(player, portalId);
  }

  private void NotifyAnswerWrong(PlayerIndex player, int portalId)
  {
    foreach (var observer in observers) 
      observer.OnAnswerWrong(player, portalId);
  }

  private void NotifyFinished()
  {
    foreach (var observer in observers)
      observer.OnQuizFinished();
  }

  private void NotifyRaceFinished(PlayerIndex winner)
  {
    foreach (var observer in observers)
      observer.OnRaceFinished(winner);
  }

  //                    LÓGICA DE PREGUNTAS

  /// <summary>
  /// Carga una pregunta aleatoria para un portal.
  /// Usa questionBank y coordina multijugador (líder vs. seguidor).
  /// </summary>
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

  /// <summary>
  /// Limpia preguntas activas y contadores.
  /// También reinicia datos de vueltas.
  /// </summary>
  public void ResetActiveQuestions()
  {
    activeQuestions.Clear();
    portalRequestCounts.Clear();

    lapsByPlayer.Clear();
    preportalPassedThisLap.Clear();
    raceFinished = false;

    Debug.Log("[QuizManager1] Datos de preguntas y vueltas reseteados.");
  }

  //                    FIN DE CARRERA

  /// <summary>
  /// Método público llamado por MetaTrigger1 para finalizar carrera
  /// (si decides usarlo al llegar a X vueltas).
  /// </summary>
  public void FinishRace(PlayerIndex winner)
  {
    Debug.Log($"[QuizManager1] Carrera terminada. Ganador: {winner}");
    NotifyRaceFinished(winner);
  }

  //                    RESPUESTA DEL JUGADOR

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

    // Si quisieras limpiar la pregunta del portal después de responder:
    // activeQuestions.Remove(portalId);
    // NotifyQuizFinished(); // Cuidado: esto podría borrar el HUD del otro jugador si no se filtra
  }

  //              NUEVO: VUELTAS + PREPORTAL

  /// <summary>
  /// Llamado desde el PreportalTrigger cuando un coche pasa por un preportal.
  /// Marca que ESTE jugador tiene un preportal válido en esta vuelta.
  /// </summary>
  public void NotifyPreportalPassed(PlayerIndex player)
  {
    if (raceFinished) return;

    EnsurePlayerInit(player);
    preportalPassedThisLap.Add(player);

    Debug.Log($"[QuizManager1] Player {player} pasó por preportal en esta vuelta.");
  }

  /// <summary>
  /// Llamado desde MetaTrigger cuando un coche cruza la meta.
  /// Suma vuelta SOLO si pasó por un preportal en esta vuelta.
  /// Muestra:
  /// - \"Vuelta 1\" tras la 1ra vuelta válida
  /// - \"Vuelta 2\" tras la 2da vuelta válida
  /// - \"¡Última vuelta!\" al llegar a la 3ra vuelta válida (solo el primero)
  /// </summary>
  public void NotifyMetaPassed(PlayerIndex player, CarController car)
{
    if (raceFinished)
        return;

    EnsurePlayerInit(player);

    // Verificar que pasó por un preportal en esta vuelta
    if (!preportalPassedThisLap.Contains(player))
    {
        Debug.Log($"[QuizManager1] Player {player} cruzó META sin preportal en esta vuelta. NO suma vuelta.");
        return;
    }

    // Consumimos el preportal de esta vuelta
    preportalPassedThisLap.Remove(player);

    // Sumar vuelta válida
    lapsByPlayer[player]++;
    int lap = lapsByPlayer[player];

    Debug.Log($"[QuizManager1] Player {player} completó vuelta válida #{lap}");

    //  MENSAJE EN LA ESQUINA (HUD DEL CARRO) 
    // Queremos algo así para totalLapsToWin = 3:
    //  1ª vuelta válida -> "Vuelta 1"
    //  2ª vuelta válida -> "Última vuelta"
    //  3ª vuelta válida -> "GANADOR" (solo el primero)
    int lastLapBeforeFinish = totalLapsToWin - 1;

    if (lap < lastLapBeforeFinish)
    {
        // Vuelta 1, 2, ... (antes de la última)
        car.SetQuestion($"Vuelta {lap}");
    }
    else if (lap == lastLapBeforeFinish)
    {
        // Vuelta inmediatamente anterior a la final
        car.SetQuestion("¡Última vuelta!");
    }

    // FIN DE CARRERA 
    if (lap >= totalLapsToWin && !raceFinished)
    {
        raceFinished = true;

        // Solo ESTE jugador es el ganador: mensaje en SU HUD de esquina
        car.SetQuestion("GANADOR");

        Debug.Log($"[QuizManager1] CARRERA TERMINADA. Ganador: {player}");

        // Frenar a TODOS los autos suavemente
        StartCoroutine(SmoothStopAllCars());

        FinishRace(player);
    }
}

  private IEnumerator SmoothStopAllCars()
  {
    Debug.Log("[QuizManager1] Frenando todos los autos...");

    CarController[] cars = FindObjectsOfType<CarController>();

    float duration = 3f;   // tiempo de frenado suave
    float timer = 0f;

    while (timer < duration)
    {
        foreach (var c in cars)
        {
            // Frenado progresivo aplicando fricción
            c.ApplyAcceleration(0f);
            c.ApplyBraking();
        }

        timer += Time.deltaTime;
        yield return null;
    }

    // Al finalizar, asegurar que no sigan acelerando
    foreach (var c in cars)
    {
        c.ApplyAcceleration(0f);
    }
  }
}