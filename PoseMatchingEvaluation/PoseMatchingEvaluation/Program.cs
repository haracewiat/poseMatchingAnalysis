using Microsoft.Data.Analysis;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace PoseMatchingEvaluation
{
    class Program
    {
        private const int BlazePoseInfoColumnIndex = 6;
        private static List<PoseMatchEstimator> poseMatchEstimators = new List<PoseMatchEstimator>()
        {
            //new RulesPoseMatchEstimator(),
            //new RelativeMetricsPoseMatchEstimator(20, 0.4f, true),
            new RelativeMetricsPoseMatchEstimator(15, 0.4f, true),
            //new RelativeMetricsPoseMatchEstimator(20, 0.4f, false),
        };

        private static Stopwatch watch = new Stopwatch();

        private static string KEY = "new";

        private static string PATH = $"C:\\Users\\beata.haracewiat\\Desktop\\Thesis\\Results\\{KEY}\\";
        private static string OUTPUT_PATH = $"C:\\Users\\beata.haracewiat\\Desktop\\Thesis\\Results\\{KEY}\\results\\";

        private static string[] labels = new string[] { "blue", "green", "pink", "yellow", "purple", "red" };


        static void Main(string[] args)
        {
            foreach (PoseMatchEstimator poseMatchEstimator in poseMatchEstimators)
            {
                foreach (string label in labels)
                {
                    Console.WriteLine($"Processing label {label} -----------------------------------------");

                    ProcessDataFrame($"{PATH}{label}.csv", label, poseMatchEstimator);
                }
            }     
        }

        private static void ProcessDataFrame(string path, string label, PoseMatchEstimator poseMatchEstimator)
        {
            // Load pose definitions
            List<PoseInfo> savedPoseInfo = LoadPoses();

            // Load dataframe
            DataFrame df = DataFrame.LoadCsv(path);

            // Prepare results dataframe
            StringDataFrameColumn file_name_column = new StringDataFrameColumn("File");

            PrimitiveDataFrameColumn<float> blue_score = new PrimitiveDataFrameColumn<float>("Blue Score");
            PrimitiveDataFrameColumn<float> green_score = new PrimitiveDataFrameColumn<float>("Green Score");
            PrimitiveDataFrameColumn<float> pink_score = new PrimitiveDataFrameColumn<float>("Pink Score");
            PrimitiveDataFrameColumn<float> yellow_score = new PrimitiveDataFrameColumn<float>("Yellow Score");
            PrimitiveDataFrameColumn<float> purple_score = new PrimitiveDataFrameColumn<float>("Purple Score");
            PrimitiveDataFrameColumn<float> red_score = new PrimitiveDataFrameColumn<float>("Red Score");

            PrimitiveDataFrameColumn<double> blue_time = new PrimitiveDataFrameColumn<double>("Blue Time");
            PrimitiveDataFrameColumn<double> green_time = new PrimitiveDataFrameColumn<double>("Green Time");
            PrimitiveDataFrameColumn<double> pink_time = new PrimitiveDataFrameColumn<double>("Pink Time");
            PrimitiveDataFrameColumn<double> yellow_time = new PrimitiveDataFrameColumn<double>("Yellow Time");
            PrimitiveDataFrameColumn<double> purple_time = new PrimitiveDataFrameColumn<double>("Purple Time");
            PrimitiveDataFrameColumn<double> red_time = new PrimitiveDataFrameColumn<double>("Red Time");


            // Interate through each row
            for (long i = 0; i < df.Rows.Count; i++)
            {
                // Read row as raw camera data
                RawCameraData rawCameraData = ReadCsvData(df.Rows[i]);

                // Store the file path
                string filePath = df.Rows[i][0].ToString();
                file_name_column.Append(filePath);

                // Retrieve the label
                Dictionary<PoseInfo, PredictionInformation> predictions = GetPredictionForEachPose(poseMatchEstimator, savedPoseInfo, rawCameraData);

                blue_score.Append(predictions.First(score => score.Key.Name.Equals("blue")).Value.Score);
                green_score.Append(predictions.First(score => score.Key.Name.Equals("green")).Value.Score);
                pink_score.Append(predictions.First(score => score.Key.Name.Equals("pink")).Value.Score);
                yellow_score.Append(predictions.First(score => score.Key.Name.Equals("yellow")).Value.Score);
                purple_score.Append(predictions.First(score => score.Key.Name.Equals("purple")).Value.Score);
                red_score.Append(predictions.First(score => score.Key.Name.Equals("red")).Value.Score);

                blue_time.Append(predictions.First(time => time.Key.Name.Equals("blue")).Value.PredictionTime.TotalMilliseconds);
                green_time.Append(predictions.First(time => time.Key.Name.Equals("green")).Value.PredictionTime.TotalMilliseconds);
                pink_time.Append(predictions.First(time => time.Key.Name.Equals("pink")).Value.PredictionTime.TotalMilliseconds);
                yellow_time.Append(predictions.First(time => time.Key.Name.Equals("yellow")).Value.PredictionTime.TotalMilliseconds);
                purple_time.Append(predictions.First(time => time.Key.Name.Equals("purple")).Value.PredictionTime.TotalMilliseconds);
                red_time.Append(predictions.First(time => time.Key.Name.Equals("red")).Value.PredictionTime.TotalMilliseconds);

                // Print stats (assigned label)
                PoseInfo predictedLabel = predictions.Aggregate((x, y) => x.Value.Score > y.Value.Score ? x : y).Key;

                if (predictions[predictedLabel].Score >= poseMatchEstimator.MinimumScoreThreshold)
                {
                    Console.WriteLine($"[{poseMatchEstimator.Name}, {label}] {predictedLabel.Name} ({(predictions[predictedLabel].Score * 100).ToString("0.00")}%)");

                }
                else
                {
                    Console.WriteLine($"[{poseMatchEstimator.Name}, {label}] -");
                }
            }

            DataFrame results_df = new DataFrame(file_name_column, blue_score, green_score, pink_score, yellow_score, purple_score, red_score, blue_time, green_time, pink_time, yellow_time, purple_time, red_time);

            // Save the file
            Directory.CreateDirectory($"{OUTPUT_PATH}{poseMatchEstimator.Name}");
            DataFrame.WriteCsv(results_df, $"{OUTPUT_PATH}{poseMatchEstimator.Name}\\{label}.csv");
            
        }

        private static RawCameraData ReadCsvData(DataFrameRow row)
        {
            string data = row[BlazePoseInfoColumnIndex].ToString();

            // Apply formatting to allow for json deserialization
            data = data.Replace("array(", "");
            data = data.Replace(")", "");

            Dictionary<string, int[]> landmarks = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(data);
            return new RawCameraData(landmarks);
        }

        private static List<PoseInfo> LoadPoses()
        {
            using StreamReader jsonReader = new StreamReader("C:\\Users\\beata.haracewiat\\Desktop\\Thesis\\PoseMatchingEvaluation\\PoseMatchingEvaluation\\poses.pd");

            return JsonConvert.DeserializeObject<List<PoseInfo>>(jsonReader.ReadToEnd());          
        }

        public static Dictionary<PoseInfo, PredictionInformation> GetPredictionForEachPose(PoseMatchEstimator poseMatchEstimator, List<PoseInfo> desiredPoses, RawCameraData currentPose)
        {
            Dictionary<PoseInfo, PredictionInformation> poseScores = new Dictionary<PoseInfo, PredictionInformation>();

            // Get score for each pose
            foreach (PoseInfo desiredPose in desiredPoses)
            {
                watch.Reset();

                watch.Start();

                float score = poseMatchEstimator.GetPoseMatchAccuracy(desiredPose, currentPose);
                
                watch.Stop();

                poseScores[desiredPose] = new PredictionInformation(score, watch.Elapsed);
            }

            return poseScores;
        }

        public struct PredictionInformation
        {
            public PredictionInformation(float score, TimeSpan predictionTime)
            {
                Score = score;
                PredictionTime = predictionTime;
            }

            public float Score { get; set; }
            public TimeSpan PredictionTime { get; set; }
        }

    }
}
