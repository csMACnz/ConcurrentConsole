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
                    ClearTheLine();
                    _prompt = value;
                    ClearTheLine();

                    ResetReadHead();
                }
            }
        }
        private object _locker = new object();
        private string _input = null;
        public string ReadLine()
        {
            _input = "";
            while (true)
            {
                var value = System.Console.ReadKey();
                if ((value.Modifiers & ConsoleModifiers.Alt) != 0) { }
                else if ((value.Modifiers & ConsoleModifiers.Control) != 0) { }
                else if(value.Key == ConsoleKey.Home){}
                else if(value.Key == ConsoleKey.End){}
                else if(value.Key == ConsoleKey.PageUp){}
                else if(value.Key == ConsoleKey.PageDown){}
                else if(value.Key == ConsoleKey.UpArrow){}
                else if(value.Key == ConsoleKey.DownArrow){}
                else if(value.Key == ConsoleKey.LeftArrow){}
                else if(value.Key == ConsoleKey.RightArrow){}
                #if net35 || net40 || net45
                else if(value.Key == ConsoleKey.Applications){}
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
                    lock (_locker)
                    {
                        ClearTheLine();
                    }
                    _input = _input.Length >0?_input.Substring(0, _input.Length-1) :_input;
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

                    ResetReadHead();
                }
            }
            var result = _input;

            WriteLine(result);

            _input = null;
            return result;
        }

        public void WriteLine(string value)
        {
            lock (_locker)
            {
                ClearTheLine();

                System.Console.WriteLine(value);

                ResetReadHead();
            }
        }

        private void ResetReadHead()
        {
            if (Prompt != null)
            {
                System.Console.Write(Prompt);
            }
            if (!string.IsNullOrEmpty(_input))
            {
                System.Console.Write(_input);
            }
        }

        private void ClearTheLine()
        {
            var maxClear = Math.Max(System.Console.CursorLeft, (_input?.Length ?? 0) + (Prompt?.Length ?? 0));
            if (maxClear > 0)
            {
                var message = new string(' ', maxClear);

                System.Console.SetCursorPosition(0, System.Console.CursorTop);

                System.Console.Write(message);
            }

            System.Console.SetCursorPosition(0, System.Console.CursorTop);
        }
    }
}
