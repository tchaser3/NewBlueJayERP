/* Title:           Update Project
 * Date:            1-5-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to update a project */

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
using ProjectMatrixDLL;
using ProductionProjectDLL;
using ProductionProjectUpdatesDLL;
using EmployeeDateEntryDLL;
using ProjectsDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateProject.xaml
    /// </summary>
    public partial class UpdateProject : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessageClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        ProductionProjectUpdatesClass TheProductionProjectUpdatesClass = new ProductionProjectUpdatesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ProjectClass TheProjectClass = new ProjectClass();

        //setting up the data
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrxiByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindProductionProjectByProjectIDDataSet TheFindProductionProjectByProjectIDDataSet = new FindProductionProjectByProjectIDDataSet();
        FindProdutionProjectsByAssignedProjectIDDataSet TheFindProductionProjectByAssignedProjectIDDataSet = new FindProdutionProjectsByAssignedProjectIDDataSet();
        FindProductionProjectUpdateByProjectIDDataSet TheFindProductionProjectUpdateByProjectIDDataSet = new FindProductionProjectUpdateByProjectIDDataSet();
        FindProductionProjectInfoDataSet TheFindProductionProjectInfoDataSet = new FindProductionProjectInfoDataSet();

        bool gblnHardRestoration;
        bool gblnQCPerformed;
        bool gblnSplicingComplete;

        //setting up global variables
        int gintStatusID;

        public UpdateProject()
        {
            InitializeComponent();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessageClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            expSendEmail.IsExpanded = false;
            TheMessageClass.LaunchEmail();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expHelp.IsExpanded = false;
            TheMessageClass.LaunchHelpSite();
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessageClass.LaunchHelpDeskTickets();
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
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
        private void ClearUpdateControls()
        {
            txtAssignedProjectID.Text = "";
            txtCustomerProjectID.Text = "";
            txtEnterProjectID.Text = "";
            txtProjectUpdates.Text = "";
            txtProjectName.Text = "";
            txtUpdateNotes.Text = "";
            txtProjectNotes.Text = "";
            chkHardRestoration.IsChecked = false;
            chkQCPerformed.IsChecked = false;
            chkSplicingComplete.IsChecked = false;

            expViewDocuments.IsEnabled = false;

            EnableRadioButtons(false);

            if ((MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "ADMIN") || (MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "IT") || (MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "SUPER USER"))
            {
            
                EnableRadioButtons(true);
            }
            else if (MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "OFFICE")
            {
                rdoOpen.IsEnabled = true;
            }
            else if (MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "MANAGERS")
            {
                rdoOnHold.IsEnabled = true;
                rdoCancel.IsEnabled = true;
                rdoInProcess.IsEnabled = true;
                rdoConComplete.IsEnabled = true;
                rdoSubmitted.IsEnabled = true;
            }
        }
        private void ResetControls()
        {    
            ClearUpdateControls();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            string strCustomerProjectID;
            string strAssignedProjectID = "";
            string strProjectName;
            string strProjectUpdates = "";
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            int intStatusID;
            string strProjectNotes;
            bool blnFatalError = false;

            try
            {
                //getting project id
                strCustomerProjectID = txtEnterProjectID.Text;
                
                if(strCustomerProjectID.Length < 1)
                {
                    TheMessageClass.ErrorMessage("The Project Information Was Not Entered");
                    return;
                }

                TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

                intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                if(intRecordsReturned > 1)
                {
                    TheMessageClass.ErrorMessage("The Project Has Been Entered More Than Once, Contact Admin");
                    return;
                }
                else if(intRecordsReturned < 1)
                {
                    strAssignedProjectID = strCustomerProjectID;

                    TheFindProjectMatrxiByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectMatrxiByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessageClass.ErrorMessage("Project Not Found");
                        return;
                    }
                    else if (intRecordsReturned > 1)
                    {
                        TheMessageClass.InformationMessage("There are Multiple Projects with this Project ID, Please use the Customer Assigned ID");
                        return;
                    }
                    else if(intRecordsReturned == 1)
                    {
                        MainWindow.gintProjectID = TheFindProjectMatrxiByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;
                        strCustomerProjectID = TheFindProjectMatrxiByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].CustomerAssignedID;
                    }
                }
                else if(intRecordsReturned == 1)
                {
                    MainWindow.gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                    strAssignedProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].AssignedProjectID;
                    
                }

                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(MainWindow.gintProjectID);
                TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(MainWindow.gintProjectID);

                strProjectName = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;
                strProjectNotes = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ProjectNotes;

                intStatusID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].CurrentStatusID;

                ClearRadioButtons();

                if (intStatusID == 1001)
                {
                    rdoOpen.IsChecked = true;
                }
                else if (intStatusID == 1002)
                {
                    rdoConComplete.IsChecked = true;
                }
                else if (intStatusID == 1003)
                {
                    rdoOnHold.IsChecked = true;
                }
                else if (intStatusID == 1004)
                {
                    rdoCancel.IsChecked = true;
                }
                else if (intStatusID == 1005)
                {
                    rdoInProcess.IsChecked = true;
                }
                else if (intStatusID == 1006)
                {
                    rdoClosed.IsChecked = true;
                }
                else if (intStatusID == 1007)
                {
                    rdoInvoiced.IsChecked = true;
                }
                else if (intStatusID == 1008)
                {
                    rdoSubmitted.IsChecked = true;
                }

                txtAssignedProjectID.Text = strAssignedProjectID;
                txtCustomerProjectID.Text = strCustomerProjectID;
                txtProjectName.Text = strProjectName;
                txtProjectNotes.Text = strProjectNotes;

                TheFindProductionProjectUpdateByProjectIDDataSet = TheProductionProjectUpdatesClass.FindProductionProjectUpdateByProjectID(MainWindow.gintProjectID);

                intNumberOfRecords = TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strProjectUpdates += Convert.ToString(TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intCounter].TransactionDate) + " - ";
                        strProjectUpdates += TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intCounter].ProjectUpdate + "\n\n";
                    }
                }

                txtProjectUpdates.Text = strProjectUpdates;

                TheFindProductionProjectInfoDataSet = TheProductionProjectClass.FindProductionProjectInfo(MainWindow.gintProjectID);

                intRecordsReturned = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    blnFatalError = TheProductionProjectClass.InsertProductionProjectInfo(MainWindow.gintProjectID, 0, " ", " ", 0);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if(intRecordsReturned > 0)
                {
                    if(TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].HardRestoration == true)
                    {
                        chkHardRestoration.IsChecked = true;
                    }
                    else
                    {
                        chkHardRestoration.IsEnabled = false;
                    }
                    if(TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].QCPerformed == true)
                    {
                        chkQCPerformed.IsChecked = true;
                    }
                    else
                    {
                        chkQCPerformed.IsChecked = false;
                    }
                    if(TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].SplicingComplete == true)
                    {
                        chkSplicingComplete.IsChecked = true;
                    }
                    else
                    {
                        chkSplicingComplete.IsChecked = false;
                    }
                }

                expViewDocuments.IsEnabled = true;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Project // Find Button " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void EnableRadioButtons(bool blnValueBoolean)
        {
            rdoCancel.IsEnabled = blnValueBoolean;
            rdoClosed.IsEnabled = blnValueBoolean;
            rdoConComplete.IsEnabled = blnValueBoolean;
            rdoInProcess.IsEnabled = blnValueBoolean;
            rdoInvoiced.IsEnabled = blnValueBoolean;
            rdoOnHold.IsEnabled = blnValueBoolean;
            rdoOpen.IsEnabled = blnValueBoolean;
            rdoSubmitted.IsEnabled = blnValueBoolean;
        }
        private void ClearRadioButtons()
        {
            rdoCancel.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoConComplete.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoOpen_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1001;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoInProcess_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1005;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoCancel_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1004;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoOnHold_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1003;
            rdoConComplete.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoConComplete_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1002;
            rdoOpen.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoSubmitted_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1008;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoOpen.IsChecked = false;
        }

        private void rdoInvoiced_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1007;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoClosed.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void rdoClosed_Checked(object sender, RoutedEventArgs e)
        {
            gintStatusID = 1006;
            rdoConComplete.IsChecked = false;
            rdoOnHold.IsChecked = false;
            rdoCancel.IsChecked = false;
            rdoInProcess.IsChecked = false;
            rdoOpen.IsChecked = false;
            rdoInvoiced.IsChecked = false;
            rdoSubmitted.IsChecked = false;
        }

        private void btnProcessUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strProjectUpdate = "";
            int intTransactionID;
            string strAssignedProjectID;

            try
            {
                strProjectUpdate = txtUpdateNotes.Text;
                if(strProjectUpdate.Length < 15)
                {
                    TheMessageClass.ErrorMessage("The Update is not Long Enough");
                    return;
                }

                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(MainWindow.gintProjectID);

                intTransactionID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].TransactionID;
                strAssignedProjectID = txtCustomerProjectID.Text;

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectStatus(intTransactionID, gintStatusID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectUpdatesClass.InsertProductionProjectUpdate(MainWindow.gintProjectID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strProjectUpdate);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessageClass.InformationMessage("The Project Has Been Updated");

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Updates Project " + strAssignedProjectID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoHardRestoration(MainWindow.gintProjectID, gblnHardRestoration);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoQCPerformed(MainWindow.gintProjectID, gblnQCPerformed);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoSplicingComplete(MainWindow.gintProjectID, gblnSplicingComplete);

                if (blnFatalError == true)
                    throw new Exception();

                ClearUpdateControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Project // Process Update Button " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void chkSplicingComplete_Click(object sender, RoutedEventArgs e)
        {
            if (chkSplicingComplete.IsChecked == true)
                gblnSplicingComplete = true;
            else
                gblnSplicingComplete = false;
        }

        private void chkHardRestoration_Click(object sender, RoutedEventArgs e)
        {
            if (chkHardRestoration.IsChecked == true)
                gblnHardRestoration = true;
            else
                gblnHardRestoration = false;
        }

        private void chkQCPerformed_Click(object sender, RoutedEventArgs e)
        {
            if (chkQCPerformed.IsChecked == true)
                gblnQCPerformed = true;
            else
                gblnQCPerformed = false;
        }

        private void btnAddProjectDocumentation_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strDocumentPath;
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;

            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();

                            blnFatalError = TheProductionProjectClass.InsertProductionProjectDocumentation(MainWindow.gintProjectID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, DateTime.Now, strDocumentPath);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }
                else
                {
                    return;
                }

                TheMessageClass.InformationMessage("The Documents have been Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Project // Add Project Documentation Button " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnAddQCDocumentationj_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strDocumentPath;
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;

            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();

                            blnFatalError = TheProductionProjectClass.InsertProductionProjectQC(MainWindow.gintProjectID, DateTime.Now, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strDocumentPath);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }
                else
                {
                    return;
                }

                TheMessageClass.InformationMessage("The Documents have been Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Project // Add QC Documentation Button " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expViewDocuments_Expanded(object sender, RoutedEventArgs e)
        {
            expViewDocuments.IsExpanded = false;

            ViewProjectDocuments ViewProjectDocuments = new ViewProjectDocuments();
            ViewProjectDocuments.ShowDialog();
        }
    }
}
