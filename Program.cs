using System.Diagnostics;
using System.Media;
using System.Runtime.InteropServices;

if (!OperatingSystem.IsWindows())
    throw new Exception("This application only runs on Windows operating systems!");

// Ensure that the system is not idle when initiating shutdown. 
InputInfo input = new InputInfo();
DateTime lastSystemInteraction = input.GetLastInputTime();
DateTime sleepWellStarted = DateTime.Now;


if (sleepWellStarted - lastSystemInteraction > TimeSpan.FromSeconds(30.0))
{
    // System has not seen user inputs for the last 30 seconds.
    bool hasUserReturned = false;
    while (!hasUserReturned)
    {
        // Every 5 minutes, check if there has been system interaction after SleepWell has been started that is not older than 30 seconds (user has returned and left again).
        Thread.Sleep(5 * 60 * 1000);
        lastSystemInteraction = input.GetLastInputTime();
        if (sleepWellStarted < lastSystemInteraction && DateTime.Now - lastSystemInteraction < TimeSpan.FromSeconds(30.0))
        {
            hasUserReturned = true;
        }
    }
}


// Set up sound player.
string pathToFile = "Applause.wav";
SoundPlayer soundPlayer = new SoundPlayer();
try
{
    soundPlayer.SoundLocation = pathToFile;
    soundPlayer.Load();
}
catch (System.Exception)
{
    throw new FileNotFoundException("The sound file was not found! Path:" + pathToFile, pathToFile);
}

// Synchronously play the sound and wait for 2 and 1 minutes. Synchronous to make sure, that the sound is played before the shutdown is initiated.
soundPlayer.PlaySync();
Thread.Sleep(2 * 60 * 1000);
soundPlayer.PlaySync();
Thread.Sleep(1 * 60 * 1000);
soundPlayer.PlaySync();


// Test if any other users are logged on.
var psi2 = new ProcessStartInfo("quser");
var process2 = new Process();
process2!.StartInfo = psi2;
process2!.StartInfo.RedirectStandardOutput = true;
process2.Start();
process2!.WaitForExit();
string userList = process2!.StandardOutput.ReadToEnd();
int loggedOnUsersAmount = userList.Count(t => t == '\n') - 1;

if (loggedOnUsersAmount > 1)
    throw new Exception("There are other users logged on. Not shutting down.");

// Shutting down.
var psi = new ProcessStartInfo("shutdown", "/s /t 0");
psi.CreateNoWindow = true;
psi.UseShellExecute = false;
Process.Start(psi);



class InputInfo
{
    [DllImport("User32.dll")]
    public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    public DateTime GetLastInputTime()
    {
        var lastInputInfo = new LASTINPUTINFO();
        lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

        GetLastInputInfo(ref lastInputInfo);

        return DateTime.Now.AddMilliseconds(-(Environment.TickCount - lastInputInfo.dwTime));
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
}
