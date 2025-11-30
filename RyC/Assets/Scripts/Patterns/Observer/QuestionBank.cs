using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewQuestionBank", menuName = "Quiz/Question Bank 1")]
public class QuestionBank1 : ScriptableObject
{
  public List<Question> questions;

  public Question GetRandomQuestion()
  {
    Debug.Log("[QuestionBank1] Se ha solicitado una pregunta aleatoria.");

    if (questions == null)
    {
      Debug.LogError("[QuestionBank1] ERROR CRÍTICO: La lista 'questions' es NULL.");
      return null;
    }

    if (questions.Count == 0)
    {
      Debug.LogError("[QuestionBank1] ERROR: La lista de preguntas está VACÍA (Count es 0). Agrega preguntas en el Inspector.");
      return null;
    }

    int index = Random.Range(0, questions.Count);
    Question q = questions[index];

    Debug.Log($"[QuestionBank1] Pregunta encontrada: '{q.questionText}' (Índice: {index})");
    return q;
  }
}