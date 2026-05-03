using AudioSystem;
using CoreSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class ResultsScreen : UIScript
    {
        private VisualElement _resultsScreen;

        private Label _currentLevel;
        private Label _currentScore;
        private Label _highScore;
        private Label _newHighScoreText;

        private Button _restart;
        private Button _settings;
        private Button _quit;

        protected override void Awake()
        {
            base.Awake();

            _resultsScreen = _root.Q<VisualElement>("ResultsScreen");
            _resultsScreen.AddToClassList("hide");

            _currentLevel = _resultsScreen.Q<Label>("CurrentLevel");
            _currentScore = _resultsScreen.Q<Label>("CurrentScore");
            _highScore = _resultsScreen.Q<Label>("HighScore");
            _newHighScoreText = _resultsScreen.Q<Label>("NewHighScoreText");

            _restart = _resultsScreen.Q<Button>("Restart");
            _settings = _resultsScreen.Q<Button>("Settings");
            _quit = _resultsScreen.Q<Button>("Quit");

            _restart.clicked += OnRestartClicked;
            _settings.clicked += OnSettingsClicked;
            _quit.clicked += OnQuitClicked;
        }

        public override void Show()
        {
            base.Show();
            if (IsActive) return;

            _resultsScreen.RemoveFromClassList("hide");
            IsActive = true;
        }

        public override void Hide()
        {
            base.Hide();
            if (!IsActive) return;

            _resultsScreen.AddToClassList("hide");
            IsActive = false;
        }

        public async Task SetResultsInfo(int currentScore, int highScore, int currentLevel)
        {
            _currentLevel.text = currentLevel.ToString();
            _currentScore.text = currentScore.ToString();
            _highScore.text = highScore.ToString();
            _newHighScoreText.text = currentScore > highScore ? "New High Score!" : "";

            await Task.CompletedTask;
        }

        private void OnRestartClicked()
        {
            AudioManager.Instance.CreateAudioBuilder()
                .WithVolume(0.8f)
                .Play(AudioCollection.Instance.StartAudio);
            GameManager.Instance.RestartLevel();
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings clicked");
            AudioCollection.Instance.PlaySelectAudio();
        }

        private void OnQuitClicked()
        {
            GameManager.Instance.InitialiseMenu();
        }
    }
}