using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;

namespace ResourceAnalyzer {
    public class Processes {
        private StringCollection _processes;

        [DllImport("Psapi.dll", SetLastError = true)]
        static extern bool EnumProcesses(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] UInt32[] processIds,
            UInt32 arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out UInt32 bytesCopied);

        public Processes(){
            _processes = new StringCollection();
            GetProc();
        }

        private void GetProc() {
            UInt32 arraySize = 150;
            UInt32 arrayBytesSize = arraySize * sizeof(UInt32);
            UInt32[] processIds = new UInt32[arraySize];
            UInt32 bytesCopied;

            EnumProcesses(processIds, arrayBytesSize, out bytesCopied);

            UInt32 numIdsCopied = bytesCopied >> 2;
            Console.WriteLine("Number " + numIdsCopied.ToString());

            for(UInt32 index = 0; index < numIdsCopied; index++) {
                this._processes.Add(GetProcessName((int)processIds[index]) + " " + processIds[index].ToString());
            }
        }

        private string GetProcessName(int id) {
            return Process.GetProcessById(id).ProcessName;
        }

        public StringCollection GetInfo() {
            return _processes;
        }
    }
}
