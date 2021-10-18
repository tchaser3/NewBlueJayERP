/* Title:           Remove Duplicate Project Matrix
 * Date:            9-29-2020
 * Author:          Terry Holmes
 * 
 * Description:     This is used to remove duplicate transactions */

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
using DesignProjectsDLL;
using NewEventLogDLL;
using ProjectMatrixDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for RemoveDuplicateProjectMatrix.xaml
    /// </summary>
    public partial class RemoveDuplicateProjectMatrix : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindDuplicateProjectMatrixDataSet TheFindDuplicateProjectMatrixDataSet = new FindDuplicateProjectMatrixDataSet();
        FindProjectMatrixByCustomerAssignedIDShortDataSet TheFindProjectMatrixByCustomerAssignedIDShortDataSet = new FindProjectMatrixByCustomerAssignedIDShortDataSet();
        DuplicateProjectsDataSet TheDuplicateProjectsDataSet = new DuplicateProjectsDataSet();

        public RemoveDuplicateProjectMatrix()
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
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strCustomerAssignedID;
            int intSecondCounter;
            int intSecondNumberOfRecords;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                TheDuplicateProjectsDataSet.duplicateprojects.Rows.Clear();

                //loading up the first data set
                TheFindDuplicateProjectMatrixDataSet = TheProjectMatrixClass.FindDuplicateProjectMatrix();

                intNumberOfRecords = TheFindDuplicateProjectMatrixDataSet.FindDuplicateProjectMatrix.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strCustomerAssignedID = TheFindDuplicateProjectMatrixDataSet.FindDuplicateProjectMatrix[intCounter].CustomerAssignedID;

                        TheFindProjectMatrixByCustomerAssignedIDShortDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerAssignedIDShort(strCustomerAssignedID);

                        intSecondNumberOfRecords = TheFindProjectMatrixByCustomerAssignedIDShortDataSet.FindProjectMatrixByCustomerAssignedIDShort.Rows.Count - 1;

                        for(intSecondCounter = 0; intSecondCounter <= intSecondNumberOfRecords; intSecondCounter++)
                        {
                            DuplicateProjectsDataSet.duplicateprojectsRow NewProjectEntry = TheDuplicateProjectsDataSet.duplicateprojects.NewduplicateprojectsRow();

                            NewProjectEntry.AssignedOffice = TheFindProjectMatrixByCustomerAssignedIDShortDataSet.FindProjectMatrixByCustomerAssignedIDShort[intSecondCounter].AssignedOffice;
                            NewProjectEntry.AssignedProjectID = TheFindProjectMatrixByCustomerAssignedIDShortDataSet.FindProjectMatrixByCustomerAssignedIDShort[intSecondCounter].AssignedProjectID;
                            NewProjectEntry.CustomerProjectID = TheFindProjectMatrixByCustomerAssignedIDShortDataSet.FindProjectMatrixByCustomerAssignedIDShort[intSecondCounter].CustomerAssignedID;
                            NewProjectEntry.ProjectID = TheFindProjectMatrixByCustomerAssignedIDShortDataSet.FindProjectMatrixByCustomerAssignedIDShort[intSecondCounter].ProjectID;
                            NewProjectEntry.RemoveProject = false;
                            NewProjectEntry.TransactionDate = TheFindProjectMatrixByCustomerAssignedIDShortDataSet.FindProjectMatrixByCustomerAssignedIDShort[intSecondCounter].TransactionDate;
                            NewProjectEntry.TransactionID = TheFindProjectMatrixByCustomerAssignedIDShortDataSet.FindProjectMatrixByCustomerAssignedIDShort[intSecondCounter].TransactionID;

                            TheDuplicateProjectsDataSet.duplicateprojects.Rows.Add(NewProjectEntry);
                        }
                    }
                }

                dgrDuplicateProjects.ItemsSource = TheDuplicateProjectsDataSet.duplicateprojects;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Remove Duplicate Project Matrix // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            int intTransactionID;

            try
            {
                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Remove Duplicate Project Matrix");

                if (blnFatalError == true)
                    throw new Exception();

                expProcess.IsExpanded = false;

                intNumberOfRecords = TheDuplicateProjectsDataSet.duplicateprojects.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheDuplicateProjectsDataSet.duplicateprojects[intCounter].RemoveProject == true)
                    {
                        intTransactionID = TheDuplicateProjectsDataSet.duplicateprojects[intCounter].TransactionID;

                        blnFatalError = TheProjectMatrixClass.RemoveDuplicateProjectMatrixTransaction(intTransactionID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("The Projects Have Been Removed");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Remove Duplicate Project Matrix // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
