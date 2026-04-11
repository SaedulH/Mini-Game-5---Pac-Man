using System.Threading.Tasks;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UserInterface
{
    [RequireComponent(typeof(UIDocument))]
    public class LoadingScreen : NonPersistentSingleton<LoadingScreen>
    {
        private VisualElement _root;
        private Label _levelNumber;
        private Slider _loadingBar;
        [field: SerializeField, Min(0f)] public float CurrentProgress { get; protected set; }
        [field: SerializeField, Min(0f)] public float MaxProgress { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root = _root.Q<VisualElement>("LoadingScreen");

            _levelNumber = _root.Q<Label>("LevelNumber");
            _loadingBar = _root.Q<Slider>("LoadingBar");
        }

        public async Task SetLevelInfo(LevelContext context)
        {
            if (context.LevelNumber == 0)
            {
                _levelNumber.AddToClassList("hide");
            }
            else
            {
                _levelNumber.RemoveFromClassList("hide");
            }

            CurrentProgress = 0f;
            MaxProgress = context.TotalWeight;

            _loadingBar.value = 0f;

            await Task.CompletedTask;
        }

        public async Task ShowLoadingScreen()
        {
            Debug.Log("Show Loading Screen");

            _root.style.display = DisplayStyle.Flex;
            await Task.Yield();
            _root.RemoveFromClassList("hide");

            await Task.Delay(400);
        }

        public async Task HideLoadingScreen()
        {
            Debug.Log("Hide Loading Screen");

            _root.AddToClassList("hide");
            await Task.Delay(400);
            _root.style.display = DisplayStyle.None;
        }

        public void UpdateLoadingProgress(float weight)
        {
            CurrentProgress += weight;
            _loadingBar.value = (CurrentProgress / MaxProgress) * 100;
        }
    }
}