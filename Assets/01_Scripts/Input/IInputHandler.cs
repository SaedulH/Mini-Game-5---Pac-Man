using Utilities;

namespace CoreSystem
{
    public interface IInputHandler
    {
        ControlInput CurrentInput { get; set; }
        bool IsActive { get; set; }
        void SetActiveState(bool isActive);
    }
}