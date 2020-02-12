using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WPFShapeChart.Models;

namespace WPFShapeChart.ViewModels
{


    public class VeryBasicChartViewModel : ViewModelBase
    {

        public ObservableCollection<ChartNode> ChartNodesCollection { get; } = new ObservableCollection<ChartNode>();


        // not working, need to generate a collection that has a generic item in it with only Coords, have derived classes for the textblocks, shapes etc
        // that will need to go all in the collection and then we need to work with data templates in the view to get it all to work
        // saved a link in Chrome that has all I need to get this done

        //public Canvas ShapeCollection { get; }
        public int ChartWidth { get; set; } = 800;
        public int ChartHeight { get; set; } = 600;
        public int YTickmarkCount { get; set; } = 5;
        private BarChartDataSeries DataList { get; set; }

        public void SetBarChartDataSeries (BarChartDataSeries InputData)
        {
            DataList = InputData;
            CalculateChart();
        }



        public bool ForceYStartAtZero { get; set; } = false;
        /// <summary>
        /// 0 means bar will fill up entire column, 1 means the bars disappear in whitespace, standard 10%
        /// </summary>
        public double AddWhiteSpacePerBar { get; set; } = 0.1;



        private List<double> TickMarkIntervals;
        private double MinDataValue;
        private double MaxDataValue;
        private double ChartHeightDifference;
        private double BarWidth;
        private double BarHeightModifier;
        private double LowestTickMarkValue; // Y-Axis
        private double HigestTickMarkValue; // Y-Axis
        private double LengthForXAxis; // used for putting the bars in the right place + create a long enough X axis



        public VeryBasicChartViewModel()
        {
            

            // generate available tickmark list, is used to decide on what tickmark values are acceptable, only Y-Axis
            // maybe there is a better way but for now this is okayish enough
            
            TickMarkIntervals = new List<double>();
            for (int TenthPower = -5; TenthPower < 10; TenthPower++)
            {
                TickMarkIntervals.Add(1 * Math.Pow(10, TenthPower));
                TickMarkIntervals.Add(2 * Math.Pow(10, TenthPower));
                TickMarkIntervals.Add(3 * Math.Pow(10, TenthPower));
                TickMarkIntervals.Add(4 * Math.Pow(10, TenthPower));
                TickMarkIntervals.Add(5 * Math.Pow(10, TenthPower));

            }


        }

        public void CalculateChart()
        {
            ChartNodesCollection.Clear(); //empty collection to make room for the newly created collection, can later be updated to only remove updating parts with the names
            CalculateMinMaxDataValue();
            GenerateTickmarks();
            GenerateBarsInChart();
            AddAxis();
            AddTargetLine();


        }







        private void AddTargetLine()
        {
            if (DataList.TargetLine.Equals(0))
            {
                return;
            }
            else
            {
                var TargetLine = new ChartLine()
                {
                    Name = "XAxis",
                    X1Position = 0,
                    X2Position = LengthForXAxis - 100,
                    Y1Position = 0,//ChartHeight - 150,
                    Y2Position = 0,// ChartHeight - 150,
                    Stroke = Brushes.Black,
                    Thickness = 2,
                    XPosition = 100
                };


                if (ForceYStartAtZero)
                {
                    TargetLine.YPosition = (ChartHeight - 150) - DataList.TargetLine * BarHeightModifier - 2;
                }
                else
                {

                    TargetLine.YPosition = (ChartHeight - 150) - (DataList.TargetLine - LowestTickMarkValue) * BarHeightModifier - 2;
                }


                ChartNodesCollection.Add(TargetLine);

            }
        }



        private void CalculateMinMaxDataValue()
        {

            MinDataValue = DataList.DataPoints[0].Value;//have to assign first one because otherwise always zero
            foreach (var DataPoint in DataList.DataPoints)
            {
                if (DataPoint.Value < MinDataValue) { MinDataValue = DataPoint.Value; }
                if (DataPoint.Value > MaxDataValue) { MaxDataValue = DataPoint.Value; }
            }

            ChartHeightDifference = MaxDataValue - MinDataValue;


            if (ForceYStartAtZero)
            {
                MinDataValue = 0;
            }
            else if (MinDataValue - ChartHeightDifference < 0)
            {
                MinDataValue = 0;
            }
        }



        private void GenerateTickmarks()
        {



            ChartHeightDifference = MaxDataValue - MinDataValue;
            double MostFittingTickMark = ChartHeightDifference / YTickmarkCount;
            double BestAvailableTickMark = TickMarkIntervals.Where(s => s > MostFittingTickMark).First();


            LowestTickMarkValue = Math.Floor(MinDataValue / BestAvailableTickMark) * BestAvailableTickMark - BestAvailableTickMark;
            if (LowestTickMarkValue < 0) { LowestTickMarkValue = 0; }
            HigestTickMarkValue = Math.Ceiling(MaxDataValue / BestAvailableTickMark) * BestAvailableTickMark;
            double TickMarksToGenerate = (HigestTickMarkValue - LowestTickMarkValue) / BestAvailableTickMark;


            // test if this gives us a good amount of tickmarks or if there are way too many (could occur sometimes!)

            if (ChartHeightDifference / BestAvailableTickMark > YTickmarkCount * 1.4)
            {
                BestAvailableTickMark = TickMarkIntervals.Where(s => s > BestAvailableTickMark).First();
            }





            int TickMarksGenerated = 0; // needed to count them somehow :S
            for (double TickMarkNumber = LowestTickMarkValue; TickMarkNumber <= HigestTickMarkValue; TickMarkNumber = TickMarkNumber + BestAvailableTickMark)
            {
                var TickMarkBox = new ChartText();
                TickMarkBox.Name = "YTickmark";
                TickMarkBox.Text = string.Format("{0}", TickMarkNumber);
                TickMarkBox.Width = 95;

                TickMarkBox.FlowDirection = FlowDirection.RightToLeft;
                TickMarkBox.XPosition = 0;
                TickMarkBox.YPosition = ChartHeight - 158 - ((TickMarksGenerated / TickMarksToGenerate) * (ChartHeight - 250));

                ChartNodesCollection.Add(TickMarkBox);

                /*
                Canvas.SetLeft(TickMarkBox, 0); //provide enough space on the left size of the axis
                TickMarkBox.FlowDirection = FlowDirection.RightToLeft;
                Canvas.SetTop(TickMarkBox, ChartHeight - 158 - ((TickMarksGenerated / TickMarksToGenerate) * (ChartHeight-250))); // 158 = 8 so the tickmark is in the middle of what it represents, 150 extra space on the bottom --- 250 is to limit the range used (100 at the top + 150 at the bottom)
                ShapeCollection.Add(TickMarkBox);
                */

                TickMarksGenerated++;

            }
        }




        private void GenerateBarsInChart()
        {
            LengthForXAxis = 1; //reset variable to zero if it is not 1 (which is the standard offset from the Y-Axis to not overlap)
            double SpaceAvailablePerBar = ((double)ChartWidth - 200) / (double)DataList.DataPoints.Count;// the 2 here should be "assignable"
            BarHeightModifier = (ChartHeight - 250) / (HigestTickMarkValue - LowestTickMarkValue);

            // space available per bar gives us the amount of pixels we are allowed to spend, then we add in emtpy spaces with BarGapModifier
            // if anything other than a number between 0 and 1, don't show any gaps between bars
            if (AddWhiteSpacePerBar < 0 || AddWhiteSpacePerBar > 1)
            {
                AddWhiteSpacePerBar = 0;
            }


            double BarWidthWhitespace = SpaceAvailablePerBar * AddWhiteSpacePerBar; //calculate whitespace per bar
            BarWidth = SpaceAvailablePerBar - BarWidthWhitespace; //leftover amounts go to the bar itself





            for (int LoopCounter = 0; LoopCounter < DataList.DataPoints.Count; LoopCounter++)//using loopcounter since we need to know on which bar we are for positioning
            {
                var CurrentBar = new ChartBar();
                CurrentBar.Name = "BarInChart"; //Identifier for canvas collection

                if (!DataList.TargetLine.Equals(null))
                {
                    if (DataList.DataPoints[LoopCounter].Value < DataList.TargetLine)
                    {
                        CurrentBar.Fill = DataList.WorseThanTargetColor;
                    }
                    else
                    {
                        CurrentBar.Fill = DataList.BetterThanTargetColor;
                    }
                }
                else
                {
                    CurrentBar.Fill = DataList.Color;
                }



                CurrentBar.Width = BarWidth; //set the bar width to represent the data

                if (ForceYStartAtZero)
                {
                    CurrentBar.Height = BarHeightModifier * DataList.DataPoints[LoopCounter].Value;
                    CurrentBar.YPosition = (ChartHeight - 150) - DataList.DataPoints[LoopCounter].Value * BarHeightModifier - 2;
                    //Canvas.SetTop(CurrentBar, (ChartHeight-150) - DataList.DataPoints[LoopCounter].Value * BarHeightModifier - 2);
                }
                else
                {
                    CurrentBar.Height = BarHeightModifier * (DataList.DataPoints[LoopCounter].Value - LowestTickMarkValue);
                    CurrentBar.YPosition = (ChartHeight - 150) - (DataList.DataPoints[LoopCounter].Value - LowestTickMarkValue) * BarHeightModifier - 2;
                    //Canvas.SetTop(CurrentBar, (ChartHeight-150) - (DataList.DataPoints[LoopCounter].Value - LowestTickMarkValue) * BarHeightModifier - 2);
                }



                //now we need to position the bar correctly on the graph, how many pixels to the right?
                // 3 is the standard offset to not hit the Y-Axis
                // BarWidth is the width of the bar itself, barWidthWhitespace is the gaps between the bars
                // the last bit ensures the first bar also has half a whitespace in front of it

                // add a bit of whitespace before the first bar, makes it look better in the chart
                if (LoopCounter == 0) { LengthForXAxis += BarWidthWhitespace / 2 + 100; }


                CurrentBar.XPosition = LengthForXAxis;
                //Canvas.SetLeft(CurrentBar, LengthForXAxis);


                ChartNodesCollection.Add(CurrentBar);

                //ShapeCollection.Children.Add(CurrentBar);

                // if the datapoint has a name, it should display below the bar



                if (!string.IsNullOrWhiteSpace(DataList.DataPoints[LoopCounter].Name))
                {
                    var BarName = new ChartText();
                    BarName.Name = "BarInChartNameBox";

                    BarName.Text = DataList.DataPoints[LoopCounter].Name;

                    BarName.YPosition = ChartHeight - 150;
                    BarName.XPosition = LengthForXAxis + SpaceAvailablePerBar / 2;

                    BarName.Width = 200;


                    //Canvas.SetTop(BarName, ChartHeight-150);
                    //Canvas.SetLeft(BarName, LengthForXAxis+SpaceAvailablePerBar/2);

                    BarName.RenderTransform = new RotateTransform(30);

                    ChartNodesCollection.Add(BarName);

                    //ShapeCollection.Children.Add(BarName);


                }

                LengthForXAxis += (BarWidth) + (BarWidthWhitespace); //prepare for adding the next bar in the chart :)







            }


        }


        private void AddAxis()
        {

            var Xaxis = new ChartLine()
            {
                Name = "XAxis",
                X1Position = 0,
                X2Position = LengthForXAxis - 100,
                Y1Position = 0,//ChartHeight - 150,
                Y2Position = 0,// ChartHeight - 150,
                Stroke = Brushes.Black,
                Thickness = 2,
                XPosition = 100,
                YPosition = ChartHeight - 150,

            };
            ChartNodesCollection.Add(Xaxis);

            var YAxis = new ChartLine()
            {
                Name = "YAxis",
                X1Position = 0,
                X2Position = 0,
                Y1Position = ChartHeight - 250,
                Y2Position = 0,
                Stroke = Brushes.Black,
                Thickness = 2,
                XPosition = 100,
                YPosition = 100,

            };
            ChartNodesCollection.Add(YAxis);

        }




    }
}
