using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;

namespace CongnitiveServerConsole
{
    class Program
    {
        private const string EXTRACT_TEXT_URL_HANDW = "https://raw.githubusercontent.com/MicrosoftDocs/azure-docs/master/articles/cognitive-services/Computer-vision/Images/readsample.jpg";
        // URL image for extracting printed text.
        private const string EXTRACT_TEXT_URL_PRINT = "https://intelligentkioskstore.blob.core.windows.net/visionapi/suggestedphotos/3.png";

        static void Main(string[] args)
        {
            var imageScan = false;
            var textScan = false;
            var textToSpeach = false;
            var speachToText = false;
            var translate = true;

            if(imageScan)
            {
                var cv = new ComputerVisionService(new ConfigurationReader());
                var ar = cv.AnalyzeImageUrl("https://moderatorsampleimages.blob.core.windows.net/samples/sample16.png").Result;
                Console.WriteLine($"Is Adult Content: {ar.Adult.IsAdultContent}");
            }

            if (textScan)
            {
                var cv = new ComputerVisionService(new ConfigurationReader());
                var results = cv.BatchReadFileUrl(EXTRACT_TEXT_URL_PRINT).Result;
                var textRecognitionLocalFileResults = results.AnalyzeResult;
                foreach (var recResult in textRecognitionLocalFileResults.ReadResults)
                {
                    foreach (Line line in recResult.Lines)
                    {
                        Console.WriteLine(line.Text);
                    }
                }
                Console.WriteLine();
            }

            if (textToSpeach)
            {
                Console.WriteLine("Type some text that you want to speak...");
                Console.Write("> ");
                var text = Console.ReadLine();
                var response = new SpeechSynthesizerService(new ConfigurationReader()).SynthesisToSpeakerAsync(text).Result;
                Console.WriteLine(response);
            }

            if(speachToText)
            {
                new SpeechSynthesizerService(new ConfigurationReader()).RecognizeSpeechAsync().Wait();
            }

            if(translate)
            {
                new SpeechSynthesizerService(new ConfigurationReader()).TranslationContinuousRecognitionAsync().Wait();
            }


        }

   
    }
}
