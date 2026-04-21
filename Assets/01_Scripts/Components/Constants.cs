using System;
using UnityEngine;

namespace Utilities
{
    public static class Constants
    {
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

        // Ghost Node Positions
        public static readonly Vector3 BLINKY_START_POSITION = new(0, 0, 0);
        public static readonly Vector3 PINKY_START_POSITION = new(0, 0, 0);
        public static readonly Vector3 INKY_START_POSITION = new(0, 0, 0);
        public static readonly Vector3 CLIVE_START_POSITION = new(0, 0, 0);

        public static readonly Vector3 BLINKY_CORNER_POSITION = new(12.5f, 14.5f, 0);
        public static readonly Vector3 INKY_CORNER_POSITION = new(12.5f, -13.5f, 0);
        public static readonly Vector3 PINKY_CORNER_POSITION = new(-12.5f, 14.5f, 0);
        public static readonly Vector3 CLIVE_CORNER_POSITION = new(-12.5f, -13.5f, 0);

        // Timers
        public const float EARLY_SCATTER_MODE_DURATION = 7f;
        public const float LATE_SCATTER_MODE_DURATION = 5f;
        public const float CHASE_MODE_DURATION = 20f;
        public const float FRIGHTENED_MODE_DURATION = 10f;

        public const float INKY_START_DELAY_DURATION = 5f;
        public const float PINKY_START_DELAY_DURATION = 10f;
        public const float CLIVE_START_DELAY_DURATION = 15f;
        public const float RESPAWN_DELAY_DURATION = 10f;

        public const int BLINKY_START_DELAY_PELLET_COUNT = -1;
        public const int INKY_START_DELAY_PELLET_COUNT = 10;
        public const int PINKY_START_DELAY_PELLET_COUNT = 20;
        public const int CLIVE_START_DELAY_PELLET_COUNT = 40;

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

        public static int GetPelletsToExitPen(GhostType ghostType)
        {
            return ghostType switch
            {
                GhostType.Blinky => BLINKY_START_DELAY_PELLET_COUNT,
                GhostType.Inky => INKY_START_DELAY_PELLET_COUNT,
                GhostType.Pinky => PINKY_START_DELAY_PELLET_COUNT,
                GhostType.Clive => CLIVE_START_DELAY_PELLET_COUNT,
                _ => -1
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