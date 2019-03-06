using System.Collections.Generic;
using QuickAD.Helper_Classes;

namespace QuickAD.Models
{
	class BatchImport
	{
		public BatchImport()
		{
			ComputerNames = new List<string>();
		}

		#region Properties

		public List<string> ComputerNames { get; set; }

		public string BatchDescription { get; set; }

		public int BatchCount
		{
			get { return ComputerNames.Count; }
		}

		#endregion // Properties

		#region Methods

		public static List<BatchImport> CreateBatchFromExcel(string file)
		{
			var sheetValues = ExcelManipulator.GetArrayFromWorkbookFile(file);
			
			var newBatchList = new List<BatchImport>(sheetValues.GetLength(0));
			for (int i = 1; i < sheetValues.GetLength(1) + 1; i++)
			{
				newBatchList.Add(new BatchImport());
				newBatchList[i - 1].BatchDescription = sheetValues[1, i].ToString();
				for (int j = 2; j < sheetValues.GetLength(0) + 1; j++)
				{
					if(sheetValues[j,i] != null)	newBatchList[i - 1].ComputerNames.Add(sheetValues[j,i].ToString());
				}
			}

			return newBatchList;
		}

		public static List<BatchImport> CreateBatchFromCsv(string file)
		{
			var sheetValues = ExcelManipulator.GetArrayFromCsvFile(file);
			var newBatchList = new List<BatchImport>(sheetValues.GetLength(1));
			for (int i = 0; i < sheetValues.GetLength(1); i++)
			{
				newBatchList.Add(new BatchImport());
				newBatchList[i].BatchDescription = sheetValues[0,i].ToString();
				for (int j = 1; j < sheetValues.GetLength(0); j++)
				{
					if(sheetValues[j,i] != null && !string.IsNullOrWhiteSpace(sheetValues[j,i].ToString()))
						newBatchList[i].ComputerNames.Add(sheetValues[j,i].ToString());
				}
			}

			return newBatchList;
		}

		#endregion //Methods
	}
}
