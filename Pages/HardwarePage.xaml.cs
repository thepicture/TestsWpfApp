using LibreHardwareMonitor.Hardware;
using System;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace TestsWpfApp
{
    /// <summary>
    /// Interaction logic for HardwarePage.xaml
    /// </summary>
    public partial class HardwarePage : Page
    {
        readonly Computer _computer = new Computer();
        public HardwarePage()
        {
            InitializeComponent();
            UpdateHardwareGrid();
            Random random = new Random();
            for (int i = 0; i < 128; i++)
            {
                HardwaresChart.Series[0].Points.AddXY(Guid.NewGuid().ToString(), random.Next(0, 32));
                HardwaresChart.Series[1].Points.AddXY(Guid.NewGuid().ToString(), random.Next(0, 64));
            }
        }

        private void UpdateHardwareGrid()
        {
            _computer.Open();

            var items = _computer.SMBios.Processor.GetType().GetProperties().Select(p =>
            {
                var pair = new
                {
                    Property = p.Name,
                    Value = p.GetValue(_computer.SMBios.Processor)
                };
                return pair;
            });
            HardwareGrid.ItemsSource = items;
        }

        /// <summary>
        /// Prints the hardware grid.
        /// </summary>
        private void OnPrint(object sender, RoutedEventArgs e)
        {
            using (PrintServer server = new PrintServer())
            {
                using (PrintQueue queue = new PrintQueue(server, "Microsoft Print to PDF"))
                {
                    PrintDialog dialog = new PrintDialog();
                    if (dialog.ShowDialog().GetValueOrDefault())
                    {
                        dialog.PrintVisual(HardwareGrid, "Print hardware info");
                    }
                }
            }
        }

        /// <summary>
        /// Updates list.
        /// </summary>
        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                UpdateHardwareGrid();
        }


        /// <summary>
        /// Exports hardware info to excel.
        /// </summary>
        private void OnExportToExcel(object sender, RoutedEventArgs e)
        {
            ExportExcel(_computer.SMBios.Processor, "Processor");
            ExportExcel(_computer.SMBios.Bios, "Bios");
            ExportExcel(_computer.SMBios.Board, "Board");
            ExportExcel(_computer.SMBios.Chassis, "Chassis");
            ExportExcel(_computer.SMBios.System, "System");

        }

        private void ExportExcel(object obj, string header)
        {
            Excel.Application app = new Excel.Application();
            Excel.Workbook workbook = app.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Sheets.Add();
            try
            {
                var items = obj.GetType().GetProperties().Select(p =>
                {
                    var pair = new
                    {
                        Property = p.Name,
                        Value = p.GetValue(obj)
                    };
                    return pair;
                });
                int startRowIndex = 1;
                Excel.Range columnRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[2][startRowIndex]];
                columnRange.Merge();
                columnRange.Value = header;
                columnRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                ++startRowIndex;
                worksheet.Cells[1][startRowIndex].Value = "Name";
                worksheet.Cells[2][startRowIndex].Value = "Value";
                foreach (var item in items)
                {
                    ++startRowIndex;
                    worksheet.Cells[1][startRowIndex].Value = item.Property;
                    worksheet.Cells[2][startRowIndex].Value = item.Value;
                }
                Excel.Range range = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[2][startRowIndex]];
                range.Columns.AutoFit();
                range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                workbook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, header + ".pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                workbook?.Close(false);
                app?.Quit();
            }
        }

        private void OnExportToWord(object sender, RoutedEventArgs e)
        {
            Word.Application app = null;
            Word.Document document = null;
            try
            {
                app = new Word.Application();
                document = app.Documents.Add();
                Word.Paragraph paragraph = document.Paragraphs.Add();
                Word.Table table = paragraph.Range.Tables.Add(paragraph.Range, 2, 2);
                table.Cell(1, 1).Range.Text = "Name";
                table.Cell(1, 2).Range.Text = "Value";
                table.Cell(2, 1).Range.Text = "ABC";
                table.Cell(2, 2).Range.Text = "123";
                table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                document.ExportAsFixedFormat("test.pdf", Word.WdExportFormat.wdExportFormatPDF);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                document?.Close(false);
                app?.Quit(false);
            }

        }
    }
}
