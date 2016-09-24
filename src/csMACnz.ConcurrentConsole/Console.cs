using System;
using System.Text;

namespace csMACnz.ConcurrentConsole
{
    //TODO: docs comments
    public class Console : IConsole
    {
#if net35
        private static Console _instance;
#else
    private static Lazy<Console> _instance = new Lazy<Console>(()=> new Console());
#endif

        public static IConsole Instance
        {
            get
            {
#if net35
                if (_instance == null)
                {
                    _instance = new Console();
                }
                return _instance;
#else
            return _instance.Value;
#endif
            }
        }

        private Console()
        {
            OutputEncoding = System.Text.Encoding.Unicode;
            InputEncoding = System.Text.Encoding.Unicode;
            TabSize = 4;
        }

        private int _tabSize;
        public int TabSize
        {
            get
            {
                return _tabSize;
            }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "TabSize cannot be negative.");
                _tabSize = value;
            }
        }

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

        public string InputPrefix { get; set; }

        public Encoding InputEncoding
        {
            get
            {
                return System.Console.InputEncoding;
            }
            set
            {
                System.Console.InputEncoding = value;
            }
        }

        public Encoding OutputEncoding
        {
            get
            {
                return System.Console.OutputEncoding;
            }
            set
            {
                System.Console.OutputEncoding = value;
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
                else if (value.Key == ConsoleKey.LeftArrow) { }//TODO: cursor offset
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
                    var remainder = _input.Length % TabSize;
                    var spaceCount = TabSize - remainder;
                    //Tabs cause trouble with delete, so just use spaces
                    _input += new string(' ', spaceCount);
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

                System.Console.WriteLine($"{InputPrefix}{result}");

                ReprintReadHead();
            }
            return result;
        }

        public void WriteLine()
        {
            LockForLineWrites(() =>
            {
                System.Console.WriteLine();
            });
        }

        public void WriteLine(char[] buffer)
        {
            LockForLineWrites(() =>
            {
                System.Console.WriteLine(buffer);
            });
        }

        public void WriteLine(char[] buffer, int index, int count)
        {
            LockForLineWrites(() =>
            {
                System.Console.WriteLine(buffer, index, count);
            });
        }

        public void WriteLine(object value)
        {
            LockForLineWrites(() =>
            {
                System.Console.WriteLine(value);
            });
        }

        public void WriteLine(string value)
        {
            LockForLineWrites(() =>
            {
                System.Console.WriteLine(value);
            });
        }

        public void WriteLine(string format, params object[] arg)
        {
            LockForLineWrites(() =>
            {
                System.Console.WriteLine(format, arg);
            });
        }

        private void LockForLineWrites(Action action)
        {
            lock (_locker)
            {
                ClearTheLine();

                action();

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
                var whitespace = new string(' ', _currentInputLength);

                System.Console.SetCursorPosition(0, System.Console.CursorTop);

                System.Console.Write(whitespace);
            }

            System.Console.SetCursorPosition(0, System.Console.CursorTop);
        }
    }
}
