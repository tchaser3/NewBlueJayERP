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
using IncentivePayDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;
using NewEmployeeDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EditIncentivePayTransaction.xaml
    /// </summary>
    public partial class EditIncentivePayTransaction : Window
    {
        //Setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        IncentivePayClass TheIncentivePayClass = new IncentivePayClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up the data
        FindIncentivePayTransactionStatusByIncentivePayTransactionIDataSet TheFindIncentivePayTransactionStatusByIncentivePayTransactionIDDataSet = new FindIncentivePayTransactionStatusByIncentivePayTransactionIDataSet();
        FindIncentivePayByTransactionIDDataSet TheFindIncentivePayByTransactionIDDataSet = new FindIncentivePayByTransactionIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        //setting up global variables
        string gstrManagerEmailAddress;

        public EditIncentivePayTransaction()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            this.Close();
        }
        private void ResetControls()
        {
            //setting up local variables
            int intManagerID;

            try
            {

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Edit Incentive Pay Transaction // Reset Controls Method " + Ex.Message);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Edit Incentive Pay Transaction // Reset Controls Method " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
    }
}
