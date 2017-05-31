using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SnakeWPF
{
    class Snake
    {
        private int _movementCounter;
        private bool _isGrowthPending;
        private List<Tuple<int, int>> _parts;
        private Direction _pendingDirection;

        public Color Color { get; private set; }
        public Color HeadColor { get; private set; }
        public Color DeadColor { get; private set; }
        public Direction Direction { get; private set; }
        public ControlScheme ControlScheme { get; private set; }
        public bool IsDead { get; private set; }
        public string Name { get; private set; }
        public int MovementDelay { get; set; }
        public IEnumerable<Tuple<int, int>> Parts
        {
            get
            {
                return _parts;
            }
        }

        public Snake(string name, int r, int c, int delay, Direction initialDirection, Color color, Color headColor, Color deadColor, ControlScheme scheme)
        {
            _parts = new List<Tuple<int, int>>();
            _parts.Add(new Tuple<int, int>(r, c));
            Name = name;
            MovementDelay = delay;
            Direction = initialDirection;
            Color = color;
            HeadColor = headColor;
            DeadColor = deadColor;
            ControlScheme = scheme;
            _pendingDirection = initialDirection;
        }

        public void Die()
        {
            IsDead = true;
            Direction = Direction.None;
        }

        public void MoveTo(Tuple<int, int> pt, bool isFood)
        {
            _movementCounter = (_movementCounter + 1) % MovementDelay;
            if (isFood)
                _isGrowthPending = true;
            if (_movementCounter != 0)
                return;
            _parts.Add(pt);
            if(!_isGrowthPending)
                _parts.Remove(_parts.First());
            Direction = _pendingDirection;
            _isGrowthPending = false;
        }

        public void ChangeDirection(Direction dir)
        {
            _pendingDirection = dir;
        }
    }
}
