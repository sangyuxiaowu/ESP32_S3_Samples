using nanoFramework.Hardware.Esp32;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using System.Device.Gpio;

namespace LCD_NV3030B
{
    public class Program
    {
        // Doc LCD https://www.waveshare.net/wiki/1.5inch_LCD_Module
        // Doc ESP32 https://www.waveshare.net/wiki/ESP32-S3-Zero
        // Doc SPI https://docs.espressif.com/projects/esp-idf/zh_CN/latest/esp32s3/api-reference/peripherals/spi_master.html

        public static void Main()
        {

            // 白色 GND   GND
            // 紫色 VCC   3.3V
            // 绿色 DIN   GP1     MOSI
            // 橙色 CLK   GP2
            // 黄色 CS    GP3     片选
            // 蓝色 D/C   GP4
            // 黑色 RST   GP5
            // 灰色 BLK   GP6

            Debug.WriteLine("LCD test!");

            // 设置引脚功能
            Configuration.SetPinFunction(1, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(2, DeviceFunction.SPI1_CLOCK);

            // 创建GPIO控制器实例
            GpioController controller = new GpioController();

            // 初始化复位、片选和数据/命令控制引脚
            GpioPin resetPin = controller.OpenPin(5, PinMode.Output);
            GpioPin chipSelectPin = controller.OpenPin(3, PinMode.Output);
            GpioPin dataCommandPin = controller.OpenPin(4, PinMode.Output);
            GpioPin blPin = controller.OpenPin(6, PinMode.Output);
            chipSelectPin.Write(PinValue.Low);

 
            //SpiBus
            //SpiBusInfo spiBusInfo = SpiDevice.GetBusInfo(1);
            //MaxClockFrequency: 40000000 MinClockFrequency: 78125
            //Debug.WriteLine($"{nameof(spiBusInfo.MaxClockFrequency)}: {spiBusInfo.MaxClockFrequency}");
            //Debug.WriteLine($"{nameof(spiBusInfo.MinClockFrequency)}: {spiBusInfo.MinClockFrequency}");


            var connectionSettings = new SpiConnectionSettings(1, 3)
            {
                ClockFrequency = 40_000_000,
                DataBitLength = 8,
                DataFlow = DataFlow.MsbFirst,
                Mode = SpiMode.Mode3
            };

            using SpiDevice spiDevice = SpiDevice.Create(connectionSettings);

            

            // 复位LCD显示屏
            resetPin.Write(PinValue.Low);
            Thread.Sleep(1); // 等待1毫秒
            resetPin.Write(PinValue.High);

            // 选择LCD显示屏
            chipSelectPin.Write(PinValue.Low);

            // 发送命令
            dataCommandPin.Write(PinValue.Low); // 设置为命令模式
            byte[] command = new byte[] { 0x01 }; // 假设0x01是一个命令
            spiDevice.Write(command);

            // 发送数据
            dataCommandPin.Write(PinValue.High); // 设置为数据模式
            byte[] data = new byte[] { 0x02, 0x03, 0x04 }; // 假设这些是数据
            spiDevice.Write(data);

            // 取消选择LCD显示屏
            chipSelectPin.Write(PinValue.High);


            Thread.Sleep(Timeout.Infinite);
        }
    }
}
