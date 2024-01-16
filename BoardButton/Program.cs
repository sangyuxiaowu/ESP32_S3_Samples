using System.Diagnostics;
using System.Threading;
using System.Device.Gpio;
using Iot.Device.Ws28xx.Esp32;
using System.Drawing;

namespace BoardButton
{
    public class Program
    {
        public static void Main()
        {
            // 1 �����飬1����
            int WS2812_Count = 1;
            //  ESP32-S3-Zero ���������
            int WS2812_Pin = 21;
            var gpioController = new GpioController();
            var leddev = new Ws2812c(WS2812_Pin, WS2812_Count);
            BitmapImage img = leddev.Image;

            var userbtn = gpioController.OpenPin(0, PinMode.InputPullDown);
            userbtn.ValueChanged += (s, e) =>
            {

                Debug.WriteLine("BOOT ��ť�¼���" + e.ChangeType.ToString());
                Debug.WriteLine("IO0 ��ֵ��" + userbtn.Read());

                if (userbtn.Read() == PinValue.Low)
                {
                    // ����
                    img.SetPixel(0, 0, Color.White);
                }
                else
                {
                    // �ص�
                    img.SetPixel(0, 0, Color.Black);
                }
                leddev.Update();
            };
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
