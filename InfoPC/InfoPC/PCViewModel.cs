using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace InfoPC
{
    public class PCViewModel : BaseViewModel
    {
        private List<string> _ipv4Adress;
        private List<string> _ipv6Adress;
        private List<CheckBoxAdapterItem> _nameAdapter = new List<CheckBoxAdapterItem>();
        private List<FreeDiskSpaceViewModel> _freeDiskSpace = new List<FreeDiskSpaceViewModel>();
        private string _pcName;
        private string _userName;
        private string _domainName;
        private string _productVersion;
        private string _buildVersionOS;
        private bool _isChecked;
        private static Timer _timer;
        public List<FreeDiskSpaceViewModel> FreeDiskSpace
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
        public string BuildVersionOS
        {
            get => _buildVersionOS;
            set
            {
                _buildVersionOS = value;
                OnPropertyChanged();
            }
        }
        public List<string> Ipv4Adress
        {
            get => _ipv4Adress;
            set
            {
                _ipv4Adress = value;
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
        public List<CheckBoxAdapterItem> NameAdapter
        {
            get => _nameAdapter;
            set
            {
                _nameAdapter = value;
                OnPropertyChanged();
            }
        }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public PCViewModel()
        {
            UpdateInfo();

            _timer = new Timer(Callback, null, 1000 * 5, Timeout.Infinite);
            CopyUserNameCommand = new RelayCommand(CopyUserNameExecute, CopyUserNameExecute => PcName != null);
            CopyComputerNameCommand = new RelayCommand(CopyComputerNameExecute, CopyComputerNameExecute => PcName != null);
            CopyDomainNameCommand = new RelayCommand(CopyDomainNameExecute, CopyDomainNameExecute => DomainName != null);
            CopyIPv4Command = new RelayCommand(CopyIPv4Execute, CopyIPv4Execute => Ipv4Adress.Count != 0);
            CopyIPv6Command = new RelayCommand(CopyIPv6Execute, CopyIPv4Execute => Ipv6Adress.Count != 0);
            CopyProductVersionCommand = new RelayCommand(CopyProductVersionExecute, CopyProductVersionExecute => ProductVersion != null);
            CloseWindowsCommand = new RelayCommand(CloseWindowExecute);
            ChangeStatusEthenetCommand = new RelayCommand(ChangeStatusEthenetExecute);

        }

        public RelayCommand CopyUserNameCommand { get; set; }
        public RelayCommand CopyComputerNameCommand { get; set; }
        public RelayCommand CopyDomainNameCommand { get; set; }
        public RelayCommand CopyIPv4Command { get; set; }
        public RelayCommand CopyIPv6Command { get; set; }
        public RelayCommand CopyProductVersionCommand { get; set; }
        public RelayCommand CloseWindowsCommand { get; set; }
        public RelayCommand ChangeStatusEthenetCommand { get; set; }
        private void GetFreeSpace()
        {
            var allDrives = DriveInfo.GetDrives();
            foreach (var drive in allDrives)
            {
                try
                {
                    FreeDiskSpace.Add(new FreeDiskSpaceViewModel(drive.Name, drive.AvailableFreeSpace / 1024 / 1024 / 1024));
                }
                catch (Exception ex)
                {

                }

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
            DomainName = Environment.UserDomainName;
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
            Application.Current.Shutdown();
        }
        private void ChangeStatusEthenetExecute(object arg)
        {
            foreach (ManagementObject item in GetAllAdapter().Get())
            {
                if (item["NetConnectionId"].Equals(arg.ToString()) && (bool)item.Properties["NetEnabled"].Value)
                {

                    item.InvokeMethod("Disable", null);

                }
                if (item["NetConnectionId"].Equals(arg.ToString()) && !(bool)item.Properties["NetEnabled"].Value)
                {
                    item.InvokeMethod("Enable", null);
                }
            }
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
            ProductVersion = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\FalconGaze\SecureTower", "ProductVersion", null);
        }
        private void GetBuildVersionOS()
        {
            var productName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            var displayVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "").ToString();
            var currentBild = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();
            BuildVersionOS = productName + ": " + displayVersion + " (" + currentBild + ")";
        }
        private void GetAdapterName()
        {
            foreach (var item in GetAllAdapter().Get())
            {
                if (((bool)item.Properties["NetEnabled"].Value) == true)
                {
                    IsChecked = true;

                }
                else
                {
                    IsChecked = false;
                }
                NameAdapter.Add(new CheckBoxAdapterItem(IsChecked, item["NetConnectionId"].ToString()));
            }
        }
        private ManagementObjectSearcher GetAllAdapter()
        {
            var wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            var searchProcedure = new ManagementObjectSearcher(wmiQuery);
            return searchProcedure;
        }
        private void UpdateInfo()
        {
            GetFreeSpace();
            GetUserName();
            GetPCName();
            GetDomainName();
            GetIPv4Adress();
            GetIPv6Adress();
            GetAdapterName();
            GetVersionNumber();
            GetBuildVersionOS();


        }

    }
}

