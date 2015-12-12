using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BBCResultParser
{
    public class BBCParser
    {
        public static List<Result> parse(String htmlCode, String optimizerName)
        {
            List<Result> optimizerResults = new List<Result>();

            if (htmlCode != null)
            {
                int startIndex = htmlCode.IndexOf("<!-- Wirklicher Inhalt -->");
                int endIndex = htmlCode.IndexOf("<!-- Ende Inhalt -->");

                string content = htmlCode.Substring(startIndex, endIndex - startIndex);

                List<String> tableContentList = new List<string>(); //starts with <h4> ends with </table> iteratively
                bool stop = false;
                do
                {
                    int startIndexTable = content.IndexOf("<h4>");
                    int endIndexTable = content.IndexOf("</table>");

                    if (startIndexTable != -1 && endIndexTable != -1)
                    {
                        string tableString = content.Substring(startIndexTable, endIndexTable - startIndexTable);
                        content = content.Substring(endIndexTable + 3); //so that </table> isn't fully included anymore. Got to hack this quick xD
                        tableContentList.Add(tableString);
                    }
                    else
                    {
                        stop = true;
                    }

                } while (!stop);

                foreach (String tableContent in tableContentList)
                {
                    Result result = new Result();
                    result.setResultFromTableContent(tableContent, optimizerName);
                    optimizerResults.Add(result);
                }

            }

            return optimizerResults;
        }

    }
}
