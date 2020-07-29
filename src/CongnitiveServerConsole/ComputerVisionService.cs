

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CongnitiveServerConsole
{
    public class ComputerVisionService
    {
        private ConfigurationReader configurationReader;

        public ComputerVisionService(ConfigurationReader configurationReader)
        {
            this.configurationReader = configurationReader;
        }

        private ComputerVisionClient GetClient()
        {
            var subscriptionKey = configurationReader.GetKey();
            var endpoint = configurationReader.GetEndpoint();

            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
            {
                Endpoint = endpoint
            };
            return client;
        }

        public async Task<ImageAnalysis> AnalyzeImageUrl(string imageUrl)
        {
            var client = this.GetClient();
            // Creating a list that defines the features to be extracted from the image. 
            var features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces,
                VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color,
                VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };
            var results = await client.AnalyzeImageAsync(imageUrl, features);

            return results;
        }

        public async Task<ReadOperationResult> BatchReadFileUrl(string urlImage, string language = "en")
        {
            var client = GetClient();
            var textHeaders = await client.ReadAsync(urlImage, language: language);
            var operationLocation = textHeaders.OperationLocation;
            const int numberOfCharsInOperationId = 36;
            var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            int i = 0;
            int maxRetries = 10;
            ReadOperationResult results;
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
                await Task.Delay(1000);
                if (i == 9)
                {
                    Console.WriteLine("Server timed out.");
                }
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted) && i++ < maxRetries);
            return results;
        }
    }
}
