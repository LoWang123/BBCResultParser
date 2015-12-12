using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BBCResultParser
{
    class Program
    {
        private const String OUTPUT_PATH = "";
        private const String BBC_RESULT_PAGE_URL = "https://www12.informatik.uni-erlangen.de/edu/bbc/results.php";

        static void Main(string[] args) //args[0] := optimizerName
        {
            string htmlCode = HTMLExport.getCodeLines(BBC_RESULT_PAGE_URL);
            if (args.Length > 0)
            {
                string optimizerName = args[0];
                System.Console.WriteLine(String.Format("Starting execution for Optimizer: {0}", optimizerName));

                if (!htmlCode.Equals(String.Empty))
                {
                    System.Console.WriteLine(String.Format("Successfully recieved content from: {0}", BBC_RESULT_PAGE_URL));
                    System.Console.WriteLine(String.Format("parsing results for: {0}", optimizerName));
                    List<Result> optimizerResults = BBCParser.parse(htmlCode, optimizerName);

                    CSVWriter writer = new CSVWriter(optimizerResults, optimizerName);
                    System.Console.WriteLine(String.Format("Writing {0} results to {1}", optimizerResults.Count, String.Format("{0}{1}",OUTPUT_PATH, writer.FileName)));
                    writer.write(OUTPUT_PATH, optimizerName);
                }
                else
                {
                    System.Console.WriteLine(String.Format("Couldn't recieve content from URL: {0}", BBC_RESULT_PAGE_URL));
                }
            }
            else
            {
                System.Console.WriteLine("Please enter a optimizer name as argument for this application.");
            }
        }
    }
}
