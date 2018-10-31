using System;
using System.Collections.Generic;
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

using System.Net;
using System.IO;
using System.Threading;

namespace chauffage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel vm = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm.ReadInfos();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            vm.StartAcq();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            vm.StopAcq();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            vm.StopAcq();
        }
    }

    public class ViewModel : NotifyBaseClass
    {
        string url_s7 = "http://192.168.43.15";
        string url_visitor = "http://10.128.0.200";

        List<string> lignes = new List<string>();

        Thread t;
        bool inProgress = false;

        public ViewModel()
        {
            t = new Thread(Execute);
            CptMeas = 0;
            CptTime = 0;
        }

        public void StartAcq()
        {
            inProgress = true;
            t.Start();
        }

        public void StopAcq()
        {
            inProgress = false;
            t.Join(2000);
        }

        public void Execute()
        {
            while (inProgress)
            {
                try
                {
                    ReadInfos();
                }
                catch(Exception e)
                { }

                for(int k=30; k>0; k--)
                {
                    if (!inProgress)
                    {
                        CptTime = -1;
                        break;
                    }
                    else
                    {
                        CptTime = k;
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        public void ReadInfos()
        {
            WebRequest wr;
            wr = WebRequest.Create(url_visitor);
            Stream s;
            using (var rep = wr.GetResponse())
            {
                using (s = rep.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(s);
                    lignes.Clear();
                    string ligne = "";
                    RawText = "";
                    while (ligne != null)
                    {
                        ligne = sr.ReadLine()?.Trim();
                        if (ligne != "" && ligne != null && !ligne.Contains("<"))
                        {
                            lignes.Add(ligne);
                            RawText += ligne + '\n';
                        }
                    }
                }
            }
            SaveToFile();
            CptMeas++;
        }

        void SaveToFile()
        {
            FileInfo f = new FileInfo("log_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + ".csv");
            if (!f.Exists)
            {
                //f.Create();
                string header = "time,";

                for(int k=0; k<10; k++)
                {
                    header += "temp" + k.ToString() + 
                        ",hum" + k.ToString() + 
                        ",cpt" + k.ToString() + ',';
                }

                for (int k=0; k<5; k++)
                {
                    header += "targ" + k.ToString() + 
                        ",meas" + k.ToString() + 
                        ",temp_start" + k.ToString() + 
                        ",temp_end" + k.ToString() + 
                        ",open" + k.ToString() + 
                        ",timeout" + k.ToString() + 
                        ",cpt" + k.ToString() + ',';
                }

                header = header.Remove(header.Length-1);
                header += '\n';

                File.AppendAllText(f.FullName, header);
                
            }

            if(lignes.Count == 15)
            {
                string l = DateTime.Now.Day.ToString() + "_" +
                   DateTime.Now.Month.ToString() + "_" +
                   DateTime.Now.Hour.ToString() + "_" +
                   DateTime.Now.Minute.ToString() + "_" +
                   DateTime.Now.Second.ToString() + ',';

                // Capt,id,temp,hum,cpt
                for(int k=0; k<10; k++)
                {
                    l += lignes[k].Split(',')[2] + ',' +
                        lignes[k].Split(',')[3] + ',' +
                        lignes[k].Split(',')[4] + ',';
                }
                
                // Valve,id,target,meas,start,end,open,timeout,cpt
                //   0    1    2     3    4    5    6      7    8
                for(int k=10; k<15; k++)
                {
                    l += lignes[k].Split(',')[2] + ',' +
                        lignes[k].Split(',')[3] + ',' +
                        lignes[k].Split(',')[4] + ',' +
                        lignes[k].Split(',')[5] + ',' +
                        lignes[k].Split(',')[6] + ',' +
                        lignes[k].Split(',')[7] + ',' +
                        lignes[k].Split(',')[8] + ',';
                }

                l = l.Remove(l.Length-1);
                l += '\n';

                File.AppendAllText(f.FullName, l);
            }
        }

        public int CptTime
        {
            get => _cptTime;
            set
            {
                if (value != _cptTime)
                {
                    _cptTime = value;
                    NotifyPropertiyChanged(nameof(CptTime));
                }
            }
        }
        private int _cptTime;   

        public int CptMeas
        {
            get => _cptMeas;
            set
            {
                if (value != _cptMeas)
                {
                    _cptMeas = value;
                    NotifyPropertiyChanged(nameof(CptMeas));
                }
            }
        }
        private int _cptMeas;       

        public string RawText
        {
            get => _raw_text;
            set
            {
                if (value != _raw_text)
                {
                    _raw_text = value;
                    NotifyPropertiyChanged(nameof(RawText));
                }
            }
        }
        private string _raw_text;

    }
}
