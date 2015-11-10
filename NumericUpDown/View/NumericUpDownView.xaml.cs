using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestM.Controls.NumericUpDown.Commands;
using TestM.Controls.NumericUpDown.ViewModel;

namespace TestM.Controls.NumericUpDown.View
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDownView : UserControl
    {
        #region Dependency Properties
        public static DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(NumericUpDownView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((NumericUpDownView)sender).ViewModel.Value = (double)e.NewValue)
                )
            );
        public static DependencyProperty StepProperty = DependencyProperty.Register(
            "Step",
            typeof(double),
            typeof(NumericUpDownView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((NumericUpDownView)sender).ViewModel.Step = (double)e.NewValue)
                )
            );
        public static DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(double),
            typeof(NumericUpDownView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((NumericUpDownView)sender).ViewModel.MinValue = (double)e.NewValue)
                )
            );
        public static DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(double),
            typeof(NumericUpDownView),
            new PropertyMetadata(new PropertyChangedCallback(
                (DependencyObject sender, DependencyPropertyChangedEventArgs e) => ((NumericUpDownView)sender).ViewModel.MaxValue = (double)e.NewValue)
                )
            );
        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(NumericUpDownViewModel),
            typeof(NumericUpDownView)
            );

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public NumericUpDownViewModel ViewModel
        {
            get { return (NumericUpDownViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        public NumericUpDownView()
        {
            InitializeComponent();
            ViewModel = new NumericUpDownViewModel(Value, Step, MinValue, MaxValue);
        } 
    }
}
