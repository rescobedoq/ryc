using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; }

    [Header("Banco de preguntas para Portal 1 (solo lectura)")]
    public TwoOptionQuestion[] portal1Questions;

    [Header("Banco de preguntas para Portal 2 (solo lectura)")]
    public TwoOptionQuestion[] portal2Questions;

    // Pregunta actualmente activa en cada portal (para esta vuelta)
    private TwoOptionQuestion currentPortal1;
    private TwoOptionQuestion currentPortal2;

    // Display de pregunta (up) por portalId (1 y 2)
    private PortalQuestionDisplay[] portalDisplays = new PortalQuestionDisplay[3]; // índice 1 y 2

    // Displays de respuestas [portalId, 0=Left, 1=Right]
    private PortalAnswerDisplay[,] answerDisplays = new PortalAnswerDisplay[3, 2];

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Cargar bancos de preguntas desde código (QuestionsBank.cs)
        portal1Questions = QuestionsBank.Portal1;
        portal2Questions = QuestionsBank.Portal2;
    }

    // ===== Registro de displays =====

    public void RegisterDisplay(int portalId, PortalQuestionDisplay display)
    {
        if (portalId <= 0 || portalId >= portalDisplays.Length) return;
        portalDisplays[portalId] = display;
    }

    public void RegisterAnswerDisplay(int portalId, bool isLeft, PortalAnswerDisplay display)
    {
        if (portalId <= 0 || portalId >= answerDisplays.GetLength(0)) return;
        int idx = isLeft ? 0 : 1;
        answerDisplays[portalId, idx] = display;
    }

    private PortalQuestionDisplay GetDisplay(int portalId)
    {
        if (portalId <= 0 || portalId >= portalDisplays.Length) return null;
        return portalDisplays[portalId];
    }

    private PortalAnswerDisplay GetAnswerDisplay(int portalId, bool isLeft)
    {
        if (portalId <= 0 || portalId >= answerDisplays.GetLength(0)) return null;
        int idx = isLeft ? 0 : 1;
        return answerDisplays[portalId, idx];
    }

    // ===== Utilidades internas =====
    private TwoOptionQuestion GetRandomQuestion(TwoOptionQuestion[] pool)
    {
        if (pool == null || pool.Length == 0) return null;
        int idx = Random.Range(0, pool.Length);
        return pool[idx];
    }

    // ========= EVENTOS PÚBLICOS LLAMADOS POR LOS TRIGGERS =========

    /// <summary>
    /// Llamado por Preportal1 o Preportal2 cuando el coche entra.
    /// portalId = 1 o 2.
    /// </summary>
    public void OnPreportalEnter(int portalId, CarController car)
    {
        TwoOptionQuestion q = null;

        switch (portalId)
        {
            case 1:
                if (currentPortal1 == null)
                    currentPortal1 = GetRandomQuestion(portal1Questions);
                q = currentPortal1;
                break;

            case 2:
                if (currentPortal2 == null)
                    currentPortal2 = GetRandomQuestion(portal2Questions);
                q = currentPortal2;
                break;
        }

        if (q == null)
        {
            car.ClearQuestion();

            var dispNull = GetDisplay(portalId);
            dispNull?.Clear();

            // limpiar también respuestas por seguridad
            var aLnull = GetAnswerDisplay(portalId, true);
            var aRnull = GetAnswerDisplay(portalId, false);
            aLnull?.Clear();
            aRnull?.Clear();

            Debug.LogWarning($"[QuizManager] No hay preguntas para el portal {portalId}");
            return;
        }

        Debug.Log($"[QuizManager] Portal {portalId} pregunta: {q.question}");

        // ========= MOSTRAR PREGUNTA =========
        car.SetQuestion(q.question); // HUD abajo izq

        var disp = GetDisplay(portalId); // texto "up"
        disp?.SetQuestion(q.question);

        // ========= MOSTRAR RESPUESTAS =========
        var ansLeft = GetAnswerDisplay(portalId, true);
        var ansRight = GetAnswerDisplay(portalId, false);

        ansLeft?.SetAnswer(q.leftAnswer);
        ansRight?.SetAnswer(q.rightAnswer);
    }

    /// <summary>
    /// Llamado cuando el coche pasa por LEFT o RIGHT de un portal.
    /// portalId = 1 o 2, isLeft = true si chocó con LEFT, false si RIGHT.
    /// </summary>
    public void OnPortalAnswer(int portalId, bool isLeft, CarController car)
    {
        TwoOptionQuestion q = null;

        switch (portalId)
        {
            case 1: q = currentPortal1; break;
            case 2: q = currentPortal2; break;
        }

        if (q == null)
        {
            Debug.LogWarning($"[QuizManager] OnPortalAnswer sin pregunta activa para portal {portalId}");
            return; // no había pregunta activa (seguridad)
        }

        bool correct = (isLeft == q.isLeftCorrect);
        Debug.Log($"[QuizManager] Portal {portalId} respuesta {(isLeft ? "LEFT" : "RIGHT")} → {(correct ? "CORRECTA" : "INCORRECTA")}");

        if (correct)
            car.ApplyBoost();
        else
            car.ApplyPenalty();

        // Limpiar HUD y texto 3D del portal
        car.ClearQuestion();
        var disp = GetDisplay(portalId);
        disp?.Clear();

        // Limpiar respuestas
        var ansLeft = GetAnswerDisplay(portalId, true);
        var ansRight = GetAnswerDisplay(portalId, false);
        ansLeft?.Clear();
        ansRight?.Clear();

        // Consumir pregunta hasta la siguiente vuelta
        if (portalId == 1) currentPortal1 = null;
        else if (portalId == 2) currentPortal2 = null;
    }

    /// <summary>
    /// Llamado por el trigger de Meta.
    /// Resetea preguntas para la siguiente vuelta.
    /// </summary>
    public void OnMeta(CarController car)
    {
        car.ClearQuestion();
        currentPortal1 = null;
        currentPortal2 = null;

        // Limpiar textos 3D de pregunta
        var d1 = GetDisplay(1);
        var d2 = GetDisplay(2);
        d1?.Clear();
        d2?.Clear();

        // Limpiar respuestas de ambos portales
        var p1L = GetAnswerDisplay(1, true);
        var p1R = GetAnswerDisplay(1, false);
        var p2L = GetAnswerDisplay(2, true);
        var p2R = GetAnswerDisplay(2, false);

        p1L?.Clear();
        p1R?.Clear();
        p2L?.Clear();
        p2R?.Clear();

        Debug.Log("[QuizManager] Meta alcanzada, preguntas reseteadas");
    }
}
