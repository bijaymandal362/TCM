using Microsoft.Extensions.Logging;

using Models.Import;
using Models.ProjectModule;

using NPOI.HSSF.Record;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infrastructure.Helper.Excel
{
	public class TestCaseFileImportHelper
	{


		public TestCaseFileImportHelper()
		{

		}
		public static List<TestCaseViewModel> ImportFile(ImportProjectModuleModel model)
		{
			var data = new List<TestCaseViewModel>();

			string sFileExtension = Path.GetExtension(model.File.FileName).ToLower();
			ISheet sheet;
			HSSFWorkbook hssfwb = new HSSFWorkbook();
			XSSFWorkbook xssfwb = new XSSFWorkbook();

			HSSFFormulaEvaluator formula = new HSSFFormulaEvaluator(hssfwb);
			XSSFFormulaEvaluator formula1 = new XSSFFormulaEvaluator(xssfwb);

			var errorData = new List<string>();
			List<string> errorList = new List<string>();

			using (var stream = model.File.OpenReadStream())
			{
				stream.Position = 0;//to check sheet 1/2/3
				if (sFileExtension == ".xls")
				{
					hssfwb = new(stream); //This will read the Excel 97-2000 formats  
					sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
					formula = new HSSFFormulaEvaluator(hssfwb);


				}
				else if (sFileExtension == ".xlsx")
				{

					xssfwb = new(stream); //This will read 2007 Excel format  
					sheet = xssfwb.GetSheetAt(0); //get first sheet from workbook   
					formula1 = new XSSFFormulaEvaluator(xssfwb);


				}
				else
				{
					throw new InvalidDataException("File Formate not-supported, Please select excel file only");
				}


				var firstRow = sheet.GetRow(sheet.FirstRowNum);
				var validateRowHeader = firstRow.Where(x => x.ColumnIndex > 5).Any();
				var nullEmptyHeaderCounts = firstRow.Cells.Where(x => x.ColumnIndex > 5).ToList();


				foreach (var item in nullEmptyHeaderCounts)
				{
					if (!string.IsNullOrWhiteSpace(item.StringCellValue))
					{
						errorData.Add($"Failed to upload excel , Header Column Exceeded on column {item.ColumnIndex} and HeaderName is {item.StringCellValue}");
					}
					if (string.IsNullOrWhiteSpace(item.StringCellValue))
					{
						continue;
					}
				}


				var testCaseNameIndex = ExcelDataHelper.GetColumnIndex(firstRow, ExcelHeadingHelper.TestCaseName);
				var testScenarioIndex = ExcelDataHelper.GetColumnIndex(firstRow, ExcelHeadingHelper.TestScenario);
				var typeIndex = ExcelDataHelper.GetColumnIndex(firstRow, ExcelHeadingHelper.Type);
				var stepsIndex = ExcelDataHelper.GetColumnIndex(firstRow, ExcelHeadingHelper.Steps);
				var descritpionIndex = ExcelDataHelper.GetColumnIndex(firstRow, ExcelHeadingHelper.Description);
				var expectedResultIndex = ExcelDataHelper.GetColumnIndex(firstRow, ExcelHeadingHelper.ExpectedResult);

				string header = null;

				if (testCaseNameIndex == null) header = ExcelHeadingHelper.TestCaseName;
				else if (testScenarioIndex == null) header = ExcelHeadingHelper.TestScenario;
				else if (typeIndex == null) header = ExcelHeadingHelper.Type;
				else if (stepsIndex == null) header = ExcelHeadingHelper.Steps;
				else if (descritpionIndex == null) header = ExcelHeadingHelper.Description;
				else if (expectedResultIndex == null) header = ExcelHeadingHelper.ExpectedResult;



				bool isValidTestCaseNameheader = firstRow.Cells.Where(x => x.StringCellValue == string.Empty && x.ColumnIndex == 0).Any();
				bool isValidTestScenarioheader = firstRow.Cells.Where(x => x.StringCellValue == string.Empty && x.ColumnIndex == 1).Any();
				bool isValidTypeheader = firstRow.Cells.Where(x => x.StringCellValue == string.Empty && x.ColumnIndex == 2).Any();
				bool isValidStepheader = firstRow.Cells.Where(x => x.StringCellValue == string.Empty && x.ColumnIndex == 3).Any();
				bool isValidDescriptionheader = firstRow.Cells.Where(x => x.StringCellValue == string.Empty && x.ColumnIndex == 4).Any();
				bool isValidExpectedResultheader = firstRow.Cells.Where(x => x.StringCellValue == string.Empty && x.ColumnIndex == 5).Any();
		
				if (isValidTestCaseNameheader)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.TestCaseName} header is missing");
					
				}
				if (isValidTestScenarioheader)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.TestScenario} header is missing");
				}
				if (isValidTypeheader)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.Type} header is missing");
				}
				if (isValidStepheader)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.Steps} header is missing");
				}
				if (isValidDescriptionheader)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.Description} header is missing");
				}
				if (isValidExpectedResultheader)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.ExpectedResult} header is missing");
				}

				bool testCaseNameHeaderMismatch = firstRow.Cells.Where(x => x.StringCellValue != ExcelHeadingHelper.TestCaseName && x.StringCellValue != string.Empty && x.ColumnIndex == 0).Any();
				bool testScenarioHeaderMismatch = firstRow.Cells.Where(x => x.StringCellValue != ExcelHeadingHelper.TestScenario && x.StringCellValue != string.Empty && x.ColumnIndex == 1).Any();
				bool typeHeaderMismatch = firstRow.Cells.Where(x => x.StringCellValue != ExcelHeadingHelper.Type && x.StringCellValue != string.Empty && x.ColumnIndex == 2).Any();
				bool stepsHeaderMismatch = firstRow.Cells.Where(x => x.StringCellValue != ExcelHeadingHelper.Steps && x.StringCellValue != string.Empty && x.ColumnIndex == 3).Any();
				bool descriptionHeaderMismatch = firstRow.Cells.Where(x => x.StringCellValue != ExcelHeadingHelper.Description && x.StringCellValue != string.Empty && x.ColumnIndex == 4).Any();
				bool expectedResultHeaderMismatch = firstRow.Cells.Where(x => x.StringCellValue != ExcelHeadingHelper.ExpectedResult && x.StringCellValue != string.Empty && x.ColumnIndex == 5).Any();

				if (testCaseNameHeaderMismatch)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.TestCaseName} header is invalid format");
				}
				if (testScenarioHeaderMismatch)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.TestScenario} header is invalid format");
				}
				if (typeHeaderMismatch)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.Type} header is invalid format");
				}
				if (stepsHeaderMismatch)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.Steps} header is invalid format");
				}
				if (descriptionHeaderMismatch)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.Description} header is invalid format");
				}
				if (expectedResultHeaderMismatch)
				{
					errorData.Add($"Failed to upload excel,  {ExcelHeadingHelper.ExpectedResult} header is invalid format");
				}


				if (isValidTestCaseNameheader || testCaseNameHeaderMismatch
					|| isValidTestScenarioheader || testScenarioHeaderMismatch
					|| isValidTypeheader || typeHeaderMismatch
					|| isValidStepheader || stepsHeaderMismatch
					|| isValidDescriptionheader || descriptionHeaderMismatch
					|| isValidExpectedResultheader || expectedResultHeaderMismatch)
				{
					throw new InvalidDataException(errorData.Aggregate((x, y) => x + ", " + Environment.NewLine + y));

				}
				for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
				{
					var rowData = new TestCaseViewModel();
					IRow row = sheet.GetRow(i);

					if (row == null) continue;
					if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
					string columnHead = ExcelHeadingHelper.TestCaseName;
					try
					{
						


						rowData.TestCaseName = ExcelDataHelper.GetStringValue(row, testCaseNameIndex.Value);
						columnHead = ExcelHeadingHelper.TestScenario;
						rowData.TestScenario = ExcelDataHelper.GetStringValue(row, testScenarioIndex.Value);
						columnHead = ExcelHeadingHelper.Type;
						rowData.Type = ExcelDataHelper.GetStringValue(row, typeIndex.Value);
						columnHead = ExcelHeadingHelper.Steps;
						rowData.Steps = ExcelDataHelper.GetIntValue(row, stepsIndex.Value);
						columnHead = ExcelHeadingHelper.Description;
						rowData.Description = ExcelDataHelper.GetStringValue(row, descritpionIndex.Value);
						columnHead = ExcelHeadingHelper.ExpectedResult;
						rowData.ExpectedResult = ExcelDataHelper.GetStringValue(row, expectedResultIndex.Value);


						if (rowData.TestCaseName == string.Empty && rowData.TestScenario == string.Empty && rowData.Description == string.Empty &&
				rowData.ExpectedResult == string.Empty && rowData.Type == string.Empty && rowData.Steps == 99999)
						{
							continue;
						}
						if (string.IsNullOrWhiteSpace(rowData.TestCaseName) && string.IsNullOrWhiteSpace(rowData.TestScenario) && string.IsNullOrWhiteSpace(rowData.Type) &&
		string.IsNullOrWhiteSpace(rowData.Description) && string.IsNullOrWhiteSpace(rowData.ExpectedResult))
						{
							continue;
						}

						if (string.IsNullOrEmpty(rowData.TestCaseName))
						{
							errorData.Add(($"TestCaseName not found. Please check row no. \"{i + 1}\" of column head \"{testCaseNameIndex.Value + 1}\""));
						}
						if (string.IsNullOrEmpty(rowData.TestScenario))
						{
							errorData.Add(($"TestScenario not found. Please check row no. \"{i + 1}\" of column head \"{testScenarioIndex.Value + 1}\""));
						}
						if (string.IsNullOrEmpty(rowData.Type))
						{
							errorData.Add(($"Type not found. Please check row no. \"{i + 1}\" of column head \"{typeIndex.Value + 1}\""));
						}
						if (string.IsNullOrEmpty(rowData.Description))
						{
							errorData.Add(($"Description not found. Please check row no. \"{i + 1}\" of column head \"{descritpionIndex.Value + 1}\""));
						}
						if (string.IsNullOrEmpty(rowData.ExpectedResult))
						{
							errorData.Add(($"ExpectedResult not found. Please check row no. \"{i + 1}\" of column head \"{expectedResultIndex.Value + 1}\""));
						}
						if (rowData.Steps == 99999)
						{
							errorData.Add(($"Step number not found or invalid format. Please check row no. \"{i + 1}\" of column head \"{stepsIndex.Value + 1}\""));
						}


					}
					catch (Exception ex)
					{

						errorData.Add($"Format not matched. Please check row no. \"{i + 1}\" of column head \"{columnHead}\"");


					}
					data.Add(rowData);

				}
	
			}

			var stepList = data.Where(x=>x.TestCaseName!=string.Empty && x.TestCaseName!=null).GroupBy(x => x.TestCaseName.ToLower().Trim()).Select(x => new StepCountModel
			{
				TestCaseName = x.Key,
				Steps = x.Select(x => x.Steps).ToList(),

			}).ToList();

			var checkListStep = stepList.Where(x => x.Steps.Count == 1).ToList();

			if (checkListStep.Count > 0)
			{
				List<string> nameList = checkListStep.Select(x => x.TestCaseName).ToList();
				string testCaseName = nameList.Aggregate((x, y) => x + ", " + Environment.NewLine + y);
				errorData.Add(testCaseName + " " + "does not contain Steps");
			}


			foreach (var item in stepList)
			{
				item.Steps = item.Steps.GroupBy(x => x)
				 .SelectMany(g => g.Skip(1)).ToList();

				if (item.Steps.Count > 0)
				{
					errorData.Add($"Duplicate TestCaseName found. {Environment.NewLine } TestCaseName: { item.TestCaseName}");
				}
			}


		

			if (errorData.Count > 0)
			{
				foreach (var item in errorData)
				{
					errorList.Add(item);
				}


			}
			if (errorList.ToList().Count > 0)
			{
				throw new InvalidDataException(errorData.Aggregate((x, y) => x + ", " + Environment.NewLine + y));
			}
			return data;
		}
	}
}