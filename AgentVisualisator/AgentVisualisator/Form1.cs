using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Net;
using AgentVisualisator.Graphic;
using System.Windows.Forms.DataVisualization.Charting;

namespace AgentVisualisator
{
    public partial class Form1 : Form
    {
        //TODO Remove unnesesary delegates and event handlers
        public Form1()
        {
            InitializeComponent();
            myDelegate = new SafeCallDelegate(runWriteTextSafe2);
            drowOnChart3D = new DrowOnChartDelegate3D(DrawPointOnChart2);
            chartHelper = new Chart3DGraphicsVisualisator();
            chartHelper.prepare3dChart(this.chart1, this.chartArea1, maxSeriesNumber);

        }
        public delegate void SafeCallDelegate(string text);//TODO  Rename
        public SafeCallDelegate myDelegate;//TODO  Rename 
        public delegate void DrowOnChartDelegate3D(System.Windows.Media.Media3D.Point3D p, int seriesId);
        public DrowOnChartDelegate3D drowOnChart3D;//TODO  Rename 

        Chart3DGraphicsVisualisator chartHelper;
        int maxSeriesNumber = 10;

        private void runWriteTextSafe2(string text)//TODO  Rename 
        {
            listBox1.Items.Add(text);
        }

        private void DrawPointOnChart2(System.Windows.Media.Media3D.Point3D p, int seriesId)//TODO  Rename 
        {
            //!!! IMPORTANT On chart Y is Z, Z is Y. Because we should swap params
            chartHelper.AddXY3d(this.chart1.Series[seriesId], p.X, p.Z, p.Y);
            string mess = "Drown on chart 3d" + seriesId + " " + p.X + " " + p.Y + " " + p.Z;
            runWriteTextSafe2(mess);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var tcpAgent = new TCPClient(0);
            tcpAgent.form = this;
            string ip = "127.0.0.1";
            string ip2 = "127.0.0.1";
            string ip3 = "127.0.0.1";
            int port1 = 38245;
            int port2 = 38246;
            int port3 = 38244;

            var t2 = Task.Run(() => tcpAgent.RunClient(IPAddress.Parse(ip), port1, 1));
            var t1 = Task.Run(() => tcpAgent.RunClient(IPAddress.Parse(ip2), port2, 2));
            var t3 = Task.Run(() => tcpAgent.RunClient(IPAddress.Parse(ip3), port3, 3));            
        }

        //used for 3D drawing on chart
        private void chart1_PostPaint(object sender, ChartPaintEventArgs e)
        {
            Chart chart = sender as Chart;
            if (chart.Series.Count < 1) return;
            if (chart.Series[0].Points.Count < 1) return;

            chartHelper.HandlePaintEventForChart(chart, e);
        }

    }
}
