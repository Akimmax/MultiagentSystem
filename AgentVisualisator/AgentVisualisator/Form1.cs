using System;
//using System.Drawing;
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
            drowOnForm = new DrowCallDelegate(DrawPointOnForm);

            //=============================Graphics
            drowOnChart = new DrowOnChartDelegate(DrawPointOnChart);
            drowOnChart3D = new DrowOnChartDelegate3D(DrawPointOnChart2);
            chartHelper = new Chart3DGraphicsVisualisator();
            chartHelper.prepare3dChart(this.chart1, this.chartArea1, maxSeriesNumber);

        }
        public delegate void SafeCallDelegate(string text);//TODO 
        public SafeCallDelegate myDelegate;//TODO 
        public delegate void DrowCallDelegate(Point p);
        public DrowCallDelegate drowOnForm;


        //System.Drawing.Bitmap image1 = new System.Drawing.Bitmap(@"D:\University\4.png", true);
        //=============================Graphics
        public delegate void DrowOnChartDelegate(Point p, int seriesId);
        public delegate void DrowOnChartDelegate3D(System.Windows.Media.Media3D.Point3D p, int seriesId);
        public DrowOnChartDelegate3D drowOnChart3D;
        public DrowOnChartDelegate drowOnChart;
        Chart3DGraphicsVisualisator chartHelper;
        int maxSeriesNumber = 10;


        public void DrowOnForm(Point p, string name, int step)
        {
            System.Drawing.Brush aBrush = (System.Drawing.Brush)System.Drawing.Brushes.Red;
            System.Drawing.Graphics g = CreateGraphics();
            g.FillRectangle(aBrush, (int)p.X, (int)p.Y, 3, 3);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void DrawPointOnForm(Point p)//TODO Remove we have 3d 
        {
            System.Drawing.Color newColor = System.Drawing.Color.FromName("SlateBlue");//Color.FromArgb(pixelColor.R, 0, 0);
            //image1.SetPixel((int)p.X, (int)p.Y, newColor);
            //pictureBox1.Image = image1;
        }

        private void runWriteTextSafe2(string text)
        {
            //textBox1.Text = text;//==>TODO remove
            //textBox1.Text += (text +"\r\n");
            listBox1.Items.Add(text);
        }

        private void DrawPointOnChart(Point p, int seriesId)
        {


            chartHelper.AddXY3d(this.chart1.Series[seriesId], p.X, p.Y, 0);
            string mess = "Drown on chart " + seriesId + " " + p.X + " " + p.Y;
            runWriteTextSafe2(mess);
        }

        private void DrawPointOnChart2(System.Windows.Media.Media3D.Point3D p, int seriesId)
        {
            //!!! IMPORTANT On chart Y is Z, Z is Y. Because we should swap params
            chartHelper.AddXY3d(this.chart1.Series[seriesId], p.X, p.Z, p.Y);
            string mess = "Drown on chart 3d" + seriesId + " " + p.X + " " + p.Y + " " + p.Z;
            runWriteTextSafe2(mess);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image = image1;
            System.Drawing.Color newColor = System.Drawing.Color.FromName("SlateBlue");
            //image1.SetPixel(100, 150, newColor);


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
            //var t4 = Task.Run(() => tcpAgent.RunClient(IPAddress.Parse("127.0.0.1"), 38243, 4));



            //try
            //{
            //   //t1.Wait();
            //}
            //catch (AggregateException ae)
            //{
            //    foreach (var ec in ae.InnerExceptions)
            //    {
            //        // Handle the custom exception.
            //        if (ec is Exception)
            //        {
            //            Console.WriteLine(ec.Message);
            //        }
            //        // Rethrow any other exception.
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //}
        }

        private void chart1_PostPaint(object sender, System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs e)
        {
            Chart chart = sender as Chart;
            if (chart.Series.Count < 1) return;
            if (chart.Series[0].Points.Count < 1) return;

            chartHelper.HandlePaintEventForChart(chart, e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
