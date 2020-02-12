using System.Collections.Generic;
using System.Windows.Media;

namespace WPFShapeChart.Models
{
    public class BarChartDataSeries
    {
        public string SeriesName { get; set; }
        public SolidColorBrush Color { get; set; }
        public List<BarChartDataPoint> DataPoints { get; set; }
        public double TargetLine { get; set; }
        public SolidColorBrush BetterThanTargetColor { get; set; } = Brushes.Green;
        public SolidColorBrush WorseThanTargetColor { get; set; } = Brushes.Red;
    }
}
