using Microsoft.ML;
using NewsAggregationPlatform.Models;
using static Microsoft.ML.DataOperationsCatalog;

namespace NewsAggregationPlatform.Services.Abstraction
{
    public interface IPositivityAnalysisService
    {
        SentimentPrediction AnalyzePositivity(string content);
        TrainTestData LoadData(MLContext mlContext);
        ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet);
    }
}
