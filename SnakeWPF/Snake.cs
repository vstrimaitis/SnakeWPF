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
        private List<Tuple<int, int>> _parts;

        public Color Color { get; private set; }
        public Color HeadColor { get; private set; }
        public Color DeadColor { get; private set; }
        public Direction Direction { get; private set; }
        public ControlScheme ControlScheme { get; private set; }
        public bool IsDead { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<Tuple<int, int>> Parts
        {
            get
            {
                return _parts;
            }
        }

        public Snake(string name, int r, int c, Direction initialDirection, Color color, Color headColor, Color deadColor, ControlScheme scheme)
        {
            _parts = new List<Tuple<int, int>>();
            _parts.Add(new Tuple<int, int>(r, c));
            Name = name;
            Direction = initialDirection;
            Color = color;
            HeadColor = headColor;
            DeadColor = deadColor;
            ControlScheme = scheme;
        }

        public void Die()
        {
            IsDead = true;
            Direction = Direction.None;
        }

        public void MoveTo(Tuple<int, int> pt, bool isFood)
        {
            _parts.Add(pt);
            if (!isFood)
                _parts.Remove(_parts.First());
        }

        public void ChangeDirection(Direction dir)
        {
            Direction = dir;
        }
    }
}
