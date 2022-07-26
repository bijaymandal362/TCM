using Models.ProjectModule;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Helper.ExcelExport
{
	public static class ExportAllTestCaseHelper
	{
		public static byte[] TestCaseDetailsToExcel(List<TestCaseViewModelForExcel> data)
		{
			try
			{
				byte[] result;
				ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
				using (var package = new ExcelPackage())
				{
					string[] totalColumns = {
								"Test Case Name",
								"Test Scenario",
								"Type",
								"Steps",
								"Description",
								"Expected Result"
					};
					var functionName = data.OrderBy(x => x.OrderDate).Select(x => x.FunctionName).Distinct().ToList();

					//var functionName = data.OrderBy(x => x.OrderDate).Select(x =>
					//new
					//{
					//	FunctionName = x.FunctionName,
					//	ProjectModuleId = x.ProjectModuleId
					//}).ToList();
					int count = 1;

					foreach (var items in functionName)
					{
						// add a new worksheet to the empty workbook
						var worksheet = package.Workbook.Worksheets.Add(items + " " + "Sheet" + " " + count++);
						using (var cells = worksheet.Cells[1, 1, 1, totalColumns.Length]) //(1,1) => (1,10)
						{
							cells.Style.Font.Bold = true;
						}

						var records = data.Where(x => x.FunctionName == items).OrderBy(x => x.TestCaseName).ThenBy(x => x.Steps).ToList();
						int totalRows = records.Count + 1; //data including header row

						for (var i = 0; i < totalColumns.Length; i++)
						{
							worksheet.Cells[1, i + 1].Value = totalColumns[i]; //populate header row
						}

						//Add values
						var j = 2; //to start data from second row after the header row.

						foreach (var item in records)
						{

							worksheet.Cells["A" + j].Value = item.TestCaseName;
							worksheet.Cells["B" + j].Value = item.TestScenario;
							worksheet.Cells["C" + j].Value = item.Type;
							worksheet.Cells["D" + j].Value = item.Steps;
							worksheet.Cells["E" + j].Value = item.Description;
							worksheet.Cells["F" + j].Value = item.ExpectedResult;

							j++;
						}

						using (ExcelRange Rng = worksheet.Cells[1, 1, totalRows, totalColumns.Length])
						{
							Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
							Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
							Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
							Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
						}
						worksheet.Cells.AutoFitColumns();
					}
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
