using EasyHook;
using R2CinematicModHook.Mod;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace R2CinematicModHook
{
    public class Hook : IEntryPoint
    {
        internal static RemoteInterface Interface { get; private set; }
        internal static Dictionary<string, LocalHook> Hooks { get; } = new Dictionary<string, LocalHook>();
        private GameManager Game { get; set; }

        public Hook(RemoteHooking.IContext context, string channelName, string keyPointsPath)
        {
            Interface = RemoteHooking.IpcConnectClient<RemoteInterface>(channelName);
            Interface.Injected(RemoteHooking.GetCurrentProcessId());
        }

        public void Run(RemoteHooking.IContext context, string channelName, string keyPointsPath)
        {
            try
            {
                // Make sure the game is fully loaded before initializing hooks
                while (true)
                {
                    // Engine state: 1 - loading game, 5 - loading level, 9 - loaded
                    if (Marshal.ReadByte((IntPtr)0x500380) > 8)
                        break;
                }

                Game = new GameManager(keyPointsPath);
                RemoteHooking.WakeUpProcess();
            }
            catch (Exception e)
            {
                Interface.HandleError(e);
            }

            while (true) Thread.Sleep(500);
        }

        ~Hook()
        {
            Interface.GameClosed();
        }
    }
}
