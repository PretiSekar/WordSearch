/*
 * Program By : Preethi Sekar
 * Net ID :pxs163930
 * Submitted On: 03/22/2018
 * Assignment: 
 * Text Search
 * Purpose: 
 * Multi threaded Programming
 * BackgroundWorker
 * UI Design
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pxs163930Asg4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // Browse button for searching text files
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select a text file";

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }
        //Search button to search the words
        private void button2_Click(object sender, EventArgs e)
        {
            //Allow only valid text file
            if (textBox1.Text.Contains(".txt"))
            {
                //Search or Cancel operation
                if (button2.Text.Equals("Search"))
                {
                    //Check only if both fields are full
                    if (textBox1.Text != null && textBox2.Text != null)
                    {
                        toolStripStatusLabel1.Text = "";
                        listView1.Items.Clear();
                        button2.Text = "Cancel";
                        this.label4.Text = "0";
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
                else
                {
                    this.backgroundWorker1.CancelAsync();
                    textBox2.Text = "";
                    button2.Text = "Search";
                }
            }
            else
            {
                toolStripStatusLabel1.Text = "Enter a valid file";
            }
            
            
        }
        //Progress Bar
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }
        //Display message to the StatusStrip
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                toolStripStatusLabel1.Text = "Error" + e.Error.Message;
                button2.Text = "Search";

            } 
            else if (e.Cancelled)
            {
                toolStripStatusLabel1.Text = "Word Counting Canceled";
                button2.Text = "Search";
            }  
            else
            {
                toolStripStatusLabel1.Text = "Finished Counting Words";
                button2.Text = "Search";
            }
                

        }
        //the main background worker to search the words
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            findMatchingLines(worker,e);

        }
        //function that searches the words in the file
        private void findMatchingLines(BackgroundWorker worker, DoWorkEventArgs e)
        {
            String filepath = textBox1.Text;
            String searchword = textBox2.Text;
            int lineCount = File.ReadLines(filepath).Count();
            String line;
            int count = 0;
            int matchlcount = 0;
            if (File.Exists(filepath))
            {
                using (StreamReader sw = new StreamReader(filepath))
                {

                    while ((line = sw.ReadLine()) != null)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                        else
                        {
                            count++;
                            if (line.ToUpper().Contains(searchword.ToUpper()))
                            {
                                matchlcount++;
                                addToList(count, line, matchlcount);
                            }

                            System.Threading.Thread.Sleep(1);

                            int percentComplete =(int)((float)count / (float)lineCount * 100);
                            int highestPercentageReached = 0;
                            if (percentComplete > highestPercentageReached)
                            {
                                highestPercentageReached = percentComplete;
                                worker.ReportProgress(percentComplete);
                            }

                        }
                    }
                }
            }
        }
        //sub function to add the items to the list
        private void addToList(int count, string line, int matchlcount)
        {
            
            ListViewItem row = new ListViewItem(count.ToString());
            row.SubItems.Add(new ListViewItem.ListViewSubItem(row, line));
            
            if (listView1.InvokeRequired)
            {
                listView1.Invoke(new MethodInvoker(delegate
                {
                    listView1.Items.Add(row);

                }));
            }
            else
            {
                listView1.Items.Add(row);
            }
            if (label4.InvokeRequired)
            {
                label4.Invoke(new MethodInvoker(delegate
                {
                    label4.Text = matchlcount.ToString();
                }));
            }
            else
            {
                label4.Text = matchlcount.ToString();
            }            
        }
    }
}
