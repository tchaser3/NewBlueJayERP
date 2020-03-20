/* Title:           Process Batch Class
 * Date:            4-14-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the class that will process the batch */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEventLogDLL;
using InventoryDLL;
using InventoryWIPDLL;
using ReceivePartsDLL;
using IssuedPartsDLL;
using BOMPartsDLL;
using ProjectsDLL;
using CharterInventoryDLL;

namespace NewBlueJayERP
{
    class ProcessBatchClass
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        InventoryWIPClass TheInventoryWIPClass = new InventoryWIPClass();
        ReceivePartsClass TheReceivePartsClass = new ReceivePartsClass();
        IssuedPartsClass TheIssuedPartsClass = new IssuedPartsClass();
        BOMPartsClass TheBOMPartsClass = new BOMPartsClass();
        ProjectClass TheProjectClass = new ProjectClass();
        CharterInventoryClass TheCharterInventoryClass = new CharterInventoryClass();

        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindWIPByPartIDAndWarehouseIDDataSet TheFindWIPByPartIDAndWarehouseIDDataSet = new FindWIPByPartIDAndWarehouseIDDataSet();
        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryPartDataSet = new FindWarehouseInventoryPartDataSet();
        FindCharterWarehouseInventoryForPartDataSet TheFindCharterWarehouseInventoryForPartDataSet = new FindCharterWarehouseInventoryForPartDataSet();

        public void UpdateInventoryTables()
        {
            int intWIPCounter;
            int intWIPNumberOfRecords;
            int intArrayCounter;
            int intPartID;
            int intQuantity;
            int intWarehouseID;
            int intProjectID;
            int intEmployeeID;
            string strPONumber;
            string strTransactionType;
            int intEnterEmployeeID;
            bool blnFatalError;
            string[] strTransactionTypeArray = new string[3];
            
            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //getting the data
                MainWindow.TheFindWIPBySessionIDDataSet = TheInventoryWIPClass.FindWIPBySessionID(MainWindow.gintSessionID);
                strTransactionTypeArray[0] = "RECEIVE";
                strTransactionTypeArray[1] = "ISSUE";
                strTransactionTypeArray[2] = "BOM";

                intWIPNumberOfRecords = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID.Rows.Count - 1;

                if (intWIPNumberOfRecords > -1)
                {
                    for(intArrayCounter = 0; intArrayCounter <= 2; intArrayCounter++)
                    {
                        for (intWIPCounter = 0; intWIPCounter <= intWIPNumberOfRecords; intWIPCounter++)
                        {
                            blnFatalError = false;
                            intPartID = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].PartID;
                            intQuantity = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].Quantity;
                            intWarehouseID = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].WarehouseID;
                            intProjectID = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].ProjectID;
                            intEmployeeID = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].EmployeeID;
                            intEnterEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                            strPONumber = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].PONumber;
                            strTransactionType = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].TransactionType;

                            if(strTransactionTypeArray[intArrayCounter] == MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].TransactionType)
                            {
                                if (MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].TransactionType == "RECEIVE")
                                {
                                    blnFatalError = UpdateCharterWarehouseInfo(intPartID, intWarehouseID, intQuantity);
                                    if (blnFatalError == false)
                                        blnFatalError = UpdateInventoryInfo(intPartID, intWarehouseID, intQuantity);
                                    if (blnFatalError == false)
                                        blnFatalError = TheReceivePartsClass.InsertReceivedPart(intProjectID, intPartID, intQuantity, intEnterEmployeeID, strPONumber, intWarehouseID);

                                    if (blnFatalError == true)
                                        TheMessagesClass.ErrorMessage("There Has Been A Problem, Contact IT");
                                }
                                else if (MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].TransactionType == "ISSUE")
                                {
                                    blnFatalError = TheIssuedPartsClass.InsertIssuedParts(intProjectID, intPartID, intQuantity, intEmployeeID, intEnterEmployeeID, intWarehouseID);

                                    intQuantity = intQuantity * -1;

                                    if (blnFatalError == false)
                                        blnFatalError = UpdateInventoryInfo(intPartID, intWarehouseID, intQuantity);

                                    if (blnFatalError == true)
                                        TheMessagesClass.ErrorMessage("There Has Been A Problem, Contact IT");
                                }
                                else if (MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID[intWIPCounter].TransactionType == "BOM")
                                {
                                    blnFatalError = TheBOMPartsClass.InsertBOMParts(intProjectID, intPartID, intQuantity, intEnterEmployeeID, intWarehouseID);

                                    intQuantity = intQuantity * -1;

                                    
                                    if (blnFatalError == false)
                                        blnFatalError = UpdateCharterWarehouseInfo(intPartID, intWarehouseID, intQuantity);

                                    if (blnFatalError == true)
                                        TheMessagesClass.ErrorMessage("There Has Been A Problem, Contact IT");
                                }
                            }
                        }
                    }
                    

                    TheInventoryWIPClass.RemoveSessionEntriesFromWIP(MainWindow.gintSessionID);

                    TheInventoryWIPClass.RemoveSession(MainWindow.gintSessionID);

                    TheInventoryWIPClass.InsertNewSession(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID);

                    MainWindow.TheFindSessionByEmployeeIDDataSet = TheInventoryWIPClass.FindSessionByEmployeeID(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID);

                    MainWindow.gintSessionID = MainWindow.TheFindSessionByEmployeeIDDataSet .FindSessionByEmployeeID[0].SessionID;

                    TheMessagesClass.InformationMessage("Batch Processing Complete.  You Are Beginning a New Session");
                }
                else
                {
                    TheMessagesClass.InformationMessage("There Are No Records To Process");
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP \\ Process Patch Class \\ Update Tables " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
        private bool UpdateInventoryInfo(int intPartID, int intWarehouseID, int intQuantity)
        {
            bool blnFatalError = false;
            int intRecordsReturned;
            int intTransactionID;
            int intTotalQuantity;

            try
            {
                TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(intPartID, intWarehouseID);

                intRecordsReturned = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    blnFatalError = TheInventoryClass.InsertInventoryPart(intPartID, intQuantity, intWarehouseID);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Has Been A Problem, Contact ID");
                        return blnFatalError;
                    }
                }
                else if(intRecordsReturned == 1)
                {
                    intTransactionID = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].TransactionID;

                    intTotalQuantity = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].Quantity + intQuantity;

                    blnFatalError = TheInventoryClass.UpdateInventoryPart(intTransactionID, intTotalQuantity);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Has Been A Problem, Contact ID");
                        return blnFatalError;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Process Patch Class // Update Inventory Info " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private bool UpdateCharterWarehouseInfo(int intPartID, int intWarehouseID, int intQuantity)
        {
            bool blnFatalError = false;
            int intTransactionID;
            int intRecordsReturned;
            int intTotalQuantity;

            try
            {
                TheFindCharterWarehouseInventoryForPartDataSet = TheCharterInventoryClass.FindCharterWarehouseInventoryForPart(intPartID, intWarehouseID);

                intRecordsReturned = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart.Rows.Count;

                if (intRecordsReturned == 0)
                {
                    blnFatalError = TheCharterInventoryClass.InsertCharterInventory(intPartID, intWarehouseID, intQuantity);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Was A Problem, Contact IT");
                        return blnFatalError;
                    }
                }
                else if (intRecordsReturned == 1)
                {
                    intTransactionID = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].TransactionID;

                    intTotalQuantity = intQuantity + TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].Quantity;

                    blnFatalError = TheCharterInventoryClass.UpdateCharterInventory(intTransactionID, intTotalQuantity);

                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Is A Problem, Contact IT");
                        return blnFatalError;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Process Batch Class // Update Charter Warehouse Info " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.Message);

                blnFatalError = true;
            }

            return blnFatalError;
        }
    }
}
