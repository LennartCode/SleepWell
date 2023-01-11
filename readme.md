# SleepWell

But most importantly: Sleep!

This programm shuts the computer down at a selected time after giving the user 3 rounds of applause for going to bed themselves. The program will delay the shutdown timer initiation, if the user has not been using the machine in the past 30 seconds after the start time. The computer will not shut down if any other users are signed in.

## System requirements

This program only runs on a Windows 7 machine or higher. Other operating systems are not supported.

## Setup SleepWell

1. Clone the repository
2. Copy the files `Applause.wav` and `SleepWell.exe` from the folder `SleepWell\bin\Release\net6.0\publish` to the folder `C:\Skripte\SleepWell`. Create the folder if necessary.
3. Import the scheduled task:

    ```xml
    <?xml version="1.0" encoding="UTF-16"?>
        <Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
        <RegistrationInfo>
            <Date>2023-01-10T18:56:08.2699817</Date>
            <Author>LennartCode</Author>
            <URI>\SleepWell</URI>
        </RegistrationInfo>
        <Triggers>
            <CalendarTrigger>
            <StartBoundary>2023-01-10T01:00:00</StartBoundary>
            <Enabled>true</Enabled>
            <ScheduleByDay>
                <DaysInterval>1</DaysInterval>
            </ScheduleByDay>
            </CalendarTrigger>
        </Triggers>
        <Principals>
            <Principal id="Author">
            <UserId>S-1-5-21-870278489-586754455-1883670123-1145</UserId>
            <LogonType>InteractiveToken</LogonType>
            <RunLevel>LeastPrivilege</RunLevel>
            </Principal>
        </Principals>
        <Settings>
            <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
            <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
            <StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>
            <AllowHardTerminate>true</AllowHardTerminate>
            <StartWhenAvailable>false</StartWhenAvailable>
            <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>
            <IdleSettings>
            <StopOnIdleEnd>true</StopOnIdleEnd>
            <RestartOnIdle>false</RestartOnIdle>
            </IdleSettings>
            <AllowStartOnDemand>true</AllowStartOnDemand>
            <Enabled>true</Enabled>
            <Hidden>false</Hidden>
            <RunOnlyIfIdle>false</RunOnlyIfIdle>
            <WakeToRun>false</WakeToRun>
            <ExecutionTimeLimit>PT72H</ExecutionTimeLimit>
            <Priority>7</Priority>
        </Settings>
        <Actions Context="Author">
            <Exec>
            <Command>C:\Skripte\SleepWell\SleepWell.exe</Command>
            </Exec>
        </Actions>
    </Task>
    ```

## Credit

- [Moon icon](https://www.flaticon.com/free-icon/moon_9334092) by artist [Graphics Plazza](https://www.flaticon.com/authors/graphics-plazza) downloaded from [flaticon](www.flaticon.com).

- Applause sound [Applause 1](https://bigsoundbank.com/detail-2363-applause-1.html) by artist [Dorian CLAIR](https://dorianclair.wixsite.com/sondier) *dead URL* downloaded from [Joseph SARDIN - BigSoundBank](https://bigsoundbank.com/)
