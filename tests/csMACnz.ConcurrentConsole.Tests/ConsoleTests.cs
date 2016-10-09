using Xunit;

namespace Tests
{
    public class ConsoleTests
    {
        [Fact]
        public void WhenGetInstanceThenInstanceNotNull() 
        {
            var console = csMACnz.ConcurrentConsole.Console.Instance;
            Assert.NotNull(console);
        }
    }
}
