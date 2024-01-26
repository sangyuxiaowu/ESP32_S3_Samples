using ImprovWifi;
using Iot.Device.Ws28xx.Esp32;
using ProjectImprovWifi.WorkLed;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace ProjectImprovWifi
{
    public class Program
    {
        /// <summary>
        /// Improv 蓝牙配网
        /// </summary>
        static Improv _imp;

        /// <summary>
        /// 灯光控制
        /// </summary>
        static BoardLedControl _led = new();

        /// <summary>
        /// 设备运行状态
        /// </summary>
        private static RunStatus _currentStatus;

        public static void Main()
        {
            _led.LedSet(Color.Red, 5000);
            _led.LedSet(Color.Green, 5000);
            _led.LedSet(Color.Blue, 5000);
            _led.LedSet(Color.Orange, 5000);
            _led.LedSet(Color.OrangeRed, 5000);
            _led.LedSet(Color.Yellow, 5000);
            _led.LedSet(Color.Pink, 5000);
            _led.LedSet(Color.Purple, 5000);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
