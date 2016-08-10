using System;

namespace csMACnz.ConcurrentConsole
{
    public class Console
    {
        private string _prompt;
        public string Prompt
        {
            get
            {
                return _prompt;
            }
            set
            {
                if (value != _prompt)
                {
                    lock (_locker)
                    {
                        ClearTheLine();
                        
                        _prompt = value;

                        ReprintReadHead();
                    }
                }
            }
        }
        private object _locker = new object();
        private string _input = null;
        private int _currentInputLength = 0;
        public string ReadLine()
        {
            _input = "";
            while (true)
            {
                var value = System.Console.ReadKey();
                if ((value.Modifiers & ConsoleModifiers.Alt) != 0) { }
                else if ((value.Modifiers & ConsoleModifiers.Control) != 0) { }
                else if (value.Key == ConsoleKey.Home) { }
                else if (value.Key == ConsoleKey.End) { }
                else if (value.Key == ConsoleKey.PageUp) { }
                else if (value.Key == ConsoleKey.PageDown) { }
                else if (value.Key == ConsoleKey.UpArrow) { }
                else if (value.Key == ConsoleKey.DownArrow) { }
                else if (value.Key == ConsoleKey.LeftArrow) { }
                else if (value.Key == ConsoleKey.RightArrow) { }
#if net35 || net40 || net45
                else if (value.Key == ConsoleKey.Applications) { }
#endif
                else if (value.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (value.Key == ConsoleKey.Escape)
                {
                    _input = "";
                }
                else if (value.Key == ConsoleKey.Backspace)
                {
                    _input = _input.Length > 0 ? _input.Substring(0, _input.Length - 1) : _input;
                }
                else if (value.Key == ConsoleKey.Tab)
                {
                    //TODO
                    _input += value.KeyChar;
                }
                else
                {
                    _input += value.KeyChar;
                }
                lock (_locker)
                {
                    ClearTheLine();

                    ReprintReadHead();
                }
            }
            var result = _input;

            lock (_locker)
            {
                ClearTheLine();

                _input = null;

                System.Console.WriteLine(result);

                ReprintReadHead();
            }
            return result;
        }

        public void WriteLine(string value)
        {
            lock (_locker)
            {
                ClearTheLine();

                System.Console.WriteLine(value);

                ReprintReadHead();
            }
        }

        private void ReprintReadHead()
        {
            _currentInputLength = 0;
            if (Prompt != null)
            {
                System.Console.Write(Prompt);
                _currentInputLength += Prompt.Length;
            }
            if (!string.IsNullOrEmpty(_input))
            {
                System.Console.Write(_input);
                _currentInputLength += _input.Length;
            }
        }

        private void ClearTheLine()
        {
            if (_currentInputLength > 0)
            {
                var whitepspace = new string(' ', _currentInputLength);

                System.Console.SetCursorPosition(0, System.Console.CursorTop);

                System.Console.Write(whitepspace);
            }

            System.Console.SetCursorPosition(0, System.Console.CursorTop);
        }
    }
}
