using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using Toe;
using System.Net.Http;

namespace R2CinematicModCommon
{
    public class KeyPointsManager
    {
        public XDocument KeyPointsDoc { get; set; }

        private string FilePath { get; set; }

        private const string FileName = "KeyPoints.xml";

        public int KeyPointsCount() => KeyPointsDoc.Element("coords").Descendants("keyPoint").Count();

        public KeyPointsManager()
        {
            FilePath = FileName;
            LoadKeyPoints();
        }

        public KeyPointsManager(string filePath)
        {
            FilePath = filePath;
            LoadKeyPoints();
        }

        private void LoadKeyPoints()
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    XmlWriter xmlWriter = XmlWriter.Create(FileName);

                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("coords");
                    xmlWriter.WriteEndElement();

                    xmlWriter.Close();
                }

                KeyPointsDoc = XDocument.Load(FilePath);
            }
            catch
            {
                throw new Exception($"Could not generate/load {FileName}.xml");
            }
        }

        public List<CurvePoint> ReadKeyPointsFromXML()
        {
            var nodes = KeyPointsDoc.Element("coords").Elements("keyPoint").ToList();

            List<CurvePoint> keyPoints = new List<CurvePoint>();
            for (int i = 0; i < nodes.Count; i++)
            {
                float yaw = float.Parse(nodes[i].Element("Yaw").Value);
                float pitch = float.Parse(nodes[i].Element("Pitch").Value);
                float roll = float.Parse(nodes[i].Element("Roll").Value);
                float fov = float.Parse(nodes[i].Element("Fov").Value);
                float coordX = float.Parse(nodes[i].Element("coordX").Value);
                float coordY = float.Parse(nodes[i].Element("coordY").Value);
                float coordZ = float.Parse(nodes[i].Element("coordZ").Value);

                // Adding each keypoint position, rotation, and fov
                keyPoints.Add(new CurvePoint(
                    new Vector3(coordX, coordY, coordZ),
                    new Vector3(yaw, pitch, roll),
                    fov));
            }

            return keyPoints;
        }

        public void AddKeyPoint(float fov, int processHandle, int off_CameraMatrix, out string message)
        {
            Matrix matrix;
            matrix = Matrix.Read(processHandle, off_CameraMatrix);

            float keyX = matrix.m.M14;
            float keyY = matrix.m.M24;
            float keyZ = matrix.m.M34;

            Quaternion rotation = matrix.m.ExtractRotation();
            Vector3 eulers = Matrix.QuaternionToEuler(rotation);

            double x = Math.Round(keyX, 3);
            double y = Math.Round(keyY, 3);
            double z = Math.Round(keyZ, 3);

            double yaw = Math.Round(eulers.Y, 3);
            double pitch = Math.Round(eulers.X, 3);
            double roll = Math.Round(eulers.Z, 3);

            WriteXML(x, y, z, yaw, pitch, roll, fov);

            message = "Key placed!\r\n" +
                " X: " + x + " Y: " + y + " Z: " + z + "\r\n" +
                " Yaw: " + yaw + " Pitch: " + pitch + " Roll: " + roll +
                " Fov: " + fov + "\r\n";
        }

        public void UndoLastKeyPoint()
        {
            if (KeyPointsDoc.Element("coords").HasElements)
            {
                var lastElem = KeyPointsDoc.Element("coords")
                     .Elements("keyPoint")
                     .LastOrDefault();

                lastElem.Remove();
                KeyPointsDoc.Save(FileName);
            }
        }

        public void ClearKeyPoints()
        {
            KeyPointsDoc.Element("coords").Elements("keyPoint").Remove();
            KeyPointsDoc.Save(FileName);
        }

        public void WriteXML(double x, double y, double z, double yaw, double pitch, double roll, float fov)
        {
            var keyPointData = new XElement("keyPoint",
                   new XElement("coordX", x.ToString()),
                   new XElement("coordY", y.ToString()),
                   new XElement("coordZ", z.ToString()),
                   new XElement("Yaw", yaw.ToString()),
                   new XElement("Pitch", pitch.ToString()),
                   new XElement("Roll", roll.ToString()),
                   new XElement("Fov", fov.ToString()));

            XElement root = KeyPointsDoc.Element("coords");

            if (root.IsEmpty)
            {
                root.Add(keyPointData);
            }
            else
            {
                IEnumerable<XElement> rows = root.Descendants("keyPoint");
                XElement lastRow = rows.Last();
                lastRow.AddAfterSelf(keyPointData);
            }

            KeyPointsDoc.Save(FileName);
        }
    }
}
