using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.ProcessTimeline.Controls.State.ViewModel;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineMemory.ViewModel;
using System.ComponentModel;
using LoganFramework;

namespace Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineMemory.View
{
    /// <summary>
    /// Interaction logic for TimelineMemoryView.xaml
    /// </summary>
    public partial class TimelineMemoryView : UserControl
    {
        #region Dependency properties

        public static DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale",
            typeof(double),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineMemoryView)sender).ViewModel.Scale = (double)e.NewValue)
                )
            );

        public static DependencyProperty StartTimeProperty = DependencyProperty.Register(
            "StartTime",
            typeof(DateTime),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineMemoryView)sender).ViewModel.StartTime = (DateTime)e.NewValue)
                )
            );
        public static DependencyProperty EndTimeProperty = DependencyProperty.Register(
            "EndTime",
            typeof(DateTime),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineMemoryView)sender).ViewModel.EndTime = (DateTime)e.NewValue)
            ));
        public static DependencyProperty SelectedStateProperty = DependencyProperty.Register(
            "SelectedState",
            typeof(StateViewModel),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
                {
                    ((TimelineMemoryView)sender).ViewModel.SelectedState = (StateViewModel)e.NewValue;
                }
                ))
            );

        public static DependencyProperty SyncHorizontalOffsetProperty = DependencyProperty.Register(
            "SyncHorizontalOffset",
            typeof(double),
            typeof(TimelineMemoryView),
            new PropertyMetadata(
                new PropertyChangedCallback(
                    (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
                    {
                        ((TimelineMemoryView)sender).ViewModel.SyncHorizontalOffset = (double)e.NewValue;
                    }
                ))
            );


        public static DependencyProperty UsedPointsProperty = DependencyProperty.Register(
            "UsedPoints",
            typeof(BindingList<Point>),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
                {
                    /*string values = (string)e.NewValue;
                    ((TimelineMemoryView)sender).Points = values;
                    var points = new List<Point>(values
                        .Split(' ')
                        .Select(p => 
                            new Point(
                                Convert.ToDouble(p.Split(',')[0]),
                                Convert.ToDouble(p.Split(',')[1])
                                )
                        )
                    );*/
                    ((TimelineMemoryView)sender).ViewModel.UsedPointList = (BindingList<Point>)e.NewValue;
                }
            )));
        public static DependencyProperty SizePointsProperty = DependencyProperty.Register(
            "SizePoints",
            typeof(BindingList<Point>),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
                {
                    ((TimelineMemoryView)sender).ViewModel.SizePointList = (BindingList<Point>)e.NewValue;
                }
            )));

        public static DependencyProperty MemoryListProperty = DependencyProperty.Register(
            "MemoryList",
            typeof(List<CListMemoryAndTime>),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
                {
                    ((TimelineMemoryView)sender).ViewModel.MemoryList = (List<CListMemoryAndTime>)e.NewValue;
                }
            )));

        public static DependencyProperty MarkedTimeProperty = DependencyProperty.Register(
            "MarkedTime",
            typeof(DateTime),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineMemoryView)sender).ViewModel.MarkedTime = (DateTime)e.NewValue)
                )
            );

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TimeLineMemoryViewModel),
            typeof(TimelineMemoryView),
            new PropertyMetadata(new PropertyChangedCallback(
          (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
              (
                  (TimelineMemoryView)sender).ViewModel = (TimeLineMemoryViewModel)e.NewValue)
              )
            );


        public double SyncHorizontalOffset
        {
            get { return (double)GetValue(SyncHorizontalOffsetProperty); }
            set { SetValue(SyncHorizontalOffsetProperty, value); }
        }
        public BindingList<Point> UsedPoints
        {
            get { return (BindingList<Point>)GetValue(UsedPointsProperty); }
            set { SetValue(UsedPointsProperty, value); }
        }
        public BindingList<Point> SizePoints
        {
            get { return (BindingList<Point>)GetValue(SizePointsProperty); }
            set { SetValue(SizePointsProperty, value); }
        }
        public List<CListMemoryAndTime> MemoryList
        {
            get { return (List<CListMemoryAndTime>)GetValue(SizePointsProperty); }
            set { SetValue(SizePointsProperty, value); }
        }
        public TimeLineMemoryViewModel ViewModel
        {
            get { return (TimeLineMemoryViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }
        public StateViewModel SelectedState
        {
            get { return (StateViewModel)GetValue(SelectedStateProperty); }
            set { SetValue(SelectedStateProperty, value); }
        }
        public DateTime MarkedTime
        {
            get { return (DateTime)GetValue(MarkedTimeProperty); }
            set { SetValue(MarkedTimeProperty, value); }
        }
        #endregion

        public TimelineMemoryView()
        {
            InitializeComponent();
            ViewModel = new TimeLineMemoryViewModel(this);
        }

    }
}
