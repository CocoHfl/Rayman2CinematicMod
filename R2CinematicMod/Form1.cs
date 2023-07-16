using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using R2CinematicModCommon;

namespace R2CinematicMod
{
    public partial class Form1 : Form
    {
        private GlobalKeyboardHook GlobalKeyboardHook { get; }
        public bool CineModEnabled { get; set; }
        public bool CineRunning {  get; set; }
        public int R2Process { get; set; }
        public int R2ProcessId { get; set; }

        Thread CinematicThread = null;

        [DllImport("user32.dll")]
        private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        public Form1()
        {
            InitializeComponent();

            textBox.Text = "Rayman 2 Cinematic Mod - By CoCo\r\n";

            R2Process = GetRayman2ProcessHandle();

            CineModEnabled = false;

            // Init fov value (default one)
            float fovFloatValue = fovBar.Value / 10f;
            fovValue.Text = "FOV: " + fovFloatValue.ToString();

            speedBar.Value = 5;
            cinematicSpeedLabel.Text = "Cinematic speed: " + speedBar.Value.ToString();

            EnableCineCommands(false);

            // Keyboard hook
            GlobalKeyboardHook = new GlobalKeyboardHook();
            GlobalKeyboardHook.KeyboardPressed += OnKeyPressed;

            // Form closed event
            FormClosed += new FormClosedEventHandler(Form1_FormClosed);

            // EasyHook inject
            var hookManager = new HookManager("R2CinematicModHook.dll", new Logger(1000), "Rayman2", "Rayman2.exe", "Rayman2.exe.noshim");
            hookManager.Inject();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CinematicMod cineMod = new CinematicMod(R2Process);

            if (checkBox1.Checked)
            {
                CineModEnabled = true;
                EnableCineCommands(true);

                cineMod.ChangeFOV(fovBar.Value / 10f);
                cineMod.SetRaymanMovementsEnabled(false);
                cineMod.EnableCinematicMod();
            }
            else
            {
                CineModEnabled = false;
                EnableCineCommands(false);

                cineMod.SetRaymanMovementsEnabled(true);
                cineMod.DisableCinematicMod();
            }
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CinematicMod cineMod = new CinematicMod(R2Process);
            cineMod.DisableCinematicMod();
        }

        private void addKey_Click(object sender, EventArgs e)
        {
            float fovFloatValue = fovBar.Value / 10f;

            CinematicMod cineMod = new CinematicMod(R2Process);

            new KeyPointsManager().AddKeyPoint(fovFloatValue, R2Process, cineMod.Off_CameraMatrix, out string message);
            textBox.Text += message;

            undoKey.Enabled = true;
            clearKeys.Enabled = true;
            launchCine.Enabled = true;
        }

        private void undoKey_Click(object sender, EventArgs e)
        {
            var kpManager = new KeyPointsManager();
            kpManager.UndoLastKeyPoint();
            textBox.Text += "Undid last key point\r\n";

            int numKeyPointsLeft = kpManager.KeyPointsCount();

            undoKey.Enabled = numKeyPointsLeft > 0;
            clearKeys.Enabled = numKeyPointsLeft > 0;
            launchCine.Enabled = numKeyPointsLeft > 0;
        }

        private void clearKeys_Click(object sender, EventArgs e)
        {
            new KeyPointsManager().ClearKeyPoints();

            textBox.Text = "Keys cleared! \r\n";

            undoKey.Enabled = false;
            clearKeys.Enabled = false;
            launchCine.Enabled = false;
        }

        private void launchCine_Click(object sender, EventArgs e)
        {
            LaunchCine();
        }

        public void LaunchCine()
        {
            if (new KeyPointsManager().KeyPointsCount() < 1)
            {
                return;
            }

            CinematicMod cineMod = new CinematicMod(R2Process);

            float speedValue = speedBar.Value / 1000f;
            
            EnableCineCommands(false);
            stopButton.Enabled = true;
            checkBox1.Enabled = false;

            Action onCompleted = () =>
            {
                Invoke(new Action(() =>
                {
                    stopButton.Enabled = false;
                    checkBox1.Enabled = true;
                    EnableCineCommands(true);
                    cineMod.ChangeFOV(fovBar.Value / 10f);
                    CineRunning = false;
                    cineMod.SetRaymanMovementsEnabled(false);
                }));
            };

            CinematicThread = new Thread(() =>
            {
                try
                {
                    CineRunning = true;
                    cineMod.SetRaymanMovementsEnabled(true);
                    cineMod.LaunchCinematic(speedValue);
                }
                finally
                {
                    onCompleted();
                }
            });

            CinematicThread.Start();
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            bool isRaymanWindowFocused = GetActiveProcessId() == R2ProcessId;

            if (isRaymanWindowFocused && CineModEnabled && !CineRunning)
            {
                CinematicMod cineMod = new CinematicMod(R2Process);

                // Only handle key down
                if (e.KeyboardState != GlobalKeyboardHook.KeyboardState.KeyDown)
                    return;

                StringBuilder name = new StringBuilder(9);
                GetKeyboardLayoutName(name);

                bool isAzerty = name.ToString() == "0000040C";

                // P
                if (e.KeyboardData.VirtualCode == 0x50)
                {
                    new KeyPointsManager().AddKeyPoint(fovBar.Value / 10f, R2Process, cineMod.Off_CameraMatrix, out string message);
                    textBox.Text += message;

                    undoKey.Enabled = true;
                    clearKeys.Enabled = true;
                    launchCine.Enabled = true;
                }
                // W (azerty => Z)
                if (e.KeyboardData.VirtualCode == (isAzerty ? 0x5A : 0x57))
                {
                    cineMod.MoveCamera("forward");
                }
                // S
                if (e.KeyboardData.VirtualCode == 0x53)
                {
                    cineMod.MoveCamera("backward");
                }
                // A (azerty => Q)
                if (e.KeyboardData.VirtualCode == (isAzerty ? 0x51 : 0x41))
                {
                    cineMod.MoveCamera("left");
                }
                // D
                if (e.KeyboardData.VirtualCode == 0x44)
                {
                    cineMod.MoveCamera("right");
                }
                // PAGE UP
                if (e.KeyboardData.VirtualCode == 0x21)
                {
                    cineMod.MoveCamera("upward");
                }
                // PAGE DOWN
                if (e.KeyboardData.VirtualCode == 0x22)
                {
                    cineMod.MoveCamera("downward");
                }
                // Right arrow
                if (e.KeyboardData.VirtualCode == 0x27)
                {
                    cineMod.MoveCamera("yawRight");
                }
                // Left arrow
                if (e.KeyboardData.VirtualCode == 0x25)
                {
                    cineMod.MoveCamera("yawLeft");
                }
                // Down arrow
                if (e.KeyboardData.VirtualCode == 0x28)
                {
                    cineMod.MoveCamera("pitchUp");
                }
                // Up arrow
                if (e.KeyboardData.VirtualCode == 0x26)
                {
                    cineMod.MoveCamera("pitchDown");
                }
                // Q (azerty => A)
                if (e.KeyboardData.VirtualCode == (isAzerty ? 0x41 : 0x51))
                {
                    cineMod.MoveCamera("rollClockW");
                }
                // E
                if (e.KeyboardData.VirtualCode == 0x45)
                {
                    cineMod.MoveCamera("rollAntiClockW");
                }
                // Enter
                if (e.KeyboardData.VirtualCode == 0x0D)
                {
                    LaunchCine();
                }
            }
        }

        private int GetActiveProcessId()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return p.Id;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float fovFloatValue = fovBar.Value / 10f;
            fovValue.Text = "FOV: " + fovFloatValue.ToString();

            CinematicMod cineMod = new CinematicMod(R2Process);
            cineMod.ChangeFOV(fovFloatValue);
        }

        private void speedBar_Scroll(object sender, EventArgs e)
        {
            cinematicSpeedLabel.Text = "Cinematic speed: " + speedBar.Value.ToString();
        }

        private void setDefaultFOV_Click(object sender, EventArgs e)
        {
            fovBar.Value = 12;
            float fovFloatValue = fovBar.Value / 10f;
            fovValue.Text = "FOV: " + fovFloatValue.ToString();

            CinematicMod cineMod = new CinematicMod(R2Process);
            cineMod.ChangeFOV(fovFloatValue);
        }

        public void EnableCineCommands(bool choice)
        {
            bool KeyPointsFileExists = new KeyPointsManager().KeyPointsDoc.Element("coords").HasElements;

            addKey.Enabled = choice;
            undoKey.Enabled = choice && KeyPointsFileExists;
            clearKeys.Enabled = choice && KeyPointsFileExists;
            launchCine.Enabled = choice && KeyPointsFileExists;
            speedBar.Enabled = choice;
            fovBar.Enabled = choice;
            setDefaultFOV.Enabled = choice;
            resetCam.Enabled = choice;
        }

        public int GetRayman2ProcessHandle()
        {
            Process process;
            if (Process.GetProcessesByName("Rayman2").Length > 0)
            {
                process = Process.GetProcessesByName("Rayman2")[0];
            }
            else if (Process.GetProcessesByName("Rayman2.exe").Length > 0)
            {
                process = Process.GetProcessesByName("Rayman2.exe")[0];
            }
            else if (Process.GetProcessesByName("Rayman2.exe.noshim").Length > 0)
            {
                process = Process.GetProcessesByName("Rayman2.exe.noshim")[0];
            }
            else
            {
                MessageBox.Show("Error opening process handle: Couldn't find process 'Rayman2'. Please make sure Rayman is running or try launching this program with Administrator rights.");
                Environment.Exit(0);
                return -1;
            }

            NotifyOnProcessExits(process, () => Environment.Exit(0));

            R2ProcessId = process.Id;

            IntPtr processHandle = Memory.OpenProcess(Memory.PROCESS_WM_READ | Memory.PROCESS_VM_WRITE | Memory.PROCESS_VM_OPERATION, false, R2ProcessId);

            return (int)processHandle;
        }

        public static void NotifyOnProcessExits(Process process, Action action)
        {
            Task.Run(() => process.WaitForExit()).ContinueWith(t => action());
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if(CinematicThread.IsAlive)
            {
                CinematicThread.Abort();
            }
        }

        private void resetCam_Click(object sender, EventArgs e)
        {
            CinematicMod cineMod = new CinematicMod(R2Process);
            cineMod.ResetCamera();
        }
    }
}
