using R2CinematicModHook.Structs;
using R2CinematicModHook.Types;
using R2CinematicModHook.Utils;
using System.Collections.Generic;
using R2CinematicModCommon;

namespace R2CinematicModHook.Mod
{
    public class CameraPathRenderer
    {
        public CameraPathRenderer(GameManager manager)
        {
            Manager = manager;
        }

        private GameManager Manager { get; }

        private bool DisplayKeyPoints { get; set; }

        private bool DisplayCurve { get; set; }

        private int CurveIndex { get; set; }

        public void InitActions()
        {
            Manager.Engine.Actions.Set("keyPoints", DrawKeyPoints);
            Manager.Engine.Actions.Set("curve", DrawCurve);
        }

        public void ToggleKeyPoints()
        {
            DisplayCurve = false;
            DisplayKeyPoints = !DisplayKeyPoints;
        }

        public void ToggleCurve()
        {
            DisplayKeyPoints = false;
            DisplayCurve = !DisplayCurve;
        }

        private void DrawKeyPoints()
        {
            if (!DisplayKeyPoints) return;

            List<CurvePoint> keyPoints = new KeyPointsManager(Manager.KeyPointsPath).ReadKeyPointsFromXML();

            if (keyPoints.Count != 0)
            {
                for (int i = 0; i < keyPoints.Count; i++)
                {
                    var keyPointPtr = new StructPtr(
                        new Toe.Vector3(keyPoints[i].Position.X, 
                                        keyPoints[i].Position.Y, 
                                        keyPoints[i].Position.Z));

                    Manager.Graphics.VCreatePart.Call(24576, keyPointPtr, 0, 0.5f, 0.5f, 0.5f, TexturePointers.cageIcon);
                }
            }
        }

        private void DrawCurve()
        {
            if (!DisplayCurve) return;

            List<CurvePoint> keyPoints = new KeyPointsManager(Manager.KeyPointsPath).ReadKeyPointsFromXML();

            if (keyPoints.Count != 0)
            {
                var curvePoints = CameraPathGenerator.GenerateCameraPath(keyPoints, 50);

                if (CurveIndex < curvePoints.Count - 1)
                {
                    CurveIndex++;
                }
                else
                {
                    CurveIndex = 0;
                }

                var ptr = new StructPtr(curvePoints[CurveIndex].Position);
                var ptr2 = new StructPtr(new Vector3(0, 0, 0));

                Manager.Graphics.VAddParticle.Call(9, ptr, ptr2, TexturePointers.blueSparkTexture, 0.05f);
            }
        }
    }
}