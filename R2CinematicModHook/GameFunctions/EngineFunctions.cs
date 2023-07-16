using R2CinematicModHook.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace R2CinematicModHook.GameFunctions
{
    public class EngineFunctions
    {
        public EngineFunctions()
        {
            Actions = new Dictionary<string, Action>();

            VEngine = new GameFunction<FVEngine>(0x40ADA0, HVEngine);
            GetCurrentLevelName = new GameFunction<FGetCurrentLevelName>(0x404DA0);
            AskToChangeLevel = new GameFunction<FAskToChangeLevel>(0x4054D0);
            Code4PersoLePlusProche = new GameFunction<FCode4PersoLePlusProche>(0x476960);
        }

        public Dictionary<string, Action> Actions { get; }

        #region VEngine

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte FVEngine();

        public GameFunction<FVEngine> VEngine { get; }

        private byte HVEngine()
        {
            byte engine = VEngine.Call();

            try
            {
                lock (Actions)
                {
                    foreach (KeyValuePair<string, Action> action in Actions)
                        action.Value.Invoke();
                }
            }
            catch (Exception e)
            {
                Hook.Interface.HandleError(e);
            }

            return engine;
        }

        #endregion

        #region GetCurrentLevelName

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate string FGetCurrentLevelName();

        public GameFunction<FGetCurrentLevelName> GetCurrentLevelName { get; }

        #endregion

        #region AskToChangeLevel

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FAskToChangeLevel(string levelName, byte save);

        public GameFunction<FAskToChangeLevel> AskToChangeLevel { get; }

        #endregion

        #region Code4PersoLePlusProche

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int FCode4PersoLePlusProche(int a, int b, int c);

        public GameFunction<FCode4PersoLePlusProche> Code4PersoLePlusProche { get; }

        #endregion
    }
}