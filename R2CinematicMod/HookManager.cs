using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using EasyHook;
using R2CinematicModHook;

namespace R2CinematicMod
{
    public class HookManager
    {
        public HookManager(string libraryName, RemoteInterface remote, params string[] processNames)
        {
            InjectionLib = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), libraryName);
            ProcessNames = processNames;

            Remote = remote;
        }

        private string _channelName;

        private IpcServerChannel ipc;
        private RemoteInterface Remote { get; }
        private string[] ProcessNames { get; }
        private string InjectionLib { get; }

        public bool IsHookAttached { get; set; }

        public string KeyPointsPath = $"{Directory.GetCurrentDirectory()}\\KeyPoints.xml";

        public void Inject()
        {
            _channelName = null;

            if (ipc == null)
            {
                ipc = RemoteHooking.IpcCreateServer<RemoteInterface>(ref _channelName,
                    WellKnownObjectMode.Singleton, Remote);
            }

            Thread injectionThread = new Thread(() =>
            {
                while (!IsHookAttached)
                {
                    int processId = GetProcessId();

                    if (processId == 0)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    try
                    {
                        RemoteHooking.Inject(processId, InjectionOptions.DoNotRequireStrongName,
                            InjectionLib, InjectionLib, _channelName, KeyPointsPath);

                        IsHookAttached = true;
                    }
                    catch
                    {
                        IsHookAttached = false;
                        Thread.Sleep(5000);
                    }
                }
            });
            injectionThread.IsBackground = true;
            injectionThread.Start();
        }

        private int GetProcessId()
        {
            foreach (string name in ProcessNames)
            {
                Process[] processes = Process.GetProcessesByName(name);

                if (processes.Length > 0)
                    return processes[0].Id;
            }

            return 0;
        }
    }
}