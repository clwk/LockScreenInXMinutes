using System.ComponentModel;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool LockWorkStation();

    private static DateTime LockTime { get; set; }
    private static int LastMinute { get; set; }

    static void Main(string[] args)
    {
        TimeSpan lockAfterTimeSpan = ParseArguments(args);
        Console.WriteLine($"Screen will be locked in {lockAfterTimeSpan.Minutes} minutes. ");

        LoopUntilLockTime(lockAfterTimeSpan);

        Console.WriteLine($"Locking NOW. ");

        Thread.Sleep(1000);
        if (!LockWorkStation())
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    private static void LoopUntilLockTime(TimeSpan lockAfterTimeSpan)
    {
        LockTime = DateTime.Now + lockAfterTimeSpan;
        LastMinute = DateTime.Now.Minute;

        while (DateTime.Now < LockTime)
        {
            if (DateTime.Now.Minute != LastMinute)
                System.Console.WriteLine($"Locking in {(LockTime - DateTime.Now).Minutes} minutes.");

            LastMinute = DateTime.Now.Minute;

            Thread.Sleep(1000);
        }
    }

    private static TimeSpan ParseArguments(string[] args)
    {
        var defaultLockMinutes = 25;
        TimeSpan lockAfterTimeSpan = default;

        var argsLength = args.Length;

        if (args.Length >= 1 && int.TryParse(args[0], out int lockAfterXMinutes))
            lockAfterTimeSpan = TimeSpan.FromMinutes(lockAfterXMinutes);
        else
            lockAfterTimeSpan = TimeSpan.FromMinutes(defaultLockMinutes);
        return lockAfterTimeSpan;
    }
}