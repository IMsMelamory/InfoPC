using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

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
            CopyUserNameCommand = new RelayCommand(CopyUserNameExecute);
            CopyComputerNameCommand = new RelayCommand(CopyComputerNameExecute);
        }
        
        public RelayCommand CopyUserNameCommand { get; set; }
        public RelayCommand CopyComputerNameCommand { get; set; }
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
        private void CopyUserNameExecute(object arg)
        {
            copyToClipboard(UserName);
        }
        private void CopyComputerNameExecute(object arg)
        {
            copyToClipboard(PcName);
        }
        private void copyToClipboard(string ipv6)
        {
            Clipboard.SetData(DataFormats.Text, (Object)ipv6);
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

