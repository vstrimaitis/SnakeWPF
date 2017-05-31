using System.Windows.Input;

namespace SnakeWPF
{
    struct ControlScheme
    {
        public Key KeyUp { get; private set; }
        public Key KeyDown { get; private set; }
        public Key KeyLeft { get; private set; }
        public Key KeyRight { get; private set; }
        public Key KeyAction { get; private set; }

        public ControlScheme(Key up, Key down, Key left, Key right, Key action)
        {
            KeyUp = up;
            KeyDown = down;
            KeyLeft = left;
            KeyRight = right;
            KeyAction = action;
        }
    }
}
