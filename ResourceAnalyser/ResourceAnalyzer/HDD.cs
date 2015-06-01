using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ResourceAnalyzer {
    public static class HDD {
        [Flags]
        private enum FileSystemFeature : uint {
            CaseSensitiveSearch = 1,
            CasePreservedNames = 2,
            UnicodeOnDisk = 4,
            PersistentACLS = 8,
            FileCompression = 0x10,
            VolumeQuotas = 0x20,
            SupportsSparseFiles = 0x40,
            SupportsReparsePoints = 0x80,
            VolumeIsCompressed = 0x8000,
            SupportsObjectIDs = 0x10000,
            SupportsEncryption = 0x20000,
            NamedStreams = 0x40000,
            ReadOnlyVolume = 0x80000,
            SequentialWriteOnce = 0x100000,
            SupportsTransactions = 0x200000,
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static bool GetVolumeInformation(string RootPathName, StringBuilder VolumeNameBuffer, int VolumeNameSize, out uint VolumeSerialNumber,
          out uint MaximumComponentLength, out FileSystemFeature FileSystemFlags, StringBuilder FileSystemNameBuffer, int nFileSystemNameSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        [DllImport("kernel32.dll")]
        static extern uint GetLogicalDrives();

        public enum DriveType : uint {
            Unknown = 0,    //DRIVE_UNKNOWN
            Error = 1,        //DRIVE_NO_ROOT_DIR
            Removable = 2,    //DRIVE_REMOVABLE(floppy/ext HDD)
            Fixed = 3,        //DRIVE_FIXED
            Remote = 4,        //DRIVE_REMOTE(network)
            CDROM = 5,        //DRIVE_CDROM
            RAMDisk = 6        //DRIVE_RAMDISK
        }

        [DllImport("kernel32.dll")]
        public static extern DriveType GetDriveType(string lpRootPathName);

        public static StringCollection Show() {
            StringCollection info = new StringCollection();
            uint shift = 1;
            uint drives = GetLogicalDrives();

            for(int i = 0; i < 26; i++) {
                if((shift & drives) == 1) {
                    string send = (char)('A' + i) + ":\\";
                    info.Add(Compose(send));
                }
                drives >>= 1;
            }
            return info;
        }

        private static string Compose(string disk) {
            string info;
            StringBuilder volname = new StringBuilder(261);
            StringBuilder fsname = new StringBuilder(261);
            uint sernum, maxlen;
            FileSystemFeature flags;
            DriveType dt = GetDriveType(disk);
            if(dt == DriveType.Fixed) {
                if(!GetVolumeInformation(disk, volname, volname.Capacity, out sernum, out maxlen, out flags, fsname, fsname.Capacity))
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                string volnamestr = volname.ToString();
                string fsnamestr = fsname.ToString();
                Console.WriteLine(volnamestr + " " + fsnamestr);
                ulong FreeBytesAvailable;
                ulong TotalNumberOfBytes;
                ulong TotalNumberOfFreeBytes;

                bool success = GetDiskFreeSpaceEx(disk, out FreeBytesAvailable, out TotalNumberOfBytes, out TotalNumberOfFreeBytes);
                if(!success)
                    throw new System.ComponentModel.Win32Exception();

                info = disk + " " + volnamestr + " " + fsnamestr + " " + FreeBytesAvailable / 1024 / 1024 / 1024 + " " + TotalNumberOfBytes / 1024 / 1024 / 1024;
                return info;
            }
            if(dt == DriveType.CDROM) {
                info = disk + " " + "CD-ROM" + " " + "NaN" + " " + "NaN" + " " + "NaN";
                return info;
            }
            if(dt == DriveType.Remote) {
                info = disk + " " + "Remote" + " " + "NaN" + " " + "NaN" + " " + "NaN";
                return info;
            }
            if(dt == DriveType.Removable) {
                info = disk + " " + "Removable" + " " + "NaN" + " " + "NaN" + " " + "NaN";
                return info;
            }
            info = disk + " " + "NaN" + " " + "NaN" + " " + "NaN" + " " + "NaN";
            return info;
        }
    }
}
