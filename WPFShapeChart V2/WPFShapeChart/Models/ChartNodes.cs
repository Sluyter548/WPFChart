using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPFShapeChart.Models
{
    

        public class ChartNode
        {
            public double XPosition { get; set; }
            public double YPosition { get; set; }
            public string Name { get; set; }


        }

        public class ChartText : ChartNode
        {
            public string Text { get; set; }

            public FlowDirection FlowDirection { get; set; }
            public RotateTransform RenderTransform { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }

        public class ChartBar : ChartNode
        {

            public Brush Fill { get; set; }
            public Brush Stroke { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }

        }


        public class ChartLine : ChartNode
        {
            public Brush Fill { get; set; }
            public Brush Stroke { get; set; }
            public double X1Position { get; set; }
            public double X2Position { get; set; }
            public double Y1Position { get; set; }
            public double Y2Position { get; set; }
            public double Thickness { get; set; }

        }





    
}
