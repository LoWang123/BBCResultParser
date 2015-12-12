using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;


namespace BBCResultParser
{
    public class Result
    {
        public String AlgorithmusName { get; private set; } // name="dtlz3"
        public String Genotype { get; private set; } //&lt;Double String&gt;
        public int ObjectiveNumber { get; private set; } //Objectives: 3</p>
        public int Ranking { get; private set; }//count <a href="optimizer.php#NOTOPTIMIZERNAME"> before <a href="optimizer.php#optimizerName">
        public double EvaluationsAverage { get; private set; } //</td><td>209.8</td><td>
        public double DominanceAverage { get; private set; } //
        public double EvaluationsDeviation { get; private set; } //
        public double DominanceDeviation { get; private set; } //
        

        public void setResultFromTableContent(String tableContent, String optimizerName)
        {
            AlgorithmusName = extractAlgorithmusName(tableContent);
            Genotype = extractGenotype(tableContent);
            ObjectiveNumber = extractObjectiveNumber(tableContent);
            
            tableContent = dropEverthingBeforeTable(tableContent);

            int nextOptimizerIndex = tableContent.IndexOf("<a href=\"optimizer.php#") + 23;
            bool stop = false;
            string test = String.Empty;
            int rank = 1;
            //REST INLINE
            while (nextOptimizerIndex > 22 && !stop) // -1 + 23
            {
                tableContent = tableContent.Substring(nextOptimizerIndex);
                int startIndexOptimizerName = 0;
                int endIndexOptimizerName = tableContent.Substring(startIndexOptimizerName).IndexOf("\">") + startIndexOptimizerName;
                string currentOptimizer = tableContent.Substring(startIndexOptimizerName, endIndexOptimizerName - startIndexOptimizerName);


                    if (optimizerName.Equals(currentOptimizer))
                    {
                        Ranking = rank;
                        int endIndexEntry = tableContent.IndexOf("<tr>");
                        string entry = tableContent.Substring(0, endIndexEntry);


                        int evalAvgStartIndex = entry.IndexOf("</td><td>") + 9;
                        int evalAvgEndIndex = entry.IndexOf("</td><td>", evalAvgStartIndex);
                        string evalAvg = entry.Substring(evalAvgStartIndex, evalAvgEndIndex - evalAvgStartIndex);

                        double tmp = 0;
                        Double.TryParse(evalAvg, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out tmp);
                        if (evalAvg.Contains('e') && tmp == 0)
                            tmp = Double.Parse(evalAvg, CultureInfo.InvariantCulture);
                        EvaluationsAverage = tmp;

                        int evalDevStartIndex = entry.IndexOf("</td><td>", evalAvgEndIndex) + 9;
                        int evalDevEndIndex = entry.IndexOf("</td><td>", evalDevStartIndex);
                        string evalDev = entry.Substring(evalDevStartIndex, evalDevEndIndex - evalDevStartIndex);

                        Double.TryParse(evalDev, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out tmp);
                        if (evalDev.Contains('e') && tmp == 0)
                            tmp = Double.Parse(evalDev, CultureInfo.InvariantCulture);
                        EvaluationsDeviation = tmp;

                        int domAvgStartIndex = entry.IndexOf("</td><td>", evalDevEndIndex) + 9;
                        int domAvgEndIndex = entry.IndexOf("</td><td>", domAvgStartIndex);
                        string domAvg = entry.Substring(domAvgStartIndex, domAvgEndIndex - domAvgStartIndex);

                        Double.TryParse(domAvg, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out tmp);
                        if (domAvg.Contains('e') && tmp == 0)
                            tmp = Double.Parse(domAvg, CultureInfo.InvariantCulture);
                        DominanceAverage = tmp;

                        int domDevStartIndex = entry.IndexOf("</td><td>", domAvgEndIndex) + 9;
                        int domDevEndIndex = entry.IndexOf("</td><td>", domDevStartIndex);
                        string domDev = entry.Substring(domDevStartIndex, domDevEndIndex - domDevStartIndex);

                        Double.TryParse(domDev, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out tmp);
                        if (domDev.Contains('e') && tmp == 0)
                            tmp = Double.Parse(domDev, CultureInfo.InvariantCulture);
                        DominanceDeviation = tmp;

                        stop = true;
                    }
                    else
                    {
                        ++rank;
                    }

                    nextOptimizerIndex = tableContent.IndexOf("<a href=\"optimizer.php#") + 23;
                
            }
            
        }

        private String extractAlgorithmusName(String tableContent)
        {
            try
            {
                string substring = tableContent.Substring(tableContent.IndexOf("name=\"") + 6);
                substring = substring.Substring(0, substring.IndexOf("\""));
                return substring;
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }

        private String extractGenotype(String tableContent)
        {
            int startIndex = tableContent.IndexOf("&lt;") + 4;
            int endIndex = tableContent.IndexOf("&gt;");
            if (startIndex > 3 && endIndex > -1)
            {
                string substring = tableContent.Substring(startIndex, endIndex - startIndex);
                return substring;
            }
            return String.Empty;
        }
        private int extractObjectiveNumber(String tableContent)
        {
            int startIndex = tableContent.IndexOf("Objectives: ") + 12;
            int endIndex = tableContent.IndexOf("</p>");
            if (startIndex > 11 && endIndex > -1)
            {
                string substring = tableContent.Substring(startIndex, endIndex - startIndex - 1); // - 1 because we need  to get rid of the \n

                int objNumber = -1;
                Int32.TryParse(substring, out objNumber);

                return objNumber;
            }
            return -1;
        }
        private String dropEverthingBeforeTable(String tableContent)
        {
            try
            {
                return tableContent.Substring(tableContent.IndexOf("<table>"));
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
