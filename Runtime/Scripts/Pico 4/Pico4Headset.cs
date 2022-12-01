#nullable enable
using UnityEngine;
using Edanoue.VR.Device.Core;
using Unity.XR.PXR;

namespace Edanoue.VR.Device.Pico
{
    public class PicoHeadset : IHeadset
    {
        bool ITracker.IsConnected => true;

        (float X, float Y, float Z) ITracker.Position
        {
            get
            {
                var sensorState = new PxrSensorState2();
                int sensorFrameIndex = 0;
                PXR_Plugin.System.UPxr_GetPredictedMainSensorStateNew(ref sensorState, ref sensorFrameIndex);
                return(
                    sensorState.pose.position.x,
                    sensorState.pose.position.y,
                    sensorState.pose.position.z * -1);
            }
        }
        
        (float W, float X, float Y, float Z) ITracker.Rotation
        {
            get
            {
                var sensorState = new PxrSensorState2();
                int sensorFrameIndex = 0;
                PXR_Plugin.System.UPxr_GetPredictedMainSensorStateNew(ref sensorState, ref sensorFrameIndex);
                Vector3 rototion = new Quaternion(sensorState.pose.orientation.x, sensorState.pose.orientation.y, sensorState.pose.orientation.z, sensorState.pose.orientation.w).eulerAngles;
                var rot = Quaternion.Euler(-rototion.x, -rototion.y, rototion.z);
                return(
                    rot.w,
                    rot.x,
                    rot.y,
                    rot.z);
            }
        }

        string IHeadset.ProductName => "PICO 4";
    }
}