using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using LitJson;
using System.IO;
using System.Diagnostics;

namespace ImportMusicalScripts
{
    public partial class Form1 : Form
    {
        string excel_file;
        string _path;
        public Form1()
        {
            FileInfo file = new FileInfo(Application.ExecutablePath);
            _path = file.Directory.FullName;

            excel_file = Path.Combine(_path, "Sample of Musical Scripts.xlsx");

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Excel files (*.xlsx) | *.xlsx";
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                excel_file = openFileDialog1.FileName;
                this.button2.Enabled = true;
                this.label1.Text = excel_file + " has been chosen. Please press \"Create Json\" button!!!";
                this.button3.Enabled = false;
                //this.button3.Text = "Open " + _path;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread tid1 = new Thread(new ThreadStart(getExcelFile));
            tid1.Start();
        }

        public void getExcelFile()
        {
            if (!File.Exists(excel_file))
            {
                MessageBox.Show("File " + excel_file + " doesn't exist!");
                return;
            }
            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@excel_file);
            Excel._Worksheet xlWorksheet;
            Excel.Range xlRange;
            string myResult = "";
            SongList PlayList = new SongList();
            try
            {
                this.progressBar1.Maximum = xlWorkbook.Sheets.Count + 2;
                this.SetProgressBarVisible(this.progressBar1, true);
                for (int i_sheet = 1; i_sheet <= 2; i_sheet++)
                {
                    xlWorksheet = xlWorkbook.Sheets[i_sheet];
                    xlRange = xlWorksheet.UsedRange;
                    try
                    {
                        this.SetInforStatus(this.label1, "Reading " + xlWorksheet.Name + " ...");
                        this.SetProgressBarStep(this.progressBar1);
                        int rowCount = xlRange.Rows.Count;
                        int colCount = xlRange.Columns.Count;

                        //iterate over the rows and columns and print to the console as it appears in the file
                        //excel is not zero based!!

                        Song aSong = new Song(xlWorksheet.Name);

                        for (int i = 2; i <= rowCount; i++)
                        {
                            List<int> aDuration = new List<int>();
                            for (int j = 1; j <= colCount; j++)
                            {
                                if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                                {
                                    int tmp_number = 0;
                                    Int32.TryParse(xlRange.Cells[i, j].Value2.ToString(), out tmp_number);
                                    aDuration.Add(tmp_number);
                                }
                            }
                            aSong.addDuration(aDuration);
                        }
                        this.SetInforStatus(this.label1, "Read " + xlWorksheet.Name + "!!!");

                        PlayList.addSong(aSong);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                    finally
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        //release com objects to fully kill excel process from running in the background
                        Marshal.ReleaseComObject(xlRange);
                        Marshal.ReleaseComObject(xlWorksheet);
                    }
                }

                this.SetInforStatus(this.label1, "Converting into Json object ...");
                myResult = JsonMapper.ToJson(PlayList);

                string filePath = Path.Combine(_path, "melody.json");
                this.SetInforStatus(this.label1, "Saving to " + filePath + " ...");
                File.WriteAllText(filePath, myResult);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.SetInforStatus(this.label1, "Closing " + excel_file + " ...");
                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //rule of thumb for releasing com objects:
                //  never use two dots, all COM objects must be referenced and released individually
                //  ex: [somthing].[something].[something] is bad

                //close and release
                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);

                //quit and release
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
                this.SetProgressBarStep(this.progressBar1);
                this.SetProgressBarVisible(this.progressBar1, false);
                this.SetInforStatus(this.label1, "Done!!! Database file \"melody.json\" has been saved at " + _path);
                this.Setbutton3Enabled(this.button3, true);
            }
        }

        public delegate void ProgressBarVisibleDelegate(ProgressBar pBar, bool visible);

        public void SetProgressBarVisible(ProgressBar pBar, bool visible)
        {
            if (pBar.InvokeRequired)
            {
                pBar.Invoke(new ProgressBarVisibleDelegate(SetProgressBarVisible), new object[] { pBar, visible });
            }
            else
            {
                pBar.Visible = visible;
            }
        }

        public delegate void ProgressBarStepDelegate(ProgressBar pBar);

        public void SetProgressBarStep(ProgressBar pBar)
        {
            if (pBar.InvokeRequired)
            {
                pBar.Invoke(new ProgressBarStepDelegate(SetProgressBarStep), new object[] { pBar });
            }
            else
            {
                pBar.PerformStep();
            }
        }

        public delegate void InforStatusDelegate(Label lbl, string mes);

        public void SetInforStatus(Label lbl, string mes)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke(new InforStatusDelegate(SetInforStatus), new object[] { lbl, mes });
            }
            else
            {
                lbl.Text = mes;
            }
        }

        public delegate void button3EnabledDelegate(Button btn, bool enabled);

        public void Setbutton3Enabled(Button btn, bool enabled)
        {
            if (btn.InvokeRequired)
            {
                btn.Invoke(new button3EnabledDelegate(Setbutton3Enabled), new object[] { btn, enabled });
            }
            else
            {
                btn.Enabled = enabled;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", @_path);
        }
    }
}
