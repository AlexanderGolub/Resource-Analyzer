using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Windows.Shapes;

namespace ResourceAnalyzer {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private System.Windows.Threading.DispatcherTimer _dispatcherTimer;
        private System.Windows.Threading.DispatcherTimer _dispatcherTimer2;
        private System.Windows.Threading.DispatcherTimer _dispatcherTimer3;
        CPU _a;
        public MainWindow() {
            InitializeComponent();
            _a = new CPU();
            _dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(this.dispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 600);
            _dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
            _dispatcherTimer2.Tick += new EventHandler(this.dispatcherTimer2_Tick);
            _dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, 600);
            _dispatcherTimer3 = new System.Windows.Threading.DispatcherTimer();
            _dispatcherTimer3.Tick += new EventHandler(this.dispatcherTimer3_Tick);
            _dispatcherTimer3.Interval = new TimeSpan(0, 0, 0, 0, 600);

            this.MemTab.GotFocus += this.MemTab_GotFocus;
            this.MemTab.LostFocus += this.MemTab_LostFocus;
            ShowGenTab();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            Int32 totalUsage = _a.Usage();
            if(totalUsage <= 100) {
                this.CPUValue.Content = totalUsage.ToString() + "%";
            }
        }

        private void dispatcherTimer2_Tick(object sender, EventArgs e) {
            ShowCPU();
        }

        private void dispatcherTimer3_Tick(object sender, EventArgs e) {
            ShowHDD();
        }

        public void ShowCPU() {
            StringCollection inf = new StringCollection();
            Processes a = new Processes();
            List<TimeData> Times = new List<TimeData>();
            inf = a.GetInfo();
            char[] delimiterChars = { ' ' };
            try {
                foreach(var obj in inf) {
                    TimeData time = new TimeData();
                    string[] words = obj.Split(delimiterChars);
                    time._id = (IntPtr)Convert.ToInt32(words[1]);
                    CPUProc.ProcUsage(time);
                    time._proc = words[0];
                    Times.Add(time);
                }
                this.ProcNum.Content = a.GetNumber().ToString();
                //Console.WriteLine(a.GetNumber().ToString());
                Thread.Sleep(300);
                foreach(var obj in Times) {
                    CPUProc.ProcUsage(obj);
                    if(obj._usage != 0) {
                        this.CPUListViev.Items.Add(new CPUData { Proc = obj._proc, ID = obj._id.ToString(), Usage = obj._usage.ToString() });
                    }
                }
            }
            catch {

            }
        }

        private void ShowHDD() {
            string inf = HDD.WMIInf();
            char[] delimiterChars = { ';' };
            string[] words = inf.Split(delimiterChars);
            this.HDDIdle.Content = words[0] + " %";
            this.HDDRead.Content = words[1] + " %";
            this.HDDWrite.Content = words[2] + " %";
            
        }

        private void ShowMemory() {
            Processes a = new Processes();
            MEMORYSTATUSEX state = new MEMORYSTATUSEX();
            Memory.GetMemory(state);
            this.APhys.Content = state.ullAvailPhys / 1024 / 1024 + " МБ";
            this.APageF.Content = state.ullAvailPageFile / 1024 / 1024 + " МБ";
            this.AVirt.Content = state.ullAvailVirtual / 1024 / 1024 + " МБ";
            this.TPhys.Content = state.ullTotalPhys / 1024 / 1024 + " МБ";
            this.TPageF.Content = state.ullTotalPageFile / 1024 / 1024 + " МБ";
            this.TVirt.Content = state.ullTotalVirtual / 1024 / 1024 + " МБ";
            this.Percent.Content = state.dwMemoryLoad + " %";
            long l = Memory.GetPhysMemory();
            this.IPhys.Content = l / 1024 + " МБ";
            this.Resv.Content = (l / 1024 - (long)state.ullTotalPhys / 1024 / 1024) + " МБ" ;
            StringCollection inf = new StringCollection();
            inf = a.GetInfo();
            char[] delimiterChars = { ' ' };
            try {
                foreach(var obj in inf) {
                    string[] words = obj.Split(delimiterChars);
                    int id = Convert.ToInt32(words[1]);
                    string filler = Memory.ProcMemory((IntPtr)id);
                    string[] words2 = filler.Split(delimiterChars);
                    this.MemListView.Items.Add(new MemData { Proc = words[0], ID = words[1], PR = words2[0], WS = words2[1], NPP = words2[2], MST=words2[3] });
                }
            }
            catch {
                this.MemListView.Items.Add(new MemData { Proc = "NaN", ID = "NaN", PR = "NaN", WS = "NaN", NPP = "NaN", MST = "NaN" });
            }
        }

        private void MemTab_GotFocus(object sender, RoutedEventArgs e) {
            ShowMemory();
        }

        private void MemTab_LostFocus(object sender, RoutedEventArgs e) {
            this.MemListView.Items.Clear();           
        }

        private void CPUTab_GotFocus(object sender, RoutedEventArgs e) {
            this.CPUModelValue.Content = CPUProc.RegInfo(1);
            this.CPUNameValue.Content = CPUProc.RegInfo(2);
            _dispatcherTimer.Start();
            _dispatcherTimer2.Start();
        }

        private void CPUTab_LostFocus(object sender, RoutedEventArgs e) {
            _dispatcherTimer.Stop();
            _dispatcherTimer2.Stop();
        }

        private void HDDTab_GotFocus(object sender, RoutedEventArgs e) {
            _dispatcherTimer3.Start();
            StringCollection output = new StringCollection();
            output = HDD.Show();
            char[] delimiterChars = { '_' };
            foreach(var obj in output) {
                string[] words = obj.Split(delimiterChars);
                this.HDDListView.Items.Add(new HDDData { Disk = words[0], Name = words[1], FS = words[2], Av = words[3], Tot = words[4] });
            }
            char[] delimiterChars2 = { ';' };
            string inf = HDD.WMIInf2();
            string[] words1 = inf.Split(delimiterChars2);
            this.IFType.Content = words1[0];
            this.HDDModel.Content = words1[1];
            this.PartNum.Content = words1[2];
            this.SerNum.Content = words1[3];
        }

        private void HDDTab_LostFocus(object sender, RoutedEventArgs e) {
            this.HDDListView.Items.Clear();
            _dispatcherTimer3.Stop();
        }

        private void GenTab_GotFocus(object sender, RoutedEventArgs e) {
            ShowGenTab();
        }

        private void GenTab_LostFocus(object sender, RoutedEventArgs e) {
            //this.GenTab.Items.Clear();
        }

        private void ShowGenTab() {
            string inf = GeneralA.ShowNames();
            char[] delimiterChars = { ';' };
            string[] words = inf.Split(delimiterChars);
            this.PCName.Content = words[1];
            this.ModelOfPc.Content = words[2];
            this.Manufact.Content = words[3];
            this.LogCPU.Content = words[4];
            this.PUName.Content = words[5];
            this.SysType.Content = words[6];
            this.WG.Content = words[7];
            inf = GeneralA.ShowBIOS();
            string[] words2 = inf.Split(delimiterChars);
            this.BIOSName.Content = words2[1];
            this.BIOSMan.Content = words2[2];
            this.CPUTemp.Content = GeneralA.CPUTemp();
            inf = GeneralA.Video();
            string[] words3 = inf.Split(delimiterChars);
            this.GPU.Content = words3[0];
            this.GPURAM.Content = words3[1] + " МБ";
        }


    }

    public class MemData {
        public string Proc { get; set; }
        public string ID { get; set; }
        public string PR { get; set; }
        public string WS { get; set; }
        public string NPP { get; set; }
        public string MST { get; set; }
    }

    public class HDDData {
        public string Disk { get; set; }
        public string Name { get; set; }
        public string FS { get; set; }
        public string Av { get; set; }
        public string Tot { get; set; }
    }

    public class CPUData {
        public string Proc { get; set; }
        public string ID { get; set; }
        public string Usage { get; set; }
    }
}
