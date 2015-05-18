using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;


namespace ResourceAnalyzer {
    class CPU {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetSystemTimes(
                       out ComTypes.FILETIME lpIdleTime,
                      out ComTypes.FILETIME lpKernelTime,
                       out ComTypes.FILETIME lpUserTime
                      );

        ComTypes.FILETIME _prevSysKernel;
        ComTypes.FILETIME _prevSysUser;
        ComTypes.FILETIME sysIdle, sysKernel, sysUser;

        TimeSpan _prevProcTotal;
        Int16 _cpuUsage;
        DateTime _lastRun;
        long _runCount;

        public CPU() {
            _cpuUsage = -1;
            _lastRun = DateTime.MinValue;
            _prevSysUser.dwHighDateTime = _prevSysUser.dwLowDateTime = 0;
            _prevSysKernel.dwHighDateTime = _prevSysKernel.dwLowDateTime = 0;

            sysIdle.dwHighDateTime = sysIdle.dwLowDateTime = 0;
            sysKernel.dwHighDateTime = sysKernel.dwLowDateTime = 0;
            sysUser.dwHighDateTime = sysUser.dwLowDateTime = 0;

            _prevProcTotal = TimeSpan.MinValue;
            _runCount = 0;
        }

        public short GetUsage() {
            short cpuCopy = _cpuUsage;
            if(Interlocked.Increment(ref _runCount) == 1) {
                if(!EnoughTimePassed) {
                    Interlocked.Decrement(ref _runCount);
                    return cpuCopy;
                }

                ComTypes.FILETIME sysIdle, sysKernel, sysUser;
                TimeSpan procTime;

                Process process = Process.GetProcessById(4);
                procTime = process.TotalProcessorTime;

                if(!GetSystemTimes(out sysIdle, out sysKernel, out sysUser)) {
                    Interlocked.Decrement(ref _runCount);
                    return cpuCopy;
                }

                if(!IsFirstRun) {
                    UInt64 sysKernelDiff =
                       SubtractTimes(sysKernel, _prevSysKernel);
                    UInt64 sysUserDiff =
                       SubtractTimes(sysUser, _prevSysUser);

                    UInt64 sysTotal = sysKernelDiff + sysUserDiff;

                    Int64 procTotal = procTime.Ticks - _prevProcTotal.Ticks;

                    if(sysTotal > 0) {
                        _cpuUsage = (short)((100.0 * procTotal) / sysTotal);
                    }
                }

                _prevProcTotal = procTime;
                _prevSysKernel = sysKernel;
                _prevSysUser = sysUser;

                _lastRun = DateTime.Now;

                cpuCopy = _cpuUsage;
            }
            Interlocked.Decrement(ref _runCount);

            return cpuCopy;
        }

        private UInt64 SubtractTimes(ComTypes.FILETIME a, ComTypes.FILETIME b) {
            UInt64 aInt =
            ((UInt64)(a.dwHighDateTime << 32)) | (UInt64)a.dwLowDateTime;
            UInt64 bInt =
              ((UInt64)(b.dwHighDateTime << 32)) | (UInt64)b.dwLowDateTime;

            return aInt - bInt;
        }

        private bool EnoughTimePassed {
            get {
                const int minimumElapsedMS = 250;
                TimeSpan sinceLast = DateTime.Now - _lastRun;
                return sinceLast.TotalMilliseconds > minimumElapsedMS;
            }
        }

        private bool IsFirstRun {
            get {
                return (_lastRun == DateTime.MinValue);
            }
        }

        public short Usage() {
            ComTypes.FILETIME sysIdle2, sysKernel2, sysUser2;
            if(_runCount == 0) {
                GetSystemTimes(out sysIdle, out sysKernel, out sysUser);
                _runCount++;
                return 0;
            }
            
            short cpuCopy;
            GetSystemTimes(out sysIdle2, out sysKernel2, out sysUser2);
            UInt64 sysKernelDiff =
                       SubtractTimes(sysKernel2, sysKernel);
            UInt64 sysUserDiff =
                       SubtractTimes(sysUser2, sysUser);
            UInt64 sysIdleDiff =
                       SubtractTimes(sysIdle2, sysIdle);

            sysIdle = sysIdle2;
            sysKernel = sysKernel2;
            sysUser = sysUser2;

            cpuCopy = (short)((sysKernelDiff + sysUserDiff - sysIdleDiff) * 100 / (sysKernelDiff + sysUserDiff));
            return cpuCopy;
        }
    }
}
