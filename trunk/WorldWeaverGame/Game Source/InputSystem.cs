using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WorldWeaver
{
    public enum ActionType
    {
        Move,
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,
        Rotate,
        Fire
    }

    public enum Control
    {
        AButton,
        BButton,
        XButton,
        YButton,
        StartButton,
        BackButton,
        LeftTrigger,
        RightTrigger,
        LeftShoulder,
        RightShoulder,
        LeftStick,
        RightStick,
        DPadUp,
        DPadDown,
        DPadLeft,
        DPadRight,
        None,
        Back,
        Tab,
        Enter,
        CapsLock,
        Escape,
        Space,
        PageUp,
        PageDown,
        End,
        Home,
        Left,
        Up,
        Right,
        Down,
        Select,
        Print,
        Execute,
        PrintScreen,
        Insert,
        Delete,
        Help,
        D0,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        D9,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        LeftWindows,
        RightWindows,
        Apps,
        Sleep,
        NumPad0,
        NumPad1,
        NumPad2,
        NumPad3,
        NumPad4,
        NumPad5,
        NumPad6,
        NumPad7,
        NumPad8,
        NumPad9,
        Multiply,
        Add,
        Separator,
        Subtract,
        Decimal,
        Divide,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        F13,
        F14,
        F15,
        F16,
        F17,
        F18,
        F19,
        F20,
        F21,
        F22,
        F23,
        F24,
        NumLock,
        Scroll,
        LeftShift,
        RightShift,
        LeftControl,
        RightControl,
        LeftAlt,
        RightAlt,
        BrowserBack,
        BrowserForward,
        BrowserRefresh,
        BrowserStop,
        BrowserSearch,
        BrowserFavorites,
        BrowserHome,
        VolumeMute,
        VolumeDown,
        VolumeUp,
        MediaNextTrack,
        MediaPreviousTrack,
        MediaStop,
        MediaPlayPause,
        LaunchMail,
        SelectMedia,
        LaunchApplication1,
        LaunchApplication2,
        OemSemicolon,
        OemPlus,
        OemComma,
        OemMinus,
        OemPeriod,
        OemQuestion,
        OemTilde,
        OemOpenBrackets,
        OemPipe,
        OemCloseBrackets,
        OemQuotes,
        Oem8,
        OemBackslash,
        ProcessKey,
        Attn,
        Crsel,
        Exsel,
        EraseEof,
        Play,
        Zoom,
        Pa1,
        OemClear,
        LeftMouseButton,
        MiddleMouseButton,
        RightMouseButton,
        MouseButton1,
        MouseButton2
    }

    public delegate void ActionDelegate(object value, GameTime gameTime);

    class InputSystem : GameComponent, IInputSystem
    {
        private GamePadState _lastGamePadState;
        private KeyboardState _lastKeyboardState;
        private GamePadState _curGamePadState;
        private KeyboardState _curKeyboardState;
        private MouseState _curMouseState;
        private MouseState _lastMouseState;

        private List<MappedAction> _mappedActions;

        public List<MappedAction> MappedActions
        {
            get { return _mappedActions; }
            set { _mappedActions = value; }
        }

        public InputSystem(Game game)
            : base(game)
        {
            _mappedActions = new List<MappedAction>();
        }

        public override void Initialize()
        {
        }

        public void Enable()
        {
            this.Enabled = true;
        }

        public void AddAction(string name, Control control, ActionType type, ActionDelegate function)
        {
            MappedAction action = new MappedAction(name, control, type, function);
            _mappedActions.Add(action);
        }

        public void SetActionControl(string name, Control control)
        {
            foreach (MappedAction action in _mappedActions)
            {
                if (action.Name == name)
                {
                    action.ActionControl = control;
                    break;
                }
            }
        }

        public void SetActionFunction(string name, ActionDelegate function)
        {
            foreach (MappedAction action in _mappedActions)
            {
                if (action.Name == name)
                {
                    action.Function = function;
                    break;
                }
            }
        }


        public override void Update(GameTime gameTime)
        {
            _curGamePadState = GamePad.GetState(PlayerIndex.One);
            _curKeyboardState = Keyboard.GetState();
            _curMouseState = Mouse.GetState();

            //we only need to look at the controls mapped to actions
            foreach (MappedAction action in _mappedActions)
            {
                switch (action.ActionControl)
                {
                    case Control.AButton:

                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Buttons.A, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Buttons.A == ButtonState.Pressed && _lastGamePadState.Buttons.A == ButtonState.Released)
                                action.Function(null, gameTime);
                        }
                        break;

                    case Control.BButton:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Buttons.B, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Buttons.B == ButtonState.Pressed && _lastGamePadState.Buttons.B == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.XButton:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Buttons.X, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Buttons.X == ButtonState.Pressed && _lastGamePadState.Buttons.X == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.YButton:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Buttons.Y, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Buttons.Y == ButtonState.Pressed && _lastGamePadState.Buttons.Y == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.StartButton:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Buttons.Start, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Buttons.Start == ButtonState.Pressed && _lastGamePadState.Buttons.Start == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.BackButton:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Buttons.Back, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Buttons.Back == ButtonState.Pressed && _lastGamePadState.Buttons.Back == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.LeftTrigger:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Triggers.Left, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Triggers.Left == 1.0f && _lastGamePadState.Triggers.Left < 1.0f)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.RightTrigger:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Triggers.Right, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Triggers.Right == 1.0f && _lastGamePadState.Triggers.Right < 1.0f)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.LeftShoulder:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Buttons.LeftShoulder, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Buttons.LeftShoulder == ButtonState.Pressed && _lastGamePadState.Buttons.LeftShoulder == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.RightShoulder:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.Buttons.RightShoulder, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.Buttons.RightShoulder == ButtonState.Pressed && _lastGamePadState.Buttons.RightShoulder == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.LeftStick:
                        if (action.Action < ActionType.Fire)
                        {
                            if (_curGamePadState.ThumbSticks.Left != Vector2.Zero)
                            {
                                action.Function(_curGamePadState.ThumbSticks.Left, gameTime);
                            }
                        }
                        else
                        {
                            if (_curGamePadState.ThumbSticks.Left != Vector2.Zero && _lastGamePadState.ThumbSticks.Left == Vector2.Zero)
                                action.Function(_curGamePadState.ThumbSticks.Left, gameTime);
                        }

                        break;

                    case Control.RightStick:
                        if (action.Action < ActionType.Fire)
                        {
                            if (_curGamePadState.ThumbSticks.Right != Vector2.Zero)
                            {
                                action.Function(_curGamePadState.ThumbSticks.Right, gameTime);
                            }
                        }
                        else
                        {
                            if (_curGamePadState.ThumbSticks.Right != Vector2.Zero && _lastGamePadState.ThumbSticks.Right == Vector2.Zero)
                                action.Function(_curGamePadState.ThumbSticks.Right, gameTime);
                        }

                        break;

                    case Control.DPadUp:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.DPad.Up, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.DPad.Up == ButtonState.Pressed && _lastGamePadState.DPad.Up == ButtonState.Released)
                                action.Function(_curGamePadState.DPad.Up, gameTime);
                        }

                        break;

                    case Control.DPadDown:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.DPad.Down, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.DPad.Down == ButtonState.Pressed && _lastGamePadState.DPad.Down == ButtonState.Released)
                                action.Function(_curGamePadState.DPad.Down, gameTime);
                        }

                        break;

                    case Control.DPadLeft:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.DPad.Left, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.DPad.Left == ButtonState.Pressed && _lastGamePadState.DPad.Left == ButtonState.Released)
                                action.Function(_curGamePadState.DPad.Left, gameTime);
                        }

                        break;

                    case Control.DPadRight:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curGamePadState.DPad.Right, gameTime);
                        }
                        else
                        {
                            if (_curGamePadState.DPad.Right == ButtonState.Pressed && _lastGamePadState.DPad.Right == ButtonState.Released)
                                action.Function(_curGamePadState.DPad.Right, gameTime);
                        }

                        break;

                    case Control.Back:
                    case Control.Tab:
                    case Control.Enter:
                    case Control.CapsLock:
                    case Control.Escape:
                    case Control.Space:
                    case Control.PageUp:
                    case Control.PageDown:
                    case Control.End:
                    case Control.Home:
                    case Control.Left:
                    case Control.Up:
                    case Control.Right:
                    case Control.Down:
                    case Control.Select:
                    case Control.Print:
                    case Control.Execute:
                    case Control.PrintScreen:
                    case Control.Insert:
                    case Control.Delete:
                    case Control.Help:
                    case Control.D0:
                    case Control.D1:
                    case Control.D2:
                    case Control.D3:
                    case Control.D4:
                    case Control.D5:
                    case Control.D6:
                    case Control.D7:
                    case Control.D8:
                    case Control.D9:
                    case Control.A:
                    case Control.B:
                    case Control.C:
                    case Control.D:
                    case Control.E:
                    case Control.F:
                    case Control.G:
                    case Control.H:
                    case Control.I:
                    case Control.J:
                    case Control.K:
                    case Control.L:
                    case Control.M:
                    case Control.N:
                    case Control.O:
                    case Control.P:
                    case Control.Q:
                    case Control.R:
                    case Control.S:
                    case Control.T:
                    case Control.U:
                    case Control.V:
                    case Control.W:
                    case Control.X:
                    case Control.Y:
                    case Control.Z:
                    case Control.LeftWindows:
                    case Control.RightWindows:
                    case Control.Apps:
                    case Control.Sleep:
                    case Control.NumPad0:
                    case Control.NumPad1:
                    case Control.NumPad2:
                    case Control.NumPad3:
                    case Control.NumPad4:
                    case Control.NumPad5:
                    case Control.NumPad6:
                    case Control.NumPad7:
                    case Control.NumPad8:
                    case Control.NumPad9:
                    case Control.Multiply:
                    case Control.Add:
                    case Control.Separator:
                    case Control.Subtract:
                    case Control.Decimal:
                    case Control.Divide:
                    case Control.F1:
                    case Control.F2:
                    case Control.F3:
                    case Control.F4:
                    case Control.F5:
                    case Control.F6:
                    case Control.F7:
                    case Control.F8:
                    case Control.F9:
                    case Control.F10:
                    case Control.F11:
                    case Control.F12:
                    case Control.F13:
                    case Control.F14:
                    case Control.F15:
                    case Control.F16:
                    case Control.F17:
                    case Control.F18:
                    case Control.F19:
                    case Control.F20:
                    case Control.F21:
                    case Control.F22:
                    case Control.F23:
                    case Control.F24:
                    case Control.NumLock:
                    case Control.Scroll:
                    case Control.LeftShift:
                    case Control.RightShift:
                    case Control.LeftControl:
                    case Control.RightControl:
                    case Control.LeftAlt:
                    case Control.RightAlt:
                    case Control.BrowserBack:
                    case Control.BrowserForward:
                    case Control.BrowserRefresh:
                    case Control.BrowserStop:
                    case Control.BrowserSearch:
                    case Control.BrowserFavorites:
                    case Control.BrowserHome:
                    case Control.VolumeMute:
                    case Control.VolumeDown:
                    case Control.VolumeUp:
                    case Control.MediaNextTrack:
                    case Control.MediaPreviousTrack:
                    case Control.MediaStop:
                    case Control.MediaPlayPause:
                    case Control.LaunchMail:
                    case Control.SelectMedia:
                    case Control.LaunchApplication1:
                    case Control.LaunchApplication2:
                    case Control.OemSemicolon:
                    case Control.OemPlus:
                    case Control.OemComma:
                    case Control.OemMinus:
                    case Control.OemPeriod:
                    case Control.OemQuestion:
                    case Control.OemTilde:
                    case Control.OemOpenBrackets:
                    case Control.OemPipe:
                    case Control.OemCloseBrackets:
                    case Control.OemQuotes:
                    case Control.Oem8:
                    case Control.OemBackslash:
                    case Control.ProcessKey:
                    case Control.Attn:
                    case Control.Crsel:
                    case Control.Exsel:
                    case Control.EraseEof:
                    case Control.Play:
                    case Control.Zoom:
                    case Control.Pa1:
                    case Control.OemClear:

                        //we can use one case here
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curKeyboardState.IsKeyDown((Keys)(action.ActionControl - (action.ActionControl - 1))), gameTime);
                        }
                        else
                        {
                            if (_curKeyboardState.IsKeyDown((Keys)(action.ActionControl - (action.ActionControl - 1))) && _lastKeyboardState.IsKeyUp((Keys)(action.ActionControl - (action.ActionControl - 1))))
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.LeftMouseButton:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curMouseState.LeftButton, gameTime);
                        }
                        else
                        {
                            if (_curMouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.MiddleMouseButton:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curMouseState.MiddleButton, gameTime);
                        }
                        else
                        {
                            if (_curMouseState.MiddleButton == ButtonState.Pressed && _lastMouseState.MiddleButton == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.RightMouseButton:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curMouseState.RightButton, gameTime);
                        }
                        else
                        {
                            if (_curMouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.MouseButton1:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curMouseState.XButton1, gameTime);
                        }
                        else
                        {
                            if (_curMouseState.XButton1 == ButtonState.Pressed && _lastMouseState.XButton1 == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;

                    case Control.MouseButton2:
                        if (action.Action < ActionType.Fire)
                        {
                            action.Function(_curMouseState.XButton2, gameTime);
                        }
                        else
                        {
                            if (_curMouseState.XButton2 == ButtonState.Pressed && _lastMouseState.XButton2 == ButtonState.Released)
                                action.Function(null, gameTime);
                        }

                        break;
                }
            }

            _lastGamePadState = _curGamePadState;
            _lastKeyboardState = _curKeyboardState;
            _lastMouseState = _curMouseState;

        }
    }
}
