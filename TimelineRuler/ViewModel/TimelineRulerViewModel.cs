using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.ProcessTimeline.Controls.State.ViewModel;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineRuler.Model;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineRuler.View;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimlineMarker.ViewModel;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.ViewModel;
using Alaska.Libraries;
using Alaska.ViewModels;

namespace Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineRuler.ViewModel
{
    public class TimelineRulerViewModel : ViewModelBase, IAlaskaTasKWorker
    {
        #region Private properties

        private AlaskaTasKWorker _taskWorker;
        private readonly object _marksLock = new object();

        private TimelineRulerView _control;

        private double _scale;
        private int _zoomRatio;
        private int _markWidth;

        private double _syncVerticalOffset;
        private double _syncHorizontalOffset;

        private ObservableCollection<TimelineRulerMarkModel> _marks;

        private DateTime _markedTime;
        private DateTime _startTime;
        private DateTime _endTime;
        private StateViewModel _selectedState;
        private double _observedWidth;



        #endregion

        #region Public properties

        public double Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                if (_scale != value)
                {
                    _scale = value;

                    //SetAllMarksWidthAsync(value);
                    BuildMarksAsync();

                    RaisePropertyChanged(() => Scale);
                    RaisePropertyChanged(() => RulerFullWidth);
                }
            }
        }
        public int ZoomRatio
        {
            get
            {
                return _zoomRatio;
            }
            set
            {
                if (_zoomRatio != value)
                {
                    _zoomRatio = value;

                    RaisePropertyChanged(() => ZoomRatio);
                }
            }
        }
        public int MarkWidth
        {
            get
            {
                return _markWidth;
            }
            set
            {
                if (_markWidth != value)
                {
                    _markWidth = value;

                    RaisePropertyChanged(() => MarkWidth);
                }
            }
        }
        public double SyncVerticalOffset
        {
            get
            {
                return _syncVerticalOffset;
            }
            set
            {
                if (_syncVerticalOffset != value)
                {
                    _syncVerticalOffset = value;

                    RaisePropertyChanged(() => SyncVerticalOffset);
                }
            }
        }
        public double SyncHorizontalOffset
        {
            get
            {
                return _syncHorizontalOffset;
            }
            set
            {
                if (_syncHorizontalOffset != value)
                {
                    _syncHorizontalOffset = value;

                    BuildMarksAsync();

                    RaisePropertyChanged(() => SyncHorizontalOffset);
                }
            }
        }
        public double ObservedWidth
        {
            get
            {
                return _observedWidth;
            }
            set
            {
                if (_observedWidth != value)
                {
                    _observedWidth = value;

                    BuildMarksAsync();

                    RaisePropertyChanged(() => ObservedWidth);
                }
            }
        }
        public double RulerFullWidth
        {
            get
            {
                return Interval.TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale;
            }
        }

        public ObservableCollection<TimelineRulerMarkModel> Marks
        {
            get
            {
                return _marks;
            }
            set
            {
                if (_marks != value)
                {
                    _marks = value;
                    RaisePropertyChanged(() => Marks);
                }
            }
        }

        public DateTime MarkedTime
        {
            get
            {
                return _markedTime;
            }
            set
            {
                if (_markedTime != value)
                {
                    _markedTime = value;

                    RaisePropertyChanged(() => MarkedTime);
                    RaisePropertyChanged(() => TimeMarkX);
                }
            }
        }
        public double TimeMarkX
        {
            get
            {
                return (MarkedTime - StartTime).TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale;
            }
        }
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;

                    BuildMarksAsync();

                    RaisePropertyChanged(() => StartTime);
                    RaisePropertyChanged(() => Interval);
                    RaisePropertyChanged(() => RulerFullWidth);
                }
            }
        }
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;

                    BuildMarksAsync();

                    RaisePropertyChanged(() => EndTime);
                    RaisePropertyChanged(() => Interval);
                    RaisePropertyChanged(() => RulerFullWidth);
                }
            }
        }
        public TimeSpan Interval
        {
            get
            {
                return EndTime - StartTime;
            }
        }
        public StateViewModel SelectedState
        {
            get
            {
                return _selectedState;
            }
            set
            {
                if (_selectedState != value)
                {
                    _selectedState = value;
                    RaisePropertyChanged(() => SelectedState);
                }
            }
        }

        #endregion

        #region Constructors

        public TimelineRulerViewModel(TimelineRulerView control, DateTime startTime, DateTime endTime, double scale)
        {
            _taskWorker = new AlaskaTasKWorker();

            //temporary solution


            Marks = new ObservableCollection<TimelineRulerMarkModel>();

            BindingOperations.EnableCollectionSynchronization(Marks, _marksLock);

            _control = control;
            Scale = scale == 0 ? 100 : scale;
            StartTime = startTime;
            EndTime = endTime;

            /* ULTRA MEGA HACK BINDING */
            var b = new Binding("ActualWidth");
            b.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            _control.SetBinding(TimelineRulerView.ObservedWidthProperty, b);
            /*-------------------------*/



            BuildMarks();

        }

        #endregion

        #region Private Methods

        private void BuildMarks()
        {
            if (Scale > 0 && Interval.TotalMilliseconds > 0)
            {
                double marksCount;
                double markWidth;

                ZoomRatio = 0;

                markWidth = GraphTimelineViewModel.WIDTH_COEF / Scale; //Marks = 0.1 second 

                while (markWidth < GraphTimelineViewModel.MIN_MARK_WIDTH)
                {
                    markWidth *= 10;
                    ZoomRatio++;
                }

                marksCount = Interval.TotalMilliseconds / Math.Pow(10, ZoomRatio);

                Marks.Clear();


                if (marksCount > 0)
                {
                    for (int i = (int)(SyncHorizontalOffset / markWidth); i < (SyncHorizontalOffset + _control.ActualWidth) / markWidth; i++)
                    {
                        var markTime = StartTime.AddMilliseconds(i * Math.Pow(10, ZoomRatio));
                        var markOffset = (markTime - StartTime).TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale;

                        Marks.Add(new TimelineRulerMarkModel(markOffset, 0, markOffset, 16, markTime));

                        var step = StartTime.AddMilliseconds((i + 1) * Math.Pow(10, ZoomRatio)) - markTime;

                        for (int s = 1; s < 5; s++)
                            Marks.Add(new TimelineRulerMarkModel(markOffset + (s * markWidth / 10), 0, markOffset + (s * markWidth / 10), 8, markTime.AddMilliseconds(s * step.TotalMilliseconds / 10)));

                        Marks.Add(new TimelineRulerMarkModel(markOffset + (5 * markWidth / 10), 0, markOffset + (5 * markWidth / 10), 12, markTime.AddMilliseconds(5 * step.TotalMilliseconds / 10)));

                        for (int s = 6; s < 10; s++)
                            Marks.Add(new TimelineRulerMarkModel(markOffset + (s * markWidth / 10), 0, markOffset + (s * markWidth / 10), 8, markTime.AddMilliseconds(s * step.TotalMilliseconds / 10)));
                    }
                }
            }
        }

        public void BuildMarksAsync()
        {
            //AddTaskToQeue(BuildMarks);
            BuildMarks();
        }
        #endregion

        public object AddTaskToQeue(Action action)
        {
            return _taskWorker.AddTaskToQeue(action);
        }

        public object AddConcurencyTask(Action action)
        {
            return _taskWorker.AddConcurencyTask(action);
        }
    }
}
