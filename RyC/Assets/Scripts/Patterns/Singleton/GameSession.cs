using UnityEngine;

namespace Patterns.Singleton
{
    /// <summary>
    /// Singleton para mantener estadísticas y configuración de sesión de juego.
    /// Persiste entre escenas usando DontDestroyOnLoad.
    /// </summary>
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        // Estadísticas de la sesión (no afectan gameplay)
        public int TotalQuestionsAnswered { get; private set; }
        public int TotalCorrectAnswers { get; private set; }
        public int TotalIncorrectAnswers { get; private set; }
        public float SessionStartTime { get; private set; }

        private void Awake()
        {
            // Patrón Singleton: solo una instancia persiste
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SessionStartTime = Time.time;
        }

        // Métodos para registrar estadísticas (uso opcional)
        public void RecordAnswer(bool isCorrect)
        {
            TotalQuestionsAnswered++;
            if (isCorrect)
                TotalCorrectAnswers++;
            else
                TotalIncorrectAnswers++;
        }

        public float GetSessionDuration()
        {
            return Time.time - SessionStartTime;
        }

        public void ResetStats()
        {
            TotalQuestionsAnswered = 0;
            TotalCorrectAnswers = 0;
            TotalIncorrectAnswers = 0;
            SessionStartTime = Time.time;
        }
    }
}