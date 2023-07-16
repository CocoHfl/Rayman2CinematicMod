using R2CinematicModHook.Structs;
using R2CinematicModHook.Types;
using System;
using System.Runtime.InteropServices;

namespace R2CinematicModHook.GameFunctions
{
    public class GfxFunctions
    {
        public GfxFunctions()
        {
            VAddParticle = new GameFunction<FVAddParticle>(0x463390, HVAddParticle);
            VCreatePart = new GameFunction<FVCreatePart>(0x4600C0, HVCreatePart);
        }

        #region VAddParticle

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FVAddParticle(uint particleType, IntPtr vector, IntPtr vector2, int texture, float a6);

        public GameFunction<FVAddParticle> VAddParticle { get; }

        private void HVAddParticle(uint particleType, IntPtr vector, IntPtr vector2, int texture, float a6)
        {
            VAddParticle.Call(particleType, vector, vector2, texture, a6);

            try
            {
                Vector3 s1 = Marshal.PtrToStructure<Vector3>(vector);
                Vector3 s2 = Marshal.PtrToStructure<Vector3>(vector2);

                Hook.Interface.Log($"VAddPart, Particle type: {particleType}, texture: 0x{Convert.ToString(texture, 16)}, a6: {a6}");
                Hook.Interface.Log($"Vector1: {s1.X} {s1.Y} {s1.Z}, Vector2: {s2.X} {s2.Y} {s2.Z}");
            }
            catch (Exception e)
            {
                Hook.Interface.HandleError(e);
            }

        }

        #endregion

        #region VCreatePart

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int FVCreatePart(int particleBehavior, IntPtr position, int a3, float a4, float a5, float a6, int texture);

        public GameFunction<FVCreatePart> VCreatePart { get; }

        private int HVCreatePart(int particleBehavior, IntPtr position, int a3, float a4, float a5, float a6, int texture)
        {
            int part = VCreatePart.Call(particleBehavior, position, a3, a4, a5, a6, texture);

            Hook.Interface.Log($"VCreatePart, Particle behavior: {particleBehavior}, texture: 0x{Convert.ToString(texture, 16)}");

            return part;
        }

        #endregion

    }
}