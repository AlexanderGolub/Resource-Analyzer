using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;


namespace ResourceAnalyzer {
    public class CPU {   

        ComTypes.FILETIME sysIdle, sysKernel, sysUser;
        long _runCount;

        
        public CPU() {
            sysIdle.dwHighDateTime = sysIdle.dwLowDateTime = 0;
            sysKernel.dwHighDateTime = sysKernel.dwLowDateTime = 0;
            sysUser.dwHighDateTime = sysUser.dwLowDateTime = 0;

            _runCount = 0;
        }

        public Int32 Usage() {
            ComTypes.FILETIME sysIdle2, sysKernel2, sysUser2;
            if(_runCount == 0) {
                CPUProc.GetSystemTimes(out sysIdle, out sysKernel, out sysUser);
                _runCount++;
                return 0;
            }
            
            Int32 cpuCopy;
            CPUProc.GetSystemTimes(out sysIdle2, out sysKernel2, out sysUser2);
            UInt64 sysKernelDiff =
                       CPUProc.SubtractTimes(sysKernel2, sysKernel);
            UInt64 sysUserDiff =
                       CPUProc.SubtractTimes(sysUser2, sysUser);
            UInt64 sysIdleDiff =
                       CPUProc.SubtractTimes(sysIdle2, sysIdle);

            sysIdle = sysIdle2;
            sysKernel = sysKernel2;
            sysUser = sysUser2;

            cpuCopy = (Int32)((sysKernelDiff + sysUserDiff - sysIdleDiff) * 100 / (sysKernelDiff + sysUserDiff));
            return cpuCopy;
        }
       
    }

    public static class CPUProc {
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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemTimes(out ComTypes.FILETIME lpIdleTime, out ComTypes.FILETIME lpKernelTime, out ComTypes.FILETIME lpUserTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, IntPtr dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetProcessTimes(IntPtr hProcess, out ComTypes.FILETIME lpCreationTime, out ComTypes.FILETIME lpExitTime,
            out ComTypes.FILETIME lpKernelTime, out ComTypes.FILETIME lpUserTime);

        public static void ProcUsage(TimeData info) {
            ComTypes.FILETIME procKernel2, procUser2, sysKernel2, sysUser2, creation, exit, idle;
            IntPtr handle = OpenProcess(ProcessAccessFlags.QueryInformation, false, info._id);
            if(info._runCount == 0) {
                GetProcessTimes(handle, out creation, out exit, out info._procKernel, out info._procUser);
                GetSystemTimes(out idle, out info._sysKernel, out info._sysUser);
                info._runCount++;
                return;
            }
            GetProcessTimes(handle, out creation, out exit, out procKernel2, out procUser2);
            GetSystemTimes(out idle, out sysKernel2, out sysUser2);

            UInt64 sysKernelDiff = SubtractTimes(sysKernel2, info._sysKernel);
            UInt64 sysUserDiff = SubtractTimes(sysUser2, info._sysUser);
            UInt64 procKernelDiff = SubtractTimes(procKernel2, info._procKernel);
            UInt64 procUserDiff = SubtractTimes(procUser2, info._procUser);

            UInt64 totalProc = procKernelDiff + procUserDiff;
            UInt64 totalSys = sysKernelDiff + sysUserDiff;

            if(totalSys > 0) {
                info._usage = (Int32)((procKernelDiff + procUserDiff) * 100 / (sysKernelDiff + sysUserDiff));
            }
        }

        public static UInt64 SubtractTimes(ComTypes.FILETIME a, ComTypes.FILETIME b) {
            UInt64 aInt =
            ((UInt64)(a.dwHighDateTime << 32)) | (UInt64)a.dwLowDateTime;
            UInt64 bInt =
              ((UInt64)(b.dwHighDateTime << 32)) | (UInt64)b.dwLowDateTime;

            return aInt - bInt;
        }
    }

    public class TimeData {
        public string _proc;
        public ComTypes.FILETIME _sysKernel, _sysUser, _procKernel, _procUser;
        public long _runCount;
        public IntPtr _id;
        public Int32 _usage;
        public TimeData() {
            _sysKernel.dwHighDateTime = _sysKernel.dwLowDateTime = 0;
            _sysUser.dwHighDateTime = _sysUser.dwLowDateTime = 0;
            _procKernel.dwHighDateTime = _procKernel.dwLowDateTime = 0;
            _procUser.dwHighDateTime = _procUser.dwLowDateTime = 0;
            _runCount = 0;
            _usage = 0;
        }
    }
}
