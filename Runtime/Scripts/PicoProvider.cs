#nullable enable
using Edanoue.VR.Device.Core;

namespace Edanoue.VR.Device.Pico
{
    public class PicoProvider : IProvider
    {
        private readonly PicoHeadset _headset;

        public PicoProvider()
        {
            _headset = new PicoHeadset();
            LeftController = new PicoControllerLeft();
            RightController = new PicoControllerRight();
        }

        public PicoControllerLeft LeftController { get; }
        public PicoControllerRight RightController { get; }

        string IProvider.FamilyName => "Pico";
        string IProvider.ProductName => "Pico";
        string IProvider.Version => "0.1.0";

        // IProvider impls

        #region IProvider impls

        IHeadset IProvider.Headset => _headset;
        IController IProvider.LeftController => LeftController;
        IController IProvider.RightController => RightController;

        #endregion
    }
}