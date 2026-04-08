using System.Collections.Generic;
using System.Threading.Tasks;
using AudioSystem;
using CoreSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

public class HUDManager : NonPersistentSingleton<HUDManager>
{
    [field: SerializeField] public VisualElement Root { get; set; }
    [field: SerializeField] public VisualElement HUDElement { get; set; }

    // Timer Elements
    [field: SerializeField] public VisualElement TimerElement { get; set; }
    [field: SerializeField] public VisualElement TimerBackground { get; set; }
    [field: SerializeField] public Label CentralTimer { get; set; }
    [field: SerializeField] public Label MedalText { get; set; }
    [field: SerializeField] public VisualElement CountdownPopup { get; set; }
    [field: SerializeField] public Label CountdownValue { get; set; }

    // Player One HUD Elements
    [field: SerializeField] public VisualElement PlayerOneHUD { get; set; }
    [field: SerializeField] public VisualElement PlayerOneHUDBackground { get; set; }
    [field: SerializeField] public Label PlayerOneName { get; set; }
    [field: SerializeField] public Label PlayerOnePosition { get; set; }
    [field: SerializeField] public Label PlayerOnePositionOrdinal { get; set; }
    [field: SerializeField] public Label PlayerOneLapCount { get; set; }
    [field: SerializeField] public Label PlayerOneLapTimer { get; set; }
    [field: SerializeField] public Label PlayerOneBestLapTime { get; set; }

    // Player Two HUD Elements
    [field: SerializeField] public VisualElement PlayerTwoHUD { get; set; }
    [field: SerializeField] public VisualElement PlayerTwoHUDBackground { get; set; }
    [field: SerializeField] public Label PlayerTwoName { get; set; }
    [field: SerializeField] public Label PlayerTwoPosition { get; set; }
    [field: SerializeField] public Label PlayerTwoPositionOrdinal { get; set; }
    [field: SerializeField] public Label PlayerTwoLapCount { get; set; }
    [field: SerializeField] public Label PlayerTwoLapTimer { get; set; }
    [field: SerializeField] public Label PlayerTwoBestLapTime { get; set; }

    [field: Header("Audio")]
    [field: SerializeField] public AudioData CountdownAudio { get; set; }
    [field: SerializeField] public AudioData GoAudio { get; set; }

    private int _playerOneCurrentPosition = 1;
    private float _playerOneCurrentLapTime = 0f;
    private bool _playerOneCompletedRace = false;

    private int _playerTwoCurrentPosition = 2;
    private float _playerTwoCurrentLapTime = 0f;
    private bool _playerTwoCompletedRace = false;

    private int _totalLapCount = 0;
    private float _totalElapsedTime = 0f;
    private float _bestLapTime = 0f;

    private Medal _currentMedal = Medal.None;
    private List<float> _currentTrackMedalTimes = new();
    private bool _isTimerRunning = false;

    private LevelContext _currentLevelContext;

    protected override void Awake()
    {
        base.Awake();

        Root = GetComponent<UIDocument>().rootVisualElement;
        HUDElement = Root.Q<VisualElement>("HUD");
        HUDElement.AddToClassList("hideUI");

        // Timer Elements
        TimerElement = HUDElement.Q<VisualElement>("TimerElement");
        TimerBackground = HUDElement.Q<VisualElement>("TimerBackground");
        CentralTimer = TimerElement.Q<Label>("Timer");
        MedalText = TimerElement.Q<Label>("MedalText");
        CountdownPopup = HUDElement.Q<VisualElement>("CountdownPopup");
        CountdownPopup.AddToClassList("hideUI");
        CountdownValue = CountdownPopup.Q<Label>("CountdownValue");

        // Player One HUD Elements
        PlayerOneHUD = HUDElement.Q<VisualElement>("PlayerOneHUD");
        PlayerOneHUDBackground = HUDElement.Q<VisualElement>("PlayerOneBackground");
        PlayerOneName = PlayerOneHUD.Q<Label>("PlayerOne");
        PlayerOnePosition = PlayerOneHUD.Q<Label>("PlayerOnePosition");
        PlayerOnePositionOrdinal = PlayerOneHUD.Q<Label>("PlayerOnePositionOrdinal");
        PlayerOneLapCount = PlayerOneHUD.Q<Label>("PlayerOneLapCount");
        PlayerOneLapTimer = PlayerOneHUD.Q<Label>("PlayerOneLapTimer");
        PlayerOneBestLapTime = PlayerOneHUD.Q<Label>("PlayerOneBestLapTime");

        // Player Two HUD Elements
        PlayerTwoHUD = HUDElement.Q<VisualElement>("PlayerTwoHUD");
        PlayerTwoHUDBackground = HUDElement.Q<VisualElement>("PlayerTwoBackground");
        PlayerTwoName = PlayerTwoHUD.Q<Label>("PlayerTwo");
        PlayerTwoPosition = PlayerTwoHUD.Q<Label>("PlayerTwoPosition");
        PlayerTwoPositionOrdinal = PlayerTwoHUD.Q<Label>("PlayerTwoPositionOrdinal");
        PlayerTwoLapCount = PlayerTwoHUD.Q<Label>("PlayerTwoLapCount");
        PlayerTwoLapTimer = PlayerTwoHUD.Q<Label>("PlayerTwoLapTimer");
        PlayerTwoBestLapTime = PlayerTwoHUD.Q<Label>("PlayerTwoBestLapTime");
    }

    private void Update()
    {
        if (_currentLevelContext == null) return;

        if (_isTimerRunning)
        {
            float deltaTime = Time.deltaTime;
            _totalElapsedTime += deltaTime;
            if (_currentLevelContext.PlayerCount == 1 &&
                _currentLevelContext.GameMode == GameMode.Timed)
            {
                UpdateTimer(_totalElapsedTime);
                CheckUpdateCurrentMedal(_totalElapsedTime);
            }

            if (!_playerOneCompletedRace)
            {
                _playerOneCurrentLapTime += deltaTime;
                UpdatePlayerLapTimer(1, _playerOneCurrentLapTime);
            }

            if (!_playerTwoCompletedRace)
            {
                _playerTwoCurrentLapTime += deltaTime;
                UpdatePlayerLapTimer(2, _playerTwoCurrentLapTime);
            }
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    public async Task SetupHUD(LevelContext LevelContext, List<float> medalTimes)
    {
        _currentLevelContext = LevelContext;
        _totalLapCount = LevelContext.LapCount;
        _playerOneCompletedRace = false;
        _playerTwoCompletedRace = false;
        _bestLapTime = 0f;
        PlayerTwoName.text = LevelContext.PlayerCount == 2 ? "Player Two" : "CPU";
        UpdatePlayerLapCount(1);
        UpdatePlayerLapCount(2);
        UpdateBestLapTime(1);
        UpdateBestLapTime(2);
        UpdatePlayerPositions(1);
        SetupTimers(LevelContext, medalTimes);

        HUDElement.RemoveFromClassList("hideUI");
        await Task.CompletedTask;
    }

    private void OnGameStateChanged(GameState state)
    {
        _isTimerRunning = state == GameState.Playing;
    }

    //public RaceCompleteDetails GetResults()
    //{
    //    RaceCompleteDetails details = new RaceCompleteDetails();
    //    details.PlayerCount = _currentLevelContext.PlayerCount;
    //    details.GameMode = _currentLevelContext.GameMode;
    //    details.WinningPlayer = _playerOneCurrentPosition == 1 ? "Player One" : "Player Two";
    //    details.WinningTime = Constants.FormatTime(_bestLapTime);
    //    details.AwardedMedal = _currentMedal;

    //    Debug.Log($"HUDManager GetResults: PlayerCount={details.PlayerCount}, GameMode={details.GameMode}, WinningPlayer={details.WinningPlayer}, WinningTime={details.WinningTime}, AwardedMedal={details.AwardedMedal}");

    //    return details;
    //}

    #region Player Position Management

    public void UpdatePlayerPositions(int playerOnePosition)
    {
        _playerOneCurrentPosition = playerOnePosition;
        _playerTwoCurrentPosition = playerOnePosition == 1 ? 2 : 1;
        SetPlayerPosition(1, _playerOneCurrentPosition);
        SetPlayerPosition(2, _playerTwoCurrentPosition);
    }

    private void SetPlayerPosition(int playerNumber, int position)
    {
        if (playerNumber == 1)
        {
            PlayerOnePosition.text = position.ToString();
            PlayerOnePositionOrdinal.text = GetOrdinal(position);
        }
        else
        {
            PlayerTwoPosition.text = position.ToString();
            PlayerTwoPositionOrdinal.text = GetOrdinal(position);
        }
    }

    private string GetOrdinal(int position)
    {
        return position switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };
    }
    #endregion

    #region Lap Count Management

    public float UpdatePlayerLapCount(int playerNumber, int lapNumber = 0)
    {
        int nextLapNumber = lapNumber + 1;
        float previousLapTime;
        if (playerNumber == 1)
        {
            previousLapTime = _playerOneCurrentLapTime;
            if (nextLapNumber <= _totalLapCount)
            {
                _playerOneCurrentLapTime = 0f;
                PlayerOneLapCount.text = $"{nextLapNumber}/{_totalLapCount}";
            }
            else
            {
                _playerOneCompletedRace = true;
                PlayerOneLapCount.text = Constants.RACE_FINISHED;
            }
        }
        else
        {
            previousLapTime = _playerTwoCurrentLapTime;
            if (nextLapNumber <= _totalLapCount)
            {
                _playerTwoCurrentLapTime = 0f;
                PlayerTwoLapCount.text = $"{nextLapNumber}/{_totalLapCount}";
            }
            else
            {
                _playerTwoCompletedRace = true;
                PlayerTwoLapCount.text = Constants.RACE_FINISHED;
            }
        }

        if (_currentLevelContext.GameMode == GameMode.Timed
            && _currentLevelContext.PlayerCount == 2
            && (previousLapTime < _bestLapTime || _bestLapTime == 0f))
        {
            string playerName = playerNumber == 1 ? PlayerOneName.text : PlayerTwoName.text;
            SetTimeToBeat(previousLapTime, playerName);
            UpdatePlayerPositions(playerNumber);
        }

        if (previousLapTime < _bestLapTime || _bestLapTime == 0f)
        {
            _bestLapTime = previousLapTime;
        }

        return previousLapTime;
    }

    #endregion

    #region Timer Management

    private void SetupTimers(LevelContext LevelContext, List<float> medalTimes)
    {
        _totalElapsedTime = 0f;
        _bestLapTime = 0f;
        _currentTrackMedalTimes = medalTimes;
        PlayerOneLapTimer.RemoveFromClassList("hideUI");
        PlayerOneBestLapTime.RemoveFromClassList("hideUI");
        if (LevelContext.GameMode == GameMode.Race)
        {
            TimerElement.AddToClassList("hideUI");
            ShowPlayerTwoHud(true);
        }
        else if (LevelContext.GameMode == GameMode.Timed)
        {
            UpdatePlayerLapTimer(1, 0f);

            if (LevelContext.PlayerCount == 1)
            {
                TimerBackground.style.width = new StyleLength(new Length(20, LengthUnit.Percent));
                ShowPlayerTwoHud(false);
                UpdateCurrentMedal(Medal.Gold, _currentTrackMedalTimes[0]);
                UpdateTimer(0);
            }
            else
            {
                TimerBackground.style.width = new StyleLength(new Length(30, LengthUnit.Percent));
                ShowPlayerTwoHud(true);
                UpdatePlayerLapTimer(2, 0f);
                SetTimeToBeat(0f, null);
            }
            TimerElement.RemoveFromClassList("hideUI");
        }
    }

    private void ShowPlayerTwoHud(bool show)
    {
        if (show)
        {
            PlayerTwoHUD.RemoveFromClassList("hideUI");
            PlayerTwoHUDBackground.RemoveFromClassList("hideUI");
            PlayerTwoLapTimer.RemoveFromClassList("hideUI");
            PlayerTwoBestLapTime.RemoveFromClassList("hideUI");
        }
        else
        {
            PlayerTwoHUD.AddToClassList("hideUI");
            PlayerTwoHUDBackground.AddToClassList("hideUI");
            PlayerTwoLapTimer.AddToClassList("hideUI");
            PlayerTwoBestLapTime.AddToClassList("hideUI");
        }
    }

    public void CheckUpdateCurrentMedal(float totalElapsedTime)
    {
        if (_currentMedal == Medal.None) return;

        if (_currentMedal == Medal.Gold && totalElapsedTime >= _currentTrackMedalTimes[0])
        {
            UpdateCurrentMedal(Medal.Silver, _currentTrackMedalTimes[1]);
        }
        else if (_currentMedal == Medal.Silver && totalElapsedTime >= _currentTrackMedalTimes[1])
        {
            UpdateCurrentMedal(Medal.Bronze, _currentTrackMedalTimes[2]);
        }
        else if (_currentMedal == Medal.Bronze && totalElapsedTime >= _currentTrackMedalTimes[2])
        {
            UpdateCurrentMedal(Medal.Failed, 0f);
        }
    }

    public void UpdateCurrentMedal(Medal medal, float medalTime)
    {
        _currentMedal = medal;
        SetMedalText(medal, medalTime);
    }

    private void SetMedalText(Medal medal, float medalTime, string playerName = null)
    {
        switch (medal)
        {
            case Medal.Failed:
                MedalText.text = "Failed";
                MedalText.style.color = Color.white;
                break;
            case Medal.Bronze:
                MedalText.text = $"Bronze: {Constants.FormatTime(medalTime)}";
                MedalText.style.color = new Color(0.804f, 0.498f, 0.196f); // Bronze color
                break;
            case Medal.Silver:
                MedalText.text = $"Silver: {Constants.FormatTime(medalTime)}";
                MedalText.style.color = new Color(0.753f, 0.753f, 0.753f); // Silver color
                break;
            case Medal.Gold:
                MedalText.text = $"Gold: {Constants.FormatTime(medalTime)}";
                MedalText.style.color = new Color(1f, 0.843f, 0f); // Gold color
                break;
            default:
                MedalText.text = $"Best Lap: {playerName}";
                MedalText.style.color = Color.white;
                break;
        }
    }

    public void UpdateTimer(float time)
    {
        CentralTimer.text = Constants.FormatTime(time);
    }

    public void SetTimeToBeat(float timeToBeat, string playerName)
    {
        _bestLapTime = timeToBeat;
        SetMedalText(Medal.None, timeToBeat, playerName);
        UpdateTimer(timeToBeat);
    }

    public void UpdatePlayerLapTimer(int playerNumber, float lapTime)
    {
        if (playerNumber == 1)
        {
            PlayerOneLapTimer.text = Constants.FormatTime(lapTime);
        }
        else
        {
            PlayerTwoLapTimer.text = Constants.FormatTime(lapTime);
        }
    }

    public void UpdateBestLapTime(int playerNumber, float lapTime = 0f)
    {
        if (playerNumber == 1)
        {
            PlayerOneBestLapTime.text = Constants.FormatTime(lapTime);
        }
        else
        {
            PlayerTwoBestLapTime.text = Constants.FormatTime(lapTime);
        }
    }

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
        CountdownPopup.RemoveFromClassList("hideUI");

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
                .Play(CountdownAudio);

            await Task.Delay(500);
            CountdownValue.style.fontSize = 120;

            await Task.Delay(500);

            secondsRemaining--;
        }

        CountdownValue.text = "GO!";
        AudioManager.Instance.CreateAudioBuilder()
            .WithParent(transform)
            .Play(GoAudio);

        //GameManager.Instance.StartRace();
    }

    public async Task HideCountdownPopup()
    {
        CountdownPopup.AddToClassList("hideUI");
        await Task.Delay(200);
        CountdownPopup.style.display = DisplayStyle.None;
    }

    #endregion
}
