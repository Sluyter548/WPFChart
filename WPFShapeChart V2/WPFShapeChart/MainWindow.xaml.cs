using System;
using System.Collections.Generic;
using System.Windows;
using WPFShapeChart.Models;
using WPFShapeChart.ViewModels;


namespace WPFShapeChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /*
            currently this is very ugly view behind code, I want to migrate this to another viewmodel + have a view with a menu select option
            But what would be the best to do this? 
            So we need to check how we can use page navigation in a good way using the MVVM model




            */


            /*
            var TestViewModel = new VeryBasicChartViewModel();

            var MyDataSeries = new BarChartDataSeries();

            MyDataSeries.TargetLine = 80;
            MyDataSeries.SeriesName = "Values";

            List<BarChartDataPoint> MyDataPoints = new List<BarChartDataPoint>();
            var RandomNumberGenerator = new Random();
            for (int DataPointCounter = 0; DataPointCounter < 25; DataPointCounter++)
            {
                var DataPoint = new BarChartDataPoint();
                DataPoint.Name = "PointA" + (DataPointCounter + 1).ToString();
                DataPoint.Value = Math.Ceiling(RandomNumberGenerator.NextDouble() * 50) + 50;
                MyDataPoints.Add(DataPoint);
            }

            MyDataSeries.DataPoints = MyDataPoints;

            TestViewModel.SetBarChartDataSeries(MyDataSeries);


            var myview = new WPFShapeChart.Views.VeryBasicBarChartView(TestViewModel);
            TestContentControl.Content = myview;

    */



        }
    }
}
