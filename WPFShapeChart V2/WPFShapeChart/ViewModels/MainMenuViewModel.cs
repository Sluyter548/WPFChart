using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFShapeChart.Models;

namespace WPFShapeChart.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private BasicChartViewModel _ChartViewModel = new BasicChartViewModel();
        
        
        public BasicChartViewModel ChartViewModel { get {
                return _ChartViewModel;
            } set
            {
                Set(ref _ChartViewModel, ChartViewModel);
            } }
        
        
        public MainMenuViewModel()
        {
            

        }
        private ICommand _RefreshCommand;

        public ICommand RefreshCommand { get
            {
                if (_RefreshCommand == null)
                {
                    _RefreshCommand = new RelayCommand(x => RefreshChart());
                }
                return _RefreshCommand;
            }set
            {
                _RefreshCommand = value;
            }
        }




        public void RefreshChart()
        {

            //ChartViewModel = new VeryBasicChartViewModel();

            var MyDataSeries = new BarChartDataSeries();

            MyDataSeries.TargetLine = 80;
            MyDataSeries.SeriesName = "Values";

            List<BarChartDataPoint> MyDataPoints = new List<BarChartDataPoint>();
            var RandomNumberGenerator = new Random();
            for (int DataPointCounter = 0; DataPointCounter < 10; DataPointCounter++)
            {
                var DataPoint = new BarChartDataPoint();
                DataPoint.Name = "Point " + (DataPointCounter + 1).ToString();
                DataPoint.Value = Math.Ceiling(RandomNumberGenerator.NextDouble() * 200) - 100;
                MyDataPoints.Add(DataPoint);
            }

            MyDataSeries.DataPoints = MyDataPoints;

            ChartViewModel.SetBarChartDataSeries(MyDataSeries);
            
            
            
            //var myview = new WPFShapeChart.Views.VeryBasicBarChartView(ChartViewModel);
            

            
            //shit, how do I handle this shit with views again? :S
            //lemme think...




        }












    }
}
