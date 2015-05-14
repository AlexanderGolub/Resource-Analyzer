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

        [StructLayout(LayoutKind.Sequential)]
        private class MEMORYSTATUSEX {
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
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        public void GetMemory(){ 
            GlobalMemoryStatusEx(lastState);
            double d = (double)lastState.ullAvailPhys;
            d = d / 1024 / 1024;
            MessageBox.Show(d.ToString("0.00"));
        }
    }
}
