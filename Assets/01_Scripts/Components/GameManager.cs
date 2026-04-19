using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [field: SerializeField, Tooltip("[0] Blinky, [1] Inky, [2] Pinky, [3] Clyde")] public GhostManager[] Ghosts { get; private set; }
        [field: SerializeField] public int TotalPelletCount { get; set; } = 246;
        [field: SerializeField] public int PelletsEaten { get; private set; } = 0;
        [field: SerializeField] public int CurrentLevel { get; private set; } = 1;

        public event Action<GameState> OnGameStateChanged;

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

        private async Task InitialiseGame()
        {
            Ghosts = new GhostManager[4];
            RemainingLives = MaxLives;
            CurrentScore = 0;
            Highscore = PlayerPrefs.GetInt("Highscore", 0);
            await Task.Delay(100);

            CurrentLevelContext = new LevelContext
            {
                MapName = MapName.Pacman,
                RemainingLives = RemainingLives,
                LevelNumber = 1,
                PacManSkinIndex = 0,
                BlinkySkinIndex = 0,
                PinkySkinIndex = 0,
                InkySkinIndex = 0,
                ClydeSkinIndex = 0
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
            OnGameStateChanged?.Invoke(newState);
        }

        public async Task AddScore(int amount, bool isPellet)
        {
            CurrentScore += amount;
            //scoreText.text = CurrentScore.ToString();
            if (isPellet)
            {
                await EatPellet();
            }
        }

        public async Task EatPellet()
        {
            PelletsEaten++;
            if (PelletsEaten == TotalPelletCount)
            {
                Debug.Log("All pellets collected! Level Complete!");
                EnterGameState(GameState.GameOver);
            }
            else if (PelletsEaten == 64 || PelletsEaten == 174)
            {
                await MazeGenerator.Instance.SpawnFruit(CurrentLevel);
            }
        }

        public async Task SetupPlayer(int skinIndex)
        {
            PacMan = Instantiate(PacManPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
            PacMan.name = "PacMan";

            PacMan.InitialisePlayer(InputActions, RemainingLives, skinIndex);
            await Task.CompletedTask;
        }

        public async Task SetupGhost(GhostType ghostType, int skinIndex)
        {
            //GhostPrefab
            GhostManager ghost = Instantiate(GhostPrefab, Vector3.zero, Quaternion.identity).GetComponent<GhostManager>();
            ghost.name = ghostType.ToString();

            ghost.InitialiseGhost(ghostType, skinIndex, TotalPelletCount, PacMan);
            SetupGhostReferences(ghost);
            await Task.CompletedTask;
        }

        private void SetupGhostReferences(GhostManager ghost)
        {
            Ghosts = Ghosts ?? new GhostManager[4];
            switch (ghost.GhostType)
            {
                case GhostType.Blinky:
                    Ghosts[0] = ghost;
                    break;
                case GhostType.Inky:
                    Ghosts[1] = ghost;
                    break;
                case GhostType.Pinky:
                    Ghosts[2] = ghost;
                    break;
                case GhostType.Clyde:
                    Ghosts[3] = ghost;
                    break;
            }
        }

        //public async Task ConfigureAI(Vehicle vehicle, string difficulty)
        //{
        //    PlayerTwo = Instantiate(AIPrefab, Vector3.zero, Quaternion.identity);
        //    PlayerTwo.name = "CPU";
        //    AIHandler input = PlayerTwo.GetOrAdd<AIHandler>();
        //    if (Enum.TryParse(difficulty, out Difficulty parsedDifficulty))
        //    {
        //        input.Initialise(AIStats[(int)parsedDifficulty]);
        //    }
        //    else
        //    {
        //        input.Initialise(AIStats[0]);
        //    }

        //    VehicleSpriteHandler spriteHandler = PlayerTwo.GetOrAdd<VehicleSpriteHandler>();
        //    spriteHandler.AssignSprite(vehicle.VisualSettings, vehicle.PlayerTwoVehicleChassisSprite);

        //    Movement movement = PlayerTwo.GetOrAdd<Movement>();
        //    movement.AssignVehicleStats(vehicle.Stats);

        //    AddToCameraTargetGroup(2, PlayerTwo);
        //    AddToCheckpointTargetGroup(2, PlayerTwo);
        //    _playerTwoCompletedRace = false;
        //    await Task.CompletedTask;
        //}
    }
}