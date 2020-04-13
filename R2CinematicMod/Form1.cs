using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Toe;
using WindowsInput;

namespace R2CinematicMod
{
    public partial class Form1 : Form
    {
        private GlobalKeyboardHook GlobalKeyboardHook { get; }
        public bool CineModEnabled { get; set; }
        public int r2Process { get; set; }
        Thread cinematicThread = null;

        public Form1()
        {
            InitializeComponent();

            r2Process = this.GetRayman2ProcessHandle();

            CineModEnabled = false;

            // Init fov value (default one)
            float fovFloatValue = fovBar.Value / 10f;
            fovValue.Text = "FOV: " + fovFloatValue.ToString();

            enableCineCommands(false);

            // Keyboard hook
            GlobalKeyboardHook = new GlobalKeyboardHook();
            GlobalKeyboardHook.KeyboardPressed += OnKeyPressed;

            // Form closed event
            FormClosed += new FormClosedEventHandler(Form1_FormClosed);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CinematicMod cineMod = new CinematicMod(this, r2Process);

            if (checkBox1.Checked)
            {
                CineModEnabled = true;
                enableCineCommands(true);

                cineMod.ChangeFOV(fovBar.Value / 10f);
                cineMod.EnableCinematicMod();
            }
            else
            {
                CineModEnabled = false;
                enableCineCommands(false);

                cineMod.DisableCinematicMod();
            }
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CinematicMod cineMod = new CinematicMod(this, r2Process);
            cineMod.DisableCinematicMod();
        }

        private void addKey_Click(object sender, EventArgs e)
        {
            float fovFloatValue = fovBar.Value / 10f;

            CinematicMod cineMod = new CinematicMod(this, r2Process);
            cineMod.AddKeyPoint(fovFloatValue);
        }

        private void clearKeys_Click(object sender, EventArgs e)
        {
            CinematicMod cineMod = new CinematicMod(this, r2Process);
            cineMod.ClearKeyPoints();
        }

        private void launchCine_Click(object sender, EventArgs e)
        {
            CinematicMod cineMod = new CinematicMod(this, r2Process);

            float speedValue = speedBar.Value / 100000f;

            enableCineCommands(false);
            stopButton.Enabled = true;
            checkBox1.Enabled = false;

            Action onCompleted = () =>
            {
                Invoke(new Action(() => 
                {
                    stopButton.Enabled = false;
                    checkBox1.Enabled = true;
                    enableCineCommands(true);
                    cineMod.ChangeFOV(fovBar.Value / 10f);
                }));              
            };

            cinematicThread = new Thread(() =>
            {
                try
                {
                    cineMod.LaunchCinematic(speedValue);
                }
                finally
                {
                    onCompleted();
                }
              });

            cinematicThread.Start();
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (CineModEnabled)
            {
                CinematicMod cineMod = new CinematicMod(this, r2Process);

                // Only handle key down
                if (e.KeyboardState != GlobalKeyboardHook.KeyboardState.KeyDown)
                    return;

                // P
                if (e.KeyboardData.VirtualCode == 0x50)
                {
                    cineMod.AddKeyPoint(fovBar.Value / 10f);
                }
                // O
                if (e.KeyboardData.VirtualCode == 0x4F)
                {
                    cineMod.MoveCamera("forward");
                }
                // L
                if (e.KeyboardData.VirtualCode == 0x4C)
                {
                    cineMod.MoveCamera("backward");
                }
                // K
                if (e.KeyboardData.VirtualCode == 0x4B)
                {
                    cineMod.MoveCamera("left");
                }
                // M
                if (e.KeyboardData.VirtualCode == 0x4D)
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
                // I
                if (e.KeyboardData.VirtualCode == 0x49)
                {
                    cineMod.MoveCamera("yawRight");
                }
                // U
                if (e.KeyboardData.VirtualCode == 0x55)
                {
                    cineMod.MoveCamera("yawLeft");
                }
                // Y
                if (e.KeyboardData.VirtualCode == 0x59)
                {
                    cineMod.MoveCamera("pitchUp");
                }
                // H
                if (e.KeyboardData.VirtualCode == 0x48)
                {
                    cineMod.MoveCamera("pitchDown");
                }
                // N
                if (e.KeyboardData.VirtualCode == 0x42)
                {
                    cineMod.MoveCamera("rollClockW");
                }
                // B
                if (e.KeyboardData.VirtualCode == 0x4E)
                {
                    cineMod.MoveCamera("rollAntiClockW");
                }
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float fovFloatValue = fovBar.Value / 10f;
            fovValue.Text = "FOV: " + fovFloatValue.ToString();

            CinematicMod cineMod = new CinematicMod(this, r2Process);
            cineMod.ChangeFOV(fovFloatValue);
        }

        private void setDefaultFOV_Click(object sender, EventArgs e)
        {
            fovBar.Value = 12;
            float fovFloatValue = fovBar.Value / 10f;
            fovValue.Text = "FOV: " + fovFloatValue.ToString();

            CinematicMod cineMod = new CinematicMod(this, r2Process);
            cineMod.ChangeFOV(fovFloatValue);
        }

        public void enableCineCommands(bool choice)
        {
            addKey.Enabled = choice;
            clearKeys.Enabled = choice;
            launchCine.Enabled = choice;
            speedBar.Enabled = choice;
            fovBar.Enabled = choice;
            setDefaultFOV.Enabled = choice;
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

            IntPtr processHandle = Memory.OpenProcess(Memory.PROCESS_WM_READ | Memory.PROCESS_VM_WRITE | Memory.PROCESS_VM_OPERATION, false, process.Id);
            return (int)processHandle;
        }

        public static void NotifyOnProcessExits(Process process, Action action)
        {
            Task.Run(() => process.WaitForExit()).ContinueWith(t => action());
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if(cinematicThread.IsAlive)
            {
                cinematicThread.Abort();

                CinematicMod cineMod = new CinematicMod(this, r2Process);
                stopButton.Enabled = false;
                checkBox1.Enabled = true;
                enableCineCommands(true);
                cineMod.ChangeFOV(fovBar.Value / 10f);
            }
        }
    }
}
