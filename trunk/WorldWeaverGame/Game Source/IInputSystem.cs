using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWeaver
{
    interface IInputSystem
    {
        List<MappedAction> MappedActions { get; set; }
        void SetActionFunction(string name, ActionDelegate function);
        void Enable();
    }
}