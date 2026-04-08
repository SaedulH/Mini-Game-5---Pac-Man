using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace CoreSystem
{
    public class GameManager : NonPersistentSingleton<GameManager>
    {
        [field: SerializeField] public GameState CurrentGameState { get; set; }
        [field: SerializeField] public LevelInfo CurrentLevelInfo { get; set; }
        [field: SerializeField] public LevelContext CurrentLevelContext { get; set; }

        [field: SerializeField] public int MaxLives { get; private set; } = 4;
        [field: SerializeReference] public int RemainingLives { get; private set; }
        [field: SerializeReference] public int CurrentScore { get; private set; }
        [field: SerializeReference] public int Highscore { get; private set; }

        [field: SerializeField] public GameObject PacMan { get; private set; }
        [field: SerializeField] public GameObject[] Ghosts { get; private set; }
        [field: SerializeField] public int PelletCount { get; private set; } = 246;

        public GameObject nodeCentre;

        [field: SerializeField] public PlayerInput Input { get; set; }
        public event Action<GameState> OnGameStateChanged;

        void Start()
        {
            RemainingLives = MaxLives;
            //highscoreText.text = PlayerPrefs.GetInt("Highscore").ToString();
            //scoreText.text = "00";

        }

        public void EnterGameState(GameState newState)
        {
            Debug.Log($"Entering Game State: {newState}");
            CurrentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
        }

        public async Task InitialiseScene(LevelInfo levelInfo, LevelContext levelContext)
        {
            CurrentLevelContext = levelContext;
            CurrentLevelInfo = levelInfo;

            await LevelInitialiser.Instance.InitialiseTrack(levelInfo, levelContext);
        }

        public void AddScore(int amount, bool isPellet)
        {
            CurrentScore += amount;
            //scoreText.text = CurrentScore.ToString();
            if (isPellet)
            {
                PelletCount--;
            }
        }
    }
}