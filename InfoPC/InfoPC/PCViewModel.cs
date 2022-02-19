using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Windows.Input;
using System.Threading.Tasks;
using InfoPC.Helpers;

namespace InfoPC
{
    public class PCViewModel : BaseViewModel
    {

        private const string _serverLogsFilesPath = @"C:\ProgramData\Falcongaze SecureTower\Logs";
        private const string _agentHostLogsFiles = @"C:\ProgramData\Falcongaze SecureTower\EPA";
        private const string _tempLogsFilesPath = @"C:\Logs\ServerLogs";
        private const string _tempConsoleLogsFilesPath = @"C:\Logs\ConsoleLogs";
        private const string _tempAgentLogsFilesPath = @"C:\Logs\AgentLogs";
        private const string _zipArchiveServerLogs = @"C:\Logs\ServerLogs.zip";
        private const string _zipArchiveConsoleLogs = @"C:\Logs\ConsoleLogs.zip";
        private const string _zipArchiveAgentsLogs = @"C:\Logs\AgentsLogs.zip";
        private readonly string _consoleLogsFiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Falcongaze SecureTower");
        private readonly string _agentCssLogsFiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Falcongaze SecureTower");
        private ObservableCollection<string> _ipv4Adress;
        private ObservableCollection<string> _ipv6Adress;
        private ObservableCollection<CheckBoxAdapterItem> _nameAdapter = new ObservableCollection<CheckBoxAdapterItem>();
        private ObservableCollection<FreeDiskSpaceViewModel> _freeDiskSpace = new ObservableCollection<FreeDiskSpaceViewModel>();
        private string _pcName;
        private string _userName;
        private string _domainName;
        private string _productVersion;
        private string _buildVersionOS;
        private bool _isChecked;
        private static Timer _timer;
        public bool IsProduct { get; set; } = false;
        public ObservableCollection<FreeDiskSpaceViewModel> FreeDiskSpace
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
        public ObservableCollection<string> Ipv4Adress
        {
            get => _ipv4Adress;
            set
            {
                _ipv4Adress = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> Ipv6Adress
        {
            get => _ipv6Adress;
            set
            {
                _ipv6Adress = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<CheckBoxAdapterItem> NameAdapter
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
            GetPCName();
            GetDomainName();
            GetBuildVersionOS();
            UpdateInfo();
            _timer = new Timer(Callback, null, 1000 * 5, Timeout.Infinite);
            CopyToClipboardCommand = new RelayCommand(CopyToClipboardExecute);
            CloseWindowsCommand = new RelayCommand(CloseWindowExecute);
            ChangeStatusEthenetCommand = new RelayCommand(ChangeStatusEthenetExecute);
            CopyLogsCommand = new RelayCommand(CopyLogsExecute);
            ShowWindowServicesCommand = new RelayCommand(ShowWindowServicesExecute);


        }
        public ICommand CopyToClipboardCommand { get; set; }
        public ICommand CloseWindowsCommand { get; set; }
        public ICommand ChangeStatusEthenetCommand { get; set; }
        public ICommand CopyLogsCommand { get; set; }
        public ICommand ShowWindowServicesCommand { get; set; }
        private void GetFreeSpace()
        {
            var allDrives = DriveInfo.GetDrives();
            Application.Current.Dispatcher.Invoke(() =>
            {
                FreeDiskSpace.Clear();
                foreach (var drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        FreeDiskSpace.Add(new FreeDiskSpaceViewModel(drive.Name, drive.AvailableFreeSpace / Math.Pow(1024, 3)));
                    }

                }
            });
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
            Ipv4Adress = new ObservableCollection<string>(Dns.GetHostEntry(Dns.GetHostName()).AddressList.
                         Where(x => x.AddressFamily == AddressFamily.InterNetwork).
                         Select(x => x.ToString()).
                         ToList());
        }
        private void GetIPv6Adress()
        {
            Ipv6Adress = new ObservableCollection<string>(Dns.GetHostEntry(Dns.GetHostName()).AddressList.
                         Where(x => x.AddressFamily == AddressFamily.InterNetworkV6).
                         Select(x => x.ToString()).
                         ToList());
        }
        private void Callback(object state)
        {
            UpdateInfo();
            _timer.Change(5000 * 10, Timeout.Infinite);
        }
        private void CopyToClipboardExecute(object arg)
        {
            if (arg is ObservableCollection<string> stringArg)
            {
                Clipboard.SetDataObject(string.Join(Environment.NewLine, stringArg.ToArray()));
            }
            else
            {
                Clipboard.SetDataObject(arg.ToString());
            }
        }
        private void CloseWindowExecute(object arg)
        {
            Application.Current.Shutdown();
        }
        private void ShowWindowServicesExecute(object arg)
        {
            Services WindowServices = new Services();
            WindowServices.Show();
        }
        private void CopyLogsExecute(object arg)
        {
            try
            {
                Directory.Delete(@"C:\Logs", true);
            }
            catch
            {
                MessageBox.Show("Не удалось удалить папку. Занята другим процессом");
            }
            if (GetFilesName(_serverLogsFilesPath).Length > 0)
            {
                Directory.CreateDirectory(_tempLogsFilesPath);
                Task.Run(async () => await CopyAndArchiveFiles(GetFilesName(_serverLogsFilesPath), _tempLogsFilesPath, _zipArchiveServerLogs));
            }
            if (GetFilesName(_consoleLogsFiles).Length > 0)
            {
                Directory.CreateDirectory(_tempConsoleLogsFilesPath);
                Task.Run(async () => await CopyAndArchiveFiles(GetFilesName(_consoleLogsFiles), _tempConsoleLogsFilesPath, _zipArchiveConsoleLogs));
            }
            if (GetFilesName(_agentHostLogsFiles).Length > 0)
            {
                Directory.CreateDirectory(_tempAgentLogsFilesPath);
                Task.Run(async () => await CopyAndArchiveFiles(GetFilesName(_agentHostLogsFiles), _tempAgentLogsFilesPath, _zipArchiveAgentsLogs));
            }
            if (GetFilesName(_agentCssLogsFiles).Length > 0)
            {
                Directory.CreateDirectory(_tempAgentLogsFilesPath);
                Task.Run(async () => await CopyAndArchiveFiles(GetFilesName(_agentCssLogsFiles), _tempAgentLogsFilesPath, _zipArchiveAgentsLogs));
            }
        }
        private string[] GetFilesName(string pathFiles)
        {
            string[] noFiles = new string[0];
            return Directory.Exists(pathFiles) ? Directory.GetFiles(pathFiles, "*.log", SearchOption.AllDirectories) : noFiles;
        }
        private async Task CopyAndArchiveFiles(string[] files, string pathFiles, string pathArchive)
        {
            await Task.Delay(100);
            foreach (var filename in files)
            {
                File.Copy(filename, Path.Combine(pathFiles, Path.GetFileName(filename)), true);
            }
            ZipFile.CreateFromDirectory(pathFiles, pathArchive);
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
        private void GetVersionNumber()
        {
            ProductVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\FalconGaze\SecureTower", "ProductVersion", "")?.ToString();
            if (ProductVersion != null)
            {
                IsProduct = true;
            }
        }
        private void GetBuildVersionOS()
        {
            var productName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "")?.ToString();
            var displayVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "")?.ToString();
            var currentBild = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "")?.ToString();
            BuildVersionOS = $"{productName}: {displayVersion} ({currentBild })";
        }
        private void GetAdapterName()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                NameAdapter = new ObservableCollection<CheckBoxAdapterItem>();
                foreach (var item in GetAllAdapter().Get())
                {
                    IsChecked = (bool)item.Properties["NetEnabled"].Value;
                    NameAdapter.Add(new CheckBoxAdapterItem(IsChecked, item["NetConnectionId"].ToString()));
                }
            });
        }
        private ManagementObjectSearcher GetAllAdapter()
        {
            return new ManagementObjectSearcher(new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL"));
        }
        private void UpdateInfo()
        {
            GetFreeSpace();
            GetUserName();
            GetIPv4Adress();
            GetIPv6Adress();
            GetAdapterName();
            GetVersionNumber();
        }
    }
}

