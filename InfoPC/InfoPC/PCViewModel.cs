using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace InfoPC
{
    public class PCViewModel : BaseViewModel
    {
        private List<string> _ipv4Adress;
        private List<string> _ipv6Adress;
        private List<string> _nameAdapter = new List<string>();
        private long _freeDiskSpace;
        private string _pcName;
        private string _userName;
        private string _domainName;
        private string _productVersion;
        private static Timer _timer;
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
        public string ProductVersion
        {
            get => _productVersion;
            set
            {
                _productVersion = value;
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
        public List<string> NameAdapter
        {
            get => _nameAdapter;
            set
            {
                _nameAdapter = value;
                OnPropertyChanged();
            }
        }

        public PCViewModel()
        {
            UpdateInfo();
            _timer = new Timer(Callback, null, 1000 * 5, Timeout.Infinite);
            CopyUserNameCommand = new RelayCommand(CopyUserNameExecute, CopyUserNameExecute => PcName != null);
            CopyComputerNameCommand = new RelayCommand(CopyComputerNameExecute, CopyComputerNameExecute => PcName != null);
            CopyDomainNameCommand = new RelayCommand(CopyDomainNameExecute, CopyDomainNameExecute => DomainName != null );
            CopyIPv4Command = new RelayCommand(CopyIPv4Execute, CopyIPv4Execute => Ipv4Adress.Count != 0);
            CopyIPv6Command = new RelayCommand(CopyIPv6Execute, CopyIPv4Execute => Ipv6Adress.Count != 0);
            CopyProductVersionCommand = new RelayCommand(CopyProductVersionExecute, CopyProductVersionExecute  => ProductVersion != null );
            CloseWindowsCommand = new RelayCommand(CloseWindowExecute);
        }
        
        public RelayCommand CopyUserNameCommand { get; set; }
        public RelayCommand CopyComputerNameCommand { get; set; }
        public RelayCommand CopyDomainNameCommand { get; set; }
        public RelayCommand CopyIPv4Command { get; set; }
        public RelayCommand CopyIPv6Command { get; set; }
        public RelayCommand CopyProductVersionCommand { get; set; }
        public RelayCommand CloseWindowsCommand { get; set; }
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
            _timer.Change(1000 * 10, Timeout.Infinite);
        }
        private void CopyIPv4Execute(object arg)
        {
            CopyIPToClipboard(Ipv4Adress);
        }
        private void CopyIPv6Execute(object arg)
        {
            CopyIPToClipboard(Ipv6Adress);
        }
        private void CopyUserNameExecute(object arg)
        {
            copyToClipboard(UserName);
        }
        private void CopyComputerNameExecute(object arg)
        {
            copyToClipboard(PcName);
        }
        private void CopyDomainNameExecute(object arg)
        {
            copyToClipboard(DomainName);
        }
        private void CopyProductVersionExecute(object arg)
        {
            copyToClipboard(ProductVersion);
        }
        private void CloseWindowExecute(object arg)
        {
            Environment.Exit(0);
        }
        private void copyToClipboard(string copyText)
        {
            Clipboard.SetData(DataFormats.Text, copyText);
        }
        private void CopyIPToClipboard(List<string> myList)
        {
            copyToClipboard(string.Join(Environment.NewLine, myList.ToArray()));
        }
        private void GetVersionNumber()
        {
            ProductVersion = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\FalconGaze\SecureTower\ProductVersion", "Installed", null);
        }
        /* private void GetAdapterName()
         {
             NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();
             foreach (NetworkInterface adapter in networks)
             {
                 NameAdapter.Add(adapter.Name);
             }
         }*/
        private void UpdateInfo()
        {
            GetFreeSpace();
            GetUserName();
            GetPCName();
            GetDomainName();
            GetIPv4Adress();
            GetIPv6Adress();
            GetVersionNumber();
        }
    }
}

