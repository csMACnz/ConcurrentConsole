using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var simple = false;
            if (args.Length > 0 && args[0] == "simple")
            {
                simple = true;
            }
            var rand = new Random();
            var console = csMACnz.ConcurrentConsole.Console.Instance;
            console.Prompt = ">";
            console.InputPrefix = ">";
            
            CancellationTokenSource source = new CancellationTokenSource();
            if (!simple)
            {
                Task t = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        source.Token.ThrowIfCancellationRequested();
                        int number = rand.Next(1, 5);
                        Thread.Sleep(number * 500);
                        console.WriteLine($"Random message {number}");
                    }
                }, source.Token);
            }
            string input;
            while (null != (input = console.ReadLine()))
            {
                if (!simple)
                {
                    console.WriteLine(input);
                }
            }
            source.Cancel();
        }
    }
}
