/* Title:           Update Selected Project
 * Date:            1-15-2021
 * Author:          Terry Holmes
 * 
 * Description:     This is used to update a project from a dashboard */

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
using ProductionProjectDLL;
using ProjectMatrixDLL;
using EmployeeDateEntryDLL;
using ProductionProjectUpdatesDLL;
using ProjectsDLL;


namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for UpdateSelectedProject.xaml
    /// </summary>
    public partial class UpdateSelectedProject : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProductionProjectUpdatesClass TheProductionProjectUpdatesClass = new ProductionProjectUpdatesClass();

        //setting up the data
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindProductionProjectByProjectIDDataSet TheFindProductionProjectByProjectIDDataSet = new FindProductionProjectByProjectIDDataSet();
        FindProductionProjectUpdateByProjectIDDataSet TheFindProductionProjectUpdateByProjectIDDataSet = new FindProductionProjectUpdateByProjectIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();

        //setting up global variables
        int gintStatusID;

        public UpdateSelectedProject()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strUpdate = "";
            int intStatusID;

            try
            {
                TheFindProjectMatrixByProjectProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(MainWindow.gintProjectID);
                TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(MainWindow.gintProjectID);
                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(MainWindow.gintProjectID);

                txtAssignedProjectID.Text = TheFindProjectMatrixByProjectProjectIDDataSet.FindProjectMatrixByProjectID[0].AssignedProjectID;
                txtCustomerProjectID.Text = TheFindProjectMatrixByProjectProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;
                txtProjectName.Text = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;
                intStatusID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].CurrentStatusID;

                TheFindProductionProjectUpdateByProjectIDDataSet = TheProductionProjectUpdatesClass.FindProductionProjectUpdateByProjectID(MainWindow.gintProjectID);

                intNumberOfRecords = TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strUpdate += Convert.ToString(TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intCounter].TransactionDate) + " - ";
                        strUpdate += TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intCounter].ProjectUpdate + "\n\n";
                    }
                }

                txtProjectUpdates.Text = strUpdate;

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
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Selected Project // Window Loaded Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
                if (strProjectUpdate.Length < 15)
                {
                    TheMessagesClass.ErrorMessage("The Update is not Long Enough");
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

                TheMessagesClass.InformationMessage("The Project Has Been Updated");

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Updates Project " + strAssignedProjectID);

                if (blnFatalError == true)
                    throw new Exception();

                this.Close();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Update Selected Project // Process Update Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
