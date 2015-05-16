using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;

namespace ResourceAnalyzer {
    public class Memory {
        MEMORYSTATUSEX lastState;
        const int PROCESS_WM_READ = 0x0010;
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

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [StructLayout(LayoutKind.Sequential, Size = 40)]
        private struct PROCESS_MEMORY_COUNTERS_EX {
            public uint cb;             // The size of the structure, in bytes (DWORD).
            public uint PageFaultCount;         // The number of page faults (DWORD).
            public uint PeakWorkingSetSize;     // The peak working set size, in bytes (SIZE_T).
            public uint WorkingSetSize;         // The current working set size, in bytes (SIZE_T).
            public uint QuotaPeakPagedPoolUsage;    // The peak paged pool usage, in bytes (SIZE_T).
            public uint QuotaPagedPoolUsage;    // The current paged pool usage, in bytes (SIZE_T).
            public uint QuotaPeakNonPagedPoolUsage; // The peak nonpaged pool usage, in bytes (SIZE_T).
            public uint QuotaNonPagedPoolUsage;     // The current nonpaged pool usage, in bytes (SIZE_T).
            public uint PagefileUsage;          // The Commit Charge value in bytes for this process (SIZE_T). Commit Charge is the total amount of memory that the memory manager has committed for a running process.
            public uint PeakPagefileUsage;      // The peak value in bytes of the Commit Charge during the lifetime of this process (SIZE_T).
            public uint PrivateUsage;
        }
        [DllImport("psapi.dll", SetLastError = true)]
        static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS_EX counters, uint size);

        [Flags]
        enum ProcessAccessFlags : uint {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, IntPtr dwProcessId);
        public void ProcMemory(IntPtr id) {
            //IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, id);
            //IntPtr processHandle = Process.GetProcessById(id).Handle;
            IntPtr hProcess = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VMRead, false, id);
            PROCESS_MEMORY_COUNTERS_EX memoryCounters;
            memoryCounters.cb = (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS_EX));
            if(GetProcessMemoryInfo(hProcess, out memoryCounters, memoryCounters.cb))
                Console.WriteLine("Mem" + memoryCounters.PageFaultCount + " " + memoryCounters.PrivateUsage + " " + memoryCounters.WorkingSetSize);
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
