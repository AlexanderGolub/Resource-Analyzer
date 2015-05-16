using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;

namespace ResourceAnalyzer {
    public class Memory {
        MEMORYSTATUSEX lastState;
        public Memory() {
            lastState = new MEMORYSTATUSEX();
        }

        
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public void GetMemory(){
            double div = 1024 * 1024;
            GlobalMemoryStatusEx(lastState);
            double d = (double)lastState.ullAvailPhys;
            d = d / div;
            MessageBox.Show(d.ToString("0.00"));
            long l;
            GetPhysicallyInstalledSystemMemory(out l);
            l = l / 1024 ;
            MessageBox.Show(l.ToString() + "MB of RAM installed.");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
        public class MEMORYSTATUSEX {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX() {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
        
}
