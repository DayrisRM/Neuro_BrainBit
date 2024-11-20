using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Neuro_BrainBit
{
    public class NeurofeedbackService
    {        
        EmotionBipolar emotionBi;
        private static System.Timers.Timer timer;
        List<NeurofeedbackDto> neurofeedbackResults;

        public NeurofeedbackService(EmotionBipolar _emotionBi) 
        { 
            emotionBi = _emotionBi;   
            neurofeedbackResults = new List<NeurofeedbackDto>();
        }

        public void Start() 
        {           
            // Configurar el timer para que dispare el evento cada 3 segundos (3000 milisegundos)
            timer = new System.Timers.Timer(3000);
            timer.Elapsed += GetRatioEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            //add separator in log file
            WriteLog($"NF start - {DateTime.Now}");
        }

        public void Stop() 
        {             
            timer.Elapsed -= GetRatioEvent;
            timer.Dispose();
        }

        private void GetRatioEvent(Object source, ElapsedEventArgs e) 
        {
            Console.WriteLine($"GetRatioEvent - datetime:{DateTime.Now}");

            //Call to Emotion to get beta y theta
            //Calculate ratio
            //Store ratio, beta, theta
            //verficar si el ratio aumenta o disminuye 
            //a la aplicación se debe informar si la atención es buena o ha empeorado. Esto se puede hacer con un método

            //si no está calibrado se salta el cálculo
            //si hay artefactos -- ver qué se hace¿?¿?¿?¿? añadir una propiedad que lo informe?? añadir una ventana de tiempo en la que si se mantienen los artefactos se deba detener el algoritmo.

            if (emotionBi._isCalibrated && !emotionBi.IsBothSidesArtifacted) 
            {
                emotionBi.ResolveSpectralData();
                var spectralData = emotionBi.LastSpectralData;
                //theta:{LastSpectralData.Theta} -- beta: {LastSpectralData.Beta}
                var theta = spectralData.Theta;
                var beta = spectralData.Beta;

                var ratio = CalculateRatio(theta, beta);

                var nfDTO = new NeurofeedbackDto(beta, theta, ratio);
                neurofeedbackResults.Add(nfDTO);
                WriteLog(BuildNFLog(nfDTO));
            }
            else
            {
                Console.WriteLine($"GetRatioEvent - error _isCalibrated:{emotionBi._isCalibrated} - IsBothSidesArtifacted:{emotionBi.IsBothSidesArtifacted}");
            }
            
        }
        private double CalculateRatio(double theta, double beta) 
        {
            // Calcular el ratio beta/theta
            //double ratio = beta / theta; es al revés

            double ratio = theta / beta;
            // Mostrar el resultado
            Console.WriteLine($"Ratio Theta/Beta: {ratio} (Beta: {beta}, Theta: {theta})");

            return ratio;
        }

        private string BuildNFLog(NeurofeedbackDto dto) 
        {
            return $"Datetime: {dto.Date} - Ratio Theta/Beta: {dto.Ratio} (Beta: {dto.Beta}, Theta: {dto.Theta})";
        }

        private void WriteLog(string logText) 
        {
            var fileName = "log.txt";

            // Obtener el directorio base del proyecto
            string projectDirectory = "C:\\Users\\dayri\\Documents\\NeuroTB\\Project\\Neuro_BrainBit\\Neuro_BrainBit\\Neuro_BrainBit\\";

            string pathFile = Path.Combine(projectDirectory, @"data\", fileName);

            File.AppendAllText(pathFile, logText + Environment.NewLine);
        }

    }
}
