using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace InfoPC
{
    public class PCViewModel
    {
        public long FreeDiskSpace { get; set; }
        public string PcName { get; set; }
        public string UserName { get; set; }
        public List<string> Ipv4Adress { get; set; }
        public List<string> Ipv6Adress { get; set; }
        public PCViewModel()
        {
            
            GetFreeSpace();
            GetUserName();
            GetPCName();
            GetIPv4Adress();
            GetIPv6Adress();
        }

        private void GetFreeSpace()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            if (allDrives[0].IsReady == true)
            {
                FreeDiskSpace = allDrives[0].AvailableFreeSpace / 1024 / 1024/1024;
            }
        }
        private void GetUserName()
        {
            UserName = Environment.UserName;
        }
        private void GetPCName()
        {
            PcName = Environment.MachineName;
        }
        private void  GetIPv4Adress()
        {
            Ipv4Adress = (from ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList 
                          where ip.AddressFamily == AddressFamily.InterNetwork 
                          select ip.
                          ToString()).
                          ToList();
        }
        private void GetIPv6Adress()
        {
            Ipv6Adress = (from ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList 
                          where ip.AddressFamily == AddressFamily.InterNetworkV6 
                          select ip.
                          ToString()).
                          ToList();
        }
}

