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
using EmployeeDateEntryDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using DesignProductivityDLL;
using DataValidationDLL;
using ProjectsDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for VoidDesignProductivity.xaml
    /// </summary>
    public partial class VoidDesignProductivity : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DesignProductivityClass TheDesignProductivityClass = new DesignProductivityClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        ProjectClass TheProjectClass = new ProjectClass();

        //setting up the data
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindDesignProductivityForVoidingDataSet TheFindDesignProductivityForVoidingDataSet = new FindDesignProductivityForVoidingDataSet();
        DesignProductivityDataSet TheDesignProductivityDataSet = new DesignProductivityDataSet();

        DateTime gdatTransactionDate;

        public VoidDesignProductivity()
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
            txtDate.Text = "";
            txtEnterLastName.Text = "";
            txtProjectID.Text = "";
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            cboSelectEmployee.SelectedIndex = 0;
            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Void Design Productivity");
            TheDesignProductivityDataSet.designproductivity.Rows.Clear();
            dgrResults.ItemsSource = TheDesignProductivityDataSet.designproductivity;
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strLastName;

            try
            {
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                strLastName = txtEnterLastName.Text;
                if(strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("The Employee Was Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Design Productivity // Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expFindItems_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnThereIsAproblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strAssignedProjectID;
            string strValueForValidation;
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                expFindItems.IsExpanded = false;

                TheDesignProductivityDataSet.designproductivity.Rows.Clear();

                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                strAssignedProjectID = txtProjectID.Text;
                if(strAssignedProjectID.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project ID was not Long Enough\n";
                }
                else
                {
                    TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Project Was Not Found\n";
                    }
                }
                strValueForValidation = txtDate.Text;
                blnThereIsAproblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAproblem == true)
                {
                    blnFatalError = true;
                    TheMessagesClass.ErrorMessage("The Date is not a Date\n");
                }
                else
                {
                    gdatTransactionDate = Convert.ToDateTime(strValueForValidation);
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindDesignProductivityForVoidingDataSet = TheDesignProductivityClass.FindDesignProducitivityForVoiding(MainWindow.gintEmployeeID, strAssignedProjectID, gdatTransactionDate);

                intNumberOfRecords = TheFindDesignProductivityForVoidingDataSet.FindDesignProductivityForVoiding.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        DesignProductivityDataSet.designproductivityRow NewProductivityRow = TheDesignProductivityDataSet.designproductivity.NewdesignproductivityRow();

                        NewProductivityRow.FirstName = TheFindDesignProductivityForVoidingDataSet.FindDesignProductivityForVoiding[intCounter].FirstName;
                        NewProductivityRow.HomeOffice = TheFindDesignProductivityForVoidingDataSet.FindDesignProductivityForVoiding[intCounter].HomeOffice;
                        NewProductivityRow.LastName = TheFindDesignProductivityForVoidingDataSet.FindDesignProductivityForVoiding[intCounter].LastName;
                        NewProductivityRow.ProjectID = TheFindDesignProductivityForVoidingDataSet.FindDesignProductivityForVoiding[intCounter].ProjectID;
                        NewProductivityRow.TransactionDate = TheFindDesignProductivityForVoidingDataSet.FindDesignProductivityForVoiding[intCounter].TransactionDate;
                        NewProductivityRow.TransactionID = TheFindDesignProductivityForVoidingDataSet.FindDesignProductivityForVoiding[intCounter].TransactionID;
                        NewProductivityRow.VoidTransaction = false;

                        TheDesignProductivityDataSet.designproductivity.Rows.Add(NewProductivityRow);
                    }
                }

                dgrResults.ItemsSource = TheDesignProductivityDataSet.designproductivity;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Design Productivity // Find Items Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }
        }

        private void expVoidItems_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            int intTransactionID;

            try
            {
                intNumberOfRecords = TheDesignProductivityDataSet.designproductivity.Rows.Count;

                if(intNumberOfRecords < 1)
                {
                    TheMessagesClass.ErrorMessage("There Are No Transactions To Void");
                    return;
                }

                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if(TheDesignProductivityDataSet.designproductivity[intCounter].VoidTransaction == true)
                    {
                        intTransactionID = TheDesignProductivityDataSet.designproductivity[intCounter].TransactionID;

                        blnFatalError = TheDesignProductivityClass.VoidDesignProductivity(intTransactionID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                TheMessagesClass.InformationMessage("All Selected Transactions Have Been Voided");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Design Productivity // Void Items Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
