using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace QuickAD.Helper_Classes
{
	static class ExcelManipulator
	{
		#region Fields

		private static Excel.Application _xlApplication;
		private static Excel.Workbook _xlWorkbook;
		private static Excel.Worksheet _xlWorksheet;
		private static Excel.Range _xlRange;

		#endregion // Fields

		#region Methods

		public static object[,] GetArrayFromWorkbookFile(string file)
		{
			_xlApplication = new Excel.Application();
			_xlWorkbook = _xlApplication.Workbooks.Open(Path.Combine(Directory.GetCurrentDirectory(), file));
			_xlWorksheet = _xlWorkbook.Sheets[1];
			_xlRange = _xlWorksheet.UsedRange;

			var valueRange = (object[,])_xlRange.Value[Excel.XlRangeValueDataType.xlRangeValueDefault];

			GC.Collect();
			GC.WaitForPendingFinalizers();

			Marshal.ReleaseComObject(_xlRange);
			Marshal.ReleaseComObject(_xlWorksheet);
			_xlWorkbook.Close();
			Marshal.ReleaseComObject(_xlWorkbook);
			_xlApplication.Quit();
			Marshal.ReleaseComObject(_xlApplication);

			return valueRange;
		}

		public static object[,] GetArrayFromCsvFile(string file)
		{
			List<string[]> rawData = new List<string[]>();
			using (var csvReader = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), file), FileMode.Open))
			using (var parser = new NotVisualBasic.FileIO.CsvTextFieldParser(csvReader))
			{
				while (!parser.EndOfData)
				{
					rawData.Add(parser.ReadFields());
				}
			}

			var data = new object[rawData.Count, rawData[0].Length];
			for (int i = 0; i < rawData.Count; i++)
			{
				for (int j = 0; j < rawData[0].Length; j++)
				{
					data[i, j] = rawData[i].GetValue(j);
				}
			}

			return data;
		}

		#endregion // Methods

	}
}
