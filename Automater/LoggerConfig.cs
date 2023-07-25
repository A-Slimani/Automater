using Serilog;

namespace Automater
{
    public static class LoggerConfig
    {
        public static ILogger ConfigureLogger()
        {
            string date = DateTime.Now.ToString("ddMMyy");

            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File($"logs/{date}.log")
                .CreateLogger();
        }
    }
}
