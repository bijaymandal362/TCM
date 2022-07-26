using Models.TestRun;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace Infrastructure.Helper.ExcelExport
{
    public static class TestRunTestCaseExportHelper
    {
        public static byte[] TestRunTestCaseByTestRunIdToExcel(List<TestRunTestCaseExportModel> data1, List<TestRunTestCaseExportTestResultCountModel> datas, List<FunctionModuleModel> funcitonData, TestRunTestCaseCountPercentageModel testRunStatusCountPercentage, TestRunExcelModel testRunDetails, List<FunctionNameWithTestCaseDetailModel> fucntionProjectModuleResult)
        {
           try
            {
                byte[] result;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    int cellNumb = 1;
                    string[] totalColumns = {
                                "SN",
                                "Test Case",
                                "Test Plan",
                                "Status"
                    };

                    string[] totalColumn = {
                                "Test Plan",
                                "Passed",
                                "Failed",
                                "Pending",
                                "Blocked",
                    };

                    string[] totalColumn3 = {
                                "Function",
                                "Count"
                    };
                    string[] totalColumn4 = {
                                "Status",
                                "Percentage",

                    };

                    string[] testRunColums = {
                                "Project",
                                "Test Run",
                                "Environment"


                    };

                    string[] functionCloumns = {
                                "Test Plan",
                                "Test Case",
                                "Scenario",
                                "Expected Result",
                                "Status",
                                "Remarks"


                    };

                    //For Title

                    // Target a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Chart");
                    var worksheets = package.Workbook.Worksheets.Add("Data");
                    //make column H wider and set the text align to the top and right
                    double columnWidthA = 35;
                    double columnWidthB = 60;
                    double columnWidthRemarks = 15;
                    worksheet.Column(1).Width = columnWidthA;
                    worksheet.Column(1).Style.WrapText = true;
                    worksheet.Column(2).Width = columnWidthB;
                    worksheet.Column(2).Style.WrapText = true;
                    worksheet.Column(3).Width = columnWidthB;
                    worksheet.Column(3).Style.WrapText = true;                    
                    worksheet.Column(5).Width = columnWidthRemarks;
                    worksheet.Column(5).Style.WrapText = true;

                    //wrap text in the cells

                    //Header for Chart
                    using (ExcelRange Rng = worksheet.Cells["A2:D2"])
                    {
                        Rng.Value = "QA Execution Report";
                        Rng.Merge = true;
                        Rng.Style.Font.Size = 16;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                        Rng.Style.Border.Bottom.Color.SetColor(Color.BlueViolet);
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    }


                    //Worksheet for Chart
                    using (var cells = worksheet.Cells[5, 1]) //(1,1) => (1,10)
                    {
                        cells.Style.Font.Bold = true;
                    }
                    using (var cells = worksheet.Cells[4, 1]) //(1,1) => (1,10)
                    {
                        cells.Style.Font.Bold = true;
                    }
                    using (var cells = worksheet.Cells[4, 3]) //(1,1) => (1,10)
                    {
                        cells.Style.Font.Bold = true;
                    }

                    //Header for Chart

                    worksheet.Cells[4, 1].Value = testRunColums[0]; //populate header row
                    worksheet.Cells[5, 1].Value = testRunColums[1]; //populate header row
                    worksheet.Cells[4, 3].Value = testRunColums[2]; //populate header row


                    //Add values


                    if (testRunDetails != null)
                    {
                        worksheet.Cells["B" + 4].Value = testRunDetails.ProjectName;
                        worksheet.Cells["B" + 5].Value = testRunDetails.TestCaseManagementSystem;
                        worksheet.Cells["D" + 4].Value = testRunDetails.Environment;

                        using (ExcelRange Rng = worksheet.Cells[4, 1, 5, testRunColums.Length + 1])
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }

                    }

                    //worksheet.InsertRow(5, 
                    var testRunStatusCountPercentages = testRunStatusCountPercentage;

                    var data = new List<KeyValuePair<string, decimal>>
                            {
                                new KeyValuePair<string, decimal>("Passed", testRunStatusCountPercentages.PassedPercentage),
                                new KeyValuePair<string, decimal>("Failed", testRunStatusCountPercentages.FailedPercentage),
                                new KeyValuePair<string, decimal>("Pending",testRunStatusCountPercentages.PendingPercentage),
                                new KeyValuePair<string, decimal>("Blocked", testRunStatusCountPercentages.BlockedPercentage),

                            };
                    for (var i = 0; i < totalColumn4.Length; i++)
                    {
                        worksheets.Cells[1, i + 16].Value = totalColumn4[i]; //populate header row
                    }

                    //Fill the table
                    var startCell = worksheets.Cells[1, 16];


                    if (testRunStatusCountPercentage != null)
                    {
                        for (var i = 0; i < data.Count(); i++)
                        {
                            startCell.Offset(i + 1, 0).Value = data[i].Key;
                            startCell.Offset(i + 1, 1).Value = data[i].Value;
                            
                        }                    
                        
                        using (ExcelRange Rng = worksheets.Cells[1, 16, 5, data.Count + 13])
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                    }

                    //Add the chart to the sheet
                    var pieChart = worksheet.Drawings.AddChart("Chart1", eChartType.Pie);
                    pieChart.SetPosition(7, 1, 0, 0);
                    pieChart.SetSize(1090, 300);
                    pieChart.Title.Text = "Test Case Excution Status";
                    pieChart.Title.Font.Bold = true;
                    pieChart.Title.Font.Size = 12;


                    //Set the data range
                    var series = pieChart.Series.Add(worksheets.Cells[2, 17, data.Count + 1, 17], worksheets.Cells[2, 16, data.Count + 1, 16]);
                    var pieSeries = (ExcelPieChartSerie)series;
                    pieSeries.Explosion = 3;

                    //Format the labels
                    pieSeries.DataLabel.Font.Bold = true;
                    pieSeries.DataLabel.ShowValue = false;
                    pieSeries.DataLabel.ShowPercent = true;
                    pieSeries.DataLabel.ShowLeaderLines = true;
                    pieSeries.DataLabel.Separator = ";";
                    pieSeries.DataLabel.Position = eLabelPosition.BestFit;
                    pieChart.Legend.Add();
                    pieChart.Legend.Border.Width = 0;
                    pieChart.Legend.Font.Size = 12;
                    pieChart.Legend.Font.Bold = true;
                    pieChart.Legend.Position = eLegendPosition.Right;

                    pieChart.StyleManager.SetChartStyle(ePresetChartStyle.StackedColumnChartStyle1, ePresetChartColors.ColorfulPalette1);


                    //Percentage and decimal validation

                    using (ExcelRange Rng = worksheets.Cells["Q1:Q5"])
                    {
                        Rng.Style.Numberformat.Format = "#0\\.00%";
                    }
                    //Format the legend

                    // add a new worksheet to the empty workbook

                    using (var cells = worksheets.Cells[1, 1, 1, totalColumns.Length]) //(1,1) => (1,10)
                    {
                        cells.Style.Font.Bold = true;
                    }
                    using (var cells = worksheets.Cells[1, 7, 1, totalColumn.Length + 7]) //(1,1) => (1,10)
                    {
                        cells.Style.Font.Bold = true;
                    }

                    using (var cells = worksheets.Cells[1, 13, 1, totalColumn3.Length + 13]) //(1,1) => (1,10)
                    {
                        cells.Style.Font.Bold = true;
                    }
                    using (var cells = worksheets.Cells[1, 16, 1, totalColumn4.Length + 16]) //(1,1) => (1,10)
                    {
                        cells.Style.Font.Bold = true;
                    }

                    var records = data1.ToList();
                    var list = datas.ToList();
                    var funValue = funcitonData.ToList();
                    int totalRows = data1.Count + 1; //data including header row
                    int totalRowsList = datas.Count + 2;

                    var totalRowsForFuntion = funcitonData.Count + 1;

                    for (var i = 0; i < totalColumns.Length; i++)
                    {
                        worksheets.Cells[1, i + 1].Value = totalColumns[i]; //populate header row
                    }
                    for (var i = 0; i < totalColumn.Length; i++)
                    {
                        worksheets.Cells[1, i + 7].Value = totalColumn[i]; //populate header row
                    }
                    for (var i = 0; i < totalColumn3.Length; i++)
                    {
                        worksheets.Cells[1, i + 13].Value = totalColumn3[i]; //populate header row
                    }

                    var chart = worksheet.Drawings.AddChart("chart test", eChartType.BarStacked);
                    chart.SetPosition(43, 1, 0, 0);
                    chart.SetSize(1090, 300);
                    chart.Title.Text = "Test Plan Wise Report";
                    chart.Title.Font.Bold = true;
                    chart.YAxis.RemoveGridlines(true, true);
                    chart.YAxis.RemoveGridlines();
                    chart.Title.Font.Size = 12;


                    chart.XAxis.MajorTickMark = eAxisTickMark.None;
                    chart.XAxis.MinorTickMark = eAxisTickMark.None;
                    chart.YAxis.MajorTickMark = eAxisTickMark.None;
                    chart.YAxis.MinorTickMark = eAxisTickMark.None;
                    //Add values
                    var j = 2; //to start data from second row after the header row.
                    var k = 2;
                    var l = 2;
                    if (data1 != null)
                    {
                        foreach (var item in records)
                        {

                            worksheets.Cells["A" + j].Value = cellNumb;
                            worksheets.Cells["B" + j].Value = item.TestCaseName;
                            worksheets.Cells["C" + j].Value = item.TestPlanName;
                            worksheets.Cells["D" + j].Value = item.Status;

                            j++;
                            cellNumb++;
                        }
                        using (ExcelRange Rng = worksheets.Cells[1, 1, totalRows, totalColumns.Length])
                        {

                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }

                    }
                    if (datas != null)
                    {
                        foreach (var item in datas)
                        {
                            worksheets.Cells["G" + k].Value = item.TestPlanNameForCount;
                            worksheets.Cells["H" + k].Value = item.TotalPassedCount;
                            worksheets.Cells["I" + k].Value = item.TotalFailedCount;
                            worksheets.Cells["J" + k].Value = item.TotalPendingCount;
                            worksheets.Cells["K" + k].Value = item.TotalBlockCount;
                            k++;

                        }


                        chart.StyleManager.SetChartStyle(ePresetChartStyle.StackedColumnChartStyle1, ePresetChartColors.ColorfulPalette1);
                        var passedSer = chart.Series.Add(worksheets.Cells[2, 8, datas.Count + 1, 8], worksheets.Cells[2, 7, datas.Count + 1, 7]);
                        passedSer.HeaderAddress = new ExcelAddress("'Data'!H1");


                        var failedSer = chart.Series.Add(worksheets.Cells[2, 9, datas.Count + 1, 9], worksheets.Cells[2, 7, datas.Count + 1, 7]);
                        failedSer.HeaderAddress = new ExcelAddress("'Data'!I1");

                        var PendingSer = chart.Series.Add(worksheets.Cells[2, 10, datas.Count + 1, 10], worksheets.Cells[2, 7, datas.Count + 1, 7]);
                        PendingSer.HeaderAddress = new ExcelAddress("'Data'!J1");


                        var blockedSer = chart.Series.Add(worksheets.Cells[2, 11, datas.Count + 1, 11], worksheets.Cells[2, 7, datas.Count + 1, 7]);
                        blockedSer.HeaderAddress = new ExcelAddress("'Data'!K1");
                        var barFailedSeries = (ExcelBarChartSerie)failedSer;
                        var barFailedSeriesZero = (ExcelBarChartSerie)failedSer;
                        var barPassedSeries = (ExcelBarChartSerie)passedSer;
                        var barBlockedSeries = (ExcelBarChartSerie)blockedSer;
                        var barPendingSeries = (ExcelBarChartSerie)PendingSer;

                        foreach (var item in datas)
                        {

                            if (item.TotalPassedCount != 0)
                            {

                                barPassedSeries.DataLabel.Font.Bold = true;
                                barPassedSeries.DataLabel.ShowValue = true;
                                barPassedSeries.DataLabel.ShowLeaderLines = true;
                                barPassedSeries.DataLabel.Separator = ";";
                                barPassedSeries.DataLabel.Position = eLabelPosition.InBase;
                            }

                            if (item.TotalBlockCount != 0)
                            {
                                barBlockedSeries.DataLabel.Font.Bold = true;
                                barBlockedSeries.DataLabel.ShowValue = true;
                                barBlockedSeries.DataLabel.ShowLeaderLines = true;
                                barBlockedSeries.DataLabel.Separator = ";";
                                barBlockedSeries.DataLabel.Position = eLabelPosition.Center;
                            }

                            if (item.TotalPendingCount != 0)
                            {

                                barPendingSeries.DataLabel.Font.Bold = true;
                                barPendingSeries.DataLabel.ShowValue = true;
                                barPendingSeries.DataLabel.ShowPercent = true;
                                barPendingSeries.DataLabel.ShowLeaderLines = true;
                                barPendingSeries.DataLabel.Separator = ";";
                                barPendingSeries.DataLabel.Position = eLabelPosition.Center;
                            }
                            if (item.TotalFailedCount != 0)
                            {

                                barFailedSeries.DataLabel.Font.Bold = true;
                                barFailedSeries.DataLabel.ShowValue = true;
                                barFailedSeries.DataLabel.ShowPercent = true;
                                barFailedSeries.DataLabel.ShowLeaderLines = true;
                                barFailedSeries.DataLabel.Separator = ";";
                                barFailedSeries.DataLabel.Position = eLabelPosition.Center;
                            }


                        }

                        //functionValue in Chart




                        //have to remove cat nodes from each series so excel autonums 1 and 2 in xaxis
                        var chartXml = chart.ChartXml;
                        var nsm = new XmlNamespaceManager(chartXml.NameTable);

                        var nsuri = chartXml.DocumentElement.NamespaceURI;
                        nsm.AddNamespace("c", nsuri);

                        //Get the Series ref and its cat
                        var serNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:bar3DChart/c:ser", nsm);
                        foreach (XmlNode serNode in serNodes)
                        {
                            //Cell any cell reference and replace it with a string literal list
                            var catNode = serNode.SelectSingleNode("c:cat", nsm);
                            catNode.RemoveAll();

                            //Create the string list elements
                            var ptCountNode = chartXml.CreateElement("c:ptCount", nsuri);
                            ptCountNode.Attributes.Append(chartXml.CreateAttribute("val", nsuri));
                            ptCountNode.Attributes[0].Value = "2";

                            var v0Node = chartXml.CreateElement("c:v", nsuri);
                            v0Node.InnerText = "opening";
                            var pt0Node = chartXml.CreateElement("c:pt", nsuri);
                            pt0Node.AppendChild(v0Node);
                            pt0Node.Attributes.Append(chartXml.CreateAttribute("idx", nsuri));
                            pt0Node.Attributes[0].Value = "0";

                            var v1Node = chartXml.CreateElement("c:v", nsuri);
                            v1Node.InnerText = "closing";
                            var pt1Node = chartXml.CreateElement("c:pt", nsuri);
                            pt1Node.AppendChild(v1Node);
                            pt1Node.Attributes.Append(chartXml.CreateAttribute("idx", nsuri));
                            pt1Node.Attributes[0].Value = "1";

                            //Create the string list node
                            var strLitNode = chartXml.CreateElement("c:strLit", nsuri);
                            strLitNode.AppendChild(ptCountNode);
                            strLitNode.AppendChild(pt0Node);
                            strLitNode.AppendChild(pt1Node);
                            catNode.AppendChild(strLitNode);
                        }

                        worksheets.Cells["G" + k].Value = "Total";
                        worksheets.Cells["H" + k].Value = datas.Sum(x => x.TotalPassedCount);
                        worksheets.Cells["I" + k].Value = datas.Sum(x => x.TotalFailedCount);
                        worksheets.Cells["J" + k].Value = datas.Sum(x => x.TotalPendingCount);
                        worksheets.Cells["K" + k].Value = datas.Sum(x => x.TotalBlockCount);

                        using (ExcelRange Rng = worksheets.Cells[1, 7, totalRowsList, totalColumn.Length + 6])
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                    }

                    if (fucntionProjectModuleResult != null)
                    {
                        var y = 62;

                        foreach (var item in fucntionProjectModuleResult)
                        {

                            worksheet.Cells[y, 1].Value = functionCloumns[0];
                            using (var cells = worksheet.Cells[y, 1]) //(1,1) => (1,10)
                            {
                                cells.Style.Font.Bold = true;
                            }
                            worksheet.Cells[y, 2].Value = item.TestPlanName;

                            for (int i = 1; i < functionCloumns.Length; i++)
                            {
                                worksheet.Cells[y + 1, i].Value = functionCloumns[i];
                                worksheet.Cells[y + 1, i].Style.Font.Bold = true;
                            }

                            using (ExcelRange Rng = worksheet.Cells[y, 1, y + 1, 2])
                            {
                                Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            }
                            var q = y + 2;
                            foreach (var item3 in item.TestCaseDetail)
                            {

                                worksheet.Cells["A" + q].Value = item3.TestCaseName;
                                worksheet.Cells["B" + q].Value = item3.PreConditon;
                                worksheet.Cells["C" + q].Value = item3.ExceptedResult;
                                worksheet.Cells["D" + q].Value = item3.Status;
                                worksheet.Cells["E" + q].Value = item3.Remarks;
                                q++;

                            }
                            using (ExcelRange Rng = worksheet.Cells[y + 1, 1, item.TestCaseDetail.Count + y + 1, functionCloumns.Length - 1])
                            {
                                Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            }
                            y = q + 3;
                        }
                    }
                    if (funcitonData != null)
                    {
                        foreach (var item in funValue)
                        {
                            worksheets.Cells["M" + l].Value = item.FunctionName;
                            worksheets.Cells["N" + l].Value = item.TotalCountTestCaseByTestRunId;
                            l++;

                        }
                        using (ExcelRange Rng = worksheets.Cells[1, 13, totalRowsForFuntion, totalColumn3.Length + 12])
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                        var redarChart = worksheet.Drawings.AddRadarChart("RadarChart", eRadarChartType.RadarMarkers);
                        redarChart.SetPosition(25, 1, 0, 0);
                        redarChart.SetSize(1090, 300);
                        redarChart.Title.Text = "Function Map";
                        redarChart.Title.Font.Bold = true;
                        redarChart.Title.Font.Size = 12;

                        var serie = redarChart.Series.Add(worksheets.Cells[2, 14, funValue.Count + 1, 14], worksheets.Cells[2, 13, funValue.Count + 1, 13]);
                        serie.HeaderAddress = new ExcelAddress("'Data'!N1");
                        redarChart.StyleManager.SetChartStyle(ePresetChartStyleMultiSeries.RadarChartStyle4);
                        redarChart.Fill.Color = System.Drawing.Color.White;
                        redarChart.Legend.Position = eLegendPosition.TopRight;
                        //If you want to apply custom styling do that after setting the chart style so its not overwritten.
                        redarChart.Legend.Effect.SetPresetShadow(ePresetExcelShadowType.OuterTopLeft);
                        var radarSeries = (ExcelRadarChartSerie)serie;
                        radarSeries.Marker.Size = 5;

                    }

                    worksheets.Cells.AutoFitColumns();
                   
                    result = package.GetAsByteArray();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Download failed : {ex.Message}");

            }

        }
    }
}
