using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResourceAnalyzer {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Processes a = new Processes();
            MEMORYSTATUSEX state = new MEMORYSTATUSEX();;
            Memory.GetMemory(state);
            this.APhys.Content = state.ullAvailPhys / 1024 / 1024 + " МБ";
            this.APageF.Content = state.ullAvailPageFile / 1024 / 1024 + " МБ";
            this.AVirt.Content = state.ullAvailVirtual / 1024 / 1024 + " МБ";
            this.TPhys.Content = state.ullTotalPhys / 1024 / 1024 + " МБ";
            this.TPageF.Content = state.ullTotalPageFile / 1024 / 1024 + " МБ";
            this.TVirt.Content = state.ullTotalVirtual / 1024 / 1024 + " МБ";
            long l = Memory.GetPhysMemory();
            this.IPhys.Content = l / 1024 + " МБ";

            StringCollection inf = new StringCollection();
            inf = a.GetInfo();
            char[] delimiterChars = { ' ' };
            foreach(var obj in inf) {
                string[] words = obj.Split(delimiterChars);
                int id = Convert.ToInt32(words[1]);
                string filler = Memory.ProcMemory((IntPtr)id);
                string[] words2 = filler.Split(delimiterChars);
                this.listview.Items.Add(new MyData { Proc = words[0], ID = words[1], PR = words2[0], WS = words2[1], NPP = words2[2] });
            }
        }
    }

    public class MyData {
        public string Proc { get; set; }
        public string ID { get; set; }
        public string PR { get; set; }
        public string WS { get; set; }
        public string NPP { get; set; }
    }
}
