using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Threading.Tasks;

namespace CongnitiveServerConsole
{
    class Program
    {
        private const string EXTRACT_TEXT_URL_HANDW = "https://raw.githubusercontent.com/MicrosoftDocs/azure-docs/master/articles/cognitive-services/Computer-vision/Images/readsample.jpg";
        // URL image for extracting printed text.
        private const string EXTRACT_TEXT_URL_PRINT = "https://intelligentkioskstore.blob.core.windows.net/visionapi/suggestedphotos/3.png";

        static async Task RunImageScanAsync()
        {
            var contninue = false;

            do
            {
                Console.Clear();
                Console.WriteLine("Enter an Image URL (e.g. https://moderatorsampleimages.blob.core.windows.net/samples/sample16.png):");
                var url = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(url))
                {
                    Console.WriteLine("URL is empty");
                }
                else
                {
                    Console.WriteLine("Analyzing the image with Azure Cognitive Service...");
                    var cv = new ComputerVisionService(new ConfigurationReader());
                    var ar = await cv.AnalyzeImageUrl(url);

                    var serializer = new YamlDotNet.Serialization.Serializer();
                    var yaml = serializer.Serialize(ar);
                    Console.WriteLine("Analysis report:");
                    Console.WriteLine(yaml);

                    Console.WriteLine("\n\nPress 'C' to continue...");
                    contninue = Char.ToUpperInvariant(Console.ReadKey().KeyChar) == 'C';
                }
            } while (contninue);
        }

        static async Task RunTextScanAsync()
        {
            var contninue = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Enter an Image (with text) URL (e.g. https://intelligentkioskstore.blob.core.windows.net/visionapi/suggestedphotos/3.png \nor https://raw.githubusercontent.com/MicrosoftDocs/azure-docs/master/articles/cognitive-services/Computer-vision/Images/readsample.jpg):");
                var url = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(url))
                {
                    Console.WriteLine("URL is empty");
                }
                else
                {
                    Console.WriteLine("Analyzing the image to extract texts with Azure Cognitive Service...");
                    var cv = new ComputerVisionService(new ConfigurationReader());
                    var results = await cv.BatchReadFileUrl(url);
                    var textRecognitionLocalFileResults = results.AnalyzeResult;
                    foreach (var recResult in textRecognitionLocalFileResults.ReadResults)
                    {
                        foreach (Line line in recResult.Lines)
                        {
                            Console.WriteLine(line.Text);
                        }
                    }
                    Console.WriteLine();

                    Console.WriteLine("\n\nPress 'C' to continue...");
                    contninue = Char.ToUpperInvariant(Console.ReadKey().KeyChar) == 'C';
                }
            } while (contninue);
        }

        static async Task RunTextToSpeechAsync()
        {
            var contninue = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Type some text that you want to speak...");
                Console.Write("> ");
                var text = Console.ReadLine();
                var response = await (new SpeechSynthesizerService(new ConfigurationReader()))
                    .SynthesisToSpeakerAsync(text);
        
                Console.WriteLine("\n\nPress 'C' to continue...");
                contninue = Char.ToUpperInvariant(Console.ReadKey().KeyChar) == 'C';

            } while (contninue);
        }

        static async Task RunSpeechToTextAsync()
        {
            var contninue = false;
            do
            {
                Console.Clear();
                await new SpeechSynthesizerService(new ConfigurationReader()).RecognizeSpeechAsync();

                Console.WriteLine("\n\nPress 'C' to continue...");
                contninue = Char.ToUpperInvariant(Console.ReadKey().KeyChar) == 'C';

            } while (contninue);
        }

        static async Task RunContinuousTranslationAsync()
        {
            bool contninue;
            do
            {
                Console.Clear();
                await new SpeechSynthesizerService(new ConfigurationReader()).TranslationContinuousRecognitionAsync();

                Console.WriteLine("\n\nPress 'C' to continue...");
                contninue = Char.ToUpperInvariant(Console.ReadKey().KeyChar) == 'C';

            } while (contninue);
        }

        static void Main(string[] args)
        {
            ConsoleKeyInfo keyCode;
            do
            {
                Console.Clear();
                Console.WriteLine("Make a choice:\n(I)mage Scan\n(T)ext Scan\nT(e)xt to Speech\n(S)peech to text\nT(r)anslate or ESC to Quit.");
                keyCode = Console.ReadKey();
                Console.Clear();

                switch (Char.ToUpperInvariant(keyCode.KeyChar))
                {
                    case 'I': RunImageScanAsync().Wait(); break;
                    case 'T': RunTextScanAsync().Wait(); break;
                    case 'E': RunTextToSpeechAsync().Wait(); break;
                    case 'S': RunSpeechToTextAsync().Wait(); break;
                    case 'R': RunContinuousTranslationAsync().Wait(); break;
                }
            } while (keyCode.Key != ConsoleKey.Escape);
        }
   
    }
}
