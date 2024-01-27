using ImprovWifi;
using Iot.Device.Ws28xx.Esp32;
using ProjectImprovWifi.WorkLed;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
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
        /// GPIO 控制
        /// </summary>
        static GpioController gpioController = new GpioController();

        public static void Main()
        {
            // 开启工作灯，蓝色引擎启动！
            _led.StartAutoUpdate();


            // 读取配置文件
            var configuration = Wireless80211Configuration.GetAllWireless80211Configurations();
            if (configuration.Length == 0)
            {
                Console.WriteLine("没有找到wifi配置文件");
            }
            else
            {
                Console.WriteLine($"SSID: {configuration[0].Ssid}, Password: {configuration[0].Password}");
            }
            

            // 初始化用户按键
            var userbtn = gpioController.OpenPin(0, PinMode.InputPullDown);
            // 按键事件
            userbtn.ValueChanged += Userbtn_ValueChanged;

            // 初始化蓝牙配网
            _imp = new Improv();
            // 被请求授权
            _imp.OnIdentify += Imp_OnIdentify;
            // 配网成功
            _imp.OnProvisioningComplete += Imp_OnProvisioningComplete;

            _imp.Start("ESP32 桑榆肖物");

            Console.WriteLine("Waiting for device to be provisioned");

            while (_imp.CurrentState != Improv.ImprovState.provisioned)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine("Device has been provisioned");

            _led.DeviceStatus = RunStatus.Working;

            _imp.Stop();
            _imp = null;


            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// 配网完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Imp_OnProvisioningComplete(object sender, EventArgs e)
        {
            _imp.RedirectUrl = "http://" + _imp.GetCurrentIPAddress() + "/start.htm";

        }

        /// <summary>
        /// 被请求识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void Imp_OnIdentify(object sender, EventArgs e)
        {
            Console.WriteLine("Identify requested");
            if (_imp.CurrentState != Improv.ImprovState.authorizationRequired)
            {
                return;
            }
            _led.DeviceStatus = RunStatus.OnIdentify;
        }

        // 记录上一次按键按下时间
        static DateTime lastClickTime = DateTime.UtcNow;

        /// <summary>
        /// 用户按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Userbtn_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            if (e.ChangeType == PinEventTypes.Rising)
            {
                lastClickTime = DateTime.UtcNow;
            }
            if(e.ChangeType == PinEventTypes.Falling)
            {
                // 按键按下时间大于 5s，重置wifi配置
                if ((DateTime.UtcNow - lastClickTime).TotalSeconds > 5)
                {
                    //Wireless80211Configuration.GetAllWireless80211Configurations()[0].SaveConfiguration();
                    Console.WriteLine("Reset wifi configuration");
                }
            }


            // 配网结束或者未开始请求授权，则不处理
            if (_imp is null || _imp.CurrentState != Improv.ImprovState.authorizationRequired)
            {
                return;
            }
            if (e.ChangeType == PinEventTypes.Rising)
            {
                Console.WriteLine("User button pressed");
                _imp.Authorise(true);
                // 验证成功，改变灯光状态
                _led.DeviceStatus = RunStatus.AuthSuccess;
            }
        }
    }
}
