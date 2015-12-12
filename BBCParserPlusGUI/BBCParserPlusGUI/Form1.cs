using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BBCParserPlusGUI
{
    public partial class Form1 : Form
    {
        private String currentOptimizerName = String.Empty; //necessary to prevent changing the name after parsing having an impact on writing.
        private BindingSource bindingSourceResultList = new BindingSource();


        private int lastColumnIndexClicked = -1; //memory for alternating sorting order.
        private bool isSortAscending = true; //memory for alternating sorting order.

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads content from url
        /// parses for results for the optimizer
        /// updates datagridview
        /// if no name is chosen yet: the filename for export is set to the name of the optimizer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonParse_Click(object sender, EventArgs e)
        {
            if (textBoxOptimizerName.Text.Equals(String.Empty))
                MessageBox.Show("Please enter optimizer name.");
            else if (textBoxURL.Text.Equals(String.Empty))
                MessageBox.Show("Please enter url.");
            else
            {
                String htmlContent = BBCResultParser.HTMLExport.getCodeLines(textBoxURL.Text);
                if (htmlContent.Equals(String.Empty))
                    MessageBox.Show(String.Format("Couldn't recieve content from {0}.", textBoxURL.Text));
                else
                {
                    List<BBCResultParser.Result> results = BBCResultParser.BBCParser.parse(htmlContent, textBoxOptimizerName.Text);
                    bindingSourceResultList = new BindingSource();
                    foreach (BBCResultParser.Result result in results)
                        bindingSourceResultList.Add(result);
                    dataGridView1.DataSource = bindingSourceResultList;

                    currentOptimizerName = textBoxOptimizerName.Text;

                    if (textBoxFileName.Text.Equals(String.Empty))
                        textBoxFileName.Text = textBoxOptimizerName.Text;
                }
            }
        }

        /// <summary>
        /// Writes the content of the datagridview to a csv file
        /// opens the directory to which the csv file was saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (bindingSourceResultList.Count <= 0)
                MessageBox.Show("Please parse results for a valid optimizer before exporting.");
            else
            {
                List<BBCResultParser.Result> results = new List<BBCResultParser.Result>();
                foreach (Object result in bindingSourceResultList)
                    results.Add((BBCResultParser.Result)result);

                BBCResultParser.CSVWriter writer = new BBCResultParser.CSVWriter(results, currentOptimizerName);
                writer.write(textBoxOutputPath.Text, textBoxFileName.Text);

                if (checkBoxOpenDirectory.Checked)
                {
                    if (textBoxOutputPath.Text.Equals(String.Empty))
                        System.Diagnostics.Process.Start("explorer", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
                    else
                        System.Diagnostics.Process.Start("explorer", textBoxOutputPath.Text);
                }
            }
        }

        /// <summary>
        /// opens a folder browser dialog
        /// if the result of the dialog is positive -> writes the selected path to the corresponding textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxOutputPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBoxOutputPath.Text = fbd.SelectedPath;
        }

        /// <summary>
        /// Sorts the datagridview according to the clicked header.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lastColumnIndexClicked == e.ColumnIndex) //Alternate ascending descending.
                isSortAscending = !isSortAscending;
            else
                lastColumnIndexClicked = e.ColumnIndex;
            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

            sortDataGridView(columnName, isSortAscending);
        }

        #region Bubblesort
        private void sortDataGridView(String columnName, bool isAscending)
        {
            List<BBCResultParser.Result> results = new List<BBCResultParser.Result>();
            foreach (Object result in bindingSourceResultList)
                results.Add((BBCResultParser.Result)result);


            List<Object> comperatorList = constructComperatorList(results, columnName);

            for (int j = 0; j < comperatorList.Count; ++j)
            {
                for (int i = 0; i < comperatorList.Count - 1; i++)
                {
                    bool swap = false;
                    if(comperatorList[i] is String)
                    {
                        if(!isAscending)
                        {
                            if(!isOrdered(comperatorList[i+1] as String, comperatorList[i] as String))
                            {
                                swap = true;
                            }
                        }
                        else
                        {
                            if(!isOrdered(comperatorList[i] as String, comperatorList[i+1] as String))
                            {
                                swap = true;
                            }
                        }
                    }
                    else if (comperatorList[i] is double)
                    {
                        if(!isAscending)
                        {
                            if(!isOrdered((double)comperatorList[i+1], (double)comperatorList[i]))
                            {
                                swap = true;
                            }
                        }
                        else
                        {
                            if(!isOrdered((double)comperatorList[i], (double)comperatorList[i+1]))
                            {
                                swap = true;
                            }
                        }
                    }
                    else if (comperatorList[i] is int)
                    {
                        if(!isAscending)
                        {
                            if(!isOrdered((int)comperatorList[i+1], (int)comperatorList[i]))
                            {
                                swap = true;
                            }
                        }
                        else
                        {
                            if (!isOrdered((int)comperatorList[i], (int)comperatorList[i + 1]))
                            {
                                swap = true;
                            }
                        }
                    }
                    
                    if(swap)
                    {
                        Swap(results, i, i + 1);
                        Swap(comperatorList, i, i + 1);
                    }
                }
            }

            bindingSourceResultList = new BindingSource();
            foreach (BBCResultParser.Result result in results)
                bindingSourceResultList.Add(result);
            dataGridView1.DataSource = bindingSourceResultList;
        }

        private static void Swap(List<BBCResultParser.Result> list, int indexA, int indexB)
        {
            BBCResultParser.Result tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private static void Swap(List<Object> list, int indexA, int indexB)
        {
            Object tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private List<Object> constructComperatorList(List<BBCResultParser.Result> results, String columnName)
        {
            List<Object> comperatorList = new List<object>();
            foreach (BBCResultParser.Result result in results)
            {
                if (columnName.Equals("AlgorithmusName"))
                {
                   comperatorList.Add((Object)result.AlgorithmusName);
                }
                else if (columnName.Equals("Genotype"))
                {
                    comperatorList.Add((Object)result.Genotype);
                }
                else if (columnName.Equals("ObjectiveNumber"))
                {
                    comperatorList.Add((Object)result.ObjectiveNumber);
                }
                else if (columnName.Equals("Ranking"))
                {
                    comperatorList.Add((Object)result.Ranking);
                }
                else if (columnName.Equals("EvaluationsAverage"))
                {
                    comperatorList.Add((Object)result.EvaluationsAverage);
                }
                else if (columnName.Equals("DominanceAverage"))
                {
                    comperatorList.Add((Object)result.DominanceAverage);
                }
                else if (columnName.Equals("EvaluationsDeviation"))
                {
                    comperatorList.Add((Object)result.EvaluationsDeviation);
                }
                else if (columnName.Equals("DominanceDeviation"))
                {
                    comperatorList.Add((Object)result.DominanceDeviation);
                }
            }


            return comperatorList;
        }


        private bool isOrdered(double smaller, double bigger)
        {
            return smaller <= bigger;
        }

        private bool isOrdered(int smaller, int bigger)
        {
            return smaller <= bigger;
        }

        private bool isOrdered(String smaller, String bigger)
        {
            char[] smallerArray = smaller.ToLower().ToCharArray();
            char[] biggerArray = bigger.ToLower().ToCharArray();

            int minSize = Math.Min(smallerArray.Length, biggerArray.Length);
            for (int i = 0; i < minSize; ++i)
            {
                if (smallerArray[i] < biggerArray[i])
                    return true;
                if (smallerArray[i] > biggerArray[i])
                    return false;
            }

            if (smallerArray.Length > biggerArray.Length)
                return false;
            return true;
        }

        #endregion
    }
}
