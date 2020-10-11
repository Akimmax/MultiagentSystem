using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
namespace AgentVisualisator.Graphic
{
    class Chart3DGraphicsRenderer
    {       
        public void renderLines(List<List<PointF>> data, Graphics graphics, Chart chart, bool curves)
        {
            for (int i = 0; i < chart.Series.Count; i++)
            {
                if (data[i].Count > 1)
                    using (Pen pen = new Pen(Color.FromArgb(64, chart.Series[i].Color), 2.5f))
                    {
                        if (curves) graphics.DrawCurve(pen, data[i].ToArray());
                        else graphics.DrawLines(pen, data[i].ToArray());
                    }
            }
        }

        public void renderPoints(List<List<PointF>> data, Graphics graphics, Chart chart, float width)
        {
            for (int s = 0; s < chart.Series.Count; s++)
            {
                Series S = chart.Series[s];
                for (int p = 0; p < S.Points.Count; p++)
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(64, S.Color)))
                    {
                        //Draw the point 
                        graphics.FillEllipse(
                            brush,
                            data[s][p].X - width / 2,
                            data[s][p].Y - width / 2,
                            width,
                            width);
                    }
                }
            }
        }

        public List<PointF> GetPointsFrom3D(ChartArea ca, Series s,
                     List<DataPoint> dPoints, ChartGraphics cg)//Please don't anything here
        {
            //Get relative positions
            Point3D[] p3t = dPoints.Select(x => new Point3D(
                (float)ca.AxisX.ValueToPosition(x.XValue),
                (float)ca.AxisY.ValueToPosition(x.YValues[0]),
                (float)ca.AxisY.ValueToPosition(x.YValues[1])
                )).ToArray();

            //Preparation before GDI+ drawing
            ca.TransformPoints(p3t.ToArray());

            //Converts 3d points to absolute 2d coordinates
            var p2t = p3t.Select(x => cg.GetAbsolutePoint(new PointF(x.X, x.Y)));

            return p2t.ToList();
        }

    }
}
