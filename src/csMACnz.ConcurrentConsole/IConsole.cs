using System.Text;

namespace csMACnz.ConcurrentConsole
{    
    public interface IConsole
    {
        //TODO: more matching overrides from System.Console
        string ReadLine();
        void WriteLine();
        void WriteLine(char[] buffer);
        void WriteLine(char[] buffer, int index, int count);
        void WriteLine(object value);
        void WriteLine(string value);
        void WriteLine(string format, params object[] arg);
        int TabSize { get; set; }
        string Prompt { get; set; }
        string InputPrefix { get; set; }
        Encoding InputEncoding { get; set; }
        Encoding OutputEncoding { get; set; }
    }
}