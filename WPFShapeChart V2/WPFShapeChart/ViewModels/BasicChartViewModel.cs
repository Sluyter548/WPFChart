using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WPFShapeChart.Models;

namespace WPFShapeChart.ViewModels
{



    public class BasicChartViewModel : ViewModelBase
    {

        public ObservableCollection<ChartNode> ChartNodesCollection { get; } = new ObservableCollection<ChartNode>();


        // create standards for the offset on the left and bottom, which makes it easier to follow

        public double ChartWidth { get; set; } = 800;
        public double ChartHeight { get; set; } = 600;
        public double ChartSpaceOffsetLeft { get; set; } = 100; // standard offset so there is some space on the left of the chart where the tickmarks + labels can appear
        public double ChartSpaceOffsetBottom { get; set; } = 150;
        public double ChartSpaceOffsetTop { get; set; } = 100;
        public double ChartSpaceOffsetRight { get; set; } = 100;


        public double YTickmarkCount { get; set; } = 5;
        private BarChartDataSeries DataList { get; set; }

        public void SetBarChartDataSeries(BarChartDataSeries InputData)
        {
            DataList = InputData;
            CalculateChart();
        }



        public bool ForceYStartAtZero { get; set; } = false;
        /// <summary>
        /// specifies the width of the bar in the chart
        /// 0 means bar will fill up entire column, 1 means the bars disappear in whitespace, standard 10%
        /// </summary>
        public double AddWhiteSpacePerBar { get; set; } = 0.1;



        private List<double> TickMarkIntervals;
        private double MinDataValue;
        private double MaxDataValue;
        private double ChartHeightDifference;
        private double BarWidth;
        private double BarHeightModifier;
        private double YAxisZeroValueOffset;
        private double LowestTickMarkValue; // Y-Axis
        private double HigestTickMarkValue; // Y-Axis
        private double LengthForXAxis; // used for putting the bars in the right place + create a long enough X axis
        private double ChartAvailableSpaceY { get { return ChartHeight - ChartSpaceOffsetBottom - ChartSpaceOffsetTop; } }




        public BasicChartViewModel()
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






        /// <summary>
        /// add the targetline to the graph if it falls within the chartrange, otherwise does nothing
        /// </summary>
        private void AddTargetLine()
        {
            if (DataList.TargetLine.Equals(0))
            {
                return;
            }
            else if(DataList.TargetLine > LowestTickMarkValue && DataList.TargetLine < HigestTickMarkValue)
            {
                var TargetLine = new ChartLine()
                {
                    Name = "TargetLine",
                    X1Position = 0,
                    X2Position = LengthForXAxis - ChartSpaceOffsetLeft,
                    Y1Position = 0,
                    Y2Position = 0,
                    Stroke = Brushes.Black,
                    Thickness = 2,
                    XPosition = ChartSpaceOffsetLeft
                };


                if (ForceYStartAtZero)
                {
                    TargetLine.YPosition = (ChartHeight - ChartSpaceOffsetBottom) - DataList.TargetLine * BarHeightModifier - 2;
                }
                else
                {

                    TargetLine.YPosition = (ChartHeight - ChartSpaceOffsetBottom) - (DataList.TargetLine - LowestTickMarkValue) * BarHeightModifier - 2;
                }


                ChartNodesCollection.Add(TargetLine);

            }
            else
            {
                return;
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
                if (!(MinDataValue<0)) //only works when there are no negative values, if there are it no longer makes sense to force to 0
                {

                    MinDataValue = 0;
                }
            }
            
        }



        private void GenerateTickmarks()
        {



            ChartHeightDifference = MaxDataValue - MinDataValue;
            double MostFittingTickMark = ChartHeightDifference / YTickmarkCount;
            double BestAvailableTickMark = TickMarkIntervals.Where(s => s > MostFittingTickMark).First();


            LowestTickMarkValue = Math.Floor(MinDataValue / BestAvailableTickMark) * BestAvailableTickMark - BestAvailableTickMark;
            //if (LowestTickMarkValue < 0) { LowestTickMarkValue = 0; }
            HigestTickMarkValue = Math.Ceiling(MaxDataValue / BestAvailableTickMark) * BestAvailableTickMark;
            double TickMarksToGenerate = (HigestTickMarkValue - LowestTickMarkValue) / BestAvailableTickMark;


            



            int TickMarksGenerated = 0; // needed to count them somehow :S
            for (double TickMarkNumber = LowestTickMarkValue; TickMarkNumber <= HigestTickMarkValue; TickMarkNumber += BestAvailableTickMark)
            {
                var TickMarkBox = new ChartText();
                TickMarkBox.Name = "YTickmark";
                TickMarkBox.Text = string.Format("{0:n0}", TickMarkNumber);
                TickMarkBox.Width = 95; // should fit enough numbers like this :)

                TickMarkBox.FlowDirection = FlowDirection.RightToLeft;
                TickMarkBox.XPosition = ChartSpaceOffsetLeft -100; // since the textbox itself is 95 long, and we want a bit of space between the Y-axis and the box itself :)
                TickMarkBox.YPosition = ChartHeight - ChartSpaceOffsetBottom - 8 - ((TickMarksGenerated / TickMarksToGenerate) * (ChartAvailableSpaceY)); //positions the numbers on the y-axis at the appropiate height

                ChartNodesCollection.Add(TickMarkBox);

                

                TickMarksGenerated++;

            }

            // set the position of the Yaxis 0-value for use later in the proces
            //can of course only be set if the 0-value is in range of the chart

            if (!(HigestTickMarkValue < 0 || LowestTickMarkValue > 0))
            {
                YAxisZeroValueOffset = ChartAvailableSpaceY / (HigestTickMarkValue - LowestTickMarkValue) * (0 - LowestTickMarkValue);
            }


        }




        private void GenerateBarsInChart()
        {
            LengthForXAxis = 1; //reset variable to zero if it is not 1 (which is the standard offset from the Y-Axis to not overlap)
            double SpaceAvailablePerBar = ((double)ChartWidth - ChartSpaceOffsetLeft - ChartSpaceOffsetRight) / (double)DataList.DataPoints.Count;// the 2 here should be "assignable"
            BarHeightModifier = (ChartAvailableSpaceY) / (HigestTickMarkValue - LowestTickMarkValue);

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
                CurrentBar.Name = "BarInChart"; //Identifier for collection

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


                // calculate the 0 y-axis value of the chart in pixels





                // bar width can stay the same as the previous chart, since that will not change with this one, however height is a different thing
                // need to figure out where the 0 is on the y-axis, and how big my bar then is required to be
                // starting with the 0 on the y-axis (this replaces the ChartHeight - 150)




                if (ForceYStartAtZero)
                {
                    CurrentBar.Height = BarHeightModifier * DataList.DataPoints[LoopCounter].Value;
                    CurrentBar.YPosition = (ChartHeight - ChartSpaceOffsetBottom) - DataList.DataPoints[LoopCounter].Value * BarHeightModifier - 2;
                    
                }
                else
                {
                    CurrentBar.Height = BarHeightModifier * (DataList.DataPoints[LoopCounter].Value - LowestTickMarkValue);
                    CurrentBar.YPosition = -YAxisZeroValueOffset+(ChartHeight - ChartSpaceOffsetBottom) - (DataList.DataPoints[LoopCounter].Value - LowestTickMarkValue) * BarHeightModifier - 2; // YAxisZeroValueOffset now follows correctly the 0 value tickmark on y-axis, need to continue with the bars now because they are not yet correct

                }



                //now we need to position the bar correctly on the graph, how many pixels to the right?
                // 3 is the standard offset to not hit the Y-Axis
                // BarWidth is the width of the bar itself, barWidthWhitespace is the gaps between the bars
                // the last bit ensures the first bar also has half a whitespace in front of it

                // add a bit of whitespace before the first bar, makes it look better in the chart
                if (LoopCounter == 0) { LengthForXAxis += BarWidthWhitespace / 2 + ChartSpaceOffsetLeft; }


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

                    BarName.YPosition = ChartHeight - ChartSpaceOffsetBottom;
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

            // need to calculate how "high" the X-axis will be in our chart
            double YPositionForXaxis;
            if ((LowestTickMarkValue == 0))
            {
                YPositionForXaxis = ChartHeight - ChartSpaceOffsetBottom;
            }
            else
            {
                YPositionForXaxis = ChartHeight - ChartSpaceOffsetBottom - (BarHeightModifier * (0 - LowestTickMarkValue));
            }


            var Xaxis = new ChartLine()
            {
                Name = "XAxis",
                X1Position = 0,
                X2Position = LengthForXAxis - ChartSpaceOffsetLeft,
                Y1Position = 0,
                Y2Position = 0,
                Stroke = Brushes.Black,
                Thickness = 2,
                XPosition = ChartSpaceOffsetLeft,
                YPosition = YPositionForXaxis, //had to calculate this first (bit above :))

            };
            ChartNodesCollection.Add(Xaxis);

            var YAxis = new ChartLine()
            {
                Name = "YAxis",
                X1Position = 0,
                X2Position = 0,
                Y1Position = ChartAvailableSpaceY,
                Y2Position = 0,
                Stroke = Brushes.Black,
                Thickness = 2,
                XPosition = ChartSpaceOffsetLeft,
                YPosition = ChartSpaceOffsetTop,

            };
            ChartNodesCollection.Add(YAxis);

        }




    }
}
