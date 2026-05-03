using EventSystem;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace CoreSystem
{
    public class GameManager : NonPersistentSingleton<GameManager>
    {

        [field: Header("Game Info")]
        [field: SerializeField] public PlayerInputActions InputActions { get; private set; }
        [field: SerializeField] public EntitySpawner EntitySpawner { get; private set; }
        [field: SerializeField] public LevelSetupHandler LevelSetupHandler { get; private set; }
        [field: SerializeField] public GameState CurrentGameState { get; set; }
        [field: SerializeField] public LevelInfo MainMenuInfo { get; set; }
        [field: SerializeField] public LevelInfo GameLevelInfo { get; set; }
        [field: SerializeField] public int MaxLives { get; private set; } = 4;

        [field: Header("Current Level Info")]
        [field: SerializeField] public int CurrentLevel { get; private set; } = 1;
        [field: SerializeField] public LevelState CurrentLevelState { get; set; }
        [field: SerializeField] public LevelContext CurrentLevelContext { get; set; }

        [field: SerializeField, Tooltip("[0] Blinky, [1] Inky, [2] Pinky, [3] Clive")] public PlayerManager Pacman { get; private set; }
        [field: SerializeField, Tooltip("[0] Blinky, [1] Inky, [2] Pinky, [3] Clive")] public GhostManager[] Ghosts { get; private set; }

        [field: Space]
        [field: SerializeReference] public int Highscore { get; private set; }
        [field: SerializeReference] public int CurrentScore { get; private set; }
        [field: SerializeReference] public int RemainingLives { get; private set; }
        [field: SerializeField] public int TotalPelletCount { get; set; } = 246;
        [field: SerializeField] public int PelletsEaten { get; private set; } = 0;
        [field: SerializeField] public float TimeSinceLastItemCollected { get; private set; } = 0;

        [field: Header("Game Events")]
        [field: SerializeField] public IntEventChannel OnScoreUpdated { get; private set; }
        [field: SerializeField] public EventChannel OnCollectItem { get; private set; }
        [field: SerializeField] public GameStateEventChannel OnGameStateUpdated { get; private set; }
        [field: SerializeField] public LevelStateEventChannel OnLevelStateUpdated { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InputActions = new PlayerInputActions();
            EntitySpawner = GetComponentInChildren<EntitySpawner>();
            LevelSetupHandler = GetComponentInChildren<LevelSetupHandler>();
        }

        void Start()
        {
            InitialiseMenu();
        }

        private void OnEnable()
        {
            InputActions.Enable();
            InputActions.Pacman.Pause.performed += OnPausedPerformed;
        }

        private void OnDisable()
        {
            InputActions.Disable();
            InputActions.Pacman.Pause.performed -= OnPausedPerformed;
        }

        private void Update()
        {
            if (!CurrentGameState.Equals(GameState.Playing)) return;

            GetNextGhostForEarlyExit();
        }

        public async void InitialiseMenu()
        {
            CurrentLevelContext = new LevelContext
            {
                MapName = MapName.Menu,
                RemainingLives = 0,
                LevelNumber = 0,
            };
            await SetupScene(MainMenuInfo, CurrentLevelContext);
        }

        private void ResetVariables()
        {
            Ghosts = new GhostManager[4];
            RemainingLives = MaxLives;
            CurrentScore = 0;
            Highscore = PlayerPrefs.GetInt("Highscore", 0);
            TimeSinceLastItemCollected = 0f;
            CurrentLevel = 1;
            PelletsEaten = 0;
        }

        public async void InitialiseLevel()
        {
            ResetVariables();
            await Task.Delay(100);

            CurrentLevelContext = new LevelContext
            {
                MapName = MapName.Pacman,
                RemainingLives = RemainingLives,
                LevelNumber = CurrentLevel,
            };

            await SetupScene(GameLevelInfo, CurrentLevelContext);
        }

        public async void RestartLevel()
        {
            ResetVariables();
            await Task.Delay(100);

            await SetupScene(GameLevelInfo, CurrentLevelContext);
        }

        private async Task GetNextLevel()
        {
            EnterLevelState(LevelState.None);
            TimeSinceLastItemCollected = 0f;
            PelletsEaten = 0;
            CurrentLevel++;
            await Task.Delay(100);

            CurrentLevelContext = new LevelContext
            {
                MapName = MapName.Pacman,
                RemainingLives = RemainingLives,
                LevelNumber = CurrentLevel,
            };

            await SetupScene(GameLevelInfo, CurrentLevelContext);
        }

        public async Task SetupScene(LevelInfo levelInfo, LevelContext levelContext)
        {
            CurrentLevelContext = levelContext;
            await LevelSetupHandler.SetupLevel(levelInfo, levelContext);
        }

        public async Task SetupEntities(int levelNumber)
        {
            var (pacman, ghosts) = EntitySpawner.SetupEntities(InputActions, levelNumber);
            Pacman = pacman;
            Ghosts = ghosts;

            await Task.CompletedTask;
        }

        public async Task SetupEntitySpawnpoints(Vector3 pacManSpawnPosition, Quaternion playerOneSpawnRotation, Vector3[] ghostSpawnPosition, Quaternion[] ghostSpawnRotation)
        {
            await EntitySpawner.SetupEntitySpawnpoints(Pacman, pacManSpawnPosition, playerOneSpawnRotation, Ghosts, ghostSpawnPosition, ghostSpawnRotation);
        }

        public void EnterGameState(GameState newState)
        {
            Debug.Log($"Entering Game State: {newState}");
            CurrentGameState = newState;
            OnGameStateUpdated.Invoke(newState);
        }

        public void EnterLevelState(LevelState newState)
        {
            Debug.Log($"Entering Level State: {newState}");
            CurrentLevelState = newState;
            OnLevelStateUpdated.Invoke(newState);
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
                await GetNextLevel();
            }
            else if (PelletsEaten == 64 || PelletsEaten == 174)
            {
                await MazeGenerator.Instance.SpawnFruit(CurrentLevel);
            }
        }

        private void OnPausedPerformed(InputAction.CallbackContext context)
        {
            OnPauseEvent();
        }

        public void OnPauseEvent()
        {
            if (CurrentGameState.Equals(GameState.Playing))
            {
                EnterGameState(GameState.Paused);
            }
            else if (CurrentGameState.Equals(GameState.Paused))
            {
                EnterGameState(GameState.Playing);
            }
        }

        public void OnStartLevel()
        {
            EnterGameState(GameState.Playing);
        }

        public void OnCollectedItem()
        {
            TimeSinceLastItemCollected = 0f;
        }

        public void OnPacManHit()
        {
            EnterLevelState(LevelState.Death);
        }

        public async void OnPacManDeath()
        {
            if (RemainingLives <= 0) return;

            RemainingLives--;
            if (RemainingLives == 0)
            {
                EnterLevelState(LevelState.Death);
                Debug.Log("Game Over!!");
            }
            else
            {
                EnterLevelState(LevelState.Respawning);
                await Task.Delay(3000);
                EnterLevelState(LevelState.Active);
            }
        }

        public void OnGhostHit()
        {
            Debug.Log("Ghost Hit, Stop time for 0.1s");
            StartCoroutine(TimeStop(0.25f));
        }

        private IEnumerator TimeStop(float duration)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }
    }
}