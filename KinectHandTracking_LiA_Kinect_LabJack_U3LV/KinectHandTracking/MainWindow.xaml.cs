using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using LabJack.LabJackUD;
using LightBuzz.Vitruvius;
using Microsoft.Kinect;
using OxyPlot;

namespace KinectHandTracking
{
    public partial class MainWindow
    {
        private const int Gameduration = 60;

        private readonly U3 _u3;

        private int _binary;

        private IEnumerable<Body> _bodies;
        private Body _body;
        private double _d1;
        private double _d2;

        private DataSet _dataSet;

        private DispatcherTimer _dispatcherTimer;

        private double _distance;

        private DataTable _dt;
        private TimeSpan _durationGame;

        private int _gameTime = Gameduration;

        private int _handOpenCloseOpen;

        private double _latestScore;

        private double _learnRate;
        private double _observedScore;

        private PlayersController _playersController;
        private double _precisionScore;
        private MultiSourceFrameReader _reader;

        private Storyboard _sbel;
        private Storyboard _sber;

        private Storyboard _sbr;

        private KinectSensor _sensor;
        private bool _showDialogBool;
        private double _speedScore;

        private bool _start = true;
        private bool _started;
        private int _t;
        private DateTime _timerStart;

        private double _x0;

        private double _xlif;
        private double _ylif;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _u3 = new U3(LJUD.CONNECTION.USB, "0", true);
                LJUD.ePut(_u3.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0);
            }
            catch (LabJackUDException)
            {
                _showDialogBool = true;
            }

            if (_u3 != null) InitializeGame();

            InitHighscore();
        }

        public int CurrentHighScore { get; private set; }

        public IList<DataPoint> Points { get; private set; }
        public IList<DataPoint> PointsTwo { get; private set; }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void LifTick(object sender, EventArgs e)
        {
            // Here I replaced digital Lock-in Feedback by code sending and receiving
            // data tot and from a live analog Lock-in Amplifier (LiA).
            // Had to work fast, but the code, and the LiA did their jobs admirably!
            // I cleaned the code up a little since, meaning this version is as of yet
            // untested with a live LiA.
            // Nevertheless, the general setup worked fine before cleanup, so the code
            // may offer a nice springboard for the next demo.

            _t += 1;
            _binary = 0;
            _started = true;

            // Retrieve Lock in Amplifier from U3 - here, X value (speed)

            LJUD.eAIN(_u3.ljhandle, 3, 31, ref _d1, -1, -1, -1, _binary);
            Console.Out.WriteLine("AIN3 = {0:0.###}\n", _d1);
            _xlif = _x0 + _d1;

            // Some sanity checks on the speed that is being set
            // (probably not needed after setting correct parameters)

            if (_xlif < 0.5) _xlif = 1;
            if (_xlif > 6) _xlif = 6;

            // Calculate the score based on the speed and measured precision

            _speedScore = _xlif * 0.4;
            _precisionScore = (400 - _distance * 2.5) / 100;
            if (_precisionScore < 0.5) _precisionScore = 0.5;
            _observedScore = _speedScore * _precisionScore / 10;

            // Send observed score via U3 on Lock in Amplifier
            LJUD.eDAC(_u3.ljhandle, 0, _observedScore, _binary, 0, 0);
            Console.Out.WriteLine("DAC0 set to {0:0.###} volts\n", _observedScore);

            // Retrieve the Y offset from the Lock in Amplifier
            LJUD.eAIN(_u3.ljhandle, 0, 31, ref _d2, -1, -1, -1, _binary);
            Console.Out.WriteLine("AIN0 = {0:0.###}\n", _d2);
            _ylif = _d2 - 0.3;

            // Set _x0
            _x0 = _x0 + _learnRate * _ylif;

            // Display results and set speed of the game

            TblScore1.Text = ((int) (_speedScore * 100)).ToString();
            TblScore2.Text = ((int) (_precisionScore * 100)).ToString();
            if (_start) TblScore3.Text = ((int) (_x0 * 1000)).ToString();
            Points.Add(new DataPoint(_t, _observedScore));
            PointsTwo.Add(new DataPoint(_t, _observedScore));

            Dispatcher.InvokeAsync(() =>
            {
                if (_x0 > 0)
                {
                    _sbr.SetSpeedRatio(_x0 * 0.8);
                    _sbel.SetSpeedRatio(_x0 * 0.8);
                    _sber.SetSpeedRatio(_x0 * 0.8);
                }
            });

            Dispatcher.InvokeAsync(() =>
            {
                _durationGame = DateTime.Now - _timerStart;
                _gameTime = Gameduration - (int) _durationGame.TotalSeconds;
                if (_gameTime < int.Parse(CountDownText.Text) || int.Parse(CountDownText.Text) == 0)
                    CountDownText.Text = _gameTime.ToString();

                if (_gameTime <= 0)
                    CompleteGame();
            });

            Dispatcher.InvokeAsync(() => { ScoreBar.Value = 100 * _observedScore; });

            Dispatcher.InvokeAsync(() => { Plot1.InvalidatePlot(); });
        }

        private void InitializeGame()
        {
            _sbr = (Storyboard) AnimatedLine.FindResource("Sbr");
            _sbel = (Storyboard) Ell.FindResource("Sbel");
            _sber = (Storyboard) Elr.FindResource("Sber");

            CountDownText.Text = Gameduration.ToString();

            DataContext = this;
            GridAddRowNumber(DataGrid);

            Points = new List<DataPoint>();
            PointsTwo = new List<DataPoint>();

            ShutdownAppShortcutF5();

            InitHighscore();
        }

        private void StartGame()
        {
            // set x0 tp 1
            _learnRate = .020;
            _x0 = 1.4;
            _t = 0;

            CountDownText.Visibility = Visibility.Visible;
            Points.Clear();
            PointsTwo.Clear();
            Plot1.InvalidatePlot();
            _gameTime = Gameduration;
            Elr.Visibility = Visibility.Visible;
            Ell.Visibility = Visibility.Visible;
            AnimationStart();
        }

        private void AnimationStart()
        {
            _sbr.Begin();
            _sbel.Begin();
            _sber.Begin();
            Init_Timer();
        }

        private void CompleteGame()
        {
            _latestScore = _x0;
            Elr.Visibility = Visibility.Hidden;
            Ell.Visibility = Visibility.Hidden;
            _dispatcherTimer.Stop();
            _sbr.Stop();
            _sbel.Stop();
            _sber.Stop();
            _dispatcherTimer.Stop();
            _handOpenCloseOpen = 0;
            _started = false;
            ShowHighDialog();
        }

        private void Init_Timer()
        {
            _timerStart = DateTime.Now;
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += LifTick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _dispatcherTimer.Start();
        }

        private void ShutdownAppShortcutF5()
        {
            var cmd = new RoutedCommand();
            AppWindow.InputBindings.Add(new KeyBinding(cmd, new KeyGesture(Key.F5)));
            AppWindow.CommandBindings.Add(new CommandBinding(cmd, cb_Executed));
        }

        private void InitHighscore()
        {
            _dt = new DataTable("Highscores");
            _dataSet = new DataSet();
            _dataSet.ReadXml("Content/HighScores.xml");
            _dt = _dataSet.Tables[0];

            DataGrid.ItemsSource = _dt.DefaultView;

            var dataView = (DataView) DataGrid.ItemsSource;
            if (dataView != null) dataView.Sort = "Score DESC";
            DataGrid.Items.Refresh();

            var data = new List<string>();
            foreach (DataRow row in _dt.Rows)
                data.Add(Convert.ToString(row["Score"]));
            var tempArray = data.ToArray();
            var myInts = tempArray.Select(int.Parse).ToArray();
            var maxIndex = Enumerable.Range(0, myInts.Length).Aggregate((max, i) => myInts[max] > myInts[i] ? max : i);
            var maxInt = myInts[maxIndex];
            CurrentHighScore = maxInt;
        }

        private void WriteHighscore(string name, int highscore)
        {
            var dr = _dt.NewRow();
            dr[0] = name;
            dr[1] = highscore.ToString().PadLeft(5, '0');
            _dt.Rows.Add(dr);

            DataGrid.ItemsSource = _dt.DefaultView;

            _dataSet.WriteXml("Content/HighScores.xml");

            var dataView = (DataView) DataGrid.ItemsSource;
            if (dataView != null) dataView.Sort = "Score DESC";
            DataGrid.Items.Refresh();

            GridSearchFocus(name, DataGrid, "Name");

            var data = new List<string>();
            foreach (DataRow row in _dt.Rows)
                data.Add(Convert.ToString(row["Score"]));
            var tempArray = data.ToArray();
            var myInts = tempArray.Select(int.Parse).ToArray();
            var maxIndex = Enumerable.Range(0, myInts.Length).Aggregate((max, i) => myInts[max] > myInts[i] ? max : i);
            var maxInt = myInts[maxIndex];
            CurrentHighScore = maxInt;
        }

        public void cb_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowHighDialog()
        {
            Points.Clear();
            PointsTwo.Clear();
            Plot1.InvalidatePlot();
            _start = false;

            CountDownText.Visibility = Visibility.Hidden;
            _showDialogBool = true;
            var localScore = (int) (_latestScore * 1000);
            YouScored.Text = "You scored " + localScore + " points!";
            ResponseText = "";

            // TODO: These can be put together into one panel etc
            StackPanel.Visibility = Visibility.Visible;
            DataGrid.Visibility = Visibility.Hidden;
            HighScoreGridTitle.Visibility = Visibility.Hidden;
            ImageClose.Visibility = Visibility.Hidden;
            ResponseTextBox.Focusable = true;

            Keyboard.Focus(ResponseTextBox);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                OKButton_Click(this, new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResponseText.Length >= 1)
            {
                var localScore = (int) (_latestScore * 1000);
                WriteHighscore(ResponseText, localScore);

                CountDownText.Visibility = Visibility.Hidden;
                StackPanel.Visibility = Visibility.Hidden;
                DataGrid.Visibility = Visibility.Visible;
                HighScoreGridTitle.Visibility = Visibility.Visible;
                ImageClose.Visibility = Visibility.Visible;
            }
        }

        private void CloseHightScore(object sender, MouseButtonEventArgs e)
        {
            StackPanel.Visibility = Visibility.Hidden;
            DataGrid.Visibility = Visibility.Hidden;
            HighScoreGridTitle.Visibility = Visibility.Hidden;
            ImageClose.Visibility = Visibility.Hidden;
            _showDialogBool = false;
            _start = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();

                _reader =
                    _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth |
                                                       FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

                _playersController = new PlayersController();
                _playersController.BodyEntered += UserReporter_BodyEntered;
                _playersController.BodyLeft += UserReporter_BodyLeft;
                _playersController.Start();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _reader?.Dispose();
            _sensor?.Close();
        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                    Camera.Source = frame.ToBitmap();
            }

            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Canvas.Children.Clear();
                    _bodies = frame.Bodies();
                    var enumerable = _bodies as IList<Body> ?? _bodies.ToList();
                    _playersController.Update(enumerable);

                    _body = enumerable.Closest();

                    if (_body != null)
                        if (_body.IsTracked)
                        {
                            var handRight = _body.Joints[JointType.HandRight];
                            var handLeft = _body.Joints[JointType.HandLeft];
                            var head = _body.Joints[JointType.Head];
                            var rhp = Canvas.DrawHand(handRight, _sensor.CoordinateMapper);
                            var lhp = Canvas.DrawHand(handLeft, _sensor.CoordinateMapper);
                            Canvas.DrawHead(head, _sensor.CoordinateMapper);

                            Canvas.GetTop(AnimatedLine);

                            _distance = Math.Abs(Canvas.GetTop(AnimatedLine) - rhp.Y);

                            var distanceR = Math.Abs(Canvas.GetTop(AnimatedLine) - rhp.Y);
                            Elr.Fill = distanceR < 100
                                ? new SolidColorBrush(Color.FromArgb(128, 0, 255, 0))
                                : new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
                            Canvas.SetLeft(Elr, rhp.X);

                            var distancel = Math.Abs(Canvas.GetTop(AnimatedLine) - lhp.Y);
                            Ell.Fill = distancel < 100
                                ? new SolidColorBrush(Color.FromArgb(128, 0, 255, 0))
                                : new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
                            Canvas.SetLeft(Ell, lhp.X);

                            Canvas.DrawSkeleton(_body, _sensor.CoordinateMapper);

                            switch (_body.HandRightState)
                            {
                                case HandState.Open:
                                    break;

                                case HandState.Closed:
                                    break;

                                case HandState.Lasso:
                                    break;

                                case HandState.Unknown:
                                    break;

                                case HandState.NotTracked:
                                    break;
                            }

                            switch (_body.HandLeftState)
                            {
                                case HandState.Open:
                                    if (_handOpenCloseOpen == 0 && !_started && !_showDialogBool) _handOpenCloseOpen++;
                                    if (_handOpenCloseOpen == 2 && !_started && !_showDialogBool)
                                    {
                                        _handOpenCloseOpen = 0;
                                        _started = true;
                                        _showDialogBool = false;
                                        //startText.Visibility = Visibility.Collapsed;
                                        StartGame();
                                    }
                                    break;

                                case HandState.Closed:
                                    if (_handOpenCloseOpen == 1 && !_started && !_showDialogBool) _handOpenCloseOpen++;
                                    break;

                                case HandState.Lasso:
                                    break;

                                case HandState.Unknown:
                                    break;

                                case HandState.NotTracked:
                                    break;
                            }
                        }
                }
            }
        }

        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

            if (rowIndex < 0 || rowIndex > dataGrid.Items.Count - 1)
                throw new ArgumentException($"{rowIndex} is an invalid row index.");

            dataGrid.SelectedItems.Clear();
            /* set the SelectedItem property */
            var item = dataGrid.Items[rowIndex]; // = Product X
            dataGrid.SelectedItem = item;

            var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
                dataGrid.ScrollIntoView(item);
        }

        public void GridAddRowNumber(DataGrid myDataGrid)
        {
            var column0 = new DataGridTextColumn {Header = "#"};
            var bindingColumn0 = new Binding
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1),
                Converter = new RowToIndexConvertor()
            };
            column0.Binding = bindingColumn0;
            myDataGrid.Columns.Add(column0);
        }

        public void GridSearchFocus(string idUniqueToFind, DataGrid myDataGrid, string myGridColumnToSearch)
        {
            var idToFind = idUniqueToFind;

            foreach (DataRowView drv in (DataView) myDataGrid.ItemsSource)
                if ((string) drv[myGridColumnToSearch] == idToFind)
                {
                    myDataGrid.SelectedItem = drv;
                    myDataGrid.ScrollIntoView(drv);
                    myDataGrid.Focus();

                    break;
                }
        }

        private void UserReporter_BodyLeft(object sender, PlayersControllerEventArgs e)
        {
            TblScore1.Text = "-";
            TblScore2.Text = "-";
            TblScore3.Text = "-";
        }

        private void UserReporter_BodyEntered(object sender, PlayersControllerEventArgs e)
        {
        }
    }
}