using System.Diagnostics;
using System.Media;
using System.Runtime.InteropServices;

// Initialize logger.
Logger logger = new Logger(Path.GetTempPath());
logger.WriteLog("SleepWell started.");

// Test for operating system as this program only runs on Windows.
if (!OperatingSystem.IsWindows())
{
    string problemText = "This application only runs on Windows operating systems!";
    logger.WriteLog(problemText);
    throw new Exception(problemText);
}
else
{
    logger.WriteLog("The operating system is Windows. SleepWell can be executed properly.");
}

// Ensure that the system is not idle when initiating shutdown. 
InputInfo input = new InputInfo();
DateTime lastSystemInteraction = input.GetLastInputTime();
DateTime sleepWellStarted = DateTime.Now;


if (sleepWellStarted - lastSystemInteraction > TimeSpan.FromSeconds(30.0))
{
    bool hasUserReturned = false;
    while (!hasUserReturned)
    {
        logger.WriteLog("The system has not seen user input for the last 30 seconds. Postponing shutdown for 5 minutes");
        // Every 5 minutes, check if there has been system interaction after SleepWell has been started that is not older than 30 seconds (user has returned and left again).
        Thread.Sleep(5 * 60 * 1000);
        lastSystemInteraction = input.GetLastInputTime();
        if (sleepWellStarted < lastSystemInteraction && DateTime.Now - lastSystemInteraction < TimeSpan.FromSeconds(30.0))
        {
            hasUserReturned = true;
        }
    }
}

logger.WriteLog("The system has seen user input in the last 30 seconds. Initiating shutdown sequence.");


// Set up sound player.
string pathToFile = @".\Applause.wav";
SoundPlayer soundPlayer = new SoundPlayer();
try
{
    soundPlayer.SoundLocation = pathToFile;
    soundPlayer.Load();
    logger.WriteLog("Sound player loaded successfully.");
}
catch (System.Exception)
{
    string problemText = "The sound file was not found! Path: " + pathToFile;
    logger.WriteLog(problemText);
    throw new FileNotFoundException("The sound file was not found! Path: " + pathToFile, pathToFile);
}

// Synchronously play the sound and wait for 2 and 1 minutes. Synchronous to make sure, that the sound is played before the shutdown is initiated.
soundPlayer.PlaySync();
logger.WriteLog("Applause is played for the first time. 3 minutes to shutdown");
Thread.Sleep(2 * 60 * 1000);
soundPlayer.PlaySync();
logger.WriteLog("Applause is played for the second time. 1 minute to shutdown");
Thread.Sleep(1 * 60 * 1000);
soundPlayer.PlaySync();
logger.WriteLog("Applause is played for the second time. Shutdown now.");


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
{
    string problemText = "There are other users logged on. Not shutting down.";
    logger.WriteLog(problemText);
    throw new Exception(problemText);
}

// Shutting down.
logger.WriteLog("Shutting down.");
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

class Logger
{
    private StreamWriter logFile;

    public Logger(string Path)
    {
        string folder = Path + "SleepWell_Log_" + DateTime.Now.ToString("HH.mm.ss") + ".txt";
        logFile = new StreamWriter(folder, append: true);
        logFile.AutoFlush = true;
    }

    public void WriteLog(string LogText)
    {
        logFile.WriteLine(DateTime.Now + ": " + LogText);
    }
}