using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using System.Device.I2s;
using System.Diagnostics;
using System.Threading;

namespace AudioBoard_WM8960
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("START");

            // 配置 I2C 引脚
            Configuration.SetPinFunction(12, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(11, DeviceFunction.I2C1_CLOCK);

            // 配置 I2C 设备
            var i2cConfig = new I2cConnectionSettings(1, 0x1A);
            var i2cDevice = I2cDevice.Create(i2cConfig);

            // 配置 I2S 引脚
            Configuration.SetPinFunction(10, DeviceFunction.I2S1_BCK); // BCLK
            Configuration.SetPinFunction(9, DeviceFunction.I2S1_WS); // LRC
            Configuration.SetPinFunction(8, DeviceFunction.I2S1_MDATA_IN); // DIN
            //Configuration.SetPinFunction(7, DeviceFunction.I2S1_DATA_OUT); // DOUT

            // 配置 I2S 设备
            var i2sConfig = new I2sConnectionSettings(1)
            {
                BitsPerSample = I2sBitsPerSample.Bit8,
                ChannelFormat = I2sChannelFormat.OnlyLeft,
                Mode = I2sMode.Master | I2sMode.Rx | I2sMode.Pdm,
                CommunicationFormat = I2sCommunicationFormat.I2S,
                SampleRate = 8_000
            };

            var i2sDevice = I2sDevice.Create(i2sConfig);

            // 初始化 WM8960
            InitializeWM8960(i2cDevice);

            ReadAllRegisters(i2cDevice);

            Debug.WriteLine("Recording");
            // 开始录制音频
            byte[] buffer = new byte[32000]; // 4秒的音频数据
            i2sDevice.Read(buffer);

            // 处理录制的音频数据
            //ProcessAudioData(buffer);
            PlayAudio(i2sDevice, buffer);

            Debug.WriteLine("End");
            Thread.Sleep(Timeout.Infinite);
        }

        private static void InitializeWM8960(I2cDevice i2cDevice)
        {
            // Reset Device
            WM8960_Write_Reg(i2cDevice, 0x0F, 0xFFFF);
            Console.WriteLine("WM8960 reset completed !!");

            // Set Power Source
            bool useBoardMic = true; // 根据需求设置
            bool useEarphoneMic = !useBoardMic;

            if (useBoardMic)
            {
                WM8960_Write_Reg(i2cDevice, 0x19, 0x00E8);
            }
            else if (useEarphoneMic)
            {
                WM8960_Write_Reg(i2cDevice, 0x19, 0x00D4);
            }
            WM8960_Write_Reg(i2cDevice, 0x1A, 0x01F8);
            WM8960_Write_Reg(i2cDevice, 0x2F, 0x003C);

            // Configure clock
            WM8960_Write_Reg(i2cDevice, 0x04, 0x0000);

            // Audio Interface
            WM8960_Write_Reg(i2cDevice, 0x07, 0x0002);

            // PGA
            WM8960_Write_Reg(i2cDevice, 0x00, 0x003F | 0x0100);
            WM8960_Write_Reg(i2cDevice, 0x01, 0x003F | 0x0100);

            if (useBoardMic)
            {
                WM8960_Write_Reg(i2cDevice, 0x20, 0x0008 | 0x0100);
                WM8960_Write_Reg(i2cDevice, 0x21, 0x0000);
            }
            else if (useEarphoneMic)
            {
                WM8960_Write_Reg(i2cDevice, 0x20, 0x0000);
                WM8960_Write_Reg(i2cDevice, 0x21, 0x0008 | 0x0100);
            }

            WM8960_Write_Reg(i2cDevice, 0x2B, 0x0000);
            WM8960_Write_Reg(i2cDevice, 0x2C, 0x0000);

            // ADC
            WM8960_Write_Reg(i2cDevice, 0x05, 0x000C);
            WM8960_Write_Reg(i2cDevice, 0x15, 0x00C3 | 0x0100);
            WM8960_Write_Reg(i2cDevice, 0x16, 0x00C3 | 0x0100);

            if (useBoardMic)
            {
                WM8960_Write_Reg(i2cDevice, 0x17, 0x01C4);
            }
            else if (useEarphoneMic)
            {
                WM8960_Write_Reg(i2cDevice, 0x17, 0x01C8);
            }

            // ALC Control
            WM8960_Write_Reg(i2cDevice, 0x14, 0x00F9);

            // Output Signal Path
            WM8960_Write_Reg(i2cDevice, 0x0A, 0x00FF | 0x0100);
            WM8960_Write_Reg(i2cDevice, 0x0B, 0x00FF | 0x0100);
            WM8960_Write_Reg(i2cDevice, 0x05, 0x0000);
            WM8960_Write_Reg(i2cDevice, 0x06, 0x0000);
            WM8960_Write_Reg(i2cDevice, 0x10, 0x0000);

            // Enabling the Outputs
            WM8960_Write_Reg(i2cDevice, 0x31, 0x00F7); // Enable Left and right speakers
        }

        /// <summary>
        /// 向指定寄存器写入16位值
        /// </summary>
        /// <param name="reg">寄存器地址</param>
        /// <param name="dat">要写入的值</param>
        /// <returns>操作结果</returns>
        public static void WM8960_Write_Reg(I2cDevice i2cDevice, byte reg, ushort dat)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)((reg << 1) | ((dat >> 8) & 0x01)); // 寄存器地址
            buffer[1] = (byte)(dat & 0x00FF); // 寄存器值
            i2cDevice.Write(buffer);
        }

        /// <summary>
        /// 读取所有寄存器的值并打印出来
        /// </summary>
        public static void ReadAllRegisters(I2cDevice i2cDevice)
        {
            for (byte reg = 0; reg < 56; reg++)
            {
                ushort value = WM8960_Read_Reg(i2cDevice, reg);
                Console.WriteLine($"Info {reg:X2}: {value:X4}");
            }
        }

        /// <summary>
        /// 从指定寄存器读取16位值
        /// </summary>
        /// <param name="reg">寄存器地址</param>
        /// <returns>读取的值</returns>
        public static ushort WM8960_Read_Reg(I2cDevice i2cDevice, byte reg)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)(reg << 1); // 寄存器地址
            i2cDevice.WriteRead(buffer, buffer);
            return (ushort)((buffer[0] << 8) | buffer[1]); // 寄存器值
        }


        private static void ProcessAudioData(byte[] buffer)
        {
            // 处理录制的音频数据，例如保存到文件或进行进一步处理
            // 这里可以根据具体需求进行实现
        }

        private static void PlayAudio(I2sDevice i2sDevice, byte[] buffer)
        {
            // 打印音频数据
            for (int i = 0; i < buffer.Length; i++)
            {
                Console.Write(buffer[i].ToString("X2") + " ");
            }

            //i2sDevice.Write(buffer);
        }


    }
}
