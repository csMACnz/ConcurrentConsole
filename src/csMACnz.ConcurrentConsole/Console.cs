using System;

namespace csMACnz.ConcurrentConsole
{
    public class Console
    {
        public string Prompt { get; set; }
        private string _input = null;
        public string ReadLine()
        {
            _input = "";
            while(true){
                var value = System.Console.ReadKey();
                if((value.Modifiers & ConsoleModifiers.Alt) != 0){}
                else if((value.Modifiers & ConsoleModifiers.Shift) != 0){}
                else if((value.Modifiers & ConsoleModifiers.Control) != 0){}
                if(value.Key == ConsoleKey.Enter){
                    break; 
                }
                else if(value.Key == ConsoleKey.Escape){
                    _input = "";
                }
                else
                {
                    _input += value.KeyChar;
                }
            }
            var result = _input;
            _input = null;
            return result;
        }

        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
    }
}
