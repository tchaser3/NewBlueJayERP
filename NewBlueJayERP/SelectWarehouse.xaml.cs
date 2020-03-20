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

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for SelectWarehouse.xaml
    /// </summary>
    public partial class SelectWarehouse : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        //setting up data
        FindPartsWarehousesDataSet TheFindPartsWarehousesDataSet = new FindPartsWarehousesDataSet();

        public SelectWarehouse()
        {
            InitializeComponent();
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
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectWarehouse.Items.Clear();
                cboSelectWarehouse.Items.Add("Select Warehouse");

                TheFindPartsWarehousesDataSet = TheEmployeeClass.FindPartsWarehouses();

                intNumberOfRecords = TheFindPartsWarehousesDataSet.FindPartsWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(TheFindPartsWarehousesDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Select Warehouse // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectWarehouse.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintWarehouseID = TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                MainWindow.gstrWarehouseName = TheFindPartsWarehousesDataSet.FindPartsWarehouses[intSelectedIndex].FirstName;

                MainWindow.EnterInventoryWindow.Visibility = Visibility.Visible;

                this.Visibility = Visibility.Hidden;
            }
        }
    }
}
