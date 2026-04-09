//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AudioSystem;
//using UnityEngine;
//using UnityEngine.UIElements;
//using Utilities;

//namespace CoreSystem
//{

//    public class MenuManager : MonoBehaviour
//    {
//        [field: SerializeField] public VisualElement Root { get; set; }
//        // Home Screen
//        [field: SerializeField] public VisualElement HomeScreen { get; set; }
//        [field: SerializeField] public VisualElement SetupScreens { get; set; }
//        [field: SerializeField] public Button PlayButton { get; set; }
//        [field: SerializeField] public Button QuitButton { get; set; }

//        // Settings
//        [field: SerializeField] public VisualElement SettingScreen { get; set; }
//        [field: SerializeField] public Button SettingsButton { get; set; }
//        [field: SerializeField] public Action ShowSettingsAction { get; set; }

//        // Game Selection
//        [field: SerializeField] public VisualElement SelectionScreen { get; set; }
//        [field: SerializeField] public Button TimedModeButton { get; set; }
//        [field: SerializeField] public Button RaceModeButton { get; set; }
//        [field: SerializeField] public Button OnePlayerButton { get; set; }
//        [field: SerializeField] public Button TwoPlayerButton { get; set; }
//        [field: SerializeField] public Button TrackOneButton { get; set; }
//        [field: SerializeField] public Button TrackTwoButton { get; set; }
//        [field: SerializeField] public Button TrackThreeButton { get; set; }
//        [field: SerializeField] public Button StartButton { get; set; }
//        [field: SerializeField] public Button BackButton { get; set; }
//        [field: SerializeField] public VisualElement SelectedMapImage { get; set; }
//        [field: SerializeField] public VisualElement SelectedMapTimedModeElement { get; set; }
//        [field: SerializeField] public Label SelectedMapLapCount { get; set; }
//        [field: SerializeField] public Label SelectedMapGoldTime { get; set; }
//        [field: SerializeField] public Label SelectedMapSilverTime { get; set; }
//        [field: SerializeField] public Label SelectedMapBronzeTime { get; set; }

//        // Vehicle Selection
//        [field: Header("Vehicle Selection")]
//        [field: SerializeField] public VisualElement VehicleScreen { get; set; }
//        [field: SerializeField] public VisualElement VehicleOneImage { get; set; }

//        private int _currentVehicleOneIndex = 0;
//        [field: SerializeField] public VisualElement VehicleTwoImage { get; set; }

//        private int _currentVehicleTwoIndex = 0;

//        [field: SerializeField] public GroupBox PlayerOneVehicleSelection { get; set; }
//        [field: SerializeField] public List<Slider> PlayerOneSliders { get; set; }
//        [field: SerializeField] public Button VehicleOneLeft { get; set; }
//        [field: SerializeField] public Button VehicleOneRight { get; set; }
//        [field: SerializeField] public Label VehicleOneName { get; set; }
//        [field: SerializeField] public GroupBox PlayerTwoVehicleSelection { get; set; }
//        [field: SerializeField] public List<Slider> PlayerTwoSliders { get; set; }
//        [field: SerializeField] public Button VehicleTwoLeft { get; set; }
//        [field: SerializeField] public Button VehicleTwoRight { get; set; }
//        [field: SerializeField] public Label VehicleTwoName { get; set; }

//        [field: Header("Audio")]
//        [field: SerializeField] public AudioData StartAudio { get; set; }
//        [field: SerializeField] public AudioData ForwardAudio { get; set; }
//        [field: SerializeField] public AudioData SelectAudio { get; set; }
//        [field: SerializeField] public AudioData BackAudio { get; set; }
//        [field: SerializeField] public AudioData HoverAudio { get; set; }

//        [field: SerializeField] public MenuScreenType CurrentScreen { get; private set; }

//        private int _playerCount;
//        private Coroutine _setPlayerOneSlidersCoroutine;
//        private Coroutine _setPlayerTwoSlidersCoroutine;

//        [field: SerializeField] public float ScreenTransitionTime { get; private set; } = 0.1f;
//        [field: SerializeField] public float SliderLerpSpeed { get; private set; } = 50f;
//        [field: SerializeField] public List<LevelInfo> LevelInfo { get; private set; }
//        [field: SerializeField] public SettingsManager SettingsManager { get; private set; }

//        private void Awake()
//        {
//            Root = GetComponent<UIDocument>().rootVisualElement;
//        }

//        private void Start()
//        {
//            if (Root == null) return;
//            InitialiseMenu();
//        }

//        private void OnEnable()
//        {
//            if (Root == null) return;

//            VisualElement MainMenu = Root.Q<VisualElement>("MainMenu");
//            // Home Screen
//            HomeScreen = Root.Q<VisualElement>("HomeScreen");
//            SetupScreens = Root.Q<VisualElement>("SetupScreens");

//            PlayButton = MainMenu.Q<Button>("Play");
//            PlayButton.clicked += OnPlayClicked;

//            SettingsButton = MainMenu.Q<Button>("Settings");
//            SettingsButton.clicked += OnSettingsClicked;

//            QuitButton = MainMenu.Q<Button>("Quit");
//            QuitButton.clicked += OnQuitClicked;
//            // Game Selection
//            SelectionScreen = MainMenu.Q<VisualElement>("SelectionScreen");

//            RaceModeButton = SelectionScreen.Q<Button>("Race");
//            RaceModeButton.clicked += () => OnRaceModeClicked();

//            TimedModeButton = SelectionScreen.Q<Button>("Timed");
//            TimedModeButton.clicked += () => OnTimedModeClicked();

//            OnePlayerButton = SelectionScreen.Q<Button>("OnePlayer");
//            OnePlayerButton.clicked += () => OnOnePlayerClicked();

//            TwoPlayerButton = SelectionScreen.Q<Button>("TwoPlayer");
//            TwoPlayerButton.clicked += () => OnTwoPlayerClicked();

//            TrackOneButton = SelectionScreen.Q<Button>("TrackOne");
//            TrackOneButton.clicked += () => OnTrackOneClicked();

//            TrackTwoButton = SelectionScreen.Q<Button>("TrackTwo");
//            TrackTwoButton.clicked += () => OnTrackTwoClicked();

//            TrackThreeButton = SelectionScreen.Q<Button>("TrackThree");
//            TrackThreeButton.clicked += () => OnTrackThreeClicked();

//            SelectedMapImage = SelectionScreen.Q<VisualElement>("MapImage");
//            SelectedMapTimedModeElement = SelectionScreen.Q<VisualElement>("SelectedMapTimedModeElement");
//            SelectedMapLapCount = SelectionScreen.Q<Label>("LapCount");
//            SelectedMapGoldTime = SelectionScreen.Q<Label>("GoldTime");
//            SelectedMapSilverTime = SelectionScreen.Q<Label>("SilverTime");
//            SelectedMapBronzeTime = SelectionScreen.Q<Label>("BronzeTime");

//            StartButton = MainMenu.Q<Button>("Start");
//            StartButton.clicked += OnStartClicked;

//            BackButton = MainMenu.Q<Button>("Back");
//            BackButton.clicked += OnBackClicked;

//            // Vehicle Selection
//            VehicleScreen = MainMenu.Q<VisualElement>("VehicleScreen");
//            PlayerOneVehicleSelection = VehicleScreen.Q<GroupBox>("PlayerOneVehicle");
//            PlayerTwoVehicleSelection = VehicleScreen.Q<GroupBox>("PlayerTwoVehicle");

//            VehicleTwoName = PlayerTwoVehicleSelection.Q<Label>("VehicleTwoName");
//            VehicleTwoImage = PlayerTwoVehicleSelection.Q<VisualElement>("VehicleTwoImage");
//            PlayerTwoSliders = PlayerTwoVehicleSelection.Query<Slider>().ToList();
//            SetupSliders(PlayerTwoSliders, "Two");
//        }

//        private void OnDisable()
//        {
//            if (SettingsManager != null)
//            {
//                SettingsManager.HideSettingsAction += HideSettingsScreen;
//            }

//            PlayButton.clicked -= OnPlayClicked;
//            QuitButton.clicked -= OnQuitClicked;
//            SettingsButton.clicked -= OnSettingsClicked;
//            RaceModeButton.clicked -= () => OnRaceModeClicked();
//            TimedModeButton.clicked -= () => OnTimedModeClicked();
//            OnePlayerButton.clicked -= () => OnOnePlayerClicked();
//            TwoPlayerButton.clicked -= () => OnTwoPlayerClicked();
//            TrackOneButton.clicked -= () => OnTrackOneClicked();
//            TrackTwoButton.clicked -= () => OnTrackTwoClicked();
//            TrackTwoButton.clicked -= () => OnTrackThreeClicked();
//            StartButton.clicked -= OnStartClicked;
//            BackButton.clicked -= OnBackClicked;

//        }

//        public void Initialize(SettingsManager settingsManager)
//        {
//            this.SettingsManager = settingsManager;
//            SettingsManager.HideSettingsAction += HideSettingsScreen;
//        }

//        private void InitialiseMenu()
//        {
//            CurrentScreen = MenuScreenType.Selection;
//            HomeScreen.RemoveFromClassList("hide");
//            SetupScreens.AddToClassList("hide");
//            SelectionScreen.AddToClassList("hide");
//            VehicleScreen.AddToClassList("hide");

//            SetupHoverAudio();
//        }

//        #region Audio 
//        private void SetupHoverAudio()
//        {
//            Root.Query<Button>().ForEach(button =>
//            {
//                bool hovered = false;
//                button.RegisterCallback<PointerEnterEvent>(_ =>
//                {
//                    if (hovered || !button.enabledSelf)
//                        return;

//                    hovered = true;

//                    PlayHoverAudio();
//                });

//                button.RegisterCallback<PointerLeaveEvent>(_ =>
//                {
//                    hovered = false;
//                });
//            });
//        }

//        private void PlayStartAudio(bool playSound = true)
//        {
//            if (!playSound) return;
//            AudioManager.Instance.CreateAudioBuilder()
//                .Play(StartAudio);
//        }

//        private void PlayForwardAudio(bool playSound = true)
//        {
//            if (!playSound) return;
//            AudioManager.Instance.CreateAudioBuilder()
//                .Play(ForwardAudio);
//        }

//        private void PlayBackAudio(bool playSound = true)
//        {
//            if (!playSound) return;
//            AudioManager.Instance.CreateAudioBuilder()
//                .Play(BackAudio);
//        }

//        private void PlaySelectAudio(bool playSound = true)
//        {
//            if (!playSound) return;
//            AudioManager.Instance.CreateAudioBuilder()
//                .Play(SelectAudio);
//        }

//        private void PlayHoverAudio(bool playSound = true)
//        {
//            if (!playSound) return;
//            AudioManager.Instance.CreateAudioBuilder()
//                .Play(HoverAudio);
//        }
//        #endregion

//        #region Screen Transitions
//        private IEnumerator ShowHomeScreen()
//        {
//            CurrentScreen = MenuScreenType.Home;
//            SelectionScreen.AddToClassList("hide");
//            yield return new WaitForSeconds(ScreenTransitionTime);
//            SetupScreens.AddToClassList("hide");
//            HomeScreen.RemoveFromClassList("hide");
//        }

//        private async void HideSettingsScreen()
//        {
//            HomeScreen.style.display = DisplayStyle.Flex;
//            await Task.Yield();

//            StartCoroutine(ShowHomeScreen());

//            await Task.Delay(200);
//        }

//        private async void ShowSettingsScreen()
//        {
//            CurrentScreen = MenuScreenType.Settings;
//            HomeScreen.AddToClassList("hide");

//            await Task.Delay((int)ScreenTransitionTime);
//            HomeScreen.style.display = DisplayStyle.None;
//            ShowSettingsAction?.Invoke();
//        }

//        private IEnumerator ShowSelectionScreen()
//        {
//            if (CurrentScreen == MenuScreenType.Vehicle)
//            {
//                VehicleScreen.AddToClassList("hide");
//            }
//            else
//            {
//                HomeScreen.AddToClassList("hide");
//            }
//            CurrentScreen = MenuScreenType.Selection;
//            yield return new WaitForSeconds(ScreenTransitionTime);
//            StartButton.text = "Next";
//            SetupScreens.RemoveFromClassList("hide");
//            SelectionScreen.RemoveFromClassList("hide");
//        }

//        private IEnumerator ShowVehicleScreen()
//        {
//            CurrentScreen = MenuScreenType.Vehicle;
//            SelectionScreen.AddToClassList("hide");
//            yield return new WaitForSeconds(ScreenTransitionTime);
//            StartButton.text = "Start";
//            VehicleScreen.RemoveFromClassList("hide");
//        }

//        private void OnPlayClicked()
//        {
//            //Debug.Log("Go to Selection Screen");
//            PlayForwardAudio();
//            ResetSelections();
//            ResetVehicleSelections();
//            StartCoroutine(ShowSelectionScreen());
//        }

//        private void OnSettingsClicked()
//        {
//            //Debug.Log("Go to Selection Screen");
//            PlayForwardAudio();
//            ShowSettingsScreen();
//        }

//        public void OnQuitClicked()
//        {
//            PlayBackAudio();
//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;
//#endif
//            Application.Quit();
//        }

//        public void OnStartClicked()
//        {
//            if (CurrentScreen == MenuScreenType.Selection)
//            {
//                PlayForwardAudio();
//                StartCoroutine(ShowVehicleScreen());
//            }
//            else if (CurrentScreen == MenuScreenType.Vehicle)
//            {
//                PlayStartAudio();
//                OnStartGame();
//            }
//        }

//        public void OnBackClicked()
//        {
//            PlayBackAudio();
//            if (CurrentScreen == MenuScreenType.Vehicle)
//            {
//                StartCoroutine(ShowSelectionScreen());
//            }
//            else if (CurrentScreen == MenuScreenType.Selection)
//            {
//                //Debug.Log("Back to Home Screen");
//                StartCoroutine(ShowHomeScreen());
//            }
//        }

//        #endregion

//        #region Race Selection Handlers

//        public void OnRaceModeClicked(bool playSound = true, bool updateMapInfo = true)
//        {
//            PlaySelectAudio(playSound);
//            //Debug.Log($"Mode: Race");
//            RaceModeButton.SetEnabled(false);
//            TimedModeButton.SetEnabled(true);

//            _gameMode = GameMode.Race;
//            UpdateMapInfo(updateMapInfo);
//        }

//        public void OnTimedModeClicked(bool playSound = true, bool updateMapInfo = true)
//        {
//            PlaySelectAudio(playSound);
//            //Debug.Log($"Mode: Timed");
//            RaceModeButton.SetEnabled(true);
//            TimedModeButton.SetEnabled(false);

//            _gameMode = GameMode.Timed;
//            UpdateMapInfo(updateMapInfo);
//        }

//        public void OnOnePlayerClicked(bool playSound = true, bool updateMapInfo = true)
//        {
//            PlaySelectAudio(playSound);
//            //Debug.Log($"PlayerCount: 1");

//            PlayerTwoVehicleSelection.SetEnabled(false);

//            OnePlayerButton.SetEnabled(false);
//            TwoPlayerButton.SetEnabled(true);

//            _playerCount = 1;
//            UpdateMapInfo(updateMapInfo);
//        }

//        public void OnTwoPlayerClicked(bool playSound = true, bool updateMapInfo = true)
//        {
//            PlaySelectAudio(playSound);
//            //Debug.Log($"PlayerCount: 2");

//            PlayerTwoVehicleSelection.SetEnabled(true);

//            OnePlayerButton.SetEnabled(true);
//            TwoPlayerButton.SetEnabled(false);

//            _playerCount = 2;
//            UpdateMapInfo(updateMapInfo);
//        }

//        #endregion

//        public async void OnStartGame()
//        {
//            LevelInfo levelInfo = LevelInfo[0];

//            int totalWeight = (int)levelInfo.StepOrder.Sum(s => s.Weight);

//            LevelContext LevelContext = new()
//            {
//                StageNumber = 0,
//                TotalWeight = totalWeight
//            };

//            await GameManager.Instance.InitialiseScene(levelInfo, LevelContext);
//        }
//    }
//}