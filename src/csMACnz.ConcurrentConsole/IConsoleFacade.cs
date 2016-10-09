using System;
using System.Text;

namespace csMACnz.ConcurrentConsole
{
    public interface IConsoleFacade
    {
        Encoding InputEncoding { get; set; }
        Encoding OutputEncoding { get; set; }
        int CursorTop { get; }

        void SetCursorPosition(int left, int top);
        ConsoleKeyInfo ReadKey();
        void WriteLine();
        void WriteLine(object value);
        void WriteLine(string value);
        void WriteLine(char[] buffer);
        void WriteLine(char[] buffer, int index, int count);
        void WriteLine(string format, params object[] arg);
        void Write(string value);
    }
}