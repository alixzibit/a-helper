using System.Windows;
using System.Windows.Threading;
using SharpDX.XInput;
using WindowsInput;



namespace ahelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //    


    public partial class MainWindow : Window
    {
        //private Controller controller;
        //private DispatcherTimer gamepadTimer;
        private Controller controller;
        private DispatcherTimer gamepadTimer;
        private Gamepad gamepadState;

        //public static event EventHandler<GamepadButtonEventArgs> GamepadButtonPressed;

        public MainWindow()
        {
            InitializeComponent();
            //controller = new Controller(UserIndex.One);

            //SetupGamepadHandling();
            //StartListeningForGamepadInput();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var inputSimulator = new InputSimulator();
            inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.TAB);
        }

        

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //click to tabcontrol item radeonm_tab

            radeondm_tab.IsSelected = true;
            
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            optimize_tab.IsSelected = true;
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            actoxb_tab.IsSelected = true;
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            //click to open exit.xaml
            exit exit = new exit();
            exit.Show();
        }

       
        //private void SetupGamepadHandling()
        //{
        //    gamepadTimer = new DispatcherTimer
        //    {
        //        Interval = TimeSpan.FromMilliseconds(100) // 
        //    };
        //    gamepadTimer.Tick += GamepadUpdate;
        //    gamepadTimer.Start();
        //}

        //private void GamepadUpdate(object sender, EventArgs e)
        //{
        //    if (!controller.IsConnected)
        //    {
        //        return; // Exit if the controller is not connected
        //    }

        //    gamepadState = controller.GetState().Gamepad;
        //    var inputSimulator = new InputSimulator();

        //    // D-Pad buttons
        //    if (gamepadState.Buttons.HasFlag(GamepadButtonFlags.DPadUp))
        //    {
        //        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.UP);
        //    }
        //    if (gamepadState.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
        //    {
        //        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.DOWN);
        //    }
        //    if (gamepadState.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
        //    {
        //        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.LEFT);
        //    }
        //    if (gamepadState.Buttons.HasFlag(GamepadButtonFlags.DPadRight))
        //    {
        //        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RIGHT);
        //    }

        //    // A, B, X, Y buttons
        //    if (gamepadState.Buttons.HasFlag(GamepadButtonFlags.A))
        //    {
        //        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN); // Simulate Enter key
        //    }
        //    if (gamepadState.Buttons.HasFlag(GamepadButtonFlags.B))
        //    {
        //        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE); // Simulate Esc key
        //    }
        //    // Add more mappings as needed
        //}



        //protected override void OnClosed(EventArgs e)
        //{
        //    gamepadTimer.Stop();
        //    base.OnClosed(e);
        //}
    }

    //public class GamepadButtonEventArgs : EventArgs
    //{
    //    public GamepadButtonFlags Button { get; private set; }

    //    public GamepadButtonEventArgs(GamepadButtonFlags button)
    //    {
    //        Button = button;
    //    }
    //}
}

//        private void MoveFocus(FocusNavigationDirection direction)
//        {
//            var focusedElement = Keyboard.FocusedElement as UIElement;
//            if (focusedElement != null)
//            {
//                var request = new TraversalRequest(direction);
//                var newFocusTarget = focusedElement.PredictFocus(direction) as UIElement;
//                if (newFocusTarget != null)
//                {
//                    newFocusTarget.Focus();
//                    Keyboard.Focus(newFocusTarget);
//                }
//            }
//        }

//        private void ClickFocusedButton()
//        {
//            Application.Current.Dispatcher.Invoke(() =>
//            {
//                if (Keyboard.FocusedElement is Button focusedButton)
//                {
//                    focusedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
//                }
//            });
//        }
