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
        private UInt32 _number;

        [DllImport("Psapi.dll", SetLastError = true)]
        static extern bool EnumProcesses(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] UInt32[] processIds,
            UInt32 arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out UInt32 bytesCopied);

        public Processes(){
            _processes = new StringCollection();
            _number = 0;
            GetProc();
        }

        private void GetProc() {
            UInt32 arraySize = 150;
            UInt32 arrayBytesSize = arraySize * sizeof(UInt32);
            UInt32[] processIds = new UInt32[arraySize];
            UInt32 bytesCopied;

            EnumProcesses(processIds, arrayBytesSize, out bytesCopied);

            this._number = bytesCopied >> 2;
            //Console.WriteLine("Number " + _number.ToString());

            for(UInt32 index = 1; index < _number; index++) {
                this._processes.Add(GetProcessName((int)processIds[index]) + " " + processIds[index].ToString());
            }
        }

        private string GetProcessName(int id) {
            try {
                return Process.GetProcessById(id).ProcessName;
            }
            catch(System.ArgumentException) {
                return "exp";
            }
        }

        public StringCollection GetInfo() {
            return _processes;
        }

        public UInt32 GetNumber() {
            return _number;
        }
    }
}
