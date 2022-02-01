using System;
namespace InfoPC
{
    public class FreeDiskSpaceViewModel
    {
        public double FreeDiskSpace { get; set; }
        public string DiskName { get; set; }
        public FreeDiskSpaceViewModel(string diskName, double freeDiskSpace)
        {
            DiskName = diskName;
            FreeDiskSpace = Math.Round(freeDiskSpace, 1);
        }
    }
}
