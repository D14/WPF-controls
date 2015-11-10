using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.ViewModel;

namespace Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineRuler.Model
{
    public class TimelineRulerMarkModel
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public DateTime Time { get; set; }


        public TimelineRulerMarkModel(double x1, double y1, double x2, double y2, DateTime time)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Time = time;
        }
    }
}
