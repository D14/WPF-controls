using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.ProcessTimeline.Controls.State.ViewModel;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineMemory.View;
using Alaska.ViewModels;
using System.ComponentModel;
using Alaska.Libraries;
using System.Windows.Controls;
using Alaska.Commands;
using Alaska.Controls.ProcessGraph.Controls.GraphTimeline.ViewModel;
using LoganFramework;

namespace Alaska.Controls.ProcessGraph.Controls.GraphTimeline.Controls.TimelineMemory.ViewModel
{
    public class TimeLineMemoryViewModel : ViewModelBase, IAlaskaTasKWorker
    {
        #region Private properties

        private AlaskaTasKWorker _taskWorker;
        private TimelineMemoryView _control;

        private System.Windows.Input.ICommand _commandAdd;
        private System.Windows.Input.ICommand _commandClear;

        private double _graphWidth;
        private double _maxSizeValue;
        private double _syncHorizontalOffset;
        private double _syncVerticalOffset;
        private double _scale;
        private StateViewModel _selectedState;
        private DateTime _startTime;
        private DateTime _endTime;
        private DateTime _markedTime;

        private BindingList<Point> _sizePointList;
        private BindingList<Point> _usedPointList;
        private List<CListMemoryAndTime> _memoryList;
        private double _timeMarkX;


        #endregion

        //private object _pointsLock = new object();

        #region Public properties

        public System.Windows.Input.ICommand CommandAdd
        {
            get
            {
                if (_commandAdd == null)
                {
                    var r = new Random();
                    _commandAdd = new AlaskaCommand(p => AddRandomPoints());
                }

                return _commandAdd;
            }
        }
        public System.Windows.Input.ICommand CommandClear
        {
            get
            {
                if (_commandClear == null)
                {
                    var r = new Random();
                    _commandClear = new AlaskaCommand(p => Clear());
                }

                return _commandClear;
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

                    RaisePropertyChanged(() => SyncHorizontalOffset);
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
                    //SetScaledMarks();
                    RecalculateGraphWidth(value);
                    RaisePropertyChanged(() => Scale);
                }
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

                    RecalculateGraphWidth(Scale);

                    RaisePropertyChanged(() => StartTime);
                    RaisePropertyChanged(() => Interval);
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

                    RecalculateGraphWidth(Scale);

                    RaisePropertyChanged(() => EndTime);
                    RaisePropertyChanged(() => Interval);
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
        /*public double MinValue
        {
            get { return _minValue; }
        }*/
        public double MaxSizeValue
        {
            get
            {
                return _maxSizeValue;
            }
            set
            {
                _maxSizeValue = value;
            }
        }
        public double Height
        {
            get { return _control.ActualHeight; }
        }
        public double Width
        {
            get { return _control.ActualWidth; }
        }
        public double GraphWidth
        {
            get
            {
                return _graphWidth;
            }
            set
            {
                if (_graphWidth != value)
                {
                    _graphWidth = value;

                    //SetAllMarksWidthAsync(value);
                    //SetScaledMarks();

                    RaisePropertyChanged(() => GraphWidth);
                }
            }
        }
        public BindingList<Point> SizePointList
        {
            get { return _sizePointList; }
            set { _sizePointList = value; }
        }
        public BindingList<Point> UsedPointList
        {
            get { return _usedPointList; }
            set { _usedPointList = value; }
        }
        public PointCollection SizePointCollection
        {
            get
            {
                if (SizePointList.Count == 0) return null;

                double pXcoef = GraphWidth / SizePointList.Select(p => p.X).Max();
                double pYcoef = Height / SizePointList.Select(p => p.Y).Max();

                var collection = new PointCollection(SizePointList.Select(p => new Point(pXcoef * p.X, pYcoef * p.Y)).OrderBy(point => point.X));

                return collection;
            }
        }
        public PointCollection UsedPointCollection
        {
            get
            {
                if (SizePointList.Count == 0 || UsedPointList.Count == 0) return null;

                double pXcoef = GraphWidth / SizePointList.Select(p => p.X).Max();
                double pYcoef = Height / SizePointList.Select(p => p.Y).Max();

                var collection = new PointCollection(UsedPointList.Select(p => new Point(pXcoef * p.X, pYcoef * p.Y)).OrderBy(point => point.X));

                return collection;
            }
        }
        public List<CListMemoryAndTime> MemoryList
        {
            get
            {
                return _memoryList;
            }
            set
            {
                ClearAsync();

                if (value != null && _memoryList != value)
                {
                    _memoryList = value;

                    if (_memoryList.Count > 0)
                    {
                        AddSizePointAsync(((_memoryList.First().Time - StartTime).TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale), 0);
                        AddUsedPointAsync(((_memoryList.First().Time - StartTime).TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale), 0);

                        foreach (var memoryItem in _memoryList)
                        {
                            AddSizePointAsync((memoryItem.Time - StartTime).TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale, memoryItem.MEMINFO.DalvikHeap == 0 ? memoryItem.MEMINFO.DalvikHeapAlloc + memoryItem.MEMINFO.DalvikHeapFree : memoryItem.MEMINFO.DalvikHeap);
                            AddUsedPointAsync((memoryItem.Time - StartTime).TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale, memoryItem.MEMINFO.DalvikHeapAlloc);
                        }

                        AddSizePointAsync(((_memoryList.Last().Time - StartTime).TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale), 0);
                        AddUsedPointAsync(((_memoryList.Last().Time - StartTime).TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale), 0);

                    }

                    RaisePropertyChanged(() => MemoryList);
                }
            }
        }

        #endregion

        #region Constructors

        public TimeLineMemoryViewModel(TimelineMemoryView control)
        {
            //SizePointCollection = new PointCollection();
            _taskWorker = new AlaskaTasKWorker();
            SizePointList = new BindingList<Point>();
            UsedPointList = new BindingList<Point>();

            _control = control;
            _control.SizeChanged += _control_SizeChanged;

            //BindingOperations.EnableCollectionSynchronization(SizePointCollection, _pointsLock);

            //AddPoint(0, 0);
        }

        #endregion

        #region Private methods

        private void _control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                RaisePropertyChanged(() => SizePointCollection);
                RaisePropertyChanged(() => UsedPointCollection);
            }
        }

        private void RecalculateGraphWidth(double scale)
        {
            GraphWidth = Interval.TotalMilliseconds * GraphTimelineViewModel.WIDTH_COEF / Scale;

            RaisePropertyChanged(() => SizePointCollection);
            RaisePropertyChanged(() => UsedPointCollection);
        }

        #endregion

        #region Public methods

        public void AddRandomPoints()
        {

            var r = new Random();
            AddTaskToQeue(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    double newY = r.Next(50) + 150;
                    double newX = r.Next(200) * 10;

                    SizePointList.Add(new Point(newX, newY));
                    UsedPointList.Add(new Point(r.Next(200) * 10, r.Next(50) + 50));

                }
                RaisePropertyChanged(() => SizePointCollection);
                RaisePropertyChanged(() => UsedPointCollection);
            });
        }

        public void AddSizePoint(double x, double y)
        {
            SizePointList.Add(new Point(x, y));

            RaisePropertyChanged(() => SizePointCollection);
        }
        public void AddSizePointAsync(double x, double y)
        {
            AddTaskToQeue(() =>
            {
                AddSizePoint(x, y);
            });
        }

        public void AddUsedPoint(double x, double y)
        {
            UsedPointList.Add(new Point(x, y));

            RaisePropertyChanged(() => UsedPointCollection);
        }
        public void AddUsedPointAsync(double x, double y)
        {
            AddTaskToQeue(() =>
            {
                AddUsedPoint(x, y);
            });
        }

        public void Clear()
        {
            /*while (SizePointList.Count > 1)
                SizePointList.RemoveAt(SizePointCollection.Count - 2);

            while (UsedPointList.Count > 1)
                UsedPointList.RemoveAt(UsedPointCollection.Count - 2);
            */

            SizePointList.Clear();
            UsedPointList.Clear();

            RaisePropertyChanged(() => SizePointCollection);
            RaisePropertyChanged(() => UsedPointCollection);
        }
        public void ClearAsync()
        {
            AddTaskToQeue(() =>
            {
                Clear();
            });
        }

        public object AddTaskToQeue(Action action)
        {
            return _taskWorker.AddTaskToQeue(action);
        }

        public object AddConcurencyTask(Action action)
        {
            return _taskWorker.AddConcurencyTask(action);
        }

        #endregion
    }
}
