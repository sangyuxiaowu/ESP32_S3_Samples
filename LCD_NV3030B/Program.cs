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

            // ��ɫ GND   GND
            // ��ɫ VCC   3.3V
            // ��ɫ DIN   GP1     MOSI
            // ��ɫ CLK   GP2
            // ��ɫ CS    GP3     Ƭѡ
            // ��ɫ D/C   GP4
            // ��ɫ RST   GP5
            // ��ɫ BLK   GP6

            Debug.WriteLine("LCD test!");

            // �������Ź���
            Configuration.SetPinFunction(1, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(2, DeviceFunction.SPI1_CLOCK);

            // ����GPIO������ʵ��
            GpioController controller = new GpioController();

            // ��ʼ����λ��Ƭѡ������/�����������
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

            

            // ��λLCD��ʾ��
            resetPin.Write(PinValue.Low);
            Thread.Sleep(1); // �ȴ�1����
            resetPin.Write(PinValue.High);

            // ѡ��LCD��ʾ��
            chipSelectPin.Write(PinValue.Low);

            // ��������
            dataCommandPin.Write(PinValue.Low); // ����Ϊ����ģʽ
            byte[] command = new byte[] { 0x01 }; // ����0x01��һ������
            spiDevice.Write(command);

            // ��������
            dataCommandPin.Write(PinValue.High); // ����Ϊ����ģʽ
            byte[] data = new byte[] { 0x02, 0x03, 0x04 }; // ������Щ������
            spiDevice.Write(data);

            // ȡ��ѡ��LCD��ʾ��
            chipSelectPin.Write(PinValue.High);


            Thread.Sleep(Timeout.Infinite);
        }
    }
}
