using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace InfoPC
{
    public class PCViewModel : BaseViewModel
    {
        private List<string> _ipv4Adress;
        private List<string> _ipv6Adress;
        private long _freeDiskSpace;
        private string _pcName;
        private string _userName;
        private string _domainName;

        public long FreeDiskSpace
        {
            get => _freeDiskSpace;
            set
            {
                _freeDiskSpace = value;
                OnPropertyChanged();
            }
        }
        public string PcName
        {
            get => _pcName;
            set
            {
                _pcName = value;
                OnPropertyChanged();
            }
        }
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
        public string DomainName
        {
            get => _domainName;
            set
            {
                _domainName = value;
                OnPropertyChanged();
            }
        }
        public List<string> Ipv4Adress
        {
            get => _ipv4Adress;
            set
            {
                _ipv4Adress  = value;
                OnPropertyChanged();
            }
        }
        public List<string> Ipv6Adress
        {
            get => _ipv6Adress;
            set
            {
                _ipv6Adress = value;
                OnPropertyChanged();
            }
        }
        private static Timer _timer;
        public PCViewModel()
        {
            UpdateInfo();
            _timer = new Timer(Callback, null, 1000 * 5, Timeout.Infinite);
            UpdateInfoCommand = new RelayCommand(UpdateInfoExecute);
        }
        public RelayCommand UpdateInfoCommand { get; set; }
        private void GetFreeSpace()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            if (allDrives[0].IsReady == true)
            {
                FreeDiskSpace = allDrives[0].AvailableFreeSpace / 1024 / 1024 / 1024;
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
        private void GetDomainName()
        {
            PcName = Environment.UserDomainName;
        }
        private void GetIPv4Adress()
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
        private void Callback(Object state)
        {
            UpdateInfo();
        }
        private void UpdateInfoExecute(object arg)
        {
            UpdateInfo();
        }
        private void UpdateInfo()
        {
            GetFreeSpace();
            GetUserName();
            GetPCName();
            GetDomainName();
            GetIPv4Adress();
            GetIPv6Adress();
        }
    }
}

