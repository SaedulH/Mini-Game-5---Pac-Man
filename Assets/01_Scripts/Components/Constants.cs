using System;

namespace Utilities
{
    public static class Constants
    {
        // Score
        public const int PELLET_SCORE = 25;
        public const int POWER_PELLET_SCORE = 50;
        public const int FRUIT_SCORE = 100;
        public const int GHOST_SCORE = 200;
        public const int LEVEL_COMPLETE_SCORE = 500;

        // UI Text
        public const string SOLO_RACE_WIN = "YOU WIN!";
        public const string SOLO_RACE_LOSE = "YOU LOSE!";
        public const string SOLO_TIMED_WIN = "AWARD: ";
        public const string SOLO_TIMED_LOSE = "FAILED!";
        public const string VERSUS_WINNER_TEXT = "WINNER: ";
        public const string BEST_LAP_TEXT = "BEST LAP TIME: ";
        public const string RACE_FINISHED = "FINISHED";

        public const float MAX_IDLE_SPEED = 0.5f;
        public const float MIN_THROTTLE = 0.1f;
        public const float MIN_SPEED_FOR_TURN = 5f;
        public const float STEERING_RIGHT_ANGLE = 90f;
        public const float STEERING_LEFT_ANGLE = -90f;
        public const float STEERING_ANIM_LERP_SPEED = 10f;
        public const float EMISSION_MOVE_TOWARDS_RATE = 10f;
        public const float IDLE_EXHAUST_RATE = 1f;
        public const float IDLE_DRIFT_RATE = 0f;

        // Collision
        public const float COLLISION_EFFECT_COOLDOWN_TIME = 0.3f;
        public const float COLLISION_VOLUME_COEFFICIENT = 0.5f;
        public const float COLLISION_DURATION_COEFFICIENT = 0.5f;
        public const float COLLISION_INTENSITY_COEFFICIENT = 0.8f;


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

        // Engine Audio
        public const float DYNAMIC_VOLUME_LERP_SPEED = 5f;
        public const float DYNAMIC_PITCH_LERP_SPEED = 5f;
        public const float THROTTLE_LERP_SPEED = 5f;
        public const float RPM_LERP_SPEED = 0.15f;
        public const float RPM_STEERING_FACTOR_THRESHOLD = 0.65f;


        public static string FormatTime(float lapTime)
        {
            string formattedTime = TimeSpan.FromSeconds(lapTime).ToString(@"mm\:ss\.ff");

            return formattedTime;
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
}