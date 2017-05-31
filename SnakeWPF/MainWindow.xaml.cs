using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _timer;
        readonly Random _rand = new Random();
        const int Rows = 30;
        const int Columns = 50;
        CheckBox[,] _boxes = new CheckBox[Rows, Columns];
        readonly Tuple<int, int>[] _allPoints = new Tuple<int, int>[Rows * Columns];

        List<Snake> _snakes = new List<Snake>();
        List<Tuple<int, int>> _foods = new List<Tuple<int, int>>();
        Dictionary<Snake, TextBox> _scoreBoxes = new Dictionary<Snake, TextBox>();
        HashSet<Key> _pressedKeys = new HashSet<Key>();

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += (s, e) =>
            {
                _pressedKeys.Add(e.Key);
            };
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 16);
            _timer.Tick += (s, e) => Update();
            _timer.Start();

            InitializeGrid();
            CreatePlayers();
        }

        private void InitializeGrid()
        {
            for (int j = 0; j < Columns; j++)
            {
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < Rows; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var cb = new CheckBox();
                    cb.IsHitTestVisible = false;
                    Grid.SetRow(cb, i);
                    Grid.SetColumn(cb, j);
                    mainGrid.Children.Add(cb);
                    _boxes[i, j] = cb;
                    _allPoints[i * Columns + j] = new Tuple<int, int>(i, j);
                }
            }
            for (int i = 0; i < 10; i++)
                _foods.Add(GenerateRandomFood());
        }

        private void CreatePlayers()
        {
            _snakes.Add(new Snake("Blue",
                                  0, 0, 5,
                                  Direction.Right,
                                  Brushes.Blue.Color,
                                  Brushes.DarkBlue.Color,
                                  Brushes.LightBlue.Color,
                                  new ControlScheme(Key.NumPad8, Key.NumPad5, Key.NumPad4, Key.NumPad6, Key.Space)));
            _snakes.Add(new Snake("Pinky",
                                  0, Columns - 1, 3,
                                  Direction.Down,
                                  Brushes.HotPink.Color,
                                  Brushes.DeepPink.Color,
                                  Brushes.LightPink.Color,
                                  new ControlScheme(Key.W, Key.S, Key.A, Key.D, Key.Space)
                                  ));
            //_snakes.Add(new Snake("Gren", Rows - 1, 0, 3, Direction.Up, Brushes.Green.Color, Brushes.DarkGreen.Color, Brushes.LightGreen.Color, new ControlScheme(Key.P, Key.OemSemicolon, Key.L, Key.OemQuotes, Key.Space)));
            //_snakes.Add(new Snake("Cyanide", Rows - 1, Columns - 1, 3, Direction.Left, Brushes.Cyan.Color, Brushes.DarkCyan.Color, Brushes.LightCyan.Color, new ControlScheme(Key.Up, Key.Down, Key.Left, Key.Right, Key.Space)));
            //_snakes.Add(new Snake("Nigga", Rows / 2, Columns / 2, 3, Direction.Up, Brushes.Gray.Color, Brushes.Black.Color, Brushes.LightGray.Color, new ControlScheme(Key.Y, Key.H, Key.G, Key.J, Key.Space)));

            //_snakes.Add(new Snake("Blue", 0, 0, 2, Direction.Right, Brushes.Blue.Color, Brushes.DarkBlue.Color, Brushes.LightBlue.Color, new ControlScheme(Key.W, Key.S, Key.A, Key.D)));
            //_snakes.Add(new Snake("Green", 1, 0, 3, Direction.Right, Brushes.Green.Color, Brushes.DarkGreen.Color, Brushes.LightGreen.Color, new ControlScheme(Key.W, Key.S, Key.A, Key.D)));
            //_snakes.Add(new Snake("Red", 2, 0, 4, Direction.Right, Brushes.Red.Color, Brushes.DarkRed.Color, Brushes.LightCoral.Color, new ControlScheme(Key.W, Key.S, Key.A, Key.D)));

            var width = this.Width / _snakes.Count;

            foreach (var snake in _snakes)
            {
                var sb = new TextBox { Text = "0", Background = new SolidColorBrush(snake.DeadColor), Width = width, TextAlignment = TextAlignment.Center, IsReadOnly = true };
                _scoreBoxes.Add(snake, sb);
                scoresPanel.Children.Add(sb);
            }
        }

        #region Update
        private void UpdateMovement()
        {
            foreach (var snake in _snakes)
            {
                if (snake.IsDead)
                    continue;
                if (_pressedKeys.Contains(snake.ControlScheme.KeyUp) && snake.Direction != Direction.Down)
                {
                    snake.ChangeDirection(Direction.Up);
                }
                else if (_pressedKeys.Contains(snake.ControlScheme.KeyDown) && snake.Direction != Direction.Up)
                {
                    snake.ChangeDirection(Direction.Down);
                }
                else if (_pressedKeys.Contains(snake.ControlScheme.KeyLeft) && snake.Direction != Direction.Right)
                {
                    snake.ChangeDirection(Direction.Left);
                }
                else if (_pressedKeys.Contains(snake.ControlScheme.KeyRight) && snake.Direction != Direction.Left)
                {
                    snake.ChangeDirection(Direction.Right);
                }
            }
        }

        private void CheckEndGame()
        {
            var snakes = _snakes.Where(x => !x.IsDead);
            if (snakes.Count() == 0)
            {
                MessageBox.Show("Everyone's dead, lol.");
                this.Close();
            }
            else if (snakes.Count() == 1)
            {
                MessageBox.Show(string.Format("`{0}` won ze game. Score: {1}, gratz :)", snakes.First().Name, _scoreBoxes[snakes.First()].Text));
                this.Close();
            }
        }

        private void ResetGrid()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    var cb = _boxes[i, j];
                    cb.IsChecked = false;
                    cb.Background = null;
                }
            }
        }

        private void Draw()
        {
            foreach (var snake in _snakes)
            {
                foreach (var part in snake.Parts)
                {
                    _boxes[part.Item1, part.Item2].IsChecked = true;
                    _boxes[part.Item1, part.Item2].Background = new SolidColorBrush(snake.IsDead ? snake.DeadColor : snake.Color);
                }
                if (!snake.IsDead)
                {
                    var head = snake.Parts.Last();
                    _boxes[head.Item1, head.Item2].Background = new SolidColorBrush(snake.HeadColor);
                }
                foreach (var food in _foods)
                {
                    _boxes[food.Item1, food.Item2].IsChecked = true;
                    _boxes[food.Item1, food.Item2].Background = new SolidColorBrush(Brushes.Red.Color);
                }
            }
        }

        private void Update()
        {
            CheckEndGame();
            UpdateMovement();
            //ResetGrid();

            foreach(var snake in _snakes)
            {
                var next = snake.Parts.Last();
                if (snake.Direction == Direction.Up)
                    next = new Tuple<int, int>(((next.Item1 - 1)%Rows+Rows)%Rows, next.Item2);
                else if(snake.Direction == Direction.Down)
                    next = new Tuple<int, int>((next.Item1 + 1) % Rows, next.Item2);
                else if (snake.Direction == Direction.Left)
                    next = new Tuple<int, int>(next.Item1, ((next.Item2 - 1) % Columns + Columns) % Columns);
                else if (snake.Direction == Direction.Right)
                    next = new Tuple<int, int>(next.Item1, (next.Item2 + 1) % Columns);

                if (CheckCollision(next))
                {
                    snake.Die();
                    continue;
                }

                bool isFood = false;
                if (_foods.Contains(next))
                {
                    isFood = true;
                    _foods.Add(GenerateRandomFood());
                    _foods.Remove(next);
                    _scoreBoxes[snake].Text = (int.Parse(_scoreBoxes[snake].Text) + 1).ToString();
                }
                if(!isFood)
                {
                    var cb = _boxes[snake.Parts.First().Item1, snake.Parts.First().Item2];
                    cb.IsChecked = false;
                    cb.Background = null;
                }
                snake.MoveTo(next, isFood);
            }


            Draw();
            _pressedKeys.Clear();
        }

        private bool CheckCollision(Tuple<int, int> pt)
        {
            var pts = _snakes.SelectMany(x => x.Parts);
            return pts.Contains(pt);
        }
        #endregion

        private Tuple<int, int> GenerateRandomFood()
        {
            var takenPoints = _snakes.SelectMany(x => x.Parts).Union(_foods);
            var possible = _allPoints.Except(takenPoints);
            int idx = _rand.Next(0, possible.Count());
            return possible.ElementAt(idx);
        }
    }
}
