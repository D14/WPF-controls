using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TestM.Controls.NumericUpDown.Commands;
using TestM.ViewModels;

namespace TestM.Controls.NumericUpDown.ViewModel
{
    public class NumericUpDownViewModel : ViewModelBase
    {

        #region Private Properties

        private double _value;
        private double _step;
        private double _minValue;
        private double _maxValue;

        private CommandBindingCollection _commandBindings = new CommandBindingCollection();

        #endregion

        #region Public Properties

        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged(() => Value);
                }
            }
        }
        public double Step
        {
            get { return _step; }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    RaisePropertyChanged(() => Step);
                }
            }
        }
        public double MinValue
        {
            get { return _minValue; }
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    RaisePropertyChanged(() => MinValue);
                }
            }
        }
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    RaisePropertyChanged(() => MaxValue);
                }
            }
        }

        public CommandBindingCollection CommandBindings
        {
            get
            {
                return _commandBindings;
            }
        }

        #endregion

        public NumericUpDownViewModel(double value = 1, double step = 1, double maxValue = 100, double minValue = 0)
        {
            InitializeBudgetCommands();
            Value = value;
            Step = step;
            MaxValue = maxValue;
            MinValue = minValue;
        }

        #region Private Methods

        private void AddCommandBinding(ICommand command, ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute)
        {
            CommandBinding newBinding = new CommandBinding(command, executed, canExecute);
            CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown.View.NumericUpDownView), newBinding);

            CommandBindings.Add(newBinding);
        }

        private void InitializeBudgetCommands()
        {
            AddCommandBinding(NumericUpDownCommands.NumericUp, NumericUpCommand_Executed, NumericUpCommand_CanExecute);
            AddCommandBinding(NumericUpDownCommands.NumericDown, NumericDownCommand_Executed, NumericDownCommand_CanExecute);
        }

        #endregion

        #region Command Handlers

        #region NumericUp
        private void NumericUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Value += Step;
            if (Value > MaxValue)
                Value = MaxValue;
        }

        private void NumericUpCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Value < MaxValue)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
        #endregion

        #region NumericDown
        private void NumericDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Value -= Step;
            if (Value < MinValue)
                Value = MinValue;
        }
        private void NumericDownCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Value > MinValue)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
        #endregion

        #endregion

    }
}
