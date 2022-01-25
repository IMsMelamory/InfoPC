
namespace InfoPC
{
    public class FreeDiskSpaceViewModel
    {
        public long FreeDiskSpace { get; set; }
        public string DiskName { get; set; }
        public FreeDiskSpaceViewModel(string diskName, long freeDiskSpace)
        {
            DiskName = diskName;
            FreeDiskSpace = freeDiskSpace;
        }
    }
}
