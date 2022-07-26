using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using System;
using System.Linq;

namespace Infrastructure.Helper.Excel
{
	public static class ExcelDataHelper
	{
		public static decimal decimalValue;
		public static decimal? nullableDecimalValue;
		public static int intValue;
		public static bool boolValue;
		public static string stringValue;


		public static decimal GetDecimalValue(IRow row, int index)
		{
			try
			{
				decimalValue = decimal.Parse(row.GetCell(index).NumericCellValue.ToString().Trim());
			}
			catch
			{
				return 0;
			}
			return decimalValue;
		}
		public static string GetStringValue(IRow row, int index)
		{


			if (row.GetCell(index) is null)
				return string.Empty;
			var rowContent = row.GetCell(index).StringCellValue.ToString()?.Replace("\n", "").Trim();
			try
			{
				if (string.IsNullOrWhiteSpace(rowContent) || string.IsNullOrEmpty(rowContent))
				{
					return string.Empty;
				}
			}
			catch
			{
				return string.Empty;
			}
			return rowContent;
		}
		public static decimal? GetNullableDecimalValue(IRow row, int index)
		{
			try
			{
				string value = row.GetCell(index).NumericCellValue.ToString().Trim();
				if (string.IsNullOrEmpty(value)) return null;
				else return decimal.Parse(value);
			}
			catch
			{
				return null;
			}
		}
		public static int GetIntValue(IRow row, int? index)
		{
			bool stepValue;
			int step;
			string val;
			val = row.GetCell(index.Value)?.ToString().Trim();
			if (val != null)
			{
				stepValue = int.TryParse(val.Trim(), out step);
				if (stepValue)
				{
					return step;
				}
				else
				{
					return 99999;
				}
			}
			else
			{
				return 99999;
			}



		}
		public static bool GetBooleanValue(IRow row, int? index)
		{
			try
			{
				string value = row.GetCell(index.Value)?.ToString().Trim().ToLower().Replace(" ", string.Empty);

				if (value == "y")
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}
		public static int? GetColumnIndex(IRow row, string header)
		{
			return row.Cells.FirstOrDefault(c => c.StringCellValue.Trim().ToLower().Replace(" ", string.Empty) == header.Trim().ToLower().Replace(" ", string.Empty))?.ColumnIndex;
		}
	}

	public static class ExcelHeadingHelper
	{
		//Column Name of Test Case Detail
		public const string TestCaseName = "Test Case Name";
		public const string TestScenario = "Test Scenario";
		public const string Type = "Type";
		public const string Steps = "Steps";
		public const string Description = "Description";
		public const string ExpectedResult = "Expected Result";
	}
}
