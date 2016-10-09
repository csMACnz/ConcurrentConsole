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
    private static Lazy<Console> _instance = new Lazy<Console>(()=> new Console(new ConsoleFacade()));
#endif

        public static IConsole Instance
        {
            get
            {
#if net35
                if (_instance == null)
                {
                    _instance = new Console(new ConsoleFacade());
                }
                return _instance;
#else
            return _instance.Value;
#endif
            }
        }

        private IConsoleFacade _actualConsole;

        private Console(IConsoleFacade actualConsole)
        {
            _actualConsole = actualConsole;
            OutputEncoding = System.Text.Encoding.Unicode;
            InputEncoding = System.Text.Encoding.Unicode;
            TabSize = 4;
        }

        public static Console CreateForTesting(IConsoleFacade testFacade)
        {
            System.Console.WriteLine("Creating a Console for testing");
            return new Console(testFacade);
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
                return _actualConsole.InputEncoding;
            }
            set
            {
                _actualConsole.InputEncoding = value;
            }
        }

        public Encoding OutputEncoding
        {
            get
            {
                return _actualConsole.OutputEncoding;
            }
            set
            {
                _actualConsole.OutputEncoding = value;
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
                var value = _actualConsole.ReadKey();
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

                _actualConsole.WriteLine($"{InputPrefix}{result}");

                ReprintReadHead();
            }
            return result;
        }

        public void WriteLine()
        {
            LockForLineWrites(() =>
            {
                _actualConsole.WriteLine();
            });
        }

        public void WriteLine(char[] buffer)
        {
            LockForLineWrites(() =>
            {
                _actualConsole.WriteLine(buffer);
            });
        }

        public void WriteLine(char[] buffer, int index, int count)
        {
            LockForLineWrites(() =>
            {
                _actualConsole.WriteLine(buffer, index, count);
            });
        }

        public void WriteLine(object value)
        {
            LockForLineWrites(() =>
            {
                _actualConsole.WriteLine(value);
            });
        }

        public void WriteLine(string value)
        {
            LockForLineWrites(() =>
            {
                _actualConsole.WriteLine(value);
            });
        }

        public void WriteLine(string format, params object[] arg)
        {
            LockForLineWrites(() =>
            {
                _actualConsole.WriteLine(format, arg);
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
                _actualConsole.Write(Prompt);
                _currentInputLength += Prompt.Length;
            }
            if (!string.IsNullOrEmpty(_input))
            {
                _actualConsole.Write(_input);
                _currentInputLength += _input.Length;
            }
        }

        private void ClearTheLine()
        {
            if (_currentInputLength > 0)
            {
                var whitespace = new string(' ', _currentInputLength);

                _actualConsole.SetCursorPosition(0, _actualConsole.CursorTop);

                _actualConsole.Write(whitespace);
            }

            _actualConsole.SetCursorPosition(0, _actualConsole.CursorTop);
        }
    }
}
