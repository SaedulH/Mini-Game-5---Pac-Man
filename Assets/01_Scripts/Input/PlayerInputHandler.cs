using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace CoreSystem
{


    public class PlayerInputHandler : MonoBehaviour, IInputHandler
    {
        [field: SerializeField] public PlayerInputActions PlayerInputActions { get; private set; }
        [field: SerializeField] public PlayerInputActions.PacmanActions PlayerInput { get; private set; }

        public ControlInput CachedInput { get; set; }

        public void SetInputActions(PlayerInputActions inputActions)
        {
            this.PlayerInputActions = inputActions;
            this.PlayerInput = inputActions.Pacman;
            SubscribeInputActions();
        }

        private void OnEnable()
        {
            if (PlayerInputActions == null)
            {
                return;
            }

            SubscribeInputActions();
        }

        private void OnDisable()
        {
            if (PlayerInputActions == null)
            {
                return;
            }

            UnsubscibeInputActions();
        }
        private void SubscribeInputActions()
        {
            PlayerInput.Up.performed += OnUpPerformed;
            PlayerInput.Down.performed += OnDownPerformed;
            PlayerInput.Left.performed += OnLeftPerformed;
            PlayerInput.Right.performed += OnRightPerformed;
        }

        private void UnsubscibeInputActions()
        {
            PlayerInput.Up.performed -= OnUpPerformed;
            PlayerInput.Down.performed -= OnDownPerformed;
            PlayerInput.Left.performed -= OnLeftPerformed;
            PlayerInput.Right.performed -= OnRightPerformed;
        }

        private void OnUpPerformed(InputAction.CallbackContext context)
        {
            if (CachedInput == ControlInput.Up) return;
            CachedInput = ControlInput.Up;
        }

        private void OnDownPerformed(InputAction.CallbackContext context)
        {
            if (CachedInput == ControlInput.Down) return;
            CachedInput = ControlInput.Down;
        }

        private void OnLeftPerformed(InputAction.CallbackContext context)
        {
            if (CachedInput == ControlInput.Left) return;
            CachedInput = ControlInput.Left;
        }

        private void OnRightPerformed(InputAction.CallbackContext context)
        {
            if (CachedInput == ControlInput.Right) return;
            CachedInput = ControlInput.Right;
        }
    }
}