using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IPAdress;
using System.Windows.Automation;

namespace Изучение_WPF_часть_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    static class Extend
    {
        public static string GetDecIPSpecs(this IPv4 ip)
        {
            return                $"IP:\t\t {ip.IP.GetStr()}\n" +
                                  $"Маска:\t\t {ip.Mask.GetStr()}\n" +
                                  $"Адрес сети:\t {ip.NetworkAddress.GetStr()}\n" +
                                  $"Первый узел:\t {ip.HostMin.GetStr()}\n" +
                                  $"Последний узел:\t {ip.HostMax.GetStr()}\n" +
                                  $"Широковещ-ый\t {ip.Broadcast.GetStr()}\n" +
                                  $"Макс. хостов:\t {ip.MaxHosts}\n";
        }
        public static string GetBinIPSpecs(this IPv4 ip)
        {
            return                $"{ip.IP.GetStr(true)}\n" +
                                  $"{ip.Mask.GetStr(true)}\n" +
                                  $"{ip.NetworkAddress.GetStr(true)}\n" +
                                  $"{ip.HostMin.GetStr(true)}\n" +
                                  $"{ip.HostMax.GetStr(true)}\n" +
                                  $"{ip.Broadcast.GetStr(true)}\n";
        }
    }
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Calculate(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] addr = IPv4Box.Text.Split('.').Select(byte.Parse).ToArray();
                byte[] mask = IPMaskBox.Text.Split('.').Select(byte.Parse).ToArray();

                IPv4 ip = new IPv4(addr, mask);

                if (!ip.Mask.Incorrect)
                {
                    DeciminalData.Text = ip.GetDecIPSpecs();
                    BinaryData.Text = ip.GetBinIPSpecs();
                    NetIndexLabel.Content = "Индекс сети: " + ip.NetIndex;

                    if (ip.NetIndex < 9)
                        NetworkClassLabel.Content = "Класс сети: A";
                    else if (ip.NetIndex < 16)
                        NetworkClassLabel.Content = "Класс сети: B";
                    else
                        NetworkClassLabel.Content = "Класс сети: C";
                }
                else
                {
                    DeciminalData.Text = "МАСКА НЕКОРРЕКТНА!!!";
                    BinaryData.Text = "МАСКА НЕКОРРЕКТНА!!!";
                }
            }
            catch
            {
                MessageBox.Show("НЕВЕРНЫЕ ДАННЫЕ!!!");
            }
        }
    }
}
