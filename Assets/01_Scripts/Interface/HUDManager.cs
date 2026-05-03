using AudioSystem;
using CoreSystem;
using EventSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface
{
    public class HUDManager : UIScript
    {
        private VisualElement _hudOverlay;

        private Label _currentLevel;
        private Label _currentScoreLabel;
        private Label _highScoreLabel;

        private Image _remainingLivesImage;
        private VisualElement _countdownPopup;
        private Label _countdownValue;

        private int _remainingLives;
        private int _highScore;

        private LevelContext _currentLevelContext;

        [field: SerializeField] public EventChannel StartLevel { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            _hudOverlay = _root.Q<VisualElement>("HUD");
            _hudOverlay.AddToClassList("hide");

            // Timer Elements
            _countdownPopup = _hudOverlay.Q<VisualElement>("CountdownPopup");
            _countdownPopup.AddToClassList("hide");
            _countdownValue = _countdownPopup.Q<Label>("CountdownValue");

            // Player Stats Elements
            _currentLevel = _hudOverlay.Q<Label>("LevelValue");
            _currentScoreLabel = _hudOverlay.Q<Label>("ScoreValue");
            _highScoreLabel = _hudOverlay.Q<Label>("HighScoreValue");
            _remainingLivesImage = _hudOverlay.Q<Image>("LivesValue");
        }

        private void Update()
        {
            if (!IsActive) return;
        }

        public async Task SetupHUD(LevelContext levelContext)
        {
            _currentLevelContext = levelContext;
            _remainingLives = levelContext.RemainingLives;
            _highScore = PlayerPrefs.GetInt("Highscore", 0);

            _currentLevel.text = _currentLevelContext.LevelNumber.ToString();
            _currentScoreLabel.text = 0.ToString();
            _highScoreLabel.text = _highScore.ToString();

            _hudOverlay.RemoveFromClassList("hide");

            await Task.CompletedTask;
        }

        public override void Show()
        {
            base.Show();
            if (IsActive) return;

            _hudOverlay.RemoveFromClassList("hide");
            IsActive = true;
        }

        public override void Hide()
        {
            base.Hide();
            if (!IsActive) return;

            _hudOverlay.AddToClassList("hide");
            IsActive = false;
        }

        public void OnUpdateScore(int score)
        {
            _currentScoreLabel.text = score.ToString();
            if (score > _highScore)
            {
                _highScore = score;
                _highScoreLabel.text = score.ToString();
            }
        }

        public void OnUpdateLives(int remainingLives)
        {
            _remainingLives = remainingLives;
        }

        public void OnUpdatePowerMode(bool enabled)
        {

        }

        #region Timer Management

        public async Task BeginCountdown(float duration)
        {
            _countdownValue.style.fontSize = 120;
            _countdownValue.text = duration.ToString();
            await Task.Delay(250);
            await ShowCountdownPopup();

            await PerformCountdown(duration);

            await Task.Delay(1000);

            await HideCountdownPopup();
        }

        public async Task ShowCountdownPopup()
        {
            _countdownPopup.style.display = DisplayStyle.Flex;
            await Task.Yield();
            _countdownPopup.RemoveFromClassList("hide");

            await Task.Delay(200);
        }

        private async Task PerformCountdown(float duration)
        {
            int secondsRemaining = Mathf.CeilToInt(duration);

            while (secondsRemaining > 0)
            {
                _countdownValue.text = secondsRemaining.ToString();

                _countdownValue.style.fontSize = 160;

                AudioManager.Instance.CreateAudioBuilder()
                    .WithParent(transform)
                    .Play(AudioCollection.Instance.CountdownAudio);

                await Task.Delay(500);
                _countdownValue.style.fontSize = 120;

                await Task.Delay(500);

                secondsRemaining--;
            }

            _countdownValue.text = "GO!";
            AudioManager.Instance.CreateAudioBuilder()
                .WithParent(transform)
                .Play(AudioCollection.Instance.BeginAudio);

            StartLevel.Invoke(new Empty());
        }

        public async Task HideCountdownPopup()
        {
            _countdownPopup.AddToClassList("hide");
            await Task.Delay(200);
            _countdownPopup.style.display = DisplayStyle.None;
        }

        #endregion
    }
}
