using R2CinematicModHook.GameFunctions;
using R2CinematicModHook.Types;
using R2CinematicModHook.Utils;

namespace R2CinematicModHook.Mod
{
    public class GameManager
    {
        public EngineFunctions Engine { get; } = new EngineFunctions();
        public GfxFunctions Graphics { get; } = new GfxFunctions();
        public InputFunctions Input { get; } = new InputFunctions();

        public CameraPathRenderer CamPathRenderer { get; }

        public string KeyPointsPath { get; set; }

        public GameManager(string keyPointsPath)
        {
            InitHooks();

            KeyPointsPath = keyPointsPath;

            CamPathRenderer = new CameraPathRenderer(this);
            CamPathRenderer.InitActions();

            Input.KeycodeActions.Set(KeyCode.F5, () => CamPathRenderer.ToggleKeyPoints());
            Input.KeycodeActions.Set(KeyCode.F6, () => CamPathRenderer.ToggleCurve());
        }

        private void InitHooks()
        {
            // Create all hooks here
            Engine.VEngine.CreateHook();
            Input.VirtualKeyToAscii.CreateHook();
        }
    }
}