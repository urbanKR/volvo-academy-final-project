using Microsoft.ML;
using NewsAggregationPlatform.Models;
using NewsAggregationPlatform.Services.Abstraction;
using static Microsoft.ML.DataOperationsCatalog;

namespace NewsAggregationPlatform.Services.Implementation
{
    public class PositivityAnalysisService : IPositivityAnalysisService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly string _yelpDataPath;

        public PositivityAnalysisService(IWebHostEnvironment env)
        {
            _mlContext = new MLContext();
            _yelpDataPath = Path.Combine(env.ContentRootPath, "Data", "yelp_labelled.txt");
            var trainTestData = LoadData(_mlContext);
            _model = BuildAndTrainModel(_mlContext, trainTestData.TrainSet);
        }

        public SentimentPrediction AnalyzePositivity(string content)
        {
            var predictionFunction = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
            var sentiment = new SentimentData { SentimentText = content };
            var predictionResult = predictionFunction.Predict(sentiment);
            predictionResult.Score = predictionResult.Probability * 100;

            return predictionResult;
        }

        public TrainTestData LoadData(MLContext mlContext)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_yelpDataPath);
            TrainTestData trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            return trainTestData;
        }

        public ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var predictor = mlContext.Transforms.Text
                .FeaturizeText("Features", nameof(SentimentData.SentimentText))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features"));

            var model = predictor.Fit(splitTrainSet);

            return model;
        }
    }
}
