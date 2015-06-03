using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Management;

namespace ResourceAnalyzer {
    public static class GeneralA {
        public static string ShowNames() {
            string info=";";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2","SELECT * FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            //Thread.Sleep(300);
            info += (string)collection.Cast<ManagementBaseObject>().First()["Name"];
            info += ";" + (string)collection.Cast<ManagementBaseObject>().First()["Model"];
            info += ";" + (string)collection.Cast<ManagementBaseObject>().First()["Manufacturer"];
            info += ";" + collection.Cast<ManagementBaseObject>().First()["NumberOfLogicalProcessors"].ToString();
            info += ";" + (string)collection.Cast<ManagementBaseObject>().First()["PrimaryOwnerName"];
            info += ";" + (string)collection.Cast<ManagementBaseObject>().First()["SystemType"];
            info += ";" + (string)collection.Cast<ManagementBaseObject>().First()["Workgroup"];
            //Console.WriteLine(info);
            return info;
        }

        public static string ShowBIOS(){
            string info = ";";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2","SELECT * FROM Win32_BIOS"); 
            ManagementObjectCollection collection = searcher.Get();
            info += (string)collection.Cast<ManagementBaseObject>().First()["Name"];
            info += ";" + (string)collection.Cast<ManagementBaseObject>().First()["Manufacturer"];
            return info;
        }

        public static string CPUTemp() {
            string info = " ";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI","SELECT * FROM MSAcpi_ThermalZoneTemperature");
            foreach(ManagementObject queryObj in searcher.Get()) {
                Double temp = Convert.ToDouble(queryObj["CurrentTemperature"].ToString());
                temp = (temp - 2732) / 10.0;
                info = temp.ToString() + " °C";
            }
            return info;
        }

        public static string Video() {
            string info = " ";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
            foreach(ManagementObject queryObj in searcher.Get()) {
                info = (string)queryObj["Name"];
                info += ";" + (Convert.ToDouble(queryObj["AdapterRAM"]) / 1024 / 1024).ToString();
            }
            return info;
        }
    }
}
