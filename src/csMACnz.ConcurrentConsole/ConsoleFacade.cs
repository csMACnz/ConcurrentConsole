using System;
using System.Text;

namespace csMACnz.ConcurrentConsole
{
    public class ConsoleFacade : IConsoleFacade
    {
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
        public int CursorTop { get { return System.Console.CursorTop; } }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return System.Console.ReadKey();
        }

        public void WriteLine()
        {
            System.Console.WriteLine();
        }

        public void WriteLine(object value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(char[] buffer)
        {
            System.Console.WriteLine(buffer);
        }

        public void WriteLine(char[] buffer, int index, int count)
        {
            System.Console.WriteLine(buffer, index, count);
        }

        public void WriteLine(string format, params object[] arg)
        {
            System.Console.WriteLine(format, arg);
        }

        public void Write(string value)
        {
            System.Console.Write(value);
        }
    }
}