using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csMACnz.ConcurrentConsole;
using Xunit;

namespace Tests
{
    public class ConsoleTests : IDisposable
    {
        private Encoding _defaultInputEncoding;
        private Encoding _defaultOutputEncoding;

        public ConsoleTests()
        {
            _defaultInputEncoding = System.Console.InputEncoding;
            _defaultOutputEncoding = System.Console.OutputEncoding;
        }

        void IDisposable.Dispose()
        {
            System.Console.InputEncoding = _defaultInputEncoding;
            System.Console.OutputEncoding = _defaultOutputEncoding;
        }

        [Fact]
        public void WhenGetInstanceThenInstanceNotNull()
        {
            var console = csMACnz.ConcurrentConsole.Console.Instance;
            Assert.NotNull(console);
        }

        [Fact]
        public void WhenIWriteALineThenANewLineIsPrinted()
        {
            TestBuffer buffer = new TestBuffer();
            var console = csMACnz.ConcurrentConsole.Console.CreateForTesting(buffer);

            console.WriteLine("Test");

            Assert.Equal("Test", buffer.PreviousLine);
        }
    }

    public class TestBuffer : IConsoleFacade
    {
        Encoding IConsoleFacade.InputEncoding { get; set; }

        Encoding IConsoleFacade.OutputEncoding { get; set; }

        int IConsoleFacade.CursorTop { get { return 0; } }

        ConsoleKeyInfo IConsoleFacade.ReadKey()
        {
            throw new NotImplementedException();
        }

        void IConsoleFacade.SetCursorPosition(int left, int top)
        {

        }

        void IConsoleFacade.Write(string value)
        {
            throw new NotImplementedException();
        }

        void IConsoleFacade.WriteLine()
        {
            throw new NotImplementedException();
        }

        void IConsoleFacade.WriteLine(char[] buffer)
        {
            throw new NotImplementedException();
        }

        void IConsoleFacade.WriteLine(string value)
        {
            Append(value);
            NewLine();
        }

        void IConsoleFacade.WriteLine(object value)
        {
            throw new NotImplementedException();
        }

        void IConsoleFacade.WriteLine(string format, params object[] arg)
        {
            throw new NotImplementedException();
        }

        void IConsoleFacade.WriteLine(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public string PreviousLine { get { return _stack.LastOrDefault(); } }

        private readonly List<string> _stack = new List<string>();
        private string _buffer = string.Empty;
        private void Append(string value)
        {
            if (value == null) return;
            var strings = value.Split('\n');
            if (strings.Length > 1)
            {
                foreach (var str in strings.Take(strings.Length - 1))
                {
                    _stack.Add(str);
                }
                _buffer += strings[strings.Length - 1];
            }
            else
            {
                _buffer += value;
            }
        }

        private void NewLine()
        {
            _stack.Add(_buffer);
            _buffer = string.Empty;
        }
    }
}
