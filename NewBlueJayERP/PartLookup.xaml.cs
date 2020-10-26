/* Title:           Parts Lookup
 * Date:            10-21-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used for looking up a part */

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
using NewPartNumbersDLL;
using NewEventLogDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.Data.SqlTypes;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for PartLookup.xaml
    /// </summary>
    public partial class PartLookup : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindPartByDescriptionKeyWordDataSet TheFindPartByDescriptionKeyWordDataSet = new FindPartByDescriptionKeyWordDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();
        FindMasterPartListPartByPartIDDataSet TheFindMasterPartListPartByPartIDDataSet = new FindMasterPartListPartByPartIDDataSet();
        PartLookupDataSet ThePartLookupDataSet = new PartLookupDataSet();

        public PartLookup()
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
            txtEnterPartInformation.Text = "";
            ThePartLookupDataSet.partlookup.Rows.Clear();

            dgrParts.ItemsSource = ThePartLookupDataSet.partlookup;
        }

        private void btnFindPart_Click(object sender, RoutedEventArgs e)
        {
            string strPartNumber;
            int intNumberOfRecords;
            int intCounter;
            int intPartID;
            bool blnFatalError = false;
            string strJDEPartNumber;
            string strPartDescription;
            int intRecordsReturned;
            string strOldPartNumber;

            try
            {
                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Part Lookup");

                if (blnFatalError == true)
                    throw new Exception();

                ThePartLookupDataSet.partlookup.Rows.Clear();

                strPartNumber = txtEnterPartInformation.Text;

                if(strPartNumber.Length < 1)
                {
                    TheMessagesClass.ErrorMessage("The Part Information Was Not Entered");
                    return;
                }

                //part number search
                TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                intNumberOfRecords = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                    strJDEPartNumber = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].JDEPartNumber;
                    strPartDescription = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartDescription;

                    TheFindMasterPartListPartByPartIDDataSet = ThePartNumberClass.FindMasterPartByPartID(intPartID);

                    strOldPartNumber = "NONE FOUND";

                    intRecordsReturned = TheFindMasterPartListPartByPartIDDataSet.FindMasterPartListPartByPartID.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        strOldPartNumber = TheFindMasterPartListPartByPartIDDataSet.FindMasterPartListPartByPartID[0].PartNumber;
                    }

                    PartLookupDataSet.partlookupRow NewPartRow = ThePartLookupDataSet.partlookup.NewpartlookupRow();

                    NewPartRow.JDEPartNumber = strJDEPartNumber;
                    NewPartRow.OldPartNumber = strOldPartNumber;
                    NewPartRow.PartDescription = strPartDescription;
                    NewPartRow.PartID = intPartID;
                    NewPartRow.PartNumber = strPartNumber;

                    ThePartLookupDataSet.partlookup.Rows.Add(NewPartRow);
                }
                else if(intNumberOfRecords < 1)
                {
                    strJDEPartNumber = strPartNumber;

                    TheFindPartByJDEPartNumberDataSet = ThePartNumberClass.FindPartByJDEPartNumber(strJDEPartNumber);

                    intNumberOfRecords = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber.Rows.Count;

                    if(intNumberOfRecords > 0)
                    {
                        strPartNumber = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartNumber;
                        intPartID = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartID;
                        strPartDescription = TheFindPartByJDEPartNumberDataSet.FindPartByJDEPartNumber[0].PartDescription;

                        TheFindMasterPartListPartByPartIDDataSet = ThePartNumberClass.FindMasterPartByPartID(intPartID);

                        strOldPartNumber = "NONE FOUND";

                        intRecordsReturned = TheFindMasterPartListPartByPartIDDataSet.FindMasterPartListPartByPartID.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            strOldPartNumber = TheFindMasterPartListPartByPartIDDataSet.FindMasterPartListPartByPartID[0].PartNumber;
                        }

                        PartLookupDataSet.partlookupRow NewPartRow = ThePartLookupDataSet.partlookup.NewpartlookupRow();

                        NewPartRow.JDEPartNumber = strJDEPartNumber;
                        NewPartRow.OldPartNumber = strOldPartNumber;
                        NewPartRow.PartDescription = strPartDescription;
                        NewPartRow.PartID = intPartID;
                        NewPartRow.PartNumber = strPartNumber;

                        ThePartLookupDataSet.partlookup.Rows.Add(NewPartRow);
                    }
                    else if(intNumberOfRecords < 1)
                    {
                        strPartDescription = strPartNumber;

                        TheFindPartByDescriptionKeyWordDataSet = ThePartNumberClass.FindPartByDescriptionKeyWord(strPartDescription);

                        intNumberOfRecords = TheFindPartByDescriptionKeyWordDataSet.FindPartByDescriptionKeyWord.Rows.Count;

                        if(intNumberOfRecords < 1)
                        {
                            TheMessagesClass.ErrorMessage("The Part Was Not Found");
                            return;
                        }
                        else if(intNumberOfRecords > 1)
                        {
                            for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                            {
                                intPartID = TheFindPartByDescriptionKeyWordDataSet.FindPartByDescriptionKeyWord[intCounter].PartID;
                                strPartNumber = TheFindPartByDescriptionKeyWordDataSet.FindPartByDescriptionKeyWord[intCounter].PartNumber;
                                strJDEPartNumber = TheFindPartByDescriptionKeyWordDataSet.FindPartByDescriptionKeyWord[intCounter].JDEPartNumber;
                                strPartDescription = TheFindPartByDescriptionKeyWordDataSet.FindPartByDescriptionKeyWord[intCounter].PartDescription;

                                TheFindMasterPartListPartByPartIDDataSet = ThePartNumberClass.FindMasterPartByPartID(intPartID);

                                strOldPartNumber = "NONE FOUND";

                                intRecordsReturned = TheFindMasterPartListPartByPartIDDataSet.FindMasterPartListPartByPartID.Rows.Count;

                                if (intRecordsReturned > 0)
                                {
                                    strOldPartNumber = TheFindMasterPartListPartByPartIDDataSet.FindMasterPartListPartByPartID[0].PartNumber;
                                }

                                PartLookupDataSet.partlookupRow NewPartRow = ThePartLookupDataSet.partlookup.NewpartlookupRow();

                                NewPartRow.JDEPartNumber = strJDEPartNumber;
                                NewPartRow.OldPartNumber = strOldPartNumber;
                                NewPartRow.PartDescription = strPartDescription;
                                NewPartRow.PartID = intPartID;
                                NewPartRow.PartNumber = strPartNumber;

                                ThePartLookupDataSet.partlookup.Rows.Add(NewPartRow);
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Part Lookup // Find Part Button " + Ex.Message);

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
                intRowNumberOfRecords = ThePartLookupDataSet.partlookup.Rows.Count;
                intColumnNumberOfRecords = ThePartLookupDataSet.partlookup.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = ThePartLookupDataSet.partlookup.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = ThePartLookupDataSet.partlookup.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Part Lookup // Export To Excel " + ex.Message);

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
