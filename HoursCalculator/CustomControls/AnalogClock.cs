using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HoursCalculator.CustomControls
{
    public class AnalogClock : Control
    {
        Line hour;
        Line minute;
        Line second;

        static AnalogClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnalogClock), new FrameworkPropertyMetadata(typeof(AnalogClock)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            hour = Template.FindName("PART_Hour", this) as Line;
            minute = Template.FindName("PART_Minute", this) as Line;
            second = Template.FindName("PART_Second", this) as Line;

            UpdateAngles();

            DispatcherTimer dispatcherTimer = new();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += (s, e) => UpdateAngles();
            dispatcherTimer.Start();
        }

        private void UpdateAngles()
        {
            hour.RenderTransform = new RotateTransform((DateTime.Now.Hour / 12.0) * 360, 0.5, 0.5);
            minute.RenderTransform = new RotateTransform((DateTime.Now.Minute / 60.0) * 360, 0.5, 0.5);
            second.RenderTransform = new RotateTransform((DateTime.Now.Second / 60.0) * 360, 0.5, 0.5);
        }

        public double HourX1
        {
            get { return (double)GetValue(HourX1Property); }
            set { SetValue(HourX1Property, value); }
        }
        public static readonly DependencyProperty HourX1Property =
            DependencyProperty.Register("HourX1", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double HourX2
        {
            get { return (double)GetValue(HourX2Property); }
            set { SetValue(HourX2Property, value); }
        }
        public static readonly DependencyProperty HourX2Property =
            DependencyProperty.Register("HourX2", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double MinuteX1
        {
            get { return (double)GetValue(MinuteX1Property); }
            set { SetValue(MinuteX1Property, value); }
        }
        public static readonly DependencyProperty MinuteX1Property =
            DependencyProperty.Register("MinuteX1", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double MinuteX2
        {
            get { return (double)GetValue(MinuteX2Property); }
            set { SetValue(MinuteX2Property, value); }
        }
        public static readonly DependencyProperty MinuteX2Property =
            DependencyProperty.Register("MinuteX2", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double SecondX1
        {
            get { return (double)GetValue(SecondX1Property); }
            set { SetValue(SecondX1Property, value); }
        }
        public static readonly DependencyProperty SecondX1Property =
            DependencyProperty.Register("SecondX1", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double SecondX2
        {
            get { return (double)GetValue(SecondX2Property); }
            set { SetValue(SecondX2Property, value); }
        }
        public static readonly DependencyProperty SecondX2Property =
            DependencyProperty.Register("SecondX2", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double ClockWidth
        {
            get { return (double)GetValue(ClockWidthProperty); }
            set { SetValue(ClockWidthProperty, value); }
        }
        public static readonly DependencyProperty ClockWidthProperty =
            DependencyProperty.Register("ClockWidth", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double ClockHeight
        {
            get { return (double)GetValue(ClockHeightProperty); }
            set { SetValue(ClockHeightProperty, value); }
        }
        public static readonly DependencyProperty ClockHeightProperty =
            DependencyProperty.Register("ClockHeight", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double ClockThickness
        {
            get { return (double)GetValue(ClockThicknessProperty); }
            set { SetValue(ClockThicknessProperty, value); }
        }
        public static readonly DependencyProperty ClockThicknessProperty =
            DependencyProperty.Register("ClockThickness", typeof(double), typeof(AnalogClock), new PropertyMetadata(1d));

        public double DotWidth
        {
            get { return (double)GetValue(DotWidthProperty); }
            set { SetValue(DotWidthProperty, value); }
        }
        public static readonly DependencyProperty DotWidthProperty =
            DependencyProperty.Register("DotWidth", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double DotHeight
        {
            get { return (double)GetValue(DotHeightProperty); }
            set { SetValue(DotHeightProperty, value); }
        }
        public static readonly DependencyProperty DotHeightProperty =
            DependencyProperty.Register("DotHeight", typeof(double), typeof(AnalogClock), new PropertyMetadata(0d));

        public double HourHandThickness
        {
            get { return (double)GetValue(HourHandThicknessProperty); }
            set { SetValue(HourHandThicknessProperty, value); }
        }
        public static readonly DependencyProperty HourHandThicknessProperty =
            DependencyProperty.Register("HourHandThickness", typeof(double), typeof(AnalogClock), new PropertyMetadata(0.8d));

        public double MinuteHandThickness
        {
            get { return (double)GetValue(MinuteHandThicknessProperty); }
            set { SetValue(MinuteHandThicknessProperty, value); }
        }
        public static readonly DependencyProperty MinuteHandThicknessProperty =
            DependencyProperty.Register("MinuteHandThickness", typeof(double), typeof(AnalogClock), new PropertyMetadata(0.8d));

        public double SecondHandThickness
        {
            get { return (double)GetValue(SecondHandThicknessProperty); }
            set { SetValue(SecondHandThicknessProperty, value); }
        }
        public static readonly DependencyProperty SecondHandThicknessProperty =
            DependencyProperty.Register("SecondHandThickness", typeof(double), typeof(AnalogClock), new PropertyMetadata(0.5d));
    }
}
