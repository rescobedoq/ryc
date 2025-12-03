using UnityEngine;

namespace Patterns.Singleton
{
    // Usa el enum GameMode que ya tienes en MenuManager.cs
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        public GameMode CurrentMode = GameMode.SinglePlayer;
        public string Car1Name = "Default";
        public string Car2Name = "Default";

        private void Awake()
        {
            // Patrón Singleton básico
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetSelection(GameMode mode, string car1Name, string car2Name)
        {
            CurrentMode = mode;
            Car1Name = car1Name;
            Car2Name = car2Name;
        }
    }
}