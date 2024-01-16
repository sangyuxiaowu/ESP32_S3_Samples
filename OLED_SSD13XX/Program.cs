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
            // 设置引脚功能
            Configuration.SetPinFunction(1, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(2, DeviceFunction.I2C1_CLOCK);

            using Ssd1306 device = new Ssd1306(I2cDevice.Create(new I2cConnectionSettings(1, Ssd1306.DefaultI2cAddress)), Ssd13xx.DisplayResolution.OLED128x64);

            device.ClearScreen();
            device.Font = new BasicFont();
            device.DrawString( 0, 0, "nanoFramework", 1);
            device.DrawString(0, 32, ".NET", 3);
            device.Display();
            Thread.Sleep(2000);

            // 滚动展示更多内容

            string str = ".net nanoFramework  ";//增加2个空格，确保显示效果
            int strWidth = device.Font.Width * str.Length; // 计算原始字符串的宽度
            int ledWidth = 128; // 设备的宽度
            int showTimes = 5; // 内容需要显示次数
            int showWidth = strWidth * showTimes - ledWidth; // 计算内容需要左移的宽度
            string showStr = "";
            // 增加 showStr + str 直到大于 showWidth
            do
            {
                showStr += str;
            }while (device.Font.Width * showStr.Length < showWidth);

            for (int i = 0; i < showWidth; i++) 
            {
                // 清除滚动区域
                device.ClearDirectAligned(0, 0, 128, 16);
                // 根据条件修正字符串的起始位置
                int x = i > strWidth ? i - strWidth : i;
                device.DrawString(-x, 0, showStr, 1); // 将字符串的起始位置向左移动
                device.Display();
                Thread.Sleep(10);
            }


            Thread.Sleep(5000);
            // 特殊点阵字体
            device.ClearScreen();
            device.Font = new IotByteFont();
            DarwString(device, 2, 32, "桑榆肖物", 2);
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
                // 字体数据  device.Font.Width * device.Font.Height 的 16 进制数据
                byte[] charBytes = device.Font[c];

                for (int i = 0; i < charBytes.Length; i++)
                {
                    byte b = charBytes[i];
                    int baseIndex = i * 8;
                    for (int j = 0; j < 8; j++)
                    {
                        // 获取二进制位
                        int bit = (b >> j) & 1;
                        // 存储二进制位到位图数组
                        bitMap[baseIndex + j] = (byte)bit;
                    }
                }

                // 按照字体大小从左到右，从上到下绘制位图
                int baseX = x + fontWidthTimesSize * inx;
                for (int i = 0; i < fontHeight; i++)
                {
                    int baseY = y + i * size;
                    for (int j = 0; j < fontWidth; j++)
                    {
                        // 获取二进制位
                        int bit = bitMap[i * fontWidth + j];
                        // 根据size绘制像素或填充矩形
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
