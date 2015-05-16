using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;

namespace ResourceAnalyzer {
    public static class Processes {
        [DllImport("Psapi.dll", SetLastError = true)]
        static extern bool EnumProcesses(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] UInt32[] processIds,
            UInt32 arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out UInt32 bytesCopied);

        public static void GetProc() {
            UInt32 arraySize = 120;
            UInt32 arrayBytesSize = arraySize * sizeof(UInt32);
            UInt32[] processIds = new UInt32[arraySize];
            UInt32 bytesCopied;

            EnumProcesses(processIds, arrayBytesSize, out bytesCopied);

            UInt32 numIdsCopied = bytesCopied >> 2;
            Console.WriteLine("Number " + numIdsCopied.ToString());
            Memory m = new Memory();
            for(UInt32 index = 0; /*index < numIdsCopied*/ index<10; index++) {
                PrintProcessNameAndID((int)processIds[index]);
                MessageBox.Show("File Path: " + index.ToString() + " " + processIds[index].ToString());
                m.ProcMemory((IntPtr)processIds[index]);
            }
        }
        private static void PrintProcessNameAndID(int id) {
            MessageBox.Show(Process.GetProcessById(id).ProcessName);
        }
    }
}
