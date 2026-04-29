using AudioSystem;
using CoreSystem;
using EventSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class HUDManager : NonPersistentSingleton<HUDManager>
    {
        [field: SerializeField] public VisualElement Root { get; set; }
        [field: SerializeField] public VisualElement HUDElement { get; set; }

        [field: SerializeField] public Label Level { get; set; }
        [field: SerializeField] public Label CurrentScore { get; set; }
        [field: SerializeField] public Label HighScore { get; set; }

        [field: SerializeField] public Image RemainingLivesImage { get; set; }
        [field: SerializeField] public VisualElement CountdownPopup { get; set; }
        [field: SerializeField] public Label CountdownValue { get; set; }

        private bool _isActive = false;
        private int _remainingLives;
        private int _highScore;

        private LevelContext _currentLevelContext;

        [field: SerializeField] public EventChannel StartLevel { get; set; }

        protected override void Awake()
        {
            base.Awake();

            Root = GetComponent<UIDocument>().rootVisualElement;
            HUDElement = Root.Q<VisualElement>("HUD");
            HUDElement.AddToClassList("hide");

            // Timer Elements
            CountdownPopup = HUDElement.Q<VisualElement>("CountdownPopup");
            CountdownPopup.AddToClassList("hide");
            CountdownValue = CountdownPopup.Q<Label>("CountdownValue");

            // Player Stats Elements
            Level = HUDElement.Q<Label>("LevelValue");
            CurrentScore = HUDElement.Q<Label>("ScoreValue");
            HighScore = HUDElement.Q<Label>("HighScoreValue");
            RemainingLivesImage = HUDElement.Q<Image>("LivesValue");
        }

        private void Update()
        {
            if (_isActive) return;
        }

        public async Task SetupHUD(LevelContext levelContext)
        {
            _currentLevelContext = levelContext;
            _remainingLives = levelContext.RemainingLives;
            _highScore = PlayerPrefs.GetInt("Highscore", 0);

            Level.text = _currentLevelContext.LevelNumber.ToString();
            CurrentScore.text = 0.ToString();
            HighScore.text = _highScore.ToString();

            HUDElement.RemoveFromClassList("hide");

            await Task.CompletedTask;
        }

        public void OnGameStateUpdated(GameState gameState)
        {
            _isActive = gameState.Equals(GameState.Playing);
            if (_isActive)
            {
                HUDElement.RemoveFromClassList("hide");
            }
            else
            {
                HUDElement.AddToClassList("hide");
            }
        }

        public void OnUpdateScore(int score)
        {
            CurrentScore.text = score.ToString();
            if (score > _highScore)
            {
                _highScore = score;
                HighScore.text = score.ToString();
            }
        }

        public void OnUpdateLives(int remainingLives)
        {
            _remainingLives = remainingLives;
        }

        #region Timer Management

        public async Task BeginCountdown(float duration)
        {
            CountdownValue.style.fontSize = 120;
            CountdownValue.text = duration.ToString();
            await Task.Delay(250);
            await ShowCountdownPopup();

            await PerformCountdown(duration);

            await Task.Delay(1000);

            await HideCountdownPopup();
        }

        public async Task ShowCountdownPopup()
        {
            CountdownPopup.style.display = DisplayStyle.Flex;
            await Task.Yield();
            CountdownPopup.RemoveFromClassList("hide");

            await Task.Delay(200);
        }

        private async Task PerformCountdown(float duration)
        {
            int secondsRemaining = Mathf.CeilToInt(duration);

            while (secondsRemaining > 0)
            {
                CountdownValue.text = secondsRemaining.ToString();

                CountdownValue.style.fontSize = 160;

                AudioManager.Instance.CreateAudioBuilder()
                    .WithParent(transform)
                    .Play(AudioCollection.Instance.CountdownAudio);

                await Task.Delay(500);
                CountdownValue.style.fontSize = 120;

                await Task.Delay(500);

                secondsRemaining--;
            }

            CountdownValue.text = "GO!";
            AudioManager.Instance.CreateAudioBuilder()
                .WithParent(transform)
                .Play(AudioCollection.Instance.BeginAudio);

            StartLevel.Invoke(new Empty());
        }

        public async Task HideCountdownPopup()
        {
            CountdownPopup.AddToClassList("hide");
            await Task.Delay(200);
            CountdownPopup.style.display = DisplayStyle.None;
        }

        #endregion
    }
}
