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
        /// Improv ��������
        /// </summary>
        static Improv _imp;

        /// <summary>
        /// �ƹ����
        /// </summary>
        static BoardLedControl _led = new();

        /// <summary>
        /// �豸����״̬
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
