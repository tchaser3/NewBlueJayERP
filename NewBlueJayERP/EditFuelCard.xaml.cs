/* Title:           Edit Fuel Card
 * Date:            5-19-20
 * 
 * Description:     This is used to update a Fuel Card */


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
using NewEmployeeDLL;
using NewEventLogDLL;
using FuelCardDLL;
using WorkTaskStatsDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditFuelCard.xaml
    /// </summary>
    public partial class EditFuelCard : Window
    {
        //settung up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        FuelCardClass TheFuelCardClass = new FuelCardClass();

        //setting up the data
        FindFuelCardEmployeeDataSet TheFindFuelCardEmployeeDataSet = new FindFuelCardEmployeeDataSet();
        FindEmployeeActiveFuelCardNumberDataSet TheFindEmployeeActiveFuelCardNumberDataSet = new FindEmployeeActiveFuelCardNumberDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();

        public EditFuelCard()
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
            txtEnterLastName.Text = "";
            txtNewCardNumber.Text = "";
            txtOldCardNumber.Text = "";
            cboSelectEmployee.Items.Clear();
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;

                if(intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");
                    txtOldCardNumber.Text = "";
                    txtNewCardNumber.Text = "";

                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if(intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("The Employee Was Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                  
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Fuel Card // Enter Last Name Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;

                    TheFindEmployeeActiveFuelCardNumberDataSet = TheFuelCardClass.FindEmployeeActiveFuelCardNumber(MainWindow.gintEmployeeID);

                    intRecordsReturned = TheFindEmployeeActiveFuelCardNumberDataSet.FindEmployeeActiveFuelCardNumber.Rows.Count;

                    if (intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("No Fuel Card Number Found");
                        return;
                    }

                    txtOldCardNumber.Text = Convert.ToString(TheFindEmployeeActiveFuelCardNumberDataSet.FindEmployeeActiveFuelCardNumber[0].FuelCardAssignment);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Fuel Card // Combo Box Selected " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private int RandomNumber()
        {
            int intFuelCardNumber = -1;
            int intRecordsReturned = 1;

            while (intRecordsReturned > 0)
            {
                Random FuelCardNumber = new Random();
                intFuelCardNumber = FuelCardNumber.Next(1000, 9999);

                TheFindFuelCardEmployeeDataSet = TheFuelCardClass.FindFuelCardEmployee(intFuelCardNumber);

                intRecordsReturned = TheFindFuelCardEmployeeDataSet.FindFuelCardEmployee.Rows.Count;
            }

            return intFuelCardNumber;
        }

        private void btnNewNumber_Click(object sender, RoutedEventArgs e)
        {
            int intFuelCardNumber;
            bool blnFatalError = false;

            try
            {
                intFuelCardNumber = RandomNumber();

                blnFatalError = TheFuelCardClass.UpdateFuelCardActive(Convert.ToInt32(txtOldCardNumber.Text), false);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheFuelCardClass.InsertFuelCard(MainWindow.gintEmployeeID, intFuelCardNumber);

                if (blnFatalError == true)
                    throw new Exception();

                txtNewCardNumber.Text = Convert.ToString(intFuelCardNumber);

                TheMessagesClass.InformationMessage("The Information has been Updated");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Fuel Card Number // New Number Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }
    }
}
