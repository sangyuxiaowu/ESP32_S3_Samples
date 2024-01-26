using Iot.Device.Ws28xx.Esp32;
using nanoFramework.Hardware.Esp32.Rmt;

namespace ProjectImprovWifi.WorkLed
{
   internal class XL_WS2812B : Ws28xx
    {
        public XL_WS2812B(int gpioPin, int width, int height = 1)
            : base(gpioPin, new BitmapImageWs2808(width, height))
        {
            ClockDivider = 2;
            OnePulse = new RmtCommand(32, true, 18, false);
            ZeroPulse = new RmtCommand(16, true, 34, false);
            ResetCommand = new RmtCommand(2000, false, 2000, false);
        }
    }
}
