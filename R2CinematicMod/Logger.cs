using System;
using System.Threading;
using R2CinematicModHook;

namespace R2CinematicMod
{
    public class Logger : RemoteInterface
    {
        public Logger(int scrollbackSize)
        {
            ScrollbackSize = scrollbackSize;
        }

        public int ScrollbackSize { get; set; }
        public bool IsPaused { get; set; }
        public ObservableQueue<string> Messages { get; } = new ObservableQueue<string>();

        public void AddMessage(string message)
        {
            if (IsPaused) return;

            while (Messages.Count > ScrollbackSize) Messages.Dequeue();
            Messages.Enqueue(message);
        }

        public override void Injected(int pid)
        {
            AddMessage($"DLL injected, PID: {pid}");
        }

        public override void Log(string msgPacket)
        {
            AddMessage(msgPacket);
        }

        public override void HandleError(Exception e)
        {
            AddMessage(e.ToString());
        }

        public override void GameClosed()
        {
            // lazy hack to make sure IPC channel is closed
            new Thread(() =>
            {
                Thread.Sleep(500);
                System.Environment.Exit(0);
            }).Start();
        }
    }
}