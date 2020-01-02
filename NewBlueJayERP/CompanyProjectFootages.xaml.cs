/* Title:           Company Project Footages
 * Date:            1-2-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to find company footages */

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
using ProjectTaskDLL;
using DataValidationDLL;
using NewEventLogDLL;
using Microsoft.Win32;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CompanyProjectFootages.xaml
    /// </summary>
    public partial class CompanyProjectFootages : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        ProjectFootagesDataSet TheProjectFootagesDataSet = new ProjectFootagesDataSet();
        FindCompanyFootagesDataSet TheFindCompanyFootagesDataSet = new FindCompanyFootagesDataSet();
        TotalWorkTaskFootagesDataSet TheTotalWorkTaskFootages = new TotalWorkTaskFootagesDataSet();

        //global variables
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        int gintCounter;
        int gintNumberOfRecords;

        public CompanyProjectFootages()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            int intCounter;
            int intNumberOfRecords;
            string strAssignedProjectID;
            string strProjectName;
            string strWorkTask;
            int intPiecesFootage;
            bool blnItemFound;
            int intSecondCounter;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                strValueForValidation = txtStartDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Date is not a Date\n";
                }
                else
                {
                    gdatStartDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtEnddate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Date is not a Date\n";
                }
                else
                {
                    gdatEndDate = Convert.ToDateTime(strValueForValidation);
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

                TheFindCompanyFootagesDataSet = TheProjectTaskClass.FindCompanyFootages(gdatStartDate, gdatEndDate);

                //getting ready for the loop
                TheProjectFootagesDataSet.projectfootages.Rows.Clear();
                gintCounter = 0;
                gintNumberOfRecords = 0;

                intNumberOfRecords = TheFindCompanyFootagesDataSet.FindCompanyFootages.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strProjectName = TheFindCompanyFootagesDataSet.FindCompanyFootages[intCounter].ProjectName;
                        strAssignedProjectID = TheFindCompanyFootagesDataSet.FindCompanyFootages[intCounter].AssignedProjectID;
                        strWorkTask = TheFindCompanyFootagesDataSet.FindCompanyFootages[intCounter].WorkTask;
                        intPiecesFootage = Convert.ToInt32(TheFindCompanyFootagesDataSet.FindCompanyFootages[intCounter].FootagePieces);
                        blnItemFound = false;

                        if(gintCounter > 0)
                        {
                            for (intSecondCounter = 0; intSecondCounter <= gintNumberOfRecords; intSecondCounter++)
                            {
                                if (strAssignedProjectID == TheProjectFootagesDataSet.projectfootages[intSecondCounter].AssignedProjectID)
                                {
                                    if(strWorkTask == TheProjectFootagesDataSet.projectfootages[intSecondCounter].WorkTask)
                                    {
                                        TheProjectFootagesDataSet.projectfootages[intSecondCounter].FootagePieces += intPiecesFootage;
                                        blnItemFound = true;
                                    }
                                }
                            }
                        }

                        if(blnItemFound == false)
                        {
                            ProjectFootagesDataSet.projectfootagesRow NewProjectRow = TheProjectFootagesDataSet.projectfootages.NewprojectfootagesRow();

                            NewProjectRow.AssignedProjectID = strAssignedProjectID;
                            NewProjectRow.ProjectName = strProjectName;
                            NewProjectRow.FootagePieces = intPiecesFootage;
                            NewProjectRow.WorkTask = strWorkTask;

                            TheProjectFootagesDataSet.projectfootages.Rows.Add(NewProjectRow);

                            gintNumberOfRecords = gintCounter;
                            gintCounter++;
                        }
                    }
                }

                intNumberOfRecords = TheProjectFootagesDataSet.projectfootages.Rows.Count - 1;
                gintCounter = 0;
                gintNumberOfRecords = 0;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strWorkTask = TheProjectFootagesDataSet.projectfootages[intCounter].WorkTask;
                        intPiecesFootage = TheProjectFootagesDataSet.projectfootages[intCounter].FootagePieces;
                        blnItemFound = false;

                        if(gintCounter > 0)
                        {
                            for(intSecondCounter = 0; intSecondCounter <= gintNumberOfRecords; intSecondCounter++)
                            {
                                if(strWorkTask == TheTotalWorkTaskFootages.totalworktaskfootages[intSecondCounter].WorkTask)
                                {
                                    TheTotalWorkTaskFootages.totalworktaskfootages[intSecondCounter].FootagePieces += intPiecesFootage;
                                    blnItemFound = true;
                                }
                            }
                        }

                        if (blnItemFound == false)
                        {
                            TotalWorkTaskFootagesDataSet.totalworktaskfootagesRow NewProjectRow = TheTotalWorkTaskFootages.totalworktaskfootages.NewtotalworktaskfootagesRow();

                            NewProjectRow.WorkTask = strWorkTask;
                            NewProjectRow.FootagePieces = intPiecesFootage;

                            TheTotalWorkTaskFootages.totalworktaskfootages.Rows.Add(NewProjectRow);
                            gintNumberOfRecords = gintCounter;
                            gintCounter++;
                        }
                    }
                }



                dgrResults.ItemsSource = TheTotalWorkTaskFootages.totalworktaskfootages;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Company Project Footages // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
        private void ExportCompanyFootages()
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
                intRowNumberOfRecords = TheProjectFootagesDataSet.projectfootages.Rows.Count;
                intColumnNumberOfRecords = TheProjectFootagesDataSet.projectfootages.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectFootagesDataSet.projectfootages.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheProjectFootagesDataSet.projectfootages.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Company Project Footages // Export Company Footages " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void ExportTotalFootages()
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
                intRowNumberOfRecords = TheTotalWorkTaskFootages.totalworktaskfootages.Rows.Count;
                intColumnNumberOfRecords = TheTotalWorkTaskFootages.totalworktaskfootages.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheTotalWorkTaskFootages.totalworktaskfootages.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheTotalWorkTaskFootages.totalworktaskfootages.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Company Project Footages // Export Total Footages " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }
        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            ExportCompanyFootages();

            ExportTotalFootages();

            PleaseWait.Close();
        }
    }
}
