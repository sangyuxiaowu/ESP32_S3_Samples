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
        /// Improv ��������
        /// </summary>
        static Improv _imp;

        /// <summary>
        /// �ƹ����
        /// </summary>
        static BoardLedControl _led = new();

        /// <summary>
        /// GPIO ����
        /// </summary>
        static GpioController gpioController = new GpioController();

        public static void Main()
        {
            // ���������ƣ���ɫ����������
            _led.StartAutoUpdate();


            // ��ȡ�����ļ�
            var configuration = Wireless80211Configuration.GetAllWireless80211Configurations();
            if (configuration.Length == 0 || string.IsNullOrEmpty(configuration[0].Ssid) || string.IsNullOrEmpty(configuration[0].Password))
            {
                Console.WriteLine("û���ҵ�wifi�����ļ�");
            }
            else
            {
                Console.WriteLine($"SSID: {configuration[0].Ssid}, Password: {configuration[0].Password}");
                // ִ������wifi�߼�;
            }

            // ��ʼ���û�����
            var userbtn = gpioController.OpenPin(0, PinMode.InputPullDown);
            // �����¼�
            userbtn.ValueChanged += Userbtn_ValueChanged;

            // ��ʼ����������
            _imp = new Improv();
            // ������ʶ��
            _imp.OnIdentify += Imp_OnIdentify;
            // �����ɹ�
            _imp.OnProvisioningComplete += Imp_OnProvisioningComplete;

            _imp.Start("ESP32 ɣ��Ф��");

            Console.WriteLine("Waiting for device to be provisioned");

            while (_imp.CurrentState != Improv.ImprovState.provisioned)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine("Device has been provisioned");

            _led.DeviceStatus = RunStatus.Working;

            _imp.Stop();
            _imp = null;

            AppRun();

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// ��Ӧ�ó�������
        /// </summary>
        private static void AppRun()
        {
            Console.WriteLine("AppRun");
        }


        /// <summary>
        /// �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Imp_OnProvisioningComplete(object sender, EventArgs e)
        {
            _imp.RedirectUrl = "http://" + _imp.GetCurrentIPAddress() + "/start.htm";

        }

        /// <summary>
        /// ������ʶ��
        /// </summary>
        private static void Imp_OnIdentify(object sender, EventArgs e)
        {
            Console.WriteLine("Identify requested");
            if (_imp.CurrentState != Improv.ImprovState.authorizationRequired)
            {
                return;
            }
            _led.DeviceStatus = RunStatus.OnIdentify;
        }

        // ��¼��һ�ΰ�������ʱ��
        static DateTime lastClickTime = DateTime.UtcNow;

        /// <summary>
        /// �û������¼�
        /// </summary>
        private static void Userbtn_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            // ��¼��������ʱ��
            if (e.ChangeType == PinEventTypes.Falling)
            {
                lastClickTime = DateTime.UtcNow;
            }
            // �����ɿ�
            if(e.ChangeType == PinEventTypes.Rising)
            {
                // ��������ʱ����� 5s������wifi����
                if ((DateTime.UtcNow - lastClickTime).TotalSeconds > 5)
                {
                    // ����wifi����
                    Console.WriteLine("Reset wifi configuration");
                    var wificonfig = new Wireless80211Configuration(0);
                    wificonfig.Ssid = "";
                    wificonfig.Password = "";
                    wificonfig.SaveConfiguration();
                    _led.DeviceStatus = RunStatus.ClearConfig;
                }
            }


            // ������������δ��ʼ������Ȩ���򲻴���
            if (_imp is null || _imp.CurrentState != Improv.ImprovState.authorizationRequired)
            {
                return;
            }
            if (e.ChangeType == PinEventTypes.Rising)
            {
                Console.WriteLine("User button pressed");
                _imp.Authorise(true);
                // ��֤�ɹ����ı�ƹ�״̬
                _led.DeviceStatus = RunStatus.AuthSuccess;
            }
        }
    }
}
