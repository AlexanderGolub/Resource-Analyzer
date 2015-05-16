using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;
//using System.IO;

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
            for(UInt32 index = 0; index < numIdsCopied; index++) {
                PrintProcessNameAndID((int)processIds[index]);
                Console.WriteLine("File Path: " + index.ToString() + " " + processIds[index].ToString());
            }
        }
        private static void PrintProcessNameAndID(int id) {
            Console.WriteLine(Process.GetProcessById(id).ProcessName);
        }
    }
}
