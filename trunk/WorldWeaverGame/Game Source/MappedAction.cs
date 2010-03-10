using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWeaver
{
    class MappedAction
    {
        private string _name;
        private Control _control;
        private ActionType _action;
        private ActionDelegate _function;

        public MappedAction(string name, Control control, ActionType action, ActionDelegate function)
        {
            _name = name;
            _control = control;
            _action = action;
            _function = function;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Control ActionControl
        {
            get { return _control; }
            set { _control = value; }
        }

        public ActionType Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public ActionDelegate Function
        {
            get { return (ActionDelegate)_function; }
            set { _function = value; }
        }

    }
}
