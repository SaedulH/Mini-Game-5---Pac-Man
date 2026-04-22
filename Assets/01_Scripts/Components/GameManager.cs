using EventSystem;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace CoreSystem
{
    public class GameManager : NonPersistentSingleton<GameManager>
    {
        [field: SerializeField] public PlayerInputActions InputActions { get; private set; }
        [field: SerializeField] public GameState CurrentGameState { get; set; }
        [field: SerializeField] public LevelInfo GameLevelInfo { get; set; }
        [field: SerializeField] public LevelContext CurrentLevelContext { get; set; }

        [field: SerializeField] public int MaxLives { get; private set; } = 4;
        [field: SerializeReference] public int RemainingLives { get; private set; }
        [field: SerializeReference] public int CurrentScore { get; private set; }
        [field: SerializeReference] public int Highscore { get; private set; }

        [field: SerializeField] public GameObject PacManPrefab { get; private set; }
        [field: SerializeField] public GameObject GhostPrefab { get; private set; }
        [field: SerializeField] public PlayerManager PacMan { get; private set; }
        [field: SerializeField, Tooltip("[0] Blinky, [1] Inky, [2] Pinky, [3] Clive")] public GhostManager[] Ghosts { get; private set; }
        [field: SerializeField, Tooltip("[0] Blinky, [1] Inky, [2] Pinky, [3] Clive")] public GhostConfig[] GhostConfigs { get; private set; }
        [field: SerializeField] public int TotalPelletCount { get; set; } = 246;
        [field: SerializeField] public float TimeSinceLastItemCollected { get; private set; } = 0;
        [field: SerializeField] public int PelletsEaten { get; private set; } = 0;
        [field: SerializeField] public int CurrentLevel { get; private set; } = 1;

        [field: SerializeField] public IntEventChannel OnScoreUpdated { get; private set; }
        [field: SerializeField] public EventChannel OnCollectItem { get; private set; }
        [field: SerializeField] public GameStateEventChannel OnGameStateUpdated { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InputActions = new PlayerInputActions();
        }

        void Start()
        {
            _ = InitialiseGame();
        }

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

        private void Update()
        {
            if (!CurrentGameState.Equals(GameState.Playing)) return;

            GetNextGhostForEarlyExit();
        }

        private async Task InitialiseGame()
        {
            Ghosts = new GhostManager[4];
            RemainingLives = MaxLives;
            CurrentScore = 0;
            Highscore = PlayerPrefs.GetInt("Highscore", 0);
            TimeSinceLastItemCollected = 0f;
            await Task.Delay(100);

            CurrentLevelContext = new LevelContext
            {
                MapName = MapName.Pacman,
                RemainingLives = RemainingLives,
                LevelNumber = 1,
            };

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
            OnGameStateUpdated.Invoke(newState);
        }

        public async Task AddScore(int amount, bool isPellet)
        {
            CurrentScore += amount;
            OnScoreUpdated.Invoke(CurrentScore);
            //scoreText.text = CurrentScore.ToString();
            if (isPellet)
            {
                OnCollectItem.Invoke(new Empty());
                await EatPellet();
            }
        }

        public async Task EatPellet()
        {
            PelletsEaten++;
            if (PelletsEaten >= TotalPelletCount)
            {
                Debug.Log("All Pellets Collected! Level Complete!");
                EnterGameState(GameState.LevelComplete);
            }
            else if (PelletsEaten == 64 || PelletsEaten == 174)
            {
                await MazeGenerator.Instance.SpawnFruit(CurrentLevel);
            }
        }

        public async Task SetupPlayer(int levelNumber)
        {
            PacMan = Instantiate(PacManPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
            PacMan.name = "PacMan";

            PacMan.InitialisePlayer(InputActions, levelNumber);
            await Task.CompletedTask;
        }

        public async Task SetupGhost(GhostType ghostType, int levelNumber)
        {
            //GhostPrefab
            GhostManager ghost = Instantiate(GhostPrefab, Vector3.zero, Quaternion.identity).GetComponent<GhostManager>();
            ghost.name = ghostType.ToString();
            SetupGhostConfiguration(ghost, ghostType, levelNumber);
            await Task.CompletedTask;
        }

        public async Task AssignTargetTransforms()
        {
            if (Ghosts == null || Ghosts.Length != 4)
            {
                Debug.LogError("Ghost references are not properly set up. Cannot assign target transforms.");
                return;
            }
            Ghosts[0].SetTargets(PacMan.transform, PacMan);
            Ghosts[1].SetTargets(Ghosts[0].transform, PacMan);
            Ghosts[2].SetTargets(PacMan.transform, PacMan);
            Ghosts[3].SetTargets(PacMan.transform, PacMan);

            await Task.CompletedTask;
        }

        private int GetGhostIndex(GhostType type)
        {
            return type switch
            {
                GhostType.Blinky => 0,
                GhostType.Inky => 1,
                GhostType.Pinky => 2,
                GhostType.Clive => 3,
                _ => 0
            };
        }

        private void SetupGhostConfiguration(GhostManager ghost, GhostType type, int levelNumber)
        {
            Ghosts ??= new GhostManager[4];

            int index = GetGhostIndex(type);

            Ghosts[index] = ghost;

            GhostConfig config = GhostConfigs[index];

            var renderer = ghost.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = config.Material;
            }

            ghost.InitialiseGhost(type, config, levelNumber);
        }

        public void SetBlinkyRespawnNode()
        {
            Ghosts[0].InputHandler.RespawnNode = Ghosts[2].StartNode;
        }


        private void GetNextGhostForEarlyExit()
        {
            TimeSinceLastItemCollected += Time.deltaTime;
            if (TimeSinceLastItemCollected >= Constants.FORCE_GHOST_EXIT_PEN_TIME)
            {
                for (int i = 0; i < Ghosts.Length; i++)
                {
                    GhostManager ghost = Ghosts[i];
                    if (ghost == null || ghost.InputHandler == null) continue;

                    if (ghost.InputHandler.AllowExitPenEarly())
                    {
                        TimeSinceLastItemCollected = 0f;
                        break;
                    }
                }
            }
        }

        public void OnCollectedItem()
        {
            TimeSinceLastItemCollected = 0f;
        }

        public void OnPacManHit()
        {
            EnterGameState(GameState.Stopped);
        }

        public void OnGhostHit()
        {
            Debug.Log("Ghost Hit, Stop time for 0.1s");
        }

        public async void OnDeath()
        {
            RemainingLives--;
            if (RemainingLives == 0)
            {
                EnterGameState(GameState.GameOver);
            }
            else
            {
                EnterGameState(GameState.Resetting);
                await Task.Delay(3000);
                EnterGameState(GameState.Playing);
            }
        }
    }
}