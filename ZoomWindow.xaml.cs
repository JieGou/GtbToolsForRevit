/*
 * Created by SharpDevelop.
 * User: m.trawczynski
 * Date: 24.08.2020
 * Time: 15:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace GtbMakros
{
	/// <summary>
	/// Interaction logic for ZoomWindow.xaml
	/// </summary>
	public partial class ZoomWindow : Window
	{
		public OpenViewsTool OpenViewsTool {get; set;}
			
		public ZoomWindow(OpenViewsTool openViewsTool)
		{
			OpenViewsTool = openViewsTool;
			InitializeComponent();
			DataContext = this;
			Topmost = true;
		}
		
//		public void FilterList(string filter)
//		{
//			List<ModelView> filteredList = RevitTools.FilterViewList(ModelViewList, filter);
//			ListBox.ItemsSource = filteredList;
//		}
		
		void button1_Click(object sender, RoutedEventArgs e)
		{
			
		}
		
		void button2_Click(object sender, RoutedEventArgs e)
		{
			OpenViewsTool.WindowResult = WindowResult.UserApply;
			Close();
		}
		
		void button3_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show(filterBox.Text);
//			FilterList(filterBox.Text);
		}
	}
}