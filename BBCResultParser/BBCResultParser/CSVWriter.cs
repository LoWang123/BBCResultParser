using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BBCResultParser
{
    public class CSVWriter
    {
        public String FileName { get; private set; }
        private List<Result> resultList = new List<Result>();
        public bool IsSortedByGenotypeToBeAdded { get; set; }

        public CSVWriter(List<Result> resultList, String optimizerName)
        {
            FileName = String.Format("{0}.csv", optimizerName);
            this.resultList = resultList;
            IsSortedByGenotypeToBeAdded = false;
        }

        public void write(String outputPath, String fileName)
        {
            FileName = String.Format("{0}.csv", fileName); ;
            List<String> lines = constructLines();

            if (IsSortedByGenotypeToBeAdded)
            {
                lines.Add(String.Empty);
                lines.Add(String.Empty);
                lines.Add("SORTED BY GENOTYPE");
                lines.Add(String.Empty);
                List<String> linesSorted = constructLinesSortedByGenotype();
                lines.AddRange(linesSorted);
            }

            string path = FileName;
            if(!outputPath.Equals(String.Empty))
                path = String.Format("{0}\\{1}", outputPath, FileName);
            System.IO.File.WriteAllLines(path, lines);
        }

        private List<String> constructLinesSortedByGenotype()
        {
            List<String> linesSorted = new List<string>();

            Dictionary<String, List<Result>> sortedResults = new Dictionary<string, List<Result>>();

            foreach (Result result in resultList)
            {
                if (!sortedResults.Keys.Contains(result.Genotype))
                {
                    sortedResults.Add(result.Genotype, new List<Result>());
                    sortedResults[result.Genotype].Add(result);
                }
                else
                    sortedResults[result.Genotype].Add(result);
            }

            foreach (string key in sortedResults.Keys)
            {
                foreach (Result result in sortedResults[key])
                    linesSorted.Add(constructResultline(0, result));
                linesSorted.Add(String.Empty);
                linesSorted.Add(String.Empty);
                linesSorted.Add(String.Empty);
            }

            return linesSorted;
        }

        private List<String> constructLines()
        {
            List<String> lines = new List<string>();

            lines.Add(String.Empty);
            string headline = constructHeadline();
            lines.Add(headline);

            int index = 1;
            foreach(Result result in resultList)
            {
                lines.Add(constructResultline(index, result));
                ++index;
            }
            return lines;
        }

        private String constructHeadline()
        {
            return String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                    String.Empty,
                    "Problem",
                    "Genotype(s)",
                    "ObjectiveNumber",
                    "rank",
                    "eval(avg)",
                    "eval(dev)",
                    "ind(avg)",
                    "ind(dev)");
        }

        private String constructResultline(int index, Result result)
        {
            return String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                    index,
                    result.AlgorithmusName,
                    result.Genotype,
                    result.ObjectiveNumber,
                    result.Ranking,
                    result.EvaluationsAverage,
                    result.EvaluationsDeviation,
                    result.DominanceAverage,
                    result.DominanceDeviation);
        }

    }
}
