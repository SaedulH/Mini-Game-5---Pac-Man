using CoreSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class LoadingScreen : UIScript
    {
        private VisualElement _loadingScreen;
        private Label _levelNumber;
        private Slider _loadingBar;
        [field: SerializeField, Min(0f)] public float CurrentProgress { get; protected set; }
        [field: SerializeField, Min(0f)] public float MaxProgress { get; protected set; }
        [field: SerializeField] public float SliderLerpSpeed { get; private set; } = 50f;
        private float _targetProgress;

        protected override void Awake()
        {
            base.Awake();

            _loadingScreen = _root.Q<VisualElement>("LoadingScreen");
            _levelNumber = _loadingScreen.Q<Label>("LevelNumber");
            _loadingBar = _loadingScreen.Q<Slider>("LoadingBar");
        }

        private void Update()
        {
            if (CurrentProgress != _targetProgress)
            {
                CurrentProgress = Mathf.Lerp(CurrentProgress, _targetProgress, SliderLerpSpeed * Time.deltaTime);
                _loadingBar.value = (CurrentProgress / MaxProgress) * 100;

                if (Mathf.Approximately(CurrentProgress, _targetProgress))
                {
                    CurrentProgress = _targetProgress;
                    _loadingBar.value = (CurrentProgress / MaxProgress) * 100;
                }
            }
        }

        public override void Show()
        {
            base.Show();
            if (IsActive) return; 
            _ = ShowLoadingScreen();
        }

        public override void Hide()
        {
            base.Hide();
            if (!IsActive) return;

            _ = HideLoadingScreen();
        }

        public async Task SetLevelInfo(LevelContext context)
        {
            if (context.LevelNumber == 0)
            {
                _levelNumber.AddToClassList("hide");
            }
            else
            {
                _levelNumber.text = $"Level {context.LevelNumber}";
                _levelNumber.RemoveFromClassList("hide");
            }

            _targetProgress = 0f;
            CurrentProgress = 0f;
            MaxProgress = context.TotalWeight;

            _loadingBar.value = 0f;

            await Task.CompletedTask;
        }

        public async Task ShowLoadingScreen()
        {
            Debug.Log("Show Loading Screen");

            _loadingScreen.style.display = DisplayStyle.Flex;
            await Task.Yield();
            _loadingScreen.RemoveFromClassList("hide");
            await Task.Delay(400);
            IsActive = true;
        }

        public async Task HideLoadingScreen()
        {
            Debug.Log("Hide Loading Screen");

            _loadingScreen.AddToClassList("hide");
            await Task.Delay(400);
            _loadingScreen.style.display = DisplayStyle.None;
            IsActive = false;
        }

        public void UpdateLoadingProgress(float weight)
        {
            _targetProgress += weight;
        }
    }
}