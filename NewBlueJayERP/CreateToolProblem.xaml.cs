/* Title:           Create Tool Problem
 * Date:            11-23-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to create a Tool Problem */

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
using NewEventLogDLL;
using NewEmployeeDLL;
using NewToolsDLL;
using ToolProblemDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateToolProblem.xaml
    /// </summary>
    public partial class CreateToolProblem : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ToolsClass TheToolClass = new ToolsClass();
        ToolProblemClass TheToolProblemClass = new ToolProblemClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        FindActiveToolByToolIDDataSet TheFindActiveToolByToolIDDataSet = new FindActiveToolByToolIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindToolProblemByDateMatchDataSet TheFindToolProblemByDateMatchDataSet = new FindToolProblemByDateMatchDataSet();
        ImportDocumentsDataSet TheImportDocumentsDataSet = new ImportDocumentsDataSet();

        //setting global variables
        string gstrDocumentType;
        bool gblnIsRepairable;
        bool gblnIsClosed;
        bool gblnDeactivateTool;
        int gintTransactionID;
        string gstrToolID;
        string gstrWarehouseStatement;

        public CreateToolProblem()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
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
            bool blnFatalError = false;

            try
            {
                txtCost.Text = "";
                txtDescription.Text = "";
                txtEnterToolID.Text = "";
                txtInspectionStatement.Text = "";
                txtTechLastName.Text = "";

                cboClosed.Items.Clear();
                cboClosed.Items.Add("Select");
                cboClosed.Items.Add("Yes");
                cboClosed.Items.Add("No");
                cboClosed.SelectedIndex = 0;

                cboSelectTech.Items.Clear();
                cboSelectTech.Items.Add("Select Employee");

                cboRepariable.Items.Clear();
                cboRepariable.Items.Add("Select");
                cboRepariable.Items.Add("Yes");
                cboRepariable.Items.Add("No");
                cboRepariable.SelectedIndex = 0;

                TheImportDocumentsDataSet.documents.Rows.Clear();
                dgrDocuments.ItemsSource = TheImportDocumentsDataSet.documents;
                expProcessTool.IsEnabled = false;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Create Tool Problem ");

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Tool Problem // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void cboRepariable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboRepariable.SelectedIndex == 1)
                gblnIsRepairable = true;
            else if (cboRepariable.SelectedIndex == 2)
            {
                gblnIsRepairable = false;

                MessageBoxResult result = MessageBox.Show("Is Tool Being Retired", "Thank You", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    gblnDeactivateTool = true;
                }
                else
                {
                    gblnDeactivateTool = false;
                }
            }
        }

        private void cboClosed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboClosed.SelectedIndex == 1)
                gblnIsClosed = true;
            else if (cboClosed.SelectedIndex == 2)
                gblnIsClosed = false;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting the local varialbes
            int intRecordsReturned;

            gstrToolID = txtEnterToolID.Text;

            //getting the tool information
            TheFindActiveToolByToolIDDataSet = TheToolClass.FindActiveToolByToolID(gstrToolID);

            intRecordsReturned = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID.Rows.Count;

            TheImportDocumentsDataSet.documents.Rows.Clear();

            dgrDocuments.ItemsSource = TheImportDocumentsDataSet.documents;

            if (intRecordsReturned == 0)
            {
                TheMessagesClass.ErrorMessage("The Tool Was Not Found");
                return;
            }
            else if (intRecordsReturned > 0)
            {
                txtCost.Text = Convert.ToString(TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolCost);
                txtDescription.Text = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolDescription;
                MainWindow.gintToolKey = TheFindActiveToolByToolIDDataSet.FindActiveToolByToolID[0].ToolKey;
            }
        }

        private void txtTechLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting varialbes
            int intCounter;
            int intNumberOfRecords;
            string strLastName;
            int intLength;

            try
            {
                strLastName = txtTechLastName.Text;

                intLength = strLastName.Length;

                if (intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;
                    cboSelectTech.Items.Clear();
                    cboSelectTech.Items.Add("Select Employee");

                    if (intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectTech.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectTech.SelectedIndex = 0;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Tool Problem // Last Name Text Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectTech_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectTech.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Tool Problems // Combo Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnAttachDocuments_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            bool blnFatalError;
            string strDocumentPath = "";
            string strDocumentType;
            bool blnRightFormat = false;

            try
            {
                TheMessagesClass.InformationMessage("Files Attached must Either PDFs or JPGs");

                blnFatalError = PerformDataValidation();

                if (blnFatalError == true)
                {
                    return;
                }

                strDocumentType = "Document";

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    strDocumentPath = dlg.FileName.ToUpper();
                }
                else
                {
                    return;
                }

                if (strDocumentPath.Contains("JPG"))
                {
                    if (gstrDocumentType == "PICTURE")
                    {
                        blnRightFormat = true;
                    }
                }
                else if (strDocumentPath.Contains("PDF"))
                {
                    if (gstrDocumentType == "DOCUMENT")
                    {
                        blnRightFormat = true;
                    }
                }
                else
                {
                    blnRightFormat = false;
                    TheMessagesClass.ErrorMessage("Documents have to be either Pictures or PDg");
                }

                ImportDocumentsDataSet.documentsRow NewDocumentRow = TheImportDocumentsDataSet.documents.NewdocumentsRow();

                NewDocumentRow.DocumentPath = strDocumentPath;
                NewDocumentRow.DocumentType = strDocumentType;

                TheImportDocumentsDataSet.documents.Rows.Add(NewDocumentRow);

                dgrDocuments.ItemsSource = TheImportDocumentsDataSet.documents;
                expProcessTool.IsEnabled = true;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Tool Problem // Attach Document Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private bool PerformDataValidation()
        {
            bool blnFatalError = false;
            string strToolID = "";
            int intSelectedIndex = 0;
            string strErrorMessage = "";

            strToolID = txtEnterToolID.Text;
            if (strToolID == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Tool ID Has Not Been Entered\n";
            }
            intSelectedIndex = cboSelectTech.SelectedIndex;
            if (intSelectedIndex < 1)
            {
                blnFatalError = true;
                strErrorMessage += "Employee Was Not Selected\n";
            }
            gstrWarehouseStatement = txtInspectionStatement.Text;
            if (gstrWarehouseStatement == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Warehouse Statement Has Not Been Added\n";
            }
            intSelectedIndex = cboRepariable.SelectedIndex;
            if (intSelectedIndex < 1)
            {
                blnFatalError = true;
                strErrorMessage += "Repairable Was Not Selected\n";
            }
            intSelectedIndex = cboClosed.SelectedIndex;
            if (intSelectedIndex < 1)
            {
                blnFatalError = true;
                strErrorMessage += "Closed Was not Selected\n";
            }
            if (blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
            }

            return blnFatalError;
        }

        private void expProcessTool_Expanded(object sender, RoutedEventArgs e)
        {
            DateTime datTransactionDate;
            DateTime datRecordDate;
            long intResult;
            string strTransactionName;
            string strNewLocation = "";
            int intCounter;
            int intNumberOfRecords;
            string strDocumentType;
            string strDocumentPath;
            bool blnFatalError = false;

            try
            {
                blnFatalError = PerformDataValidation();

                if (dgrDocuments.Items.Count == 0)
                {
                    TheMessagesClass.ErrorMessage("There Are No Documents Assigned to the Problem");
                    return;
                }

                //loading the problem
                datRecordDate = DateTime.Now;

                blnFatalError = TheToolProblemClass.InsertToolProblem(MainWindow.gintToolKey, datRecordDate, MainWindow.gintEmployeeID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, gstrWarehouseStatement, gblnIsRepairable, gblnIsClosed);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindToolProblemByDateMatchDataSet = TheToolProblemClass.FindToolProblemByDateMatch(datRecordDate);

                MainWindow.gintProblemID = TheFindToolProblemByDateMatchDataSet.FindToolProblemByDateMatch[0].ProblemID;

                blnFatalError = TheToolProblemClass.InsertToolProblemUpdate(MainWindow.gintProblemID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, gstrWarehouseStatement);

                if (blnFatalError == true)
                    throw new Exception();

                intNumberOfRecords = TheImportDocumentsDataSet.documents.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strDocumentType = TheImportDocumentsDataSet.documents[intCounter].DocumentType.ToUpper();
                    strDocumentPath = TheImportDocumentsDataSet.documents[intCounter].DocumentPath;

                    if (strDocumentType == "DOCUMENT")
                    {
                        datTransactionDate = DateTime.Now;

                        intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                        strTransactionName = Convert.ToString(intResult) + Convert.ToString(intCounter);

                        strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\ToolProblemFiles\\" + strTransactionName + ".pdf";
                    }
                    if (strDocumentType == "PICTURE")
                    {
                        datTransactionDate = DateTime.Now;

                        intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                        strTransactionName = Convert.ToString(intResult) + Convert.ToString(intCounter);

                        strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\ToolProblemFiles\\" + strTransactionName + ".jpg";
                    }

                    System.IO.File.Copy(strDocumentPath, strNewLocation);

                    blnFatalError = TheToolProblemClass.InsertToolProblemDocument(MainWindow.gintProblemID, strDocumentType, strNewLocation);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                if (gblnDeactivateTool == true)
                {
                    blnFatalError = TheToolClass.UpdateToolActive(MainWindow.gintToolKey, false);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Tool Problem Has Been Reported");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay ERP // Report Tool Problem // Process Tool " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
