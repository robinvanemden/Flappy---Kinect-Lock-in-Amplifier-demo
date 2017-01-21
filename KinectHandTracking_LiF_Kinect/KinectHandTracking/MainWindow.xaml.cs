using LightBuzz.Vitruvius;
using Microsoft.Kinect;
using OxyPlot;
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

namespace KinectHandTracking
{
    public partial class MainWindow
    {
        private const int Gameduration = 3;

        private IEnumerable<Body> _bodies;
        private Body _body;

        private int _currentHighScore;
        private DataSet _dataSet;

        private DispatcherTimer _dispatcherTimer;

        private double _distance;

        private DataTable _dt;
        private TimeSpan _durationGame;
        private MultiSourceFrameReader _frameReader;

        private int _gameTime = Gameduration;

        private GestureController _gestureController;

        private int _handOpenCloseOpen;

        private KinectSensor _kinectSensor;
        private double _latestScore;
        private double[] _liFReturn;

        private Lift _lift;

        private double _lifX;
        private double _lifY;

        private PlayersController _playersController;
        private double _precisionScore;
        private Storyboard _sbel;
        private Storyboard _sber;

        private Storyboard _sbr;
        private bool _showDialogBool;
        private double _speedScore;
        private bool _started;
        private DateTime _timerStart;

        private double _x0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        public IList<DataPoint> Points { get; private set; }
        public IList<DataPoint> PointsTwo { get; private set; }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void LifTick(object sender, EventArgs e)
        {
            if (_lift == null) _lift = new Lift(50, 1.3, 0.1, .07, 0.5);

            _lifX = _lift.GetLifX();
            _speedScore = _lifX * 0.6;
            if (_speedScore < 0.8) _speedScore = 0.8;
            if (_speedScore > 2.5) _speedScore = 2.5;
            _precisionScore = (900 - _distance * 3) / 100;
            _lifY = _speedScore * _precisionScore;

            _liFReturn = _lift.DoLifY(_lifY);

            UpdateUi();
        }

        private void UpdateUi()
        {
            TblScore1.Text = ((int) (_speedScore * 100)).ToString();
            TblScore2.Text = ((int) (_precisionScore * 100)).ToString();
            TblScore3.Text = ((int) (_lifY * 100)).ToString(); //max 10

            Points.Add(new DataPoint(_liFReturn[0], _liFReturn[3]));
            PointsTwo.Add(new DataPoint(_liFReturn[0], _liFReturn[1]));

            Dispatcher.InvokeAsync(() =>
            {
                if (!(_liFReturn[3] > 0)) return;

                _sbr.SetSpeedRatio(_liFReturn[3] * 0.8);
                _sbel.SetSpeedRatio(_liFReturn[3] * 0.8);
                _sber.SetSpeedRatio(_liFReturn[3] * 0.8);
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

            Dispatcher.InvokeAsync(() =>
            {
                Plot1.InvalidatePlot();
                _x0 = _liFReturn[1];
                ScoreBar.Value = _liFReturn[3];
            });
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

        private void HighBarSet()
        {
            var margin = HighBar.Margin;
            margin.Top =
                (int)
                ((AppWindow.Height - 72 - HighBar.Height / 2) *
                 ((ScoreBar.Maximum - _currentHighScore / 1000.0) / ScoreBar.Maximum)); //;
            HighBar.Margin = margin;
        }

        private void StartGame()
        {
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
            InitTimer();
        }

        private void CompleteGame()
        {
            Elr.Visibility = Visibility.Hidden;
            Ell.Visibility = Visibility.Hidden;
            _dispatcherTimer.Stop();
            _sbr.Stop();
            _sbel.Stop();
            _sber.Stop();
            _latestScore = _x0;
            _handOpenCloseOpen = 0;
            ShowHighDialog();
            _showDialogBool = true;
            _started = false;
        }

        private void InitTimer()
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
            AppWindow.CommandBindings.Add(new CommandBinding(cmd, ExecuteClose));
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

            var tempArray = (from DataRow row in _dt.Rows select Convert.ToString(row["Score"])).ToArray();
            var myInts = tempArray.Select(int.Parse).ToArray();
            var maxIndex = Enumerable.Range(0, myInts.Length).Aggregate((max, i) => myInts[max] > myInts[i] ? max : i);
            var maxInt = myInts[maxIndex];
            _currentHighScore = maxInt;
            HighBarSet();
        }

        private void WriteHighscore(string name, int highscore)
        {
            var dr = _dt.NewRow();
            dr[0] = name;
            dr[1] = highscore.ToString();
            _dt.Rows.Add(dr);

            DataGrid.ItemsSource = _dt.DefaultView;

            _dataSet.WriteXml("Content/HighScores.xml");

            var dataView = (DataView) DataGrid.ItemsSource;
            if (dataView != null) dataView.Sort = "Score DESC";
            DataGrid.Items.Refresh();

            GridSearchFocus(name, DataGrid, "Name");

            var tempArray = (from DataRow row in _dt.Rows select Convert.ToString(row["Score"])).ToArray();
            var myInts = tempArray.Select(int.Parse).ToArray();
            var maxIndex = Enumerable.Range(0, myInts.Length).Aggregate((max, i) => myInts[max] > myInts[i] ? max : i);
            var maxInt = myInts[maxIndex];
            _currentHighScore = maxInt;
            HighBarSet();
        }

        public void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowHighDialog()
        {
            Points.Clear();
            PointsTwo.Clear();
            Plot1.InvalidatePlot();

            CountDownText.Visibility = Visibility.Collapsed;
            var localScore = (int) (_latestScore * 1000);
            YouScored.Text = "You scored " + localScore + " points!";
            ResponseText = "";

            StackPanel.Visibility = Visibility.Visible;
            DataGrid.Visibility = Visibility.Collapsed;
            HighScoreGridTitle.Visibility = Visibility.Collapsed;
            ImageClose.Visibility = Visibility.Collapsed;
            ResponseTextBox.Focusable = true;
            Keyboard.Focus(ResponseTextBox);
        }

        private void ReturnEnteredName(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                ClickEnteredName(this, new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void ClickEnteredName(object sender, RoutedEventArgs e)
        {
            if (ResponseText.Length >= 1)
            {
                var localScore = (int) (_latestScore * 1000);
                WriteHighscore(ResponseText, localScore);
                ShowHightScore();
            }
        }

        private void ShowHightScore()
        {
            CountDownText.Visibility = Visibility.Collapsed;
            StackPanel.Visibility = Visibility.Collapsed;
            DataGrid.Visibility = Visibility.Visible;
            HighScoreGridTitle.Visibility = Visibility.Visible;
            ImageClose.Visibility = Visibility.Visible;
        }

        private void CloseHighScore(object sender, MouseButtonEventArgs e)
        {
            StackPanel.Visibility = Visibility.Collapsed;
            DataGrid.Visibility = Visibility.Collapsed;
            HighScoreGridTitle.Visibility = Visibility.Collapsed;
            ImageClose.Visibility = Visibility.Collapsed;
            StartText.Visibility = Visibility.Visible;
            _showDialogBool = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _kinectSensor = KinectSensor.GetDefault();

            if (_kinectSensor == null) return;

            _kinectSensor.Open();

            _gestureController = new GestureController();
            _gestureController.GestureRecognized += GestureController_GestureRecognized;

            _frameReader =
                _kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth |
                                                         FrameSourceTypes.Infrared | FrameSourceTypes.Body);
            _frameReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            _playersController = new PlayersController();
            _playersController.BodyEntered += UserReporterBodyEntered;
            _playersController.BodyLeft += UserReporterBodyLeft;
            _playersController.Start();
        }

        private void GestureController_GestureRecognized(object sender, GestureEventArgs e)
        {
            if (e.GestureType != GestureType.WaveLeft && e.GestureType != GestureType.WaveRight) return;
            if (_started || _showDialogBool) return;

            _handOpenCloseOpen = 0;
            _started = true;
            _showDialogBool = false;
            StartGame();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _frameReader?.Dispose();
            _kinectSensor?.Close();
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
                if (frame == null) return;
                Canvas.Children.Clear();
                _bodies = frame.Bodies();
                var enumerable = _bodies as IList<Body> ?? _bodies.ToList();
                _playersController.Update(enumerable);

                _body = enumerable.Closest();

                if (_body != null)
                    if (_body.IsTracked)
                    {
                        _gestureController.Update(_body);

                        var handRight = _body.Joints[JointType.HandRight];
                        var handLeft = _body.Joints[JointType.HandLeft];
                        var head = _body.Joints[JointType.Head];
                        var rhp = Canvas.DrawHand(handRight, _kinectSensor.CoordinateMapper);
                        var lhp = Canvas.DrawHand(handLeft, _kinectSensor.CoordinateMapper);
                        Canvas.DrawHead(head, _kinectSensor.CoordinateMapper);

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

                        Canvas.DrawSkeleton(_body, _kinectSensor.CoordinateMapper);

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

                            default:
                                throw new ArgumentOutOfRangeException();
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
                                    StartText.Visibility = Visibility.Collapsed;
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

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
            }
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

        private void UserReporterBodyLeft(object sender, PlayersControllerEventArgs e)
        {
            TblScore1.Text = "-";
            TblScore2.Text = "-";
            TblScore3.Text = "-";
        }

        private static void UserReporterBodyEntered(object sender, PlayersControllerEventArgs e)
        {
        }
    }
}