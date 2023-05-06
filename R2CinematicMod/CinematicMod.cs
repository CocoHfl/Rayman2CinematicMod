using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Toe;

namespace R2CinematicMod
{
    class CinematicMod
    {
        private int ProcessHandle { get; set; }

        private int BytesReadOrWritten = 0;

        private readonly int Off_DNM_p_stDynamicsCameraMechanics = 0x4359D0;
        private readonly int Off_ForceCameraPos = 0x473420;
        private readonly int Off_ForceCameraTgt = 0x473480;

        private readonly int Off_CameraMatrix;
        private readonly int Off_CameraTarget;
        private readonly int Off_CameraFov;
        private readonly int Off_RaymanMatrix;
        private readonly int Off_RaymanDsgVar16;

        public void EnableCinematicMod()
        {
            var buffer = new byte[] { 0xC3 };
            Matrix matrix;

            Memory.WriteProcessMemory(ProcessHandle, Off_DNM_p_stDynamicsCameraMechanics, buffer, buffer.Length, ref BytesReadOrWritten);
            Memory.WriteProcessMemory(ProcessHandle, Off_ForceCameraPos, buffer, buffer.Length, ref BytesReadOrWritten);
            Memory.WriteProcessMemory(ProcessHandle, Off_ForceCameraTgt, buffer, buffer.Length, ref BytesReadOrWritten);

            matrix = Matrix.Read(ProcessHandle, Off_RaymanMatrix);

            // Disable Rayman movements
            Memory.WriteProcessMemoryByte(ProcessHandle, Off_RaymanDsgVar16, 0);

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

            // Enable Rayman movements
            Memory.WriteProcessMemoryByte(ProcessHandle, Off_RaymanDsgVar16, 1);
        }

        public void ResetCamera()
        {
            Matrix matrix = Matrix.Read(ProcessHandle, Off_CameraMatrix);

            matrix.m = matrix.m.ClearRotation();
            matrix.Write(ProcessHandle, Off_CameraMatrix);
        }

        public void AddKeyPoint(XDocument keyPointsDoc, float fov)
        {
            Matrix matrix;
            matrix = Matrix.Read(ProcessHandle, Off_CameraMatrix);

            float keyX = matrix.m.M14;
            float keyY = matrix.m.M24;
            float keyZ = matrix.m.M34;

            Quaternion rotation = matrix.m.ExtractRotation();
            Vector3 eulers = Matrix.QuaternionToEuler(rotation);

            float yaw = eulers.Y;
            float pitch = eulers.X;
            float roll = eulers.Z;

            WriteXML(
                keyPointsDoc,
                Math.Round(keyX, 3), 
                Math.Round(keyY, 3), 
                Math.Round(keyZ, 3), 
                Math.Round(yaw, 3), 
                Math.Round(pitch, 3), 
                Math.Round(roll, 3), 
                Math.Round(fov, 3));
        }

        public void UndoLastKeyPoint(XDocument keyPointsDoc)
        {
            if (keyPointsDoc.Element("coords").HasElements)
            {
                var lastElem = keyPointsDoc.Element("coords")
                     .Elements("keyPoint")
                     .LastOrDefault();

                lastElem.Remove();
                keyPointsDoc.Save("KeyPoints.xml");

                Console.Write("Undid last key point.\n\n");
            }
        }

        public void ClearKeyPoints(XDocument keyPointsDoc)
        {
            keyPointsDoc.Element("coords").Elements("keyPoint").Remove();
            keyPointsDoc.Save("KeyPoints.xml");

            Console.Clear();
            Console.Write("Keys cleared! \n\n");
        }

        public static void WriteXML(XDocument keyPointsDoc, double x, double y, double z, double yaw, double pitch, double roll, double fov)
        {
            var keyPointData = new XElement("keyPoint",
                   new XElement("coordX", x.ToString()),
                   new XElement("coordY", y.ToString()),
                   new XElement("coordZ", z.ToString()),
                   new XElement("Yaw", yaw.ToString()),
                   new XElement("Pitch", pitch.ToString()),
                   new XElement("Roll", roll.ToString()),
                   new XElement("Fov", fov.ToString()));

            XElement root = keyPointsDoc.Element("coords");

            if(root.IsEmpty)
            {
                root.Add(keyPointData);
            }
            else
            {
                IEnumerable<XElement> rows = root.Descendants("keyPoint");
                XElement lastRow = rows.Last();
                lastRow.AddAfterSelf(keyPointData);
            }

            keyPointsDoc.Save("KeyPoints.xml");

            Console.Write("Key placed at X: " + x + " Y: " + y + " Z: " + z + " Yaw: " + yaw + " Pitch: " + pitch + " Roll: " + roll + " Fov: " + fov + "\n\n");
        }

        public void LaunchCinematic(XDocument keyPointsDoc, float speed)
        {
            var nodes = keyPointsDoc.Element("coords").Elements("keyPoint").ToList();

            // Read key points from xml and store in a list
            List<BezierPoint> keyPoints = new List<BezierPoint>();
            for (int i = 0; i < nodes.Count; i++)
            {
                float yaw = float.Parse(nodes[i].Element("Yaw").Value);
                float pitch = float.Parse(nodes[i].Element("Pitch").Value);
                float roll = float.Parse(nodes[i].Element("Roll").Value);
                float fov = float.Parse(nodes[i].Element("Fov").Value);
                float coordX = float.Parse(nodes[i].Element("coordX").Value);
                float coordY = float.Parse(nodes[i].Element("coordY").Value);
                float coordZ = float.Parse(nodes[i].Element("coordZ").Value);

                // Adding each keypoint position, rotation and fov
                keyPoints.Add(new BezierPoint(
                    new Vector3(coordX, coordY, coordZ),
                    new Vector3(yaw, pitch, roll),
                    fov));
            }

            // If there are less than 4 key points, add default points
            while (keyPoints.Count < 4)
            {
                // Use the last key point as the default point
                BezierPoint defaultKeyPoint = keyPoints[keyPoints.Count - 1];
                keyPoints.Add(defaultKeyPoint);
            }

            // Check if the last group is incomplete (needs 3 points)
            if ((keyPoints.Count - 4) % 3 != 0)
            {
                int lastGroupSize = (keyPoints.Count - 4) % 3;

                //Set default key point, based on previous key point
                BezierPoint defaultKeyPoint = keyPoints[keyPoints.Count - 1];

                if (lastGroupSize == 1)
                {
                    // Add two default key points
                    keyPoints.Add(defaultKeyPoint);
                    keyPoints.Add(defaultKeyPoint);
                }
                else if (lastGroupSize == 2)
                {
                    // Add one default key point
                    keyPoints.Add(defaultKeyPoint);
                }
            }

            List<BezierPoint> curvePoints = new List<BezierPoint>();
            // Process key points in group of 4 to create the curves
            for (int i = 0; i < keyPoints.Count - 3; i += 3)
            {
                BezierPoint bp0 = keyPoints[i];
                BezierPoint bp1 = keyPoints[i + 1];
                BezierPoint bp2 = keyPoints[i + 2];
                BezierPoint bp3 = keyPoints[i + 3];

                float t = 0;
                while (t <= 1)
                {
                    // Processing cubic Bezier curve
                    Vector3 pointOnCurve = CubicBezier(t, bp0.Position, bp1.Position, bp2.Position, bp3.Position);

                    // Interpolate rotation
                    Vector3 rotationOnCurve = SphericalInterpolation(t, bp0.Rotation, bp1.Rotation, bp2.Rotation, bp3.Rotation);

                    // Lerping fov
                    float fov01 = Lerp(t, bp0.Fov, bp1.Fov);
                    float fov12 = Lerp(t, bp1.Fov, bp2.Fov);
                    float fov23 = Lerp(t, bp2.Fov, bp3.Fov);

                    float fov012 = Lerp(t, fov01, fov12);
                    float fov123 = Lerp(t, fov12, fov23);

                    float fovOnCurve = Lerp(t, fov012, fov123);

                    // Add curve point data
                    curvePoints.Add(new BezierPoint(pointOnCurve, rotationOnCurve, fovOnCurve));

                    t += 0.001f; // Step size
                }
            }

            float time = 0f;
            float stepSize = speed / curvePoints.Count;

            Matrix matrix = Matrix.Read(ProcessHandle, Off_CameraMatrix);

            // Render camera movement gradually in while loop
            while (time < 1f)
            {
                int curveIndex = Convert.ToInt32(time * curvePoints.Count);
                if (curveIndex >= curvePoints.Count)
                {
                    break;
                }

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

        private float Lerp(float f, float a, float b)
        {
            return a + f * (b - a);
        }

        // Cubic Bezier interpolation function
        private Vector3 CubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        private static Vector3 SphericalInterpolation(float t, Vector3 start, Vector3 mid1, Vector3 mid2, Vector3 end)
        {
            Quaternion qStart = Quaternion.EulerRotation(start.Z, start.X, start.Y);
            Quaternion qMid1 = Quaternion.EulerRotation(mid1.Z, mid1.X, mid1.Y);
            Quaternion qMid2 = Quaternion.EulerRotation(mid2.Z, mid2.X, mid2.Y);
            Quaternion qEnd = Quaternion.EulerRotation(end.Z, end.X, end.Y);

            Quaternion q1 = Quaternion.Slerp(qStart, qMid1, t);
            Quaternion q2 = Quaternion.Slerp(qMid1, qMid2, t);
            Quaternion q3 = Quaternion.Slerp(qMid2, qEnd, t);

            Quaternion q4 = Quaternion.Slerp(q1, q2, t);
            Quaternion q5 = Quaternion.Slerp(q2, q3, t);

            Quaternion q6 = Quaternion.Slerp(q4, q5, t);

            return Matrix.QuaternionToEuler(q6);
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

            matrix.m = matrix.m * Toe.Matrix4.CreateRotationZ(yaw) * Toe.Matrix4.CreateRotationX(pitch) * Toe.Matrix4.CreateRotationY(roll);

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
