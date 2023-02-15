using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPAdress
{
    class Extension
    {
        public static IEnumerable<string> chunkSplit(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
    }

    public class IPAddress
    {
        public IPAddress() { }
        public IPAddress(string[] binAddr)
        {
            Dec = binAddr.Take(4).Select(i => Convert.ToByte(i, 2)).ToArray();
            Bin = binAddr.Take(4).ToArray();
        }
        public IPAddress(byte[] addr)
        {
            Dec = addr.Take(4).ToArray();
            Bin = addr
                .Take(4)
                .Select(x => Convert.ToString(x, 2))
                .Select(x => ((x.Length < 8) ? String.Concat(Enumerable.Repeat("0", 8 - x.Length)) + x : x))
                .ToArray();
        }

        public void Print(bool bin = false)
        {
            if (bin)
                Console.WriteLine(string.Join(".", Bin));
            else
                Console.WriteLine(string.Join(".", Dec));
        }

        public string GetStr(bool bin = false)
        {
            if (bin)
                return string.Join(".", Bin);
            else
                return string.Join(".", Dec);
        }

        public byte[] Dec { get; private set; } = new byte[4]; //Десятичная форма
        public string[] Bin { get; private set; } = new string[4]; //Двоичная форма
                                                                   //4 - столько элементов в массиве, ибо IP состоит из 4-ох 8-ми битных сегментов
    }
    public class IPMask : IPAddress
    {
        public IPMask(byte[] mask) : base(mask)
        {
            byte unitCount = 0;
            bool zeroFound = false;

            for (byte i = 0; i < Bin.Length; i++)
            {
                foreach (var binNum in Bin[i])
                {
                    if (zeroFound && binNum == '1') { Incorrect = true; break; }
                    else if (binNum == '1') unitCount++;
                    else if (!zeroFound) { zeroFound = true; FirstZeroIndex = i; }
                }
            }
            ZeroCount = (byte)(32 - unitCount);
        }

        public bool Incorrect { get; private set; } = false;
        public byte ZeroCount { get; private set; }
        public byte FirstZeroIndex { get; private set; }
    }

    public class IPv4
    {
        public IPv4(byte[] addr, byte[] mask)
        {
            IP = new IPAddress(addr);
            Mask = new IPMask(mask);
            NetIndex = (byte)(32 - Mask.ZeroCount);
            MaxHosts = (int)Math.Pow(2, Mask.ZeroCount) - 2;
            CalculateNetworkAddress();
            CalculateHostMin();
            CalculateHostMax();
        }

        private void CalculateNetworkAddress()
        {
            string[] temp = { "", "", "", "" };

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                    if (Mask.Bin[i][j] == '1')
                        temp[i] += IP.Bin[i][j];
                if (temp[i].Length < 8)
                    temp[i] += String.Concat(Enumerable.Repeat("0", 8 - temp[i].Length));
            }

            NetworkAddress = new IPAddress(temp);
        }
        private void CalculateHostMin()
        {
            StringBuilder sbNetAddr = new StringBuilder(NetworkAddress.GetStr(true));
            sbNetAddr[sbNetAddr.Length - 1] = '1';
            string[] hostMin = sbNetAddr.ToString().Split('.');
            HostMin = new IPAddress(hostMin);
        }
        private void CalculateHostMax()
        {
            byte firstZeroIndex = (byte)string.Join("", Mask.GetStr(true).Split('.')).IndexOf('0');
            StringBuilder maxHost = new StringBuilder(string.Join("", IP.GetStr(true).Split('.')));

            for (int i = firstZeroIndex; i < maxHost.Length; i++)
                maxHost[i] = '1';
            Broadcast = new IPAddress(string.Join(".", Extension.chunkSplit(maxHost.ToString(), 8)).Split('.'));
            maxHost[maxHost.Length - 1] = '0';
            string[] hostMax = (string.Join(".", Extension.chunkSplit(maxHost.ToString(), 8))).Split('.');
            HostMax = new IPAddress(hostMax);
        }

        public void PrintSpecs()
        {

            if (!Mask.Incorrect)
            {
                Console.WriteLine($"IP/{NetIndex}:\t\t {IP.GetStr()}\t\t {IP.GetStr(true)}\n" +
                                  $"Маска:\t\t {Mask.GetStr()}\t\t {Mask.GetStr(true)}\n" +
                                  $"Адрес сети:\t {NetworkAddress.GetStr()}\t\t {NetworkAddress.GetStr(true)}\n" +
                                  $"Первый узел:\t {HostMin.GetStr()}\t\t {HostMin.GetStr(true)}\n" +
                                  $"Последний узел:\t {HostMax.GetStr()}\t {HostMax.GetStr(true)}\n" +
                                  $"Широковещ-ый\t {Broadcast.GetStr()}\t {Broadcast.GetStr(true)}\n" +
                                  $"Макс. хостов:\t {MaxHosts}\n");
            }
            else
                Console.WriteLine("Маска некорректна!");
        }


        public IPAddress IP;
        public IPMask Mask;
        public int MaxHosts = 0;
        public byte NetIndex = 0; //Индекс сети - это количество единиц в маске
        public IPAddress HostMin { get; private set; }
        public IPAddress HostMax { get; private set; }
        public IPAddress Broadcast { get; private set; }
        public IPAddress NetworkAddress { get; private set; }
    }

    //public static void Main()
    //{
    //    int j = 0;
    //    byte[] inputIP = new byte[4];
    //    Console.Write("Введите IP: ");
    //    foreach (var ipSect in Console.ReadLine().Split('.').Select(byte.Parse))
    //        inputIP[j++] = ipSect;

    //    j = 0;
    //    byte[] inputMask = { 255, 240, 0, 0 };
    //    Console.Write("Введите маску: ");
    //    foreach (var maskSect in Console.ReadLine().Split('.').Select(byte.Parse))
    //        inputMask[j++] = maskSect;

    //    IPv4 ip = new IPv4(inputIP, inputMask);
    //    ip.PrintSpecs();
    //    Console.WriteLine("Нажмите любую кнопку...");
    //    Console.ReadKey();
    //}
}