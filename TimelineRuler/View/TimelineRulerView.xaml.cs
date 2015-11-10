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
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineRuler.ViewModel;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.ViewModel;

namespace Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineRuler.View
{
    /// <summary>
    /// Interaction logic for TimelineRulerView.xaml
    /// </summary>
    public partial class TimelineRulerView : UserControl
    {
        #region Dependency properties

        public static DependencyProperty SyncVerticalOffsetProperty = DependencyProperty.Register(
            "SyncVerticalOffset",
            typeof(double),
            typeof(TimelineRulerView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineRulerView)sender).ViewModel.SyncVerticalOffset = (double)e.NewValue)
                )
            );
        public static DependencyProperty SyncHorizontalOffsetProperty = DependencyProperty.Register(
            "SyncHorizontalOffset",
            typeof(double),
            typeof(TimelineRulerView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineRulerView)sender).ViewModel.SyncHorizontalOffset = (double)e.NewValue)
                )
            );
        public static DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale",
            typeof(double),
            typeof(TimelineRulerView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineRulerView)sender).ViewModel.Scale = (double)e.NewValue)
                )
            );

        public static DependencyProperty StartTimeProperty = DependencyProperty.Register(
            "StartTime",
            typeof(DateTime),
            typeof(TimelineRulerView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineRulerView)sender).ViewModel.StartTime = (DateTime)e.NewValue)
                )
            );
        public static DependencyProperty EndTimeProperty = DependencyProperty.Register(
            "EndTime",
            typeof(DateTime),
            typeof(TimelineRulerView),
            new PropertyMetadata(new PropertyChangedCallback(
          (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
          {
              ((TimelineRulerView)sender).ViewModel = new TimelineRulerViewModel(((TimelineRulerView)sender), ((TimelineRulerView)sender).StartTime, (DateTime)e.NewValue, ((TimelineRulerView)sender).Scale);
          })
            ));
        public static DependencyProperty SelectedStateProperty = DependencyProperty.Register(
            "SelectedState",
            typeof(StateViewModel),
            typeof(TimelineRulerView),
            new PropertyMetadata(new PropertyChangedCallback(
          (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineRulerView)sender).ViewModel.SelectedState = (StateViewModel)e.NewValue)
          )
            );

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TimelineRulerViewModel),
            typeof(TimelineRulerView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
                    (
                        (TimelineRulerView)sender).ViewModel = (TimelineRulerViewModel)e.NewValue)
                    )
            );


        public static DependencyProperty ObservedWidthProperty = DependencyProperty.Register(
            "ObservedWidth",
            typeof(double),
            typeof(TimelineRulerView),
            new PropertyMetadata(
                new PropertyChangedCallback(
                    (DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
                    {
                        ((TimelineRulerView)sender).ViewModel.ObservedWidth = (double)e.NewValue;
                    }
                )
            )
        );
        public static DependencyProperty MarkedTimeProperty = DependencyProperty.Register(
            "MarkedTime",
            typeof(DateTime),
            typeof(TimelineRulerView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((TimelineRulerView)sender).ViewModel.MarkedTime = (DateTime)e.NewValue)
            )
        );

        public double ObservedWidth
        {
            get { return (double)GetValue(ObservedWidthProperty); }
            set { SetValue(ObservedWidthProperty, value); }
        }

        public double SyncVerticalOffset
        {
            get { return (double)GetValue(SyncVerticalOffsetProperty); }
            set { SetValue(SyncVerticalOffsetProperty, value); }
        }
        public double SyncHorizontalOffset
        {
            get { return (double)GetValue(SyncHorizontalOffsetProperty); }
            set { SetValue(SyncHorizontalOffsetProperty, value); }
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
        public TimelineRulerViewModel ViewModel
        {
            get { return (TimelineRulerViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        
        #endregion

        public TimelineRulerView()
        {
            ViewModel = new TimelineRulerViewModel(this, StartTime, EndTime, Scale);

            InitializeComponent();

        }
    }
}
