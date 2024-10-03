using NeuroSDK;
using SignalMath;

namespace Neuro_BrainBit
{
    public class EmotionBipolar
    {

        public bool IsArtifactedSequence { get; set; }
        public bool IsBothSidesArtifacted { get; set; }
        public double ProgressCalibration { get; set; }
        public SpectralDataPercents LastSpectralData { get; set; }
        public RawSpectVals RawSpectralData { get; set; }
        public MindData LastMindData { get; set; }

        private EegEmotionalMath _math;
        private bool _isCalibrated;

        public EmotionBipolar()
        {
            int samplingFrequency = 250;
            var mls = new MathLibSetting
            {
                sampling_rate = (uint)samplingFrequency,
                process_win_freq = 25,
                n_first_sec_skipped = 4,
                fft_window = (uint)(samplingFrequency * 4),
                bipolar_mode = true,
                channels_number = 4,
                channel_for_analysis = 0
            };

            var ads = new ArtifactDetectSetting
            {
                art_bord = 110,
                allowed_percent_artpoints = 70,
                raw_betap_limit = 800_000,
                total_pow_border = (uint)(40 * 1e7),
                global_artwin_sec = 4,
                spect_art_by_totalp = true,
                num_wins_for_quality_avg = 125,
                hanning_win_spectrum = false,
                hamming_win_spectrum = true
            };

            var sads = new ShortArtifactDetectSetting
            {
                ampl_art_detect_win_size = 200,
                ampl_art_zerod_area = 200,
                ampl_art_extremum_border = 25
            };

            var mss = new MentalAndSpectralSetting
            {
                n_sec_for_averaging = 2,
                n_sec_for_instant_estimation = 4
            };

            _math = new EegEmotionalMath(mls, ads, sads, mss);

            // setting calibration length
            int calibrationLength = 6;
            _math.SetCallibrationLength(calibrationLength);

            // type of evaluation of instant mental levels
            bool independentMentalLevels = false;
            _math.SetMentalEstimationMode(independentMentalLevels);

            // number of windows after the artifact with the previous actual value
            int nwinsSkipAfterArtifact = 10;
            _math.SetSkipWinsAfterArtifact(nwinsSkipAfterArtifact);

            // calculation of mental levels relative to calibration values
            _math.SetZeroSpectWaves(true, 0, 1, 1, 1, 0);

            // spectrum normalization by bandwidth
            _math.SetSpectNormalizationByBandsWidth(true);

            _isCalibrated = false;
        }

        public void StartCalibration()
        {
            _math.StartCalibration();
        }

        public void Finish()
        {
            _math.StartCalibration();
        }

        public void EmotionProcessData(ISensor sensor, BrainBitSignalData[] data)
        {
            RawChannels[] bipolars = new RawChannels[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                bipolars[i].LeftBipolar = data[i].T3 - data[i].O1;
                bipolars[i].RightBipolar = data[i].T4 - data[i].O2;
            }

            _math.PushData(bipolars);
            _math.ProcessDataArr();

            ResolveArtifacted();

            if (!_isCalibrated)
            {
                ProcessCalibration();
            }
            else
            {
                ResolveSpectralData();
                ResolveRawSpectralData();
                ResolveMindData();
                PrintValues();
            }
        }

        public void ResolveArtifacted()
        {
            // sequence artifacts
            bool isArtifactedSequence = _math.IsArtifactedSequence();
            IsArtifactedSequence = isArtifactedSequence;

            // both sides artifacts
            bool isBothSideArtifacted = _math.IsBothSidesArtifacted();
            IsBothSidesArtifacted = isBothSideArtifacted;

            Console.WriteLine($"isArtifactedSequence: {isArtifactedSequence} - isBothSideArtifacted:{isBothSideArtifacted}");

        }

        public void ProcessCalibration()
        {
            _isCalibrated = _math.CalibrationFinished();
            if (!_isCalibrated)
            {
                double progress = _math.GetCallibrationPercents();
                ProgressCalibration = progress;
                Console.WriteLine($"calibration progress: {progress}");
            }
            else
            {
                Console.WriteLine("It's calibrated!");
            }
        }

        private void ResolveSpectralData()
        {
            var spectralValues = _math.ReadSpectralDataPercentsArr(); //spectral data Percents
            if (spectralValues is not null && spectralValues.Length > 0)
            {
                var spectralVal = spectralValues[^1];
                LastSpectralData = spectralVal;
            }
        }

        private void ResolveRawSpectralData()
        {
            var rawSpectralValues = _math.ReadRawSpectralVals(); //alpha and beta
            RawSpectralData = rawSpectralValues;
        }

        private void ResolveMindData()
        {
            var mentalValues = _math.ReadMentalDataArr();  //attention, relaxation
            if (mentalValues is not null && mentalValues.Length > 0)
            {
                var mentalVal = mentalValues[^1];
                LastMindData = mentalVal;
            }
        }

        private void PrintValues()
        {
            Console.WriteLine($"LastSpectralData (Percents) -- theta:{LastSpectralData.Theta} -- beta: {LastSpectralData.Beta} -- alpha: {LastSpectralData.Alpha} -- gamma: {LastSpectralData.Gamma} -- delta: {LastSpectralData.Delta}");
            Console.WriteLine($"RawSpectralData (Raw) -- alpha: {RawSpectralData.Alpha} -- beta: {RawSpectralData.Beta}");
            Console.WriteLine($"LastMindData -- RelAttention:{LastMindData.RelAttention} -- RelRelaxation: {LastMindData.RelRelaxation} -- InstAttention: {LastMindData.InstAttention} -- InstRelaxation: {LastMindData.InstRelaxation}");

            Console.WriteLine("<-----------------");
            Console.WriteLine("----------------->");

        }
    }
}
