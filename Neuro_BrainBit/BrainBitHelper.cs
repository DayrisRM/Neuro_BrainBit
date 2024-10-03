using NeuroSDK;

namespace Neuro_BrainBit
{
    public static class BrainBitHelper
    {
        static EmotionBipolar emotionBi = new EmotionBipolar();

        public static Scanner CreateScanner() 
        {
            //Options: SensorFamily.SensorLEBrainBitFlex, SensorFamily.SensorLEBrainBit, SensorFamily.SensorLEBrainBit2
            return new Scanner(SensorFamily.SensorLEBrainBitFlex, SensorFamily.SensorLEBrainBit, SensorFamily.SensorLEBrainBit2);
        }

        public static IReadOnlyList<SensorInfo> SearchDevice(Scanner scanner, int seconds = 10)
        {
            //Search device
            Console.WriteLine($"Search devices ({seconds}s):");           
            scanner.EventSensorsChanged += Scanner_Founded;
            scanner.Start();
            Thread.Sleep(15000);


            IReadOnlyList<SensorInfo> sensors = scanner.Sensors;

            //Stop search after 15s
            Console.WriteLine("-----------------");
            Console.WriteLine($"Stop search after {seconds}s");
            scanner.Stop();
            scanner.EventSensorsChanged -= Scanner_Founded;            


            if (scanner.Sensors.Count == 0)
            {
                throw new Exception("No devices found");
            }

            Console.WriteLine("-----------------");
            Console.WriteLine($"Scanners found: {scanner.Sensors.Count}");

            return sensors;
        }

        public static BrainBitSensor ConnectDevice(Scanner scanner, SensorInfo sens, out bool is_Connected)
        {            
            Console.WriteLine($"Connect to {sens.Name} ({sens.Address})");

            var sensor = scanner.CreateSensor(sens) as BrainBitSensor; //TODO: BrainBitSensor Or BrainBit2Sensor
            if (sensor != null)
            {
                Console.WriteLine($"Successfully connected to {sens.Name} - family:{sensor.SensFamily}!");

                var features = sensor?.Features;
                Console.WriteLine($"{sensor?.Name} features:");
                foreach (var feature in features)
                {
                    Console.WriteLine(feature.ToString());
                }

                is_Connected = true;

                return sensor;
            }

            throw new Exception("Connect error");
        }

        public static void DisconnectDevice(BrainBitSensor sensor, out bool is_Connected)
        {            
            Console.WriteLine($"Disconnecting from {sensor.Name}...");
            sensor.Disconnect();
            Console.WriteLine($"And remove device...");
            sensor.Dispose();
            //scanner.Dispose(); ??
            is_Connected = false;
        }

        public static void GetResistance(BrainBitSensor sensor, int seconds = 10)
        {          
            Console.WriteLine($"Resistance ({seconds}s):");
            sensor.EventBrainBitResistDataRecived += Sensor_EventBrainBitResistDataRecived;
            sensor.ExecCommand(SensorCommand.CommandStartResist);

            Thread.Sleep((int)(TimeSpan.FromSeconds(seconds).TotalMilliseconds));

            sensor.ExecCommand(SensorCommand.CommandStopResist);
            sensor.EventBrainBitResistDataRecived -= Sensor_EventBrainBitResistDataRecived;
        }

        public static void EventConnectionStateStart(BrainBitSensor sensor)
        {
            sensor.EventSensorStateChanged += Sensor_EventSensorStateChanged;
        }

        public static void EventConnectionStateEnd(BrainBitSensor sensor)
        {          
            sensor.EventSensorStateChanged -= Sensor_EventSensorStateChanged;
        }

        public static void EventBateryStatusStart(BrainBitSensor sensor)
        {
            sensor.EventBatteryChanged += Sensor_EventBatteryChanged;            
        }

        public static void EventBateryStatusStop(BrainBitSensor sensor)
        {            
            sensor.EventBatteryChanged -= Sensor_EventBatteryChanged;
        }

        public static void StartSignalWithPrint(BrainBitSensor sensor, int seconds = 10)
        {
            Console.WriteLine($"Signal in mV:");
            sensor.EventBrainBitSignalDataRecived += Sensor_EventBrainBitSignalDataRecived;
            sensor.ExecCommand(SensorCommand.CommandStartSignal);            

            Thread.Sleep((int)(TimeSpan.FromSeconds(seconds).TotalMilliseconds));
            StopSignal(sensor);
        }       

        public static void StartSignal(BrainBitSensor sensor)
        {
            sensor.ExecCommand(SensorCommand.CommandStartSignal);            
        }

        public static void StopSignal(BrainBitSensor sensor)
        {            
            sensor.EventBrainBitSignalDataRecived -= Sensor_EventBrainBitSignalDataRecived;
            sensor.ExecCommand(SensorCommand.CommandStopSignal);            
        }

        public static void StartEmotionBipolar(BrainBitSensor sensor, int seconds = 10)
        {
            sensor.EventBrainBitSignalDataRecived += emotionBi.EmotionProcessData;            
            StartSignal(sensor);
            emotionBi.StartCalibration();

            Thread.Sleep((int)(TimeSpan.FromSeconds(seconds).TotalMilliseconds));

            StopEmotionBipolar(sensor);
        }

        public static void StopEmotionBipolar(BrainBitSensor sensor)
        {
            sensor.EventBrainBitSignalDataRecived -= emotionBi.EmotionProcessData;
            StopSignal(sensor);
            emotionBi.Finish();
            Console.WriteLine("StopEmotionBipolar");
        }


        //BrainBit events
        static void Scanner_Founded(IScanner scanner, IReadOnlyList<SensorInfo> sensors)
        {
            Console.WriteLine($"Found {sensors.Count} devices");
            foreach (var sensor in sensors)
            {
                Console.WriteLine($"With name {sensor.Name} and adress {sensor.Address}");
            }
        }

        static void Sensor_EventBrainBitResistDataRecived(ISensor sensor, BrainBitResistData data)
        {
           // Console.WriteLine($"O1: {data.O1 * 1e3} O2: {data.O2 * 1e3} T3: {data.T3 * 1e3} T4: {data.T4 * 1e3}");
            Console.WriteLine($"O1: {data.O1} O2: {data.O2} T3: {data.T3} T4: {data.T4}");
            Console.WriteLine("-");
        }

        static void Sensor_EventSensorStateChanged(ISensor sensor, SensorState sensorState)
        {
            Console.WriteLine($"{sensor.Name} connection state is {sensorState}");
        }

        static void Sensor_EventBatteryChanged(ISensor sensor, int battPower)
        {
            Console.WriteLine($"{sensor.Name} battery is {battPower}");
        }

        static void Sensor_EventBrainBitSignalDataRecived(ISensor sensor, BrainBitSignalData[] data)
        {
            foreach (BrainBitSignalData signal in data)
            {
                Console.WriteLine($"O1: {signal.O1 * 1e3} O2: {signal.O2 * 1e3} T3: {signal.T3 * 1e3} T4: {signal.T4 * 1e3} - PackNumber: {signal.PackNum}");
            }
        }


    }
}
