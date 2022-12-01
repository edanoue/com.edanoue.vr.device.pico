using System;
using System.Runtime.InteropServices;

namespace Edanoue.VR.Device.Pico
{
    [StructLayout((LayoutKind.Sequential))]
    public struct PxrVector2f
    {
        public float x;
        public float y;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct PxrControllerInputState
    {
        public PxrVector2f Joystick;
        public int homeValue;
        public int backValue;
        public int touchpadValue;
        public int volumeUp;
        public int volumeDown;
        public float triggerValue;
        public int batteryValue;
        public int AXValue;
        public int BYValue;
        public int sideValue;
        public float gripValue;
        public int reserved_key_0;
        public int reserved_key_1;
        public int reserved_key_2;
        public int reserved_key_3;
        public int reserved_key_4;

        public int AXTouchValue;
        public int BYTouchValue;
        public int rockerTouchValue;
        public int triggerTouchValue;
        public int thumbrestTouchValue;
        public int reserved_touch_0;
        public int reserved_touch_1;
        public int reserved_touch_2;
        public int reserved_touch_3;
        public int reserved_touch_4;
    }

    public static class NativeAPI
    {
        private const string PXR_SDK_VERSION = "2.1.2.2";
        private const string PXR_API_DLL = "pxr_api";

        [DllImport(PXR_API_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pxr_GetControllerInputState(UInt32 deviceId, ref PxrControllerInputState state);

    }
}