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

        public Form1()
        {
            InitializeComponent();
            writeText = new WriteTextDelegate(WriteText);
            drawOnChart = new DrawOnChartDelegate(DrawPointOnChart);

            visualisator = new Chart3DGraphicsVisualisator();
            visualisator.prepare3dChart(this.chart1, this.chartArea1, maxSeriesNumber);

        }
        public delegate void WriteTextDelegate(string text);
        public WriteTextDelegate writeText;
        public delegate void DrawOnChartDelegate(System.Windows.Media.Media3D.Point3D p, int seriesId);
        public DrawOnChartDelegate drawOnChart;

        Chart3DGraphicsVisualisator visualisator;
        int maxSeriesNumber = 10;

        private void WriteText(string text)
        {
            listBox1.Items.Add(text);
        }

        private void DrawPointOnChart(System.Windows.Media.Media3D.Point3D p, int seriesId)//TODO  Rename 
        {
            //!!! IMPORTANT On chart Y is Z, Z is Y. Because we should swap params
            visualisator.AddXY3d(this.chart1.Series[seriesId], p.X, p.Z, p.Y);
            string message = "Drown on chart 3d" + seriesId + " " + p.X + " " + p.Y + " " + p.Z;
            WriteText(message);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var tcpAgent = new TCPClient(id : 0);
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

            visualisator.HandlePaintEventForChart(chart, e);
        }

    }
}
