using EasyHook;
using System;
using System.Runtime.InteropServices;

namespace R2CinematicModHook.Types
{
    public class GameFunction<T> where T : Delegate
    {
        public GameFunction(int pointer)
        {
            Name = Guid.NewGuid().ToString();
            Pointer = (IntPtr)pointer;
            Call = Marshal.GetDelegateForFunctionPointer<T>(Pointer);
        }

        public GameFunction(int pointer, T hook) : this(pointer)
        {
            Hook = hook;
        }

        private string Name { get; }
        public IntPtr Pointer { get; }
        public T Call { get; }
        private T Hook { get; }

        public void CreateHook()
        {
            R2CinematicModHook.Hook.Hooks[Name] = LocalHook.Create(Pointer, Hook, this);
            R2CinematicModHook.Hook.Hooks[Name].ThreadACL.SetExclusiveACL(new[] { 0 });
            R2CinematicModHook.Hook.Interface.Log($"Attached hook:\n{typeof(T).FullName}");
        }

        public void DeleteHook()
        {
            R2CinematicModHook.Hook.Hooks[Name].Dispose();
            R2CinematicModHook.Hook.Hooks.Remove(Name);
        }
    }
}