using Neuro_BrainBit;
using NeuroSDK;
using System;


//Global variables
bool activeConsole = true;
bool is_Connected = false;

Scanner scanner = default;
BrainBitSensor? sensor = null;

IReadOnlyList<SensorInfo> sensors = [];



//Console

Console.WriteLine("<-- BrainBit device -->");
Console.WriteLine("Write help to see the list of commands.");
var commands = ConsoleHelper.InitializeCommands();

RunConsoleLoop();


//Methods
void RunConsoleLoop()
{
    do 
    {
        Console.WriteLine("Insert command:");
        var line = Console.ReadLine();

        if (!string.IsNullOrEmpty(line))
        {
            if (line == "exit")
            {
                activeConsole = false;
                break;
            }

            var commandParam = ConsoleHelper.ExtractParameters(line);

            ExecuteCommand(commandParam.Item1, commandParam.Item2);
        }
        else 
        {
            ConsoleHelper.WriteLine(2, "Please enter a valid option. Use the help command to see all options.", ConsoleColor.Red);
        }
        
        Console.WriteLine("----------------->");
    }
    while (activeConsole);
}

void ExecuteCommand(string command, string parameter)
{
    switch (command.ToLower())
    {
        case "help":
            ShowHelp();
            break;
        case "search":
            SearchDevices();
            break;
        case "connect":
            ConnectDevice();
            break;
        case "search-and-connect":
            SearchAndConnectDevices();
            break;
        case "disconnect":
            DisconnectDevice();
            break;
        case "resistance":
            GetResistance(parameter);
            break;
        case "connection-state-start":
            StartConnectionState();
            break;
        case "connection-state-end":
            EndConnectionState();
            break;
        case "battery-start":
            StartBatteryStatus();
            break;
        case "battery-stop":
            StopBatteryStatus();
            break;
        case "start-signal-print":
            StartSignalWithPrint(parameter);
            break;
        case "emotion-start":
            StartEmotion(parameter);
            break;
        case "emotion-stop":
            StopEmotion();
            break;
        default:
            ConsoleHelper.WriteLine(2, "Please insert a correct option", ConsoleColor.Red);
            break;
    }

    Console.WriteLine("-----------------");
}

void ShowHelp()
{
    ConsoleHelper.ShowHelp(commands);
}

void SearchDevices()
{
    scanner = BrainBitHelper.CreateScanner();
    sensors = BrainBitHelper.SearchDevice(scanner);
}

void ConnectDevice()
{
    if (sensors.Any() && scanner != null)
    {
        var firstSensor = sensors.First();
        sensor = BrainBitHelper.ConnectDevice(scanner, firstSensor, out is_Connected);
    }
    else
    {
        ConsoleHelper.WriteLine(2, "Scanner null or sensors empty", ConsoleColor.Red);
    }
}

void SearchAndConnectDevices()
{
    SearchDevices();
    ConnectDevice();
}

void DisconnectDevice()
{
    if (CheckConnection())
    {
        BrainBitHelper.DisconnectDevice(sensor, out is_Connected);
    }
}

void GetResistance(string parameter)
{
    if (CheckConnection())
    {
        BrainBitHelper.GetResistance(sensor, ConsoleHelper.GetSeconds(parameter));
    }
}

void StartConnectionState()
{
    if (CheckConnection())
    {
        BrainBitHelper.EventConnectionStateStart(sensor);
    }
}

void EndConnectionState()
{
    if (CheckConnection())
    {
        BrainBitHelper.EventConnectionStateEnd(sensor);
    }
}

void StartBatteryStatus()
{
    if (CheckConnection())
    {
        BrainBitHelper.EventBateryStatusStart(sensor);
    }
}

void StopBatteryStatus()
{
    if (CheckConnection())
    {
        BrainBitHelper.EventBateryStatusStop(sensor);
    }
}

void StartSignalWithPrint(string parameter)
{
    if (CheckConnection())
    {
        BrainBitHelper.StartSignalWithPrint(sensor, ConsoleHelper.GetSeconds(parameter));
    }
}

void StartEmotion(string parameter)
{
    if (CheckConnection())
    {
        BrainBitHelper.StartEmotionBipolar(sensor, ConsoleHelper.GetSeconds(parameter));
    }
}

void StopEmotion()
{
    if (CheckConnection())
    {
        BrainBitHelper.StopEmotionBipolar(sensor);
    }
}

bool CheckConnection()
{
    if (!is_Connected || sensor == null)
    {
        ConsoleHelper.WriteLine(2, "You should connect the device first!", ConsoleColor.Red);
        return false;
    }
    return true;
}



Console.WriteLine("-----------------");
