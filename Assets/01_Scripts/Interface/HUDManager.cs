using AudioSystem;
using CoreSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

[RequireComponent(typeof(UIDocument))]
public class HUDManager : NonPersistentSingleton<HUDManager>
{
    [field: SerializeField] public VisualElement Root { get; set; }
    [field: SerializeField] public VisualElement HUDElement { get; set; }
    [field: SerializeField] public Label CurrentStage { get; set; }
    [field: SerializeField] public Label CurrentScore { get; set; }
    [field: SerializeField] public Label HighScore { get; set; }
    [field: SerializeField] public Label RemainingLivesCount { get; set; }
    [field: SerializeField] public Image RemainingLivesImage { get; set; }
    [field: SerializeField] public VisualElement CountdownPopup { get; set; }
    [field: SerializeField] public Label CountdownValue { get; set; }

    private int _remainingLives = 0;
    private float _currentScore = 0f;
    private float _highScore = 0f;

    private LevelContext _currentLevelContext;

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
        CurrentScore = HUDElement.Q<Label>("CurrentScore");
        HighScore = HUDElement.Q<Label>("HighScore");
        RemainingLivesCount = HUDElement.Q<Label>("RemainingLivesCount");
        RemainingLivesImage = HUDElement.Q<Image>("RemainingLivesImage");
    }

    private void Update()
    {
        if (_currentLevelContext == null) return;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    public async Task SetupHUD(LevelContext LevelContext)
    {
        _currentLevelContext = LevelContext;
        CurrentStage.text = $"Stage {_currentLevelContext.StageNumber + 1}";
        CurrentScore.text = "0";
        HighScore.text = PlayerPrefs.GetInt("Highscore", 0).ToString();
        RemainingLivesCount.text = GameManager.Instance.RemainingLives.ToString();
        HUDElement.RemoveFromClassList("hide");
        await Task.CompletedTask;
    }

    private void OnGameStateChanged(GameState state)
    {
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

        //GameManager.Instance.StartRace();
    }

    public async Task HideCountdownPopup()
    {
        CountdownPopup.AddToClassList("hide");
        await Task.Delay(200);
        CountdownPopup.style.display = DisplayStyle.None;
    }

    #endregion
}
