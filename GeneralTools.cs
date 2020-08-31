/*
 * Created by SharpDevelop.
 * User: m.trawczynski
 * Date: 08/24/2020
 * Time: 16:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace GtbMakros
{
	/// <summary>
	/// Description of Tools.
	/// </summary>
	public class GeneralTools
	{
		
		public GeneralTools()
		{
		}
		
		public bool CreatePersonalDirectory()
		{
			bool result = true;
			string dirName = Path.Combine(@"H:\Revit\Makros\Gemeinsam genutzte Dateien", Environment.UserName);
			if(Directory.Exists(dirName) != true)
			{
				Directory.CreateDirectory(dirName);
				result = false;
			}
			return result;
		}
	}
}
