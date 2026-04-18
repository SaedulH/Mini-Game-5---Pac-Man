using System;

namespace Utilities
{
    public static class Constants
    {
        // Score
        public const int PELLET_SCORE = 50;
        public const int POWER_PELLET_SCORE = 250;
        public const int FRUIT_SCORE = 500;
        public const int GHOST_SCORE = 800;
        public const int LEVEL_COMPLETE_SCORE = 1000;

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
                NodeType.Fruit => FRUIT_SCORE,
                _ => 0
            };
        }
    }

    public enum GhostType
    {
        Blinky,
        Pinky,
        Inky,
        Clyde
    }

    public enum GameState
    {
        Menu,
        Loading,
        Playing,
        Paused,
        GameOver
    }

    public enum MenuScreenType
    {
        Selection,
        Settings
    }

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
        GhostStart
    }
}