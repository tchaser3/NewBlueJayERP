/* Title:           Void Inventory Transaction
 * Date:            12-7-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to void a inventory transaction */

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
using IssuedPartsDLL;
using InventoryDLL;
using EmployeeDateEntryDLL;
using ProjectMatrixDLL;


namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for VoidInventoryTransaction.xaml
    /// </summary>
    public partial class VoidInventoryTransaction : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        IssuedPartsClass TheIssuedPartsClass = new IssuedPartsClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        //getting data
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindIssuedPartsByProjectIDDataSet TheFindIssuedPartsByProjectIDDataSet = new FindIssuedPartsByProjectIDDataSet();
        MaterialIssuedDataSet TheMaterialIssuedDataSet = new MaterialIssuedDataSet();
        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryPartDataSet = new FindWarehouseInventoryPartDataSet();

        public VoidInventoryTransaction()
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //setting up data
            string strAssignedProjectID = "";
            string strCustomerProjectID = "";
            int intRecordsReturned;
            int intProjectID = 0;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strCustomerProjectID = txtProjectNumber.Text;
                if(strCustomerProjectID.Length < 4)
                {
                    TheMessagesClass.ErrorMessage("The Project ID is not Long Enough");
                    return;
                }

                TheMaterialIssuedDataSet.materialissued.Rows.Clear();

                //getting the data
                TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

                intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                if(intRecordsReturned < 1)
                {
                    strAssignedProjectID = strCustomerProjectID;

                    TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                    if(intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("Project Was Not Found");
                        return;
                    }
                    else if(intRecordsReturned > 0)
                    {
                        intProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;
                        strCustomerProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].CustomerAssignedID;
                    }
                }
                else if(intRecordsReturned > 0)
                {
                    intProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                    strAssignedProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].CustomerAssignedID;
                }

                TheFindIssuedPartsByProjectIDDataSet = TheIssuedPartsClass.FindIssuedPartsByProjectID(intProjectID);

                intNumberOfRecords = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        MaterialIssuedDataSet.materialissuedRow NewTransactionRow = TheMaterialIssuedDataSet.materialissued.NewmaterialissuedRow();

                        NewTransactionRow.AssignedProjectID = strAssignedProjectID;
                        NewTransactionRow.CustomerProjectID = strCustomerProjectID;
                        NewTransactionRow.PartID = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].PartID;
                        NewTransactionRow.PartNumber = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].PartNumber;
                        NewTransactionRow.Quantity = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].Quantity;
                        NewTransactionRow.TransactionDate = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].TransactionDate;
                        NewTransactionRow.TransactionID = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].TransactionID;
                        NewTransactionRow.VoidTransaction = false;
                        NewTransactionRow.WarehouseID = TheFindIssuedPartsByProjectIDDataSet.FindIssuedPartsByProjectID[intCounter].WarehouseID;

                        TheMaterialIssuedDataSet.materialissued.Rows.Add(NewTransactionRow);
                    }

                    MainWindow.gintProjectID = intProjectID;
                    MainWindow.gstrAssignedProjectID = strAssignedProjectID;
                }

                dgrTransactions.ItemsSource = TheMaterialIssuedDataSet.materialissued;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Inventory Transaction // Search Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
            txtProjectNumber.Text = "";
            txtTransactionNotes.Text = "";

            TheMaterialIssuedDataSet.materialissued.Rows.Clear();

            dgrTransactions.ItemsSource = TheMaterialIssuedDataSet.materialissued;
        }

        private void btnVoidTransactions_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intPartID;
            int intWarehouseID;
            int intTransactionID;
            int intQuantity;
            int intWarehouseQuantity;
            int intWarehouseTransactionID;
            bool blnFatalError = false;
            string strTransactionNotes;

            try
            {
                intNumberOfRecords = TheMaterialIssuedDataSet.materialissued.Rows.Count;

                if(txtTransactionNotes.Text.Length < 25)
                {
                    TheMessagesClass.ErrorMessage("The Notes is not Long Enough");
                    return;
                }

                strTransactionNotes = MainWindow.gstrAssignedProjectID + " " + txtTransactionNotes.Text;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(TheMaterialIssuedDataSet.materialissued[intCounter].VoidTransaction == true)
                        {
                            intPartID = TheMaterialIssuedDataSet.materialissued[intCounter].PartID;
                            intWarehouseID = TheMaterialIssuedDataSet.materialissued[intCounter].WarehouseID;
                            intQuantity = TheMaterialIssuedDataSet.materialissued[intCounter].Quantity;
                            intTransactionID = TheMaterialIssuedDataSet.materialissued[intCounter].TransactionID;

                            TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(intPartID, intWarehouseID);

                            intWarehouseQuantity = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].Quantity;
                            intWarehouseTransactionID = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].TransactionID;

                            intWarehouseQuantity = intWarehouseQuantity - intQuantity;

                            blnFatalError = TheInventoryClass.UpdateInventoryPart(intWarehouseTransactionID, intWarehouseQuantity);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheIssuedPartsClass.UpdateIssuedParts(intTransactionID, 0);

                            if(blnFatalError == true)
                                throw new Exception();

                            
                        }
                    }

                    blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strTransactionNotes);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, strTransactionNotes);

                    TheMessagesClass.InformationMessage("The Transactions Have Been Voided");

                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Void Inventory Transaction // Void Transactions Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
