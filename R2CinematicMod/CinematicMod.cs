using System;
using System.Collections.Generic;
using Toe;
using R2CinematicModCommon;

namespace R2CinematicMod
{
    public class CinematicMod
    {
        private int ProcessHandle { get; set; }

        private int BytesReadOrWritten = 0;

        public readonly int Off_DNM_p_stDynamicsCameraMechanics = 0x4359D0;
        public readonly int Off_ForceCameraPos = 0x473420;
        public readonly int Off_ForceCameraTgt = 0x473480;

        public readonly int Off_CameraMatrix;
        public readonly int Off_CameraTarget;
        public readonly int Off_CameraFov;
        public readonly int Off_RaymanMatrix;
        public readonly int Off_RaymanDsgVar16;

        public void EnableCinematicMod()
        {
            var buffer = new byte[] { 0xC3 };
            Matrix matrix;

            Memory.WriteProcessMemory(ProcessHandle, Off_DNM_p_stDynamicsCameraMechanics, buffer, buffer.Length, ref BytesReadOrWritten);
            Memory.WriteProcessMemory(ProcessHandle, Off_ForceCameraPos, buffer, buffer.Length, ref BytesReadOrWritten);
            Memory.WriteProcessMemory(ProcessHandle, Off_ForceCameraTgt, buffer, buffer.Length, ref BytesReadOrWritten);

            matrix = Matrix.Read(ProcessHandle, Off_RaymanMatrix);

            matrix.m = matrix.m.ClearRotation();
            matrix.m.M24 += 15.0f;
            matrix.m.M34 += 2.0f;

            matrix.Write(ProcessHandle, Off_CameraMatrix);
        }

        public void DisableCinematicMod()
        {
            // Restore camera
            var buffer = new byte[] { 0x81 };
            Memory.WriteProcessMemory(ProcessHandle, Off_DNM_p_stDynamicsCameraMechanics, buffer, buffer.Length, ref BytesReadOrWritten);
            buffer = new byte[] { 0x53 };
            Memory.WriteProcessMemory(ProcessHandle, Off_ForceCameraPos, buffer, buffer.Length, ref BytesReadOrWritten);
            buffer = new byte[] { 0x83 };
            Memory.WriteProcessMemory(ProcessHandle, Off_ForceCameraTgt, buffer, buffer.Length, ref BytesReadOrWritten);

            Memory.WriteProcessMemoryFloat(ProcessHandle, Off_CameraFov, 1.2f);
        }

        public void ResetCamera()
        {
            Matrix matrix = Matrix.Read(ProcessHandle, Off_CameraMatrix);

            matrix.m = matrix.m.ClearRotation();
            matrix.Write(ProcessHandle, Off_CameraMatrix);
        }

        public void LaunchCinematic(float speed)
        {
            List<CurvePoint> keyPoints = new KeyPointsManager().ReadKeyPointsFromXML();
            List<CurvePoint> curvePoints = CameraPathGenerator.GenerateCameraPath(keyPoints, 1000);

            RenderCameraPath(curvePoints, speed);
        }

        private void RenderCameraPath(List<CurvePoint> curvePoints, float speed)
        {
            float time = 0f;
            float stepSize = speed / curvePoints.Count;

            Matrix matrix = Matrix.Read(ProcessHandle, Off_CameraMatrix);

            // Render camera movement gradually in while loop
            while (time < 1f)
            {
                int curveIndex = Convert.ToInt32(time * curvePoints.Count);

                if (curveIndex >= curvePoints.Count)
                    break;

                // Set fov
                Memory.WriteProcessMemoryFloat(ProcessHandle, Off_CameraFov, curvePoints[curveIndex].Fov);

                Vector3 rotationOnCurve = curvePoints[curveIndex].Rotation;
                // Set rotation
                matrix.m = matrix.m.ClearRotation();
                matrix.m = matrix.m *
                    Matrix4.CreateRotationZ(rotationOnCurve.Y) *
                    Matrix4.CreateRotationX(rotationOnCurve.X) *
                    Matrix4.CreateRotationY(rotationOnCurve.Z);


                Vector3 pointOnCurve = curvePoints[curveIndex].Position;
                // Set position
                matrix.m.M14 = pointOnCurve.X;
                matrix.m.M24 = pointOnCurve.Y;
                matrix.m.M34 = pointOnCurve.Z;

                // Write matrix
                matrix.Write(ProcessHandle, Off_CameraMatrix);

                time += stepSize;
            }
        }

        public void MoveCamera(string direction)
        {
            Matrix matrix;
            matrix = Matrix.Read(ProcessHandle, Off_CameraMatrix);

            float yaw = 0;
            float pitch = 0;
            float roll = 0;

            switch (direction)
            {
                case "yawRight":
                    yaw = 0.25f;
                    break;
                case "yawLeft":
                    yaw = -0.25f;
                    break;
                case "pitchUp":
                    pitch = 0.25f;
                    break;
                case "pitchDown":
                    pitch = -0.25f;
                    break;
                case "rollClockW":
                    roll = -0.25f;
                    break;
                case "rollAntiClockW":
                    roll = 0.25f;
                    break;
            }

            matrix.m = matrix.m * Matrix4.CreateRotationZ(yaw) * Matrix4.CreateRotationX(pitch) * Matrix4.CreateRotationY(roll);

            Quaternion rotation = matrix.m.ExtractRotation();
            Vector3 eulers = Matrix.QuaternionToEuler(rotation); //in radians

            switch (direction)
            {
                case "forward":
                    matrix.m.M24 -= (float)Math.Cos(eulers.Z);
                    matrix.m.M14 -= (float)Math.Sin(eulers.Z);
                    matrix.m.M34 += (float)Math.Sin(eulers.X);
                    break;
                case "backward":
                    matrix.m.M24 += (float)Math.Cos(eulers.Z);
                    matrix.m.M14 += (float)Math.Sin(eulers.Z);
                    matrix.m.M34 -= (float)Math.Sin(eulers.X);
                    break;
                case "left":
                    matrix.m.M24 += (float)Math.Cos(eulers.Z + Math.PI / 2);
                    matrix.m.M14 += (float)Math.Sin(eulers.Z + Math.PI / 2);
                    break;
                case "right":
                    matrix.m.M24 += (float)Math.Cos(eulers.Z - Math.PI / 2);
                    matrix.m.M14 += (float)Math.Sin(eulers.Z - Math.PI / 2);
                    break;
                case "upward":
                    matrix.m.M34 += 0.5f;
                    break;
                case "downward":
                    matrix.m.M34 += -0.5f;
                    break;
            }

            matrix.Write(ProcessHandle, Off_CameraMatrix);
        }

        public void ChangeFOV(float fov)
        {
            Memory.WriteProcessMemoryFloat(ProcessHandle, Off_CameraFov, fov);
        }

        public void SetRaymanMovementsEnabled(bool enable)
        {
            Memory.WriteProcessMemoryByte(ProcessHandle, Off_RaymanDsgVar16, Convert.ToByte(enable ? 1 : 0));
        }

        public CinematicMod(int processId)
        {
            ProcessHandle = processId;

            Off_CameraMatrix = Memory.GetPointerPath(ProcessHandle, Constants.off_cameraArrayPointer, 0, 0x20);
            Off_CameraTarget = Memory.GetPointerPath(ProcessHandle, Constants.off_cameraArrayPointer, 0, 0x4, 0x10, 0xc) + 0x68;
            Off_CameraFov = Memory.GetPointerPath(ProcessHandle, Constants.off_cameraArrayPointer, 0, 0x4, 0x10, 0x4) + 0x5c;

            Off_RaymanMatrix = Memory.GetPointerPath(ProcessHandle, Off_CameraTarget, 0x20);
            Off_RaymanDsgVar16 = Memory.GetPointerPath(ProcessHandle, Constants.off_mainChar, 4, 0xC, 0, 0xC, 8) + 0x203;
        }
    }
}
