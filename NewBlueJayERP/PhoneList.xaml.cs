﻿/* Title:           Phone List
 * Date:            4-5-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used for the phone list */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PhonesDLL;
using NewEventLogDLL;
using NewEmployeeDLL;
using DataValidationDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for PhoneList.xaml
    /// </summary>
    public partial class PhoneList : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        FindSortedPhoneListOnlyDataSet TheFindSortedPhoneListOnlyDataSet = new FindSortedPhoneListOnlyDataSet();
        FindCellPhoneByEmployeeIDDataSet TheFindCellPhoneByEmployeeIDDataSet = new FindCellPhoneByEmployeeIDDataSet();
        CompletePhoneListDataSet TheCompletePhoneListDataSet = new CompletePhoneListDataSet();
        
        public PhoneList()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = false;
            TheMessagesClass.LaunchEmail();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessagesClass.LaunchHelpSite();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            string strCellNumber = "";
            string strPhoneExtension;
            string strFirstName;
            string strLastName;
            string strHomeOffice;
            string strDirectNumber;
            int intEmployeeID;

            try
            {
                TheCompletePhoneListDataSet.completephonelist.Rows.Clear();

                TheFindSortedPhoneListOnlyDataSet = ThePhonesClass.FindSortedPhoneListOnly();

                intNumberOfRecords = TheFindSortedPhoneListOnlyDataSet.FindSortedPhoneListOnly.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    strPhoneExtension = Convert.ToString(TheFindSortedPhoneListOnlyDataSet.FindSortedPhoneListOnly[intCounter].Extension);
                    strFirstName = TheFindSortedPhoneListOnlyDataSet.FindSortedPhoneListOnly[intCounter].FirstName;
                    strLastName = TheFindSortedPhoneListOnlyDataSet.FindSortedPhoneListOnly[intCounter].LastName;
                    strHomeOffice = TheFindSortedPhoneListOnlyDataSet.FindSortedPhoneListOnly[intCounter].HomeOffice;
                    strDirectNumber = TheFindSortedPhoneListOnlyDataSet.FindSortedPhoneListOnly[intCounter].DirectNumber;
                    intEmployeeID = TheFindSortedPhoneListOnlyDataSet.FindSortedPhoneListOnly[intCounter].EmployeeID;

                    TheFindCellPhoneByEmployeeIDDataSet = ThePhonesClass.FindCellPhoneByEmployeeID(intEmployeeID);

                    intRecordsReturned = TheFindCellPhoneByEmployeeIDDataSet.FindCellPhoneByEmployeeID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        strCellNumber = " ";
                    }
                    else if(intRecordsReturned > 0)
                    {
                        strCellNumber = TheFindCellPhoneByEmployeeIDDataSet.FindCellPhoneByEmployeeID[0].PhoneNumber;
                    }

                    if(strFirstName == "OPEN")
                    {
                        strCellNumber = " ";
                        strDirectNumber = " ";
                    }

                    if(strDirectNumber == "NONE")
                    {
                        strDirectNumber = " ";
                    }

                    CompletePhoneListDataSet.completephonelistRow NewPhoneRow = TheCompletePhoneListDataSet.completephonelist.NewcompletephonelistRow();

                    NewPhoneRow.CellNumber = strCellNumber;
                    NewPhoneRow.DirectNumber = strDirectNumber;
                    NewPhoneRow.Extension = strPhoneExtension;
                    NewPhoneRow.FirstName = strFirstName;
                    NewPhoneRow.HomeOffice = strHomeOffice;
                    NewPhoneRow.LastName = strLastName;

                    TheCompletePhoneListDataSet.completephonelist.Rows.Add(NewPhoneRow);

                }

                dgrPhoneList.ItemsSource = TheCompletePhoneListDataSet.completephonelist;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Phone List // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

       

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                expExportToExcel.IsExpanded = false;

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheCompletePhoneListDataSet.completephonelist.Rows.Count;
                intColumnNumberOfRecords = TheCompletePhoneListDataSet.completephonelist.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCompletePhoneListDataSet.completephonelist.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheCompletePhoneListDataSet.completephonelist.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Phone List// Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
    }
}
