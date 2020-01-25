/* Title:           Department Production Email
 * Date:            1-25-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to set the automated reports */

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
using DepartmentDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for DepartmentProductionEmail.xaml
    /// </summary>
    public partial class DepartmentProductionEmail : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();

        //setting up global variables
        int gintEmployeeID;
        int gintDepartmentID;

        public DepartmentProductionEmail()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            this.Visibility = Visibility.Hidden;
        }


        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            txtEnterLastName.Text = "";
            txtEnterProjectSuffix.Text = "";
            cboSelectEmployee.Items.Clear();
            cboProjectSelectDepartment.Items.Clear();
            cboSelectDepartment.Items.Clear();

            TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

            intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

            cboSelectDepartment.Items.Add("Select Department");
            cboProjectSelectDepartment.Items.Add("Select Department");

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboProjectSelectDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
                cboSelectDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
            }

            cboSelectDepartment.SelectedIndex = 0;
            cboProjectSelectDepartment.SelectedIndex = 0;
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintDepartmentID = TheFindSortedDepartmentDataSet.FindSortedDepartment[intSelectedIndex].DepartmentID;
        }

        private void cboProjectSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboProjectSelectDepartment.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintDepartmentID = TheFindSortedDepartmentDataSet.FindSortedDepartment[intSelectedIndex].DepartmentID;
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if (intSelectedIndex > -1)
                gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up to find an employee
            string strLastName;
            int intLenght;
            int intNumberOfRecords;
            int intCounter;

            strLastName = txtEnterLastName.Text;

            intLenght = strLastName.Length;

            if(intLenght > 2)
            {
                cboSelectEmployee.Items.Clear();

                TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;
                cboSelectEmployee.Items.Add("Select Employee");

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Employee Not Found");
                    return;
                }

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up varibles
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Was Not Selected\n";
                }
                if(cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheDepartmentClass.InsertDepartmentProductionEmail(gintEmployeeID, gintDepartmentID);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Employee Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Department Production Email // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProjectProcess_Click(object sender, RoutedEventArgs e)
        {
            string strProjectSuffix;
            int intLength;
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                if(cboProjectSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Wasn't Selected";
                }
                strProjectSuffix = txtEnterProjectSuffix.Text;
                intLength = strProjectSuffix.Length;
                if(intLength < 6)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Project Suffix was not Long Enough";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheDepartmentClass.InsertDepartmentProductionEmailProject(gintDepartmentID, strProjectSuffix);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Department Project Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Department Production Email // Project Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
