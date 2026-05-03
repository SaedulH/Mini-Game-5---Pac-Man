using CoreSystem;
using SettingsSystem;
using System.Threading.Tasks;
using UnityEngine;
using UserInterface;
using Utilities;

public class UIManager : NonPersistentSingleton<UIManager>
{
    [field: SerializeField] public UIState CurrentUIState { get; private set; }
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

    protected override void Awake()
    {
        base.Awake();

        UIBackground = GetComponentInChildren<UIBackground>();
        HUDManager = GetComponentInChildren<HUDManager>();
        PauseScreen = GetComponentInChildren<PauseScreen>();
        StartScreen = GetComponentInChildren<StartScreen>();
        SettingsManager = GetComponentInChildren<SettingsManager>();
        ResultsScreen = GetComponentInChildren<ResultsScreen>();
        LoadingScreen = GetComponentInChildren<LoadingScreen>();
    }

    public void OnGameStateUpdated(GameState gameState)
    {
        switch (gameState)
        {
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

    public void OnUIStateChanged(UIState newUIState)
    {
        if (CurrentUIState == newUIState)
            return;

        _previousUIState = CurrentUIState;

        Debug.Log($"UI State changed from [{_previousUIState}] to [{newUIState}]");

        // 1. Hide previous UI
        HideUI(_previousUIState);

        // 2. Update state
        CurrentUIState = newUIState;

        // 3. Show new UI
        ShowUI(newUIState);

        // 4. Background handling
        UIBackground.EnableBackground(newUIState, IsOverlayUI(newUIState));
    }

    private void ShowUI(UIState state)
    {
        switch (state)
        {
            case UIState.Start:
                StartScreen.Show();
                break;

            case UIState.HUD:
                HUDManager.Show();
                break;

            case UIState.Pause:
                PauseScreen.Show();
                break;

            case UIState.Settings:
                SettingsManager.Show();
                break;

            case UIState.Result:
                ResultsScreen.Show();
                break;

            case UIState.Loading:
                LoadingScreen.Show();
                break;
        }
    }

    private void HideUI(UIState state)
    {
        switch (state)
        {
            case UIState.Start:
                StartScreen.Hide();
                break;

            case UIState.HUD:
                HUDManager.Hide();
                break;

            case UIState.Pause:
                PauseScreen.Hide();
                break;

            case UIState.Settings:
                SettingsManager.Hide();
                break;

            case UIState.Result:
                ResultsScreen.Hide();
                break;

            case UIState.Loading:
                LoadingScreen.Hide();
                break;
        }
    }

    private bool IsOverlayUI(UIState uIState)
    {
        return uIState switch
        {
            UIState.Start => StartScreen.IsOverlay,
            UIState.HUD => HUDManager.IsOverlay,
            UIState.Pause => PauseScreen.IsOverlay,
            UIState.Settings => SettingsManager.IsOverlay,
            UIState.Result => ResultsScreen.IsOverlay,
            UIState.Loading => LoadingScreen.IsOverlay,
            _ => false,
        };
    }

    #region HUD Manager
    public async Task SetupHUD(LevelContext context)
    {
        await HUDManager.SetupHUD(context);
    }

    public async Task BeginCountdown(float duration)
    {
        await HUDManager.BeginCountdown(duration);
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
