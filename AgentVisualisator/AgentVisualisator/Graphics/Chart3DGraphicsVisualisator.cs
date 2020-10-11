using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
namespace AgentVisualisator.Graphic
{
    class Chart3DGraphicsVisualisator
    {
        Chart3DGraphicsRenderer renderingHelper;

        const int pointwidth = 4;
        public Chart3DGraphicsVisualisator()
        {
            renderingHelper = new Chart3DGraphicsRenderer();
        }

        bool isDrawingLinesRequired = false;

        public void prepare3dChart(Chart chart, ChartArea chartArea, int maxSeriesNumber)//TODO Apply builder pattern
        {
            ConfigureChartArea(chartArea);
            AddSeriesCollections(chart, maxSeriesNumber);
            AddInitialData(chart);
        }

        void ConfigureChartArea(ChartArea ca)
        {
            // set the chartarea to 3D!
            ca.Area3DStyle.Enable3D = true;  //!!! In case of NullReference Exception on this line -  please check initialisation of this.chartArea1 in Designer file. it should be like this.chartArea1 = new ChartArea();

            ca.AxisX.Minimum = -250;
            ca.AxisY.Minimum = -250;
            ca.AxisX.Maximum = 250;
            ca.AxisY.Maximum = 250;
            ca.AxisX.Interval = 50;
            ca.AxisY.Interval = 50;
            ca.AxisX.Title = "X-Achse";
            ca.AxisY.Title = "Y-Achse";
            ca.AxisX.MajorGrid.Interval = 250;
            ca.AxisY.MajorGrid.Interval = 250;
            ca.AxisX.MinorGrid.Enabled = true;
            ca.AxisY.MinorGrid.Enabled = true;
            ca.AxisX.MinorGrid.Interval = 50;
            ca.AxisY.MinorGrid.Interval = 50;
            ca.AxisX.MinorGrid.LineColor = Color.LightSlateGray;
            ca.AxisY.MinorGrid.LineColor = Color.LightSlateGray;
        }

        void AddSeriesCollections(Chart chart, int maxSeries)
        {
            chart.Series.Clear();
            for (int i = 0; i < maxSeries; i++)
            {
                Series s = chart.Series.Add("S" + i.ToString("00"));
                s.ChartType = SeriesChartType.Bubble;   // this ChartType has a YValue array
                s.MarkerStyle = MarkerStyle.Circle;
                s["PixelPointWidth"] = "100";
                s["PixelPointGapDepth"] = "1";
            }
            chart.ApplyPaletteColors();
        }

        public int AddXY3d(Series s, double xVal, double yVal, double zVal)
        {
            int p = s.Points.AddXY(xVal, yVal, zVal);

            // the DataPoint are transparent to the regular chart drawing:
            s.Points[p].Color = Color.Transparent;
            return p;
        }

        public void AddInitialData(Chart chart)
        {
            // Adding some initial points to make chart with proper size and form from beginning
            AddXY3d(chart.Series[0], 0, 0, 0);
            AddXY3d(chart.Series[0], 200, 0, 0);
            AddXY3d(chart.Series[0], 200, 200, 0);
            AddXY3d(chart.Series[0], 200, 200, 200);
            //chartHelper.AddXY3d(chart.Series[0], 90, 20, 40);
            AddXY3d(chart.Series[1], 0, 0, 50);
            AddXY3d(chart.Series[2], 0, 0, 0);
            AddXY3d(chart.Series[3], 50, 0, 0);
            //AddXY3d(chart.Series[4], 0, 0, 50);
        }

        public void HandlePaintEventForChart(Chart chart, ChartPaintEventArgs e)
        {
            ChartArea ca = chart.ChartAreas[0];
            e.ChartGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            List<List<PointF>> data = new List<List<PointF>>();
            foreach (Series s in chart.Series)
            {
                var points = renderingHelper.GetPointsFrom3D(ca, s, s.Points.ToList(), e.ChartGraphics);
                data.Add(points);
            }
            renderingHelper.renderPoints(data, e.ChartGraphics.Graphics, chart, pointwidth);   // pick one!

            if (isDrawingLinesRequired)
                renderingHelper.renderLines(data, e.ChartGraphics.Graphics, chart, true);  // pick one!            
        }    
    }
}
