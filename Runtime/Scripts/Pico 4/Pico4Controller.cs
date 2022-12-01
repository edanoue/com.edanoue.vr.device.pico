#nullable enable

using System;
using Edanoue.VR.Device.Core;
using Unity.XR.PXR;
using UnityEngine;

namespace Edanoue.VR.Device.Pico
{
    public abstract class PicoControllerBase : IController, IUpdatable
    {
        private Action<float, float>? _changedStickDelegate;
        private bool _isConnected;

        // Primary Button
        private int _isPressedPrimary;

        // Thumb Stick
        private int _isPressedStick;
        private int _isTouchedPrimary;
        private int _isTouchedStick;
        private Action<bool>? _pressedPrimaryDelegate;
        private Action<bool>? _pressedStickDelegate;

        // Position
        private float _px;
        private float _py;
        private float _pz;

        // Rotations
        private float _rw;
        private float _rx;
        private float _ry;
        private float _rz;
        private float _stickX;
        private float _stickY;
        private Action<bool>? _touchedPrimaryDelegate;
        private Action<bool>? _touchedStickDelegate;

        protected abstract PXR_Input.Controller _PicoControllerDomain { get; }

        bool ITracker.IsConnected => _isConnected;

        (float X, float Y, float Z) ITracker.Position => (_px, _py, _pz);

        (float W, float X, float Y, float Z) ITracker.Rotation => (_rw, _rx, _ry, _ry);

        // Primary Button
        bool IController.IsPressedPrimary => _isPressedPrimary > 0;
        bool IController.IsTouchedPrimary => _isTouchedPrimary > 0;

        event Action<bool>? IController.PressedPrimary
        {
            add => _pressedPrimaryDelegate += value;
            remove => _pressedPrimaryDelegate -= value;
        }

        event Action<bool>? IController.TouchedPrimary
        {
            add => _touchedPrimaryDelegate += value;
            remove => _touchedPrimaryDelegate -= value;
        }

        // Thumb Stick
        bool IController.IsPressedStick => _isPressedStick > 0;
        bool IController.IsTouchedStick => _isTouchedStick > 0;
        (float X, float Y) IController.Stick => (_stickX, _stickY);

        event Action<bool>? IController.PressedStick
        {
            add => _pressedStickDelegate += value;
            remove => _pressedPrimaryDelegate -= value;
        }

        event Action<bool>? IController.TouchedStick
        {
            add => _touchedStickDelegate += value;
            remove => _touchedStickDelegate -= value;
        }

        event Action<float, float>? IController.ChangedStick
        {
            add => _changedStickDelegate += value;
            remove => _changedStickDelegate -= value;
        }

        void IUpdatable.Update(float deltaTime)
        {
            // Check connection
            _isConnected = PXR_Input.IsControllerConnected(_PicoControllerDomain);

            if (_isConnected == false) return;

            // Fetch positions and rotations
            var controllerTracking = new PxrControllerTracking();
            var headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };
            const double predictTime = 0.0d;

            PXR_Plugin.Controller.UPxr_GetControllerTrackingState((uint)_PicoControllerDomain, predictTime,
                headData, ref controllerTracking);
            _px = controllerTracking.localControllerPose.pose.position.x;
            _py = controllerTracking.localControllerPose.pose.position.y;
            _pz = controllerTracking.localControllerPose.pose.position.z * -1;

            // Convert rotatios to Unity space
            var nativeorient = controllerTracking.localControllerPose.pose.orientation;
            var nativerot = new Quaternion(nativeorient.x, nativeorient.y, nativeorient.z, nativeorient.w);
            var rot = Quaternion.Euler(-nativerot.x, -nativerot.y, nativerot.z);
            _rw = rot.w;
            _rx = rot.x;
            _ry = rot.y;
            _rz = rot.z;

            // Fetch inputs
            var controllerInputState = new PxrControllerInputState();

#if UNITY_ANDROID && !UNITY_EDITOR
            NativeAPI.Pxr_GetControllerInputState((uint)this._PicoControllerDomain, ref controllerInputState);
#endif

            if (_isPressedPrimary != controllerInputState.AXValue)
            {
                _isPressedPrimary = controllerInputState.AXValue;
                _pressedPrimaryDelegate?.Invoke(_isPressedPrimary > 0);
            }

            if (_isTouchedPrimary != controllerInputState.AXTouchValue)
            {
                _isTouchedPrimary = controllerInputState.AXTouchValue;
                _touchedPrimaryDelegate?.Invoke(_isTouchedPrimary > 0);
            }

            if (_isPressedStick != controllerInputState.touchpadValue)
            {
                _isPressedStick = controllerInputState.touchpadValue;
                _pressedStickDelegate?.Invoke(_isPressedStick > 0);
            }

            if (_isTouchedStick != controllerInputState.rockerTouchValue)
            {
                _isTouchedStick = controllerInputState.rockerTouchValue;
                _touchedStickDelegate?.Invoke(_isTouchedStick > 0);
            }

            if (_stickX != controllerInputState.Joystick.x || _stickY != controllerInputState.Joystick.y)
            {
                _stickX = controllerInputState.Joystick.x;
                _stickY = controllerInputState.Joystick.y;
                _changedStickDelegate?.Invoke(_stickX, _stickY);
            }
        }
    }

    public class PicoControllerLeft : PicoControllerBase
    {
        protected override PXR_Input.Controller _PicoControllerDomain => PXR_Input.Controller.LeftController;
    }

    public class PicoControllerRight : PicoControllerBase
    {
        protected override PXR_Input.Controller _PicoControllerDomain => PXR_Input.Controller.RightController;
    }
}