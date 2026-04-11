using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class ResultsScreen : MonoBehaviour
    {
        private VisualElement _root;

        private Label _currentStage;
        private Label _currentScore;
        private Label _highScore;
        private Label _newHighScoreText;

        private Button _retryButton;
        private Button _quitButton;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _currentStage = _root.Q<Label>("CurrentStage");
            _currentScore = _root.Q<Label>("CurrentScore");
            _highScore = _root.Q<Label>("HighScore");
            _newHighScoreText = _root.Q<Label>("NewHighScoreText");

            _retryButton = _root.Q<Button>("RetryButton");
            _quitButton = _root.Q<Button>("QuitButton");

            _retryButton.clicked += OnRetryClicked;
            _quitButton.clicked += OnQuitClicked;
        }

        private void OnRetryClicked()
        {
            throw new NotImplementedException();
        }

        private void OnSettingsClicked()
        {
            throw new NotImplementedException();
        }

        private void OnQuitClicked()
        {
            throw new NotImplementedException();
        }
    }
}