using System;
using System.Collections.Generic;
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

            excel_file = _path;

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            excel_file = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            getExcelFile();
        }

        public void getExcelFile()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@excel_file);
            Excel._Worksheet xlWorksheet;
            Excel.Range xlRange;
            string myResult = "";
            SongList PlayList = new SongList();

            for (int i_sheet = 1; i_sheet <= xlWorkbook.Sheets.Count; i_sheet++)
            {
                xlWorksheet = xlWorkbook.Sheets[i_sheet];
                xlRange = xlWorksheet.UsedRange;

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

                PlayList.addSong(aSong);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                //rule of thumb for releasing com objects:
                //  never use two dots, all COM objects must be referenced and released individually
                //  ex: [somthing].[something].[something] is bad

                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);
            }
            myResult = JsonMapper.ToJson(PlayList);

            string filePath = Path.Combine(_path, "melody.json");

            try
            {
                File.WriteAllText(filePath, myResult);
            }
            catch(Exception ex)
            {
                MessageBox.Show(filePath + " " + ex.ToString());
            }

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
        }
    }
}
