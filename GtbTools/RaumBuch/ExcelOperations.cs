using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;

namespace RaumBuch
{
    public class ExcelOperations
    {
        public ExcelOperations()
        {

        }

		public static Dictionary<string, List<int>> GetExcelDataModel(string path)
		{
			Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
			Application excelApp = new Application();
			Workbook excelBook = excelApp.Workbooks.Open(path);
			Worksheet worksheet = (Worksheet)excelBook.ActiveSheet;
			if((string)worksheet.Cells[1, 1].Text == "GTB_EXP_01")
            {
				int endIndex = 417;
				for (int i = 4; i < endIndex; i++)
				{
					string value5 = (string)worksheet.Cells[i, 5].Text; //cell address
					string value6 = (string)worksheet.Cells[i, 6].Text; //symbolId
					if (value5 == null || value5 == "0" || value5 == "x" || value5 == "") continue;
					if (value6 == null || value6 == "0" || value6 == "x" || value6 == "") continue;

					int integer = 0;
					if (Int32.TryParse(value6, out integer))
					{
						if (result.ContainsKey(value5))
						{
							result[value5].Add(integer);
						}
						else
						{

							List<int> list = new List<int>() { integer };
							result.Add(value5, list);
						}
					}
				}
				Marshal.ReleaseComObject(worksheet);
				excelBook.Close(false);
				excelApp.Quit();
				return result;
			}
			else
            {
				Marshal.ReleaseComObject(worksheet);
				excelBook.Close(false);
				excelApp.Quit();
				TaskDialog.Show("Info", "Die ausgewählte Datei ist falsch!");
				return null;
            }

		}

		public static string WriteToSheets(List<ExportedRoom> exportedRooms, string templatePath)
        {
			string result = templatePath + Environment.NewLine;
			Application excelApp = new Application();
			Workbook excelBook = excelApp.Workbooks.Open(templatePath);
			foreach (Worksheet worksheet in excelBook.Sheets)
            {
				if (!TestTheSheet(worksheet))
				{
					result += "Tabelle: " + worksheet.Name + ", hat ein anderes Format." + Environment.NewLine;
					continue;
				}
				ExportedRoom exportedRoom = exportedRooms.Where(e => e.MepRoomNumber == worksheet.Name).FirstOrDefault();
				if(exportedRoom == null)
                {
					result += "Tabelle: " + worksheet.Name + ", kann keinen entsprechenden MepRaum im Projekt finden." + Environment.NewLine;
					continue;
				}
				// if goes through tests then fill up the sheet
				int kaltwasser = 0;
				int warmwasser = 0;
                foreach (KeyValuePair<string, List<FamilyInstance>> pair in exportedRoom.ExportItems)
                {
					List<int> rowcol = ReadCellAddress(pair.Key);
					int row = rowcol[0];
					int column = rowcol[1];
					string fillText = pair.Value.Count.ToString();
					worksheet.Cells[row, column] = fillText;
					if (pair.Key == "H69" || pair.Key == "H82" || pair.Key == "H84") kaltwasser += pair.Value.Count;
					if (pair.Key == "H69" || pair.Key == "H82") warmwasser += pair.Value.Count;
				}
				if(kaltwasser > 0)
                {
					List<int> rowcol = ReadCellAddress("H87");
					int row = rowcol[0];
					int column = rowcol[1];
					string fillText = kaltwasser.ToString();
					worksheet.Cells[row, column] = fillText;
				}
				if (warmwasser > 0)
				{
					List<int> rowcol = ReadCellAddress("H88");
					int row = rowcol[0];
					int column = rowcol[1];
					string fillText = warmwasser.ToString();
					worksheet.Cells[row, column] = fillText;
				}
				Marshal.ReleaseComObject(worksheet);
			}
			string date = DateTime.Now.ToString("dd.MM.yy HH-mm-ss");
			string directory = Path.GetDirectoryName(templatePath);
			DirectoryInfo dirInfo = Directory.CreateDirectory(Path.Combine(directory, "GTB_ExportedTemplates"));
			string fileName = Path.GetFileNameWithoutExtension(templatePath) + "_exported_" + date + ".xlsx";
			string fullExportPath = Path.Combine(dirInfo.FullName, fileName);
			excelBook.SaveAs(fullExportPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, 
								Type.Missing, Type.Missing, false, false, XlSaveAsAccessMode.xlNoChange,
									Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
			excelApp.Quit();
			return result;
		}

		private static bool TestTheSheet(Worksheet worksheet)
        {
			bool result = false;
				try
				{
					string value1 = (string)worksheet.Cells[174, 3].Value;
					if (value1 != null)
					{
						if (value1.Contains("RJ45")) result = true;												
					}
					return result;
				}
				catch (Exception)
				{
					return result;
				}			
		}

		private static List<int> ReadCellAddress(string address)
        {
			List<int> result = new List<int>();
			char columnLetter = address[0];
			int columnNo = char.ToUpper(columnLetter) - 64;
			string rowString = address.Substring(1);
			int rowNo = 0;
			Int32.TryParse(rowString, out rowNo);
			result.Add(rowNo);
			result.Add(columnNo);
			return result;
        }

		
	}
}
