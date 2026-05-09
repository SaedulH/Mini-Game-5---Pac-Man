using CoreSystem;
using SettingsSystem;
using System.Threading.Tasks;
using UnityEngine;
using UserInterface;
using Utilities;

public class UIManager : NonPersistentSingleton<UIManager>
{
    [field: SerializeField] public UIState CurrentUIState { get; private set; }
    [field: SerializeField] public UIScript CurrentUIScript { get; private set; }
    [field: SerializeField] public UIBackground UIBackground { get; private set; }

    [field: Header("Full Screens")]
    [field: SerializeField] public StartScreen StartScreen { get; private set; }
    [field: SerializeField] public SettingsManager SettingsManager { get; private set; }
    [field: SerializeField] public LoadingScreen LoadingScreen { get; private set; }

    [field: Header("Overlay Screens")]
    [field: SerializeField] public HUDManager HUDManager { get; private set; }
    [field: SerializeField] public PauseScreen PauseScreen { get; private set; }
    [field: SerializeField] public ResultsScreen ResultsScreen { get; private set; }

    private UIState _previousUIState;
    private UIScript _previousUIScript;

    protected override void Awake()
    {
        base.Awake();
        InitialiseUIScripts();
    }

    private void InitialiseUIScripts()
    {
        UIBackground = GetComponentInChildren<UIBackground>();
        UIBackground.Initialise(this);

        HUDManager = GetComponentInChildren<HUDManager>();
        HUDManager.Initialise(this);

        PauseScreen = GetComponentInChildren<PauseScreen>();
        PauseScreen.Initialise(this);

        StartScreen = GetComponentInChildren<StartScreen>();
        StartScreen.Initialise(this);

        SettingsManager = GetComponentInChildren<SettingsManager>();
        SettingsManager.Initialise(this);

        ResultsScreen = GetComponentInChildren<ResultsScreen>();
        ResultsScreen.Initialise(this);

        LoadingScreen = GetComponentInChildren<LoadingScreen>();
        LoadingScreen.Initialise(this);
    }

    public void OnGameStateUpdated(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Menu:
                OnUIStateChanged(UIState.Menu);
                break;

            case GameState.Loading:
                OnUIStateChanged(UIState.Loading);
                break;

            case GameState.Playing:
                OnUIStateChanged(UIState.HUD);
                break;

            case GameState.Paused:
                OnUIStateChanged(UIState.Pause);
                break;
        }
    }

    public void OnLevelStateUpdated(LevelState levelState)
    {
        switch (levelState)
        {
            case LevelState.GameOver:
                OnUIStateChanged(UIState.Result);
                break;
            default:
                break;
        }
    }

    public void OnBackPerformed()
    {
        CurrentUIScript.OnBackClicked();
    }

    public void ReturnToPreviousUIState()
    {
        OnUIStateChanged(_previousUIState);
    }

    public void OnUIStateChanged(UIState newUIState)
    {
        if (CurrentUIState == newUIState)
            return;

        _previousUIState = CurrentUIState;
        _previousUIScript = CurrentUIScript;

        Debug.Log($"UI State changed from [{_previousUIState}] to [{newUIState}]");

        // 1. Hide previous UI
        if (_previousUIScript != null)
        {
            _previousUIScript.Hide();
        }

        // 2. Update state
        CurrentUIState = newUIState;
        CurrentUIScript = newUIState switch
        {
            UIState.Menu => StartScreen,
            UIState.HUD => HUDManager,
            UIState.Pause => PauseScreen,
            UIState.Settings => SettingsManager,
            UIState.Result => ResultsScreen,
            UIState.Loading => LoadingScreen,
            _ => null,
        };

        // 3. Show new UI
        if (CurrentUIScript != null)
        {
            CurrentUIScript.Show();
        }

        // 4. Background handling
        UIBackground.EnableBackground(newUIState, CurrentUIScript.IsOverlay);
    }

    #region HUD Manager
    public async Task SetupHUD(LevelContext context)
    {
        await HUDManager.SetupHUD(context);
    }

    public async Task StartCountdown(float duration)
    {
        await HUDManager.StartCountdown(duration);
    }
    #endregion

    #region Loading Screen
    public async Task SetLoadingScreenInfo(LevelContext context)
    {
        await LoadingScreen.SetLevelInfo(context);
    }

    public void UpdateLoadingProgress(float weight)
    {
        LoadingScreen.UpdateLoadingProgress(weight);
    }
    #endregion

}
