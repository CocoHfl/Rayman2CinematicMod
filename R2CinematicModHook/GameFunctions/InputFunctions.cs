using R2CinematicModHook.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace R2CinematicModHook.GameFunctions
{
    public class InputFunctions
    {
        public InputFunctions()
        {
            Actions = new Dictionary<char, Action>();
            KeycodeActions = new Dictionary<KeyCode, Action>();

            VirtualKeyToAscii = new GameFunction<FVirtualKeyToAscii>(0x496110, HVirtualKeyToAscii);
            VReadInput = new GameFunction<FVReadInput>(0x496510, HVReadInput);
        }

        public Dictionary<char, Action> Actions { get; }
        public Dictionary<KeyCode, Action> KeycodeActions { get; }

        public Action<char, KeyCode> ExclusiveInput { get; set; }

        #region VirtualKeyToAscii

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate short FVirtualKeyToAscii(byte ch, int a2);

        public GameFunction<FVirtualKeyToAscii> VirtualKeyToAscii { get; }

        private short HVirtualKeyToAscii(byte ch, int a2)
        {
            short result = VirtualKeyToAscii.Call(ch, a2);

            Hook.Interface.Log($"VirtualKeyToAscii result: {(char)result}, char: {ch}, a2: {a2}");

            // Prevent custom binds from activating on pause screen
            if (Marshal.ReadByte((IntPtr)0x500faa) != 0) return result;

            if (ExclusiveInput == null)
            {
                lock (Actions)
                {
                    lock (KeycodeActions)
                    {
                        if (Actions.TryGetValue((char)result, out Action action) ||
                            KeycodeActions.TryGetValue((KeyCode)ch, out action))
                            action.Invoke();
                    }
                }
            }
            else ExclusiveInput.Invoke((char)result, (KeyCode)ch);

            return result;
        }

        #endregion

        #region VReadInput

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate short FVReadInput(int a1);

        public GameFunction<FVReadInput> VReadInput { get; }

        private short HVReadInput(int a1)
        {
            short result = VReadInput.Call(a1);

            Hook.Interface.Log($"VReadInput Output: {result}, Pointer:");
            Hook.Interface.Log($"0x{Convert.ToString(a1, 16)}");

            return result;
        }

        #endregion

        private int dword50A560;

        public void DisableGameInput()
        {
            if (dword50A560 == 0)
                dword50A560 = Marshal.ReadInt32((IntPtr)0x50A560);

            Marshal.WriteInt32((IntPtr)0x50A560, 0);
        }

        public void EnableGameInput()
        {
            Marshal.WriteInt32((IntPtr)0x50A560, dword50A560);
            dword50A560 = 0;
        }
    }
}