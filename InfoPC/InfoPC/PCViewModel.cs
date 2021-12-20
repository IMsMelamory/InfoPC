using System;
using System.IO;
using System.Net;
using System.Threading;

namespace InfoPC
{
    public class PCViewModel
    {
        public long FreeDiskSpace { get; set; }
        public string PcName { get; set; }
        public string UserName { get; set; }
        public string IpAdress { get; set; }
        public PCViewModel()
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(5);

            var timer = new Timer((e) =>
            {
                GetFreeSpace();
                GetUserName();
                GetPCName();
                GetIPAdress();
            }, null, startTimeSpan, periodTimeSpan);

            



        }

        private void GetFreeSpace()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            if (allDrives[0].IsReady == true)
            {
                FreeDiskSpace = allDrives[0].AvailableFreeSpace / 1024 / 1024;
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
        private void GetIPAdress()
        {
            IpAdress= Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
        }
    }
}

