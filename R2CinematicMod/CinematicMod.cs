using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Toe;

namespace R2CinematicMod
{
    class CinematicMod
    {
        public int processHandle { get; set; }

        public int bytesReadOrWritten = 0;

        public void EnableCinematicMod()
        {
            byte[] buffer = new byte[4];
            buffer = new byte[] { 0xC3 };
            Matrix matrix;

            int off_DNM_p_stDynamicsCameraMechanics = 0x4359D0; // original byte = 81, replaced by ret = C3
            int off_ForceCameraPos = 0x473420; // original byte = 53, replaced by ret = C3
            int off_ForceCameraTgt = 0x473480; // original byte = 83, replaced by ret = C3

            Memory.WriteProcessMemory(processHandle, off_DNM_p_stDynamicsCameraMechanics, buffer, buffer.Length, ref bytesReadOrWritten);
            Memory.WriteProcessMemory(processHandle, off_ForceCameraPos, buffer, buffer.Length, ref bytesReadOrWritten);
            Memory.WriteProcessMemory(processHandle, off_DNM_p_stDynamicsCameraMechanics, buffer, buffer.Length, ref bytesReadOrWritten);
            Memory.WriteProcessMemory(processHandle, off_ForceCameraTgt, buffer, buffer.Length, ref bytesReadOrWritten);

            int off_cameraMatrix = Memory.GetPointerPath(processHandle, Constants.off_cameraArrayPointer, 0, 0x20);
            int off_cameraTarget = Memory.GetPointerPath(processHandle, Constants.off_cameraArrayPointer, 0, 0x4, 0x10, 0xc) + 0x68;

            int off_raymanMatrix = Memory.GetPointerPath(processHandle, off_cameraTarget, 0x20);
            matrix = Matrix.Read(processHandle, off_raymanMatrix);

            matrix.m = matrix.m.ClearRotation();
            matrix.m.M24 += 15.0f;
            matrix.m.M34 += 2.0f;

            matrix.Write(processHandle, off_cameraMatrix);
        }

        public void DisableCinematicMod()
        {
            byte[] buffer = new byte[4];

            int off_DNM_p_stDynamicsCameraMechanics = 0x4359D0; // original byte = 81, replaced by ret = C3
            int off_ForceCameraPos = 0x473420; // original byte = 53, replaced by ret = C3
            int off_ForceCameraTgt = 0x473480; // original byte = 83, replaced by ret = C3

            // Restore camera
            buffer = new byte[] { 0x81 };
            Memory.WriteProcessMemory(processHandle, off_DNM_p_stDynamicsCameraMechanics, buffer, buffer.Length, ref bytesReadOrWritten);
            buffer = new byte[] { 0x53 };
            Memory.WriteProcessMemory(processHandle, off_ForceCameraPos, buffer, buffer.Length, ref bytesReadOrWritten);
            buffer = new byte[] { 0x83 };
            Memory.WriteProcessMemory(processHandle, off_ForceCameraTgt, buffer, buffer.Length, ref bytesReadOrWritten);

            int off_cameraFOV = Memory.GetPointerPath(processHandle, Constants.off_cameraArrayPointer, 0, 0x4, 0x10, 0x4) + 0x5c;
            Memory.WriteProcessMemoryFloat(processHandle, off_cameraFOV, 1.2f);
        }

        public void AddKeyPoint(float fov)
        {
            int off_cameraMatrix = Memory.GetPointerPath(processHandle, Constants.off_cameraArrayPointer, 0, 0x20);

            Matrix matrix;
            matrix = Matrix.Read(processHandle, off_cameraMatrix);

            float keyX = matrix.m.M14;
            float keyY = matrix.m.M24;
            float keyZ = matrix.m.M34;

            Quaternion rotation = new Quaternion();
            rotation = matrix.m.ExtractRotation();
            Vector3 eulers = Matrix.QuaternionToEuler(rotation);

            float yaw = eulers.Z;
            float pitch = eulers.X;
            float roll = eulers.Y;

            WriteXML(keyX, keyY, keyZ, yaw, pitch, roll, fov);
            Console.Write("Key placed at X: " + keyX + " Y: " + keyY + " Z: " + keyZ + " Yaw: " + yaw + " Pitch: " + pitch + " Roll: " + roll + " Fov: " + fov + "\n\n");
        }

        public static void WriteXML(float x, float y, float z, float yaw, float pitch, float roll, float fov)
        {
            if (!File.Exists("KeyPoints.xml"))
            {
                XmlWriter xmlWriter = XmlWriter.Create("KeyPoints.xml");

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("coords");
                xmlWriter.WriteStartElement("keyPoint");

                xmlWriter.WriteElementString("coordX", x.ToString());
                xmlWriter.WriteElementString("coordY", y.ToString());
                xmlWriter.WriteElementString("coordZ", z.ToString());
                xmlWriter.WriteElementString("Yaw", yaw.ToString());
                xmlWriter.WriteElementString("Pitch", pitch.ToString());
                xmlWriter.WriteElementString("Roll", roll.ToString());
                xmlWriter.WriteElementString("Fov", fov.ToString());

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.Close();
            }
            else
            {
                XDocument xDocument = XDocument.Load("KeyPoints.xml");
                XElement root = xDocument.Element("coords");
                IEnumerable<XElement> rows = root.Descendants("keyPoint");
                XElement firstRow = rows.Last();
                firstRow.AddAfterSelf(new XElement("keyPoint",
                   new XElement("coordX", x.ToString()),
                   new XElement("coordY", y.ToString()),
                   new XElement("coordZ", z.ToString()),
                   new XElement("Yaw", yaw.ToString()),
                   new XElement("Pitch", pitch.ToString()),
                   new XElement("Roll", roll.ToString()),
                   new XElement("Fov", fov.ToString())));
                xDocument.Save("KeyPoints.xml");
            }
        }

        public void ClearKeyPoints()
        {
            File.Delete("KeyPoints.xml");
            Console.Clear();
            Console.Write("Keys cleared! \n\n");
        }

        public void LaunchCinematic(float speed)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load("KeyPoints.xml");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("You cannot launch a cinematic without registering key points.");
                return;
            }

            int off_cameraMatrix = Memory.GetPointerPath(processHandle, Constants.off_cameraArrayPointer, 0, 0x20);
            int off_cameraFOV = Memory.GetPointerPath(processHandle, Constants.off_cameraArrayPointer, 0, 0x4, 0x10, 0x4) + 0x5c;

            int i = 0;
            float fov = 0;

            // Read key points in xml
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                float coordX = float.Parse(node.SelectSingleNode("coordX").InnerText);
                float coordY = float.Parse(node.SelectSingleNode("coordY").InnerText);
                float coordZ = float.Parse(node.SelectSingleNode("coordZ").InnerText);

                float yaw = float.Parse(node.SelectSingleNode("Yaw").InnerText);
                float pitch = float.Parse(node.SelectSingleNode("Pitch").InnerText);
                float roll = float.Parse(node.SelectSingleNode("Roll").InnerText);

                // First coords, set camera position
                if (i == 0)
                {
                    Matrix matrix;
                    matrix = Matrix.Read(processHandle, off_cameraMatrix);
                    matrix.m = matrix.m.ClearRotation();

                    fov = float.Parse(node.SelectSingleNode("Fov").InnerText);
                    Memory.WriteProcessMemoryFloat(processHandle, off_cameraFOV, fov);

                    matrix.m = matrix.m * Toe.Matrix4.CreateRotationX(pitch) * Toe.Matrix4.CreateRotationZ(yaw) * Toe.Matrix4.CreateRotationY(roll);

                    matrix.m.M14 = coordX;
                    matrix.m.M24 = coordY;
                    matrix.m.M34 = coordZ;

                    matrix.Write(processHandle, off_cameraMatrix);
                }
                else
                {
                    Matrix matrix;
                    matrix = Matrix.Read(processHandle, off_cameraMatrix);

                    Quaternion targetRotation = new Quaternion(pitch, roll, yaw, 0);
                    Vector3 targetRotationVector = new Vector3(targetRotation.X, targetRotation.Y, targetRotation.Z);

                    Vector3 targetVector = new Vector3(coordX, coordY, coordZ);
                    Vector3 position = new Vector3(matrix.m.M14, matrix.m.M24, matrix.m.M34);

                    float targetFov = float.Parse(node.SelectSingleNode("Fov").InnerText);

                    var time = 0.0;

                    while(time < 1.0)
                    {
                        // Lerp position
                        position = Vector3.Lerp(position, targetVector, speed);

                        Quaternion rotation = matrix.m.ExtractRotation();
                        Vector3 eulers = Matrix.QuaternionToEuler(rotation);
                        // Lerp rotation
                        eulers = Vector3.Lerp(eulers, targetRotationVector, speed);

                        // Lerp fov
                        fov = lerp(fov, targetFov, speed);
                        // Set fov
                        Memory.WriteProcessMemoryFloat(processHandle, off_cameraFOV, fov);

                        // Set matrix
                        matrix.m = matrix.m.ClearRotation();
                        matrix.m = matrix.m * Toe.Matrix4.CreateRotationZ(eulers.Z) * Toe.Matrix4.CreateRotationX(eulers.X) * Toe.Matrix4.CreateRotationY(eulers.Y);

                        matrix.m.M14 = position.X;
                        matrix.m.M24 = position.Y;
                        matrix.m.M34 = position.Z;

                        matrix.Write(processHandle, off_cameraMatrix);

                        time += speed;
                    }
                }

                i++;
            }
        }

        public static float lerp(float a, float b, float f)
        {
            return a + f * (b - a);
        }

        public void MoveCamera(string direction)
        {
            int off_cameraMatrix = Memory.GetPointerPath(processHandle, Constants.off_cameraArrayPointer, 0, 0x20);

            Matrix matrix;
            matrix = Matrix.Read(processHandle, off_cameraMatrix);

            float yaw = 0;
            float pitch = 0;
            float roll = 0;

            switch(direction)
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
                    matrix.m.M24 += (float)Math.Cos(eulers.Z + Math.PI/2);
                    matrix.m.M14 += (float)Math.Sin(eulers.Z + Math.PI/2);
                    break;
                case "right":
                    matrix.m.M24 += (float)Math.Cos(eulers.Z - Math.PI/2);
                    matrix.m.M14 += (float)Math.Sin(eulers.Z - Math.PI/2);
                    break;
                case "upward":
                    matrix.m.M34 += 0.5f;
                    break;
                case "downward":
                    matrix.m.M34 += -0.5f;
                    break;
            }

            //Console.Write("cos: " + (float)Math.Cos(eulers.Z) + "\n");
            //Console.Write("sin: " + (float)Math.Sin(eulers.Z) + "\n");

            //Console.Write("x: " + matrix.m.M24 + "\n");
            //Console.Write("y: " + matrix.m.M14 + "\n");

            matrix.Write(processHandle, off_cameraMatrix);
        }

        public void ChangeFOV(float fov)
        {
            int off_cameraFOV = Memory.GetPointerPath(processHandle, Constants.off_cameraArrayPointer, 0, 0x4, 0x10, 0x4) + 0x5c;
            Memory.WriteProcessMemoryFloat(processHandle, off_cameraFOV, fov);
        }

        public CinematicMod(Form1 f, int processId)
        {
            processHandle = processId;
        }
    }
}
