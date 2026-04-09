using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace CoreSystem
{
    public class GameManager : NonPersistentSingleton<GameManager>
    {
        [field: SerializeField] public PlayerInput Input { get; set; }
        [field: SerializeField] public GameState CurrentGameState { get; set; }
        [field: SerializeField] public LevelInfo GameLevelInfo { get; set; }
        [field: SerializeField] public LevelContext CurrentLevelContext { get; set; }

        [field: SerializeField] public int MaxLives { get; private set; } = 4;
        [field: SerializeReference] public int RemainingLives { get; private set; }
        [field: SerializeReference] public int CurrentScore { get; private set; }
        [field: SerializeReference] public int Highscore { get; private set; }

        [field: SerializeField] public GameObject PacManPrefab { get; private set; }
        [field: SerializeField] public GameObject PacMan { get; private set; }
        [field: SerializeField] public GameObject GhostPrefab { get; private set; }
        [field: SerializeField] public GameObject[] Ghosts { get; private set; }
        [field: SerializeField] public int PelletCount { get; private set; } = 246;

        public GameObject nodeCentre;

        public event Action<GameState> OnGameStateChanged;

        void Start()
        {
            _ = InitialiseGame();
        }

        private async Task InitialiseGame()
        {
            RemainingLives = MaxLives;
            CurrentScore = 0;
            Highscore = PlayerPrefs.GetInt("Highscore", 0);
            await Task.Delay(100);

            await SetupScene(GameLevelInfo, CurrentLevelContext);
        }

        public async Task SetupScene(LevelInfo levelInfo, LevelContext levelContext)
        {
            CurrentLevelContext = levelContext;
            await LevelSetupHandler.Instance.SetupLevel(levelInfo, levelContext);
        }

        public void EnterGameState(GameState newState)
        {
            Debug.Log($"Entering Game State: {newState}");
            CurrentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
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

        public async Task SetupPlayer(int skinIndex)
        {
            //PacManPrefab
        }

        public async Task SetupGhost(GhostType ghostType, int skinIndex)
        {
            //GhostPrefab
        }
    }
}