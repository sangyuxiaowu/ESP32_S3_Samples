using System;
using System.Diagnostics;
using System.Threading;
using System.Device.Gpio;
using nanoFramework.Hardware.Esp32;
using Iot.Device.Ssd13xx;
using System.Device.I2c;
using Iot.ByteFont;

namespace OLED_SSD13XX
{
    public class Program
    {
        public static void Main()
        {
            // �������Ź���
            Configuration.SetPinFunction(1, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(2, DeviceFunction.I2C1_CLOCK);

            using Ssd1306 device = new Ssd1306(I2cDevice.Create(new I2cConnectionSettings(1, Ssd1306.DefaultI2cAddress)), Ssd13xx.DisplayResolution.OLED128x64);

            device.ClearScreen();
            device.Font = new BasicFont();
            device.DrawString( 0, 0, "nanoFramework", 1);
            device.DrawString(0, 32, ".NET", 3);
            device.Display();
            Thread.Sleep(2000);

            // ����չʾ��������

            string str = ".net nanoFramework  ";//����2���ո�ȷ����ʾЧ��
            int strWidth = device.Font.Width * str.Length; // ����ԭʼ�ַ����Ŀ��
            int ledWidth = 128; // �豸�Ŀ��
            int showTimes = 5; // ������Ҫ��ʾ����
            int showWidth = strWidth * showTimes - ledWidth; // ����������Ҫ���ƵĿ��
            string showStr = "";
            // ���� showStr + str ֱ������ showWidth
            do
            {
                showStr += str;
            }while (device.Font.Width * showStr.Length < showWidth);

            for (int i = 0; i < showWidth; i++) 
            {
                // �����������
                device.ClearDirectAligned(0, 0, 128, 16);
                // �������������ַ�������ʼλ��
                int x = i > strWidth ? i - strWidth : i;
                device.DrawString(-x, 0, showStr, 1); // ���ַ�������ʼλ�������ƶ�
                device.Display();
                Thread.Sleep(10);
            }


            Thread.Sleep(5000);
            // �����������
            device.ClearScreen();
            device.Font = new IotByteFont();
            DarwString(device, 2, 32, "ɣ��Ф��", 2);
            DarwString(device, 0, 0, "IotByteFont", 1);
            device.Display();

            Thread.Sleep(Timeout.Infinite);

        }

        public static void DarwString(Ssd13xx device, int x, int y, string str, byte size = 1)
        {
            int inx = 0;
            int fontWidth = device.Font.Width;
            int fontHeight = device.Font.Height;

            int fontWidthTimesSize = fontWidth * size;
            int fontArea = fontWidth * fontHeight;

            byte[] bitMap = new byte[fontArea];

            foreach (char c in str)
            {
                // ��������  device.Font.Width * device.Font.Height �� 16 ��������
                byte[] charBytes = device.Font[c];

                for (int i = 0; i < charBytes.Length; i++)
                {
                    byte b = charBytes[i];
                    int baseIndex = i * 8;
                    for (int j = 0; j < 8; j++)
                    {
                        // ��ȡ������λ
                        int bit = (b >> j) & 1;
                        // �洢������λ��λͼ����
                        bitMap[baseIndex + j] = (byte)bit;
                    }
                }

                // ���������С�����ң����ϵ��»���λͼ
                int baseX = x + fontWidthTimesSize * inx;
                for (int i = 0; i < fontHeight; i++)
                {
                    int baseY = y + i * size;
                    for (int j = 0; j < fontWidth; j++)
                    {
                        // ��ȡ������λ
                        int bit = bitMap[i * fontWidth + j];
                        // ����size�������ػ�������
                        if (size == 1)
                        {
                            device.DrawPixel(baseX + j * size, baseY, bit == 1);
                        }
                        else
                        {
                            device.DrawFilledRectangle((baseX + j * size), baseY, size, size, bit == 1);
                        }
                    }
                }
                inx++;
            }
        }


    }
}
