using System;

namespace Utilities
{
    public static class Constants
    {
        public const float BASE_PACMAN_SPEED = 6f;
        public const float BASE_SPEED = 5f;
        public const float LEVEL_SPEED_MULTIPLIER = 0.3f;
        public const float GHOST_RETURN_SPEED = 12f;

        // Score
        public const int PELLET_SCORE = 10;
        public const int POWER_PELLET_SCORE = 50;

        public const int FRUIT_CHERRY_SCORE = 100;
        public const int FRUIT_STRAWBERRY_SCORE = 300;
        public const int FRUIT_ORANGE_SCORE = 500;
        public const int FRUIT_APPLE_SCORE = 700;
        public const int FRUIT_MELON_SCORE = 1000;
        public const int FRUIT_BELL_SCORE = 2000;
        public const int FRUIT_KEY_SCORE = 4000;

        public const int GHOST_SCORE = 200;
        public const int LEVEL_COMPLETE_SCORE = 1000;

        // Timers
        public const float EARLY_SCATTER_MODE_DURATION = 7f;
        public const float LATE_SCATTER_MODE_DURATION = 5f;
        public const float CHASE_MODE_DURATION = 20f;
        public const float FRIGHTENED_MODE_DURATION = 10f;

        public const float RESPAWN_DELAY_DURATION = 10f;
        public const float FORCE_GHOST_EXIT_PEN_TIME = 5f;

        // UI Text
        public const string SOLO_RACE_WIN = "YOU WIN!";
        public const string SOLO_RACE_LOSE = "YOU LOSE!";
        public const string SOLO_TIMED_WIN = "AWARD: ";
        public const string SOLO_TIMED_LOSE = "FAILED!";
        public const string VERSUS_WINNER_TEXT = "WINNER: ";
        public const string BEST_LAP_TEXT = "BEST LAP TIME: ";
        public const string RACE_FINISHED = "FINISHED";

        // Camera
        public const float DYNAMIC_CAMERA_LOOK_AHEAD_TIME = 0.5f;
        public const float DYNAMIC_CAMERA_LOOK_AHEAD_SMOOTHING = 5f;
        public const float MIN_ORTHOGRAPHIC_CAMERA_SIZE = 70f;
        public const float MAX_ORTHOGRAPHIC_CAMERA_SIZE = 114f;
        public const float ZOOM_FACTOR_CONSTANT = 0.5f;

        // Audio
        public const string MASTER_AUDIO_MIXER = "Master";
        public const string MUSIC_AUDIO_MIXER = "Music";
        public const string UI_AUDIO_MIXER = "UI";
        public const string EFFECTS_AUDIO_MIXER = "Effects";

        public const float AUDIO_MUSIC_FADE_IN_TIME = 0.5f;
        public const float AUDIO_EFFECTS_FADE_IN_TIME = 0.25f;
        public const float AUDIO_EFFECTS_FADE_OUT_TIME = 0.5f;

        public const float DYNAMIC_VOLUME_LERP_SPEED = 5f;
        public const float DYNAMIC_PITCH_LERP_SPEED = 5f;

        public const string PACMAN_ACTION_MAP = "Pacman";
        public const string MENU_ACTION_MAP = "Menu";
        public const string HORIZONTAL_ACTION = "Horizontal";
        public const string VERTICAL_ACTION = "Vertical";

        public static string FormatTime(float lapTime)
        {
            string formattedTime = TimeSpan.FromSeconds(lapTime).ToString(@"mm\:ss\.ff");

            return formattedTime;
        }

        public static int GetItemScore(NodeType itemType)
        {
            return itemType switch
            {
                NodeType.Pellet => PELLET_SCORE,
                NodeType.PowerPellet => POWER_PELLET_SCORE,
                _ => 0
            };
        }

        public static FruitType GetFruitTypeForLevel(int currentLevel)
        {
            return currentLevel switch
            {
                1 => FruitType.Cherry,
                2 => FruitType.Strawberry,
                3 => FruitType.Orange,
                4 => FruitType.Orange,
                5 => FruitType.Apple,
                6 => FruitType.Apple,
                7 => FruitType.Melon,
                8 => FruitType.Melon,
                9 => FruitType.Bell,
                10 => FruitType.Key,
                _ => FruitType.Key
            };
        }

        public static int GetFruitScore(FruitType fruitType)
        {
            return fruitType switch
            {
                FruitType.Cherry => FRUIT_CHERRY_SCORE,
                FruitType.Strawberry => FRUIT_STRAWBERRY_SCORE,
                FruitType.Orange => FRUIT_ORANGE_SCORE,
                FruitType.Apple => FRUIT_APPLE_SCORE,
                FruitType.Melon => FRUIT_MELON_SCORE,
                FruitType.Bell => FRUIT_BELL_SCORE,
                FruitType.Key => FRUIT_KEY_SCORE,
                _ => 0
            };
        }
    }

    public enum GhostType
    {
        Blinky,
        Pinky,
        Inky,
        Clive
    }

    public enum GhostState
    {
        Waiting,
        Returning,
        Chasing,
        Scattering,
        Frightened
    }

    public enum GameState
    {
        Menu,
        Loading,
        Playing,
        Stopped,
        Resetting,
        Paused,
        LevelComplete,
        GameOver
    }

    public enum MenuScreenType
    {
        Selection,
        Settings
    }

    #region Settings

    public enum SettingScreenType
    {
        Game,
        Audio,
        Controls,
        InputPopup
    }

    public enum CameraMode
    {
        Fixed,
        Dynamic
    }

    public enum ScreenShake
    {
        Off,
        Low,
        High
    }

    public enum AudioGroup
    {
        Master,
        Music,
        UI,
        Effects
    }

    #endregion

    public enum ControlInput
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public enum AIState
    {
        Idle,
        Driving,
        Reversing
    }

    public enum MapName
    {
        Menu,
        Pacman,
        MsPacman1,
        MsPacman2,
        MsPacman3,
        MsPacman4,
        Random
    }

    public enum NodeType
    {
        Path,
        Pellet,
        PowerPellet,
        Fruit,
        Teleport,
        PacManStart,
        GhostStart
    }

    public enum FruitType
    {
        Apple,
        Orange,
        Cherry,
        Strawberry,
        Melon,
        Bell,
        Key
    }
}