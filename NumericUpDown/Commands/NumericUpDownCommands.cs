using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TestM.Controls.NumericUpDown.Commands
{
    class NumericUpDownCommands
    {
        #region Fields

        private static RoutedUICommand _numericUp;
        private static RoutedUICommand _numericDown;
        

        #endregion

        #region Properties

        public static RoutedUICommand NumericUp
        {
            get { return _numericUp; }
        }

        public static RoutedUICommand NumericDown
        {
            get { return _numericDown; }
        }

        #endregion

        static NumericUpDownCommands()
        {
            initializeNumericUpCommand();
            initializeNumericDownCommand();
            
        }

        #region Methods

        private static void initializeNumericUpCommand()
        {
            _numericUp = new RoutedUICommand("NumericUp", "NumericUp", typeof(NumericUpDownCommands));
        }

        private static void initializeNumericDownCommand()
        {
            _numericDown = new RoutedUICommand("NumericDown", "NumericDown", typeof(NumericUpDownCommands));
        }

        
        #endregion
    }
}
