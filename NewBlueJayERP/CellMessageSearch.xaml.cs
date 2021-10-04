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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using DataValidationDLL;
using NewEventLogDLL;
using CellPhoneCallsDLL;
using PhonesDLL;
using EmployeeDateEntryDLL;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CellMessageSearch.xaml
    /// </summary>
    public partial class CellMessageSearch : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CellPhoneCallsClass TheCellPhoneCallsClass = new CellPhoneCallsClass();
        PhonesClass ThePhonesClass = new PhonesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindCellPhoneMessagesForEmployeeDataSet TheFindCellPhoneMesagesForEmployeeDataSet = new FindCellPhoneMessagesForEmployeeDataSet();
        FindCellPhoneMessagesForNumberDataSet TheFindCellPhoneMessagesForNumberDataSet = new FindCellPhoneMessagesForNumberDataSet();
        MessagesForEmployeeDataSet TheMessagesForEmployeeDataSet = new MessagesForEmployeeDataSet();
        FindCellPhoneByLastFourDataSet TheFindCellPhoneByLastForDataSet = new FindCellPhoneByLastFourDataSet();
        FindEmployeeByLastFourPhoneDigitsDataSet TheFindEmployeeBylastFourPhoneDigitsDataSet = new FindEmployeeByLastFourPhoneDigitsDataSet();

        string gstrTransactionNumber;
        int gintEmployeeID;
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        string gstrFullName;

        public CellMessageSearch()
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
            //setting up the controls
            txtEndDate.Text = "";
            txtStartDate.Text = "";
            txtEnterInfo.Text = "";

            cboReportType.Items.Clear();
            cboReportType.Items.Add("Select Report Type");
            cboReportType.Items.Add("Number Search");
            cboReportType.Items.Add("Employee Search");
            cboReportType.SelectedIndex = 0;

            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            cboSelectEmployee.SelectedIndex = 0;
            cboSelectEmployee.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;

            lblEnterInfo.Content = "Select Report Type";
            txtEndDate.IsReadOnly = true;
            txtStartDate.IsReadOnly = true;
            txtEnterInfo.IsReadOnly = true;

            TheMessagesForEmployeeDataSet.messagesforemployee.Rows.Clear();

            dgrCellMessages.ItemsSource = TheMessagesForEmployeeDataSet.messagesforemployee;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Cell Messages Search");
        }

        private void cboReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboReportType.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                txtEndDate.IsReadOnly = false;
                txtEnterInfo.IsReadOnly = false;
                txtStartDate.IsReadOnly = false;
                ClearTextBoxes();

                if(intSelectedIndex == 1)
                {
                    lblEnterInfo.Content = "Enter Last Four Numbers";
                    lblSelectEmployee.Visibility = Visibility.Hidden;
                    cboSelectEmployee.Visibility = Visibility.Hidden;
                }
                else if(intSelectedIndex == 2)
                {
                    lblEnterInfo.Content = "Enter Last Name";
                    lblSelectEmployee.Visibility = Visibility.Visible;
                    cboSelectEmployee.Visibility = Visibility.Visible;
                }
            }
        }
        private void ClearTextBoxes()
        {
            txtEndDate.Text = "";
            txtEnterInfo.Text = "";
            txtStartDate.Text = "";
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            cboSelectEmployee.SelectedIndex = 0;
        }
        private void txtEnterInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strEnterInfo;
            int intCounter;
            int intNumberOfRecords;

            if(cboReportType.SelectedIndex == 1)
            {
                gstrTransactionNumber = txtEnterInfo.Text;

                if(gstrTransactionNumber.Length > 4)
                {
                    TheMessagesClass.ErrorMessage("To Many Digits");
                    return;
                }
            }
            if(cboReportType.SelectedIndex == 2)
            {
                strEnterInfo = txtEnterInfo.Text;

                if(strEnterInfo.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strEnterInfo);

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                gstrFullName = TheComboEmployeeDataSet.employees[intSelectedIndex].FullName;
            }
        }

        private void expFindMessages_Expanded(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            int intCounter;
            int intNumberOfRecords;
            string strLastFour;
            string strRespondent;
            DateTime datTransactionDate;
            string strPhoneNumber;
            string strTransactionNumber;
            string strMessageDirection;
            string strMessageType;
            int intRecordsReturned;
            bool blnItemFound;

            try
            {
                expFindMessages.IsExpanded = false;

                if (cboReportType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Report Type Was Not Selected\n";
                }
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date Is Not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEndDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date Is Not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
                }
                if(cboReportType.SelectedIndex == 1)
                {
                    if(gstrTransactionNumber.Length != 4)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Number Of Digits is not Correct\n";
                    }
                }
                else if(cboReportType.SelectedIndex == 2)
                {
                    if(cboSelectEmployee.SelectedIndex < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Employee Was Not Selected\n";
                    }
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The Start Date is after the End Date");
                        return;
                    }
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();


                TheMessagesForEmployeeDataSet.messagesforemployee.Rows.Clear();

                if (cboReportType.SelectedIndex == 1)
                {
                    TheFindCellPhoneMessagesForNumberDataSet = TheCellPhoneCallsClass.FindCellPhoneMessagesForNumber(gstrTransactionNumber, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindCellPhoneMessagesForNumberDataSet.FindCellPhoneMessagesForNumber.Rows.Count;

                    if(intNumberOfRecords > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            blnItemFound = false;
                            strRespondent = "UNKNOWN";

                            datTransactionDate = TheFindCellPhoneMessagesForNumberDataSet.FindCellPhoneMessagesForNumber[intCounter].TransactionDate;
                            gstrFullName = TheFindCellPhoneMessagesForNumberDataSet.FindCellPhoneMessagesForNumber[intCounter].FirstName + " ";
                            gstrFullName += TheFindCellPhoneMessagesForNumberDataSet.FindCellPhoneMessagesForNumber[intCounter].LastName;
                            strPhoneNumber = TheFindCellPhoneMessagesForNumberDataSet.FindCellPhoneMessagesForNumber[intCounter].PhoneNumber;
                            strTransactionNumber = TheFindCellPhoneMessagesForNumberDataSet.FindCellPhoneMessagesForNumber[intCounter].TransactionNumber;
                            strLastFour = strTransactionNumber.Substring(8, 4);
                            strMessageDirection = TheFindCellPhoneMessagesForNumberDataSet.FindCellPhoneMessagesForNumber[intCounter].MessageDirection;
                            strMessageType = TheFindCellPhoneMessagesForNumberDataSet.FindCellPhoneMessagesForNumber[intCounter].MessageType;

                            TheFindCellPhoneByLastForDataSet = ThePhonesClass.FindCellPhoneByLastFour(strLastFour);

                            intRecordsReturned = TheFindCellPhoneByLastForDataSet.FindCellPhoneByLastFour.Rows.Count;

                            if (intRecordsReturned > 0)
                            {
                                strRespondent = TheFindCellPhoneByLastForDataSet.FindCellPhoneByLastFour[0].FirstName + " ";
                                strRespondent += TheFindCellPhoneByLastForDataSet.FindCellPhoneByLastFour[0].LastName;
                                blnItemFound = true;                                
                            }
                            if (blnItemFound == false)
                            {
                                TheFindEmployeeBylastFourPhoneDigitsDataSet = TheEmployeeClass.FindEmployeeByLastFourPhoneDigits(strLastFour);

                                intRecordsReturned = TheFindEmployeeBylastFourPhoneDigitsDataSet.FindEmployeeByLastFourPhoneDigits.Rows.Count;

                                if (intRecordsReturned > 0)
                                {
                                    if(strTransactionNumber == TheFindEmployeeBylastFourPhoneDigitsDataSet.FindEmployeeByLastFourPhoneDigits[0].PhoneNumber)
                                    {
                                        strRespondent = TheFindEmployeeBylastFourPhoneDigitsDataSet.FindEmployeeByLastFourPhoneDigits[0].FirstName + " ";
                                        strRespondent += TheFindEmployeeBylastFourPhoneDigitsDataSet.FindEmployeeByLastFourPhoneDigits[0].LastName;
                                    }                                    
                                }
                            }

                            MessagesForEmployeeDataSet.messagesforemployeeRow NewMessageRow = TheMessagesForEmployeeDataSet.messagesforemployee.NewmessagesforemployeeRow();

                            NewMessageRow.Employee = gstrFullName;
                            NewMessageRow.MessageDirection = strMessageDirection;
                            NewMessageRow.MessageType = strMessageType;
                            NewMessageRow.PhoneNumber = strPhoneNumber;
                            NewMessageRow.Respondent = strRespondent;
                            NewMessageRow.TransactionDate = datTransactionDate;
                            NewMessageRow.TransactionNumber = strTransactionNumber;

                            TheMessagesForEmployeeDataSet.messagesforemployee.Rows.Add(NewMessageRow);
                        }
                    }
                }
                else if(cboReportType.SelectedIndex == 2)
                {                    
                    TheFindCellPhoneMesagesForEmployeeDataSet = TheCellPhoneCallsClass.FindCellPhoneMessagesForEmployee(gintEmployeeID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindCellPhoneMesagesForEmployeeDataSet.FindCellPhoneMessagesForEmployee.Rows.Count;

                    if(intNumberOfRecords > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            blnItemFound = false;
                            strRespondent = "UNKNOWN";
                            datTransactionDate = TheFindCellPhoneMesagesForEmployeeDataSet.FindCellPhoneMessagesForEmployee[intCounter].TransactionDate;
                            strPhoneNumber = TheFindCellPhoneMesagesForEmployeeDataSet.FindCellPhoneMessagesForEmployee[intCounter].PhoneNumber;
                            strTransactionNumber = TheFindCellPhoneMesagesForEmployeeDataSet.FindCellPhoneMessagesForEmployee[intCounter].TransactionNumber;
                            strLastFour = strTransactionNumber.Substring(8, 4);
                            strMessageDirection = TheFindCellPhoneMesagesForEmployeeDataSet.FindCellPhoneMessagesForEmployee[intCounter].MessageDirection;
                            strMessageType = TheFindCellPhoneMesagesForEmployeeDataSet.FindCellPhoneMessagesForEmployee[intCounter].MessageType;

                            TheFindCellPhoneByLastForDataSet = ThePhonesClass.FindCellPhoneByLastFour(strLastFour);

                            intRecordsReturned = TheFindCellPhoneByLastForDataSet.FindCellPhoneByLastFour.Rows.Count;

                            if(intRecordsReturned > 0)
                            {
                                strRespondent = TheFindCellPhoneByLastForDataSet.FindCellPhoneByLastFour[0].FirstName + " ";
                                strRespondent += TheFindCellPhoneByLastForDataSet.FindCellPhoneByLastFour[0].LastName;
                                blnItemFound = true;
                                
                            }
                            if(blnItemFound == false)
                            {
                                TheFindEmployeeBylastFourPhoneDigitsDataSet = TheEmployeeClass.FindEmployeeByLastFourPhoneDigits(strLastFour);

                                intRecordsReturned = TheFindEmployeeBylastFourPhoneDigitsDataSet.FindEmployeeByLastFourPhoneDigits.Rows.Count;

                                if (intRecordsReturned > 0)
                                {
                                    if (strTransactionNumber == TheFindEmployeeBylastFourPhoneDigitsDataSet.FindEmployeeByLastFourPhoneDigits[0].PhoneNumber)
                                    {
                                        strRespondent = TheFindEmployeeBylastFourPhoneDigitsDataSet.FindEmployeeByLastFourPhoneDigits[0].FirstName + " ";
                                        strRespondent += TheFindEmployeeBylastFourPhoneDigitsDataSet.FindEmployeeByLastFourPhoneDigits[0].LastName;
                                    }
                                }
                            }

                            MessagesForEmployeeDataSet.messagesforemployeeRow NewMessageRow = TheMessagesForEmployeeDataSet.messagesforemployee.NewmessagesforemployeeRow();

                            NewMessageRow.Employee = gstrFullName;
                            NewMessageRow.MessageDirection = strMessageDirection;
                            NewMessageRow.MessageType = strMessageType;
                            NewMessageRow.PhoneNumber = strPhoneNumber;
                            NewMessageRow.Respondent = strRespondent;
                            NewMessageRow.TransactionDate = datTransactionDate;
                            NewMessageRow.TransactionNumber = strTransactionNumber;
                            
                            TheMessagesForEmployeeDataSet.messagesforemployee.Rows.Add(NewMessageRow);
                        }
                    }                    
                }

                dgrCellMessages.ItemsSource = TheMessagesForEmployeeDataSet.messagesforemployee;

                PleaseWait.Close();
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Cell Message Search // Find Messages Expander " + Ex.Message);

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

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheMessagesForEmployeeDataSet.messagesforemployee.Rows.Count;
                intColumnNumberOfRecords = TheMessagesForEmployeeDataSet.messagesforemployee.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheMessagesForEmployeeDataSet.messagesforemployee.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheMessagesForEmployeeDataSet.messagesforemployee.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Cell Messages Search // Export Employees " + ex.Message);

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
