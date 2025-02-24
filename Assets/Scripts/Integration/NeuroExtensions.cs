using NeuroSdk.Actions;

namespace Assets.Scripts.Integration
{
    public static class NeuroExtensions
    {
        public static ActionWindow AddActionIf(this ActionWindow window, INeuroAction action, bool condition)
        {
            if(condition)
                window.AddAction(action);
            return window;
        }
    }
}
