namespace Bimber
{
    static class Program
    {
        private static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45FD-A8CF-72F04E6BDE8F}");

        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                mutex.ReleaseMutex();
            }
            else
            {
                // Send message to existing instance
            }
        }
    }

}