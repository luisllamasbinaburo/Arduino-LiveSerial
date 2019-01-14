using Arduino_LiveSerial.View.Dialogs.Predefined;
using Newtonsoft.Json;
using OfficeOpenXml;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_LiveSerial.ViewModels
{
    public class SerieVisualizerViewModel
    {
        #region Constructor
        public SerieVisualizerViewModel()
        {
            ExportToCsvCommand = ReactiveCommand.Create(ExportToCsv);
            ExportToExcelCommand = ReactiveCommand.Create(ExportToExcel);
            ExportToJsonCommand = ReactiveCommand.Create(ExportToJson);
        }
        #endregion


        #region Commands
        public ReactiveCommand<Unit, Unit> ExportToCsvCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ExportToExcelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ExportToJsonCommand { get; set; }
        #endregion


        #region Properties
        public Domain.SerialData[] Data { get; set; }

        public string SerieName { get; set; }
        #endregion


        #region Methods
        private void ExportToExcel()
        {
            try
            {
                var path = getPath("Excel files (*.xlsx)|*.xlsx", ".xlsx");
                if (path != null)
                {
                    using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(path)))
                    {
                        var sheet = xlPackage.Workbook.Worksheets.Add("Data");
                        var range = sheet.Cells[1, 1].LoadFromCollection(Data, true, OfficeOpenXml.Table.TableStyles.Light9);

                        // formating
                        var tbl = sheet.Tables[0];
                        var dateStyle = xlPackage.Workbook.Styles.CreateNamedStyle("TableDate");
                        dateStyle.Style.Numberformat.Format = "hh:mm:ss.000";
                        tbl.Columns[3].DataCellStyleName = "TableDate";
                        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                        var chart = sheet.Drawings.AddChart("Chart", OfficeOpenXml.Drawing.Chart.eChartType.XYScatterLinesNoMarkers);
                        chart.SetPosition(20, 300);
                        chart.SetSize(500, 400);

                        var ser = chart.Series.Add(range.Offset(1, 1, range.End.Row - 1, 1), range.Offset(1, 3, range.End.Row - 1, 1));
                        ser.Header = SerieName;
                        chart.Style = OfficeOpenXml.Drawing.Chart.eChartStyle.Style7;

                        xlPackage.Save();

                        PredefinedDialogs.MessageDialog(this, "Done!", (s, e) => { }, (s, e) => { }, "ViewSerieDialog");
                    }
                }
            }
            catch (Exception)
            {
                PredefinedDialogs.MessageDialog(this, "Error", (s, e) => { }, (s, e) => { }, "ViewSerieDialog");
            }
        }

        private void ExportToCsv()
        {
            try
            {
                var path = getPath("csv files (*.csv)|*.csv", ".csv");
                if (path != null)
                {
                    using (StreamWriter outputFile = new StreamWriter(path))
                    {

                        var writer = new StringWriter();
                        var textData = Data.Select(x => new[] { x.Key.ToString(),
                                                    x.Value.ToString(),
                                                    x.Millis.ToString(),
                                                    x.Time.ToString(),});

                        Csv.CsvWriter.Write(writer, new string[] { "Name", "Value", "Millis", "Time" }, textData, ',');
                        outputFile.WriteLine(writer);

                        PredefinedDialogs.MessageDialog(this, "Done!", (s, e) => { }, (s, e) => { }, "ViewSerieDialog");
                    }
                }
            }
            catch (Exception)
            {
                PredefinedDialogs.MessageDialog(this, "Error", (s, e) => { }, (s, e) => { }, "ViewSerieDialog");
            }

        }

        private void ExportToJson()
        {
            try
            {
                var path = getPath("json files (*.json)|*.json", ".json");
                if (path != null)
                {
                    string json = JsonConvert.SerializeObject(Data);
                    System.IO.File.WriteAllText(path, json);

                    PredefinedDialogs.MessageDialog(this, "Done!", (s, e) => { }, (s, e) => { }, "ViewSerieDialog");
                }
            }
            catch (Exception)
            {
                PredefinedDialogs.MessageDialog(this, "Error", (s, e) => { }, (s, e) => { }, "ViewSerieDialog");
            }
        }

        private string getPath(string filter, string extension)
        {
            Microsoft.Win32.SaveFileDialog savedialog = new Microsoft.Win32.SaveFileDialog();
            savedialog.FileName = SerieName;
            savedialog.Filter = filter;
            savedialog.FilterIndex = 2;
            savedialog.RestoreDirectory = true;

            if (savedialog.ShowDialog() == true)
            {
                var path = savedialog.FileName;
                System.IO.Path.ChangeExtension(path, extension);
                if (File.Exists(path)) File.Delete(path);
                return path;
            }

            return null;
        }
        #endregion
    }
}