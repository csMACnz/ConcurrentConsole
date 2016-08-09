using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rand = new Random();
            var console = new csMACnz.ConcurrentConsole.Console();
            console.Prompt = "::";

            CancellationTokenSource source = new CancellationTokenSource();
            Task t = Task.Factory.StartNew(() =>
            {
                while(true){
                    source.Token.ThrowIfCancellationRequested();
                    int number = rand.Next(1, 5);
                    Thread.Sleep(number * 500);
                    console.WriteLine($"Random message {number}");
                }
            }, source.Token);
            string input;
            while (null != (input = console.ReadLine()))
            {
                console.WriteLine(input);
            }
            source.Cancel();
        }
    }
}
