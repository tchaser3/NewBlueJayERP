/* Title:           Create Asset Type
 * Date:            7-6-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to create the asset type */

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
using AssetDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateAssetType.xaml
    /// </summary>
    public partial class CreateAssetType : Window
    {
        //Setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        AssetClass TheAssetClass = new AssetClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //settting up the data
        FindAssetTypeByAssetTypeDataSet TheFindAssetTypeByAssetTypeDataSet = new FindAssetTypeByAssetTypeDataSet();

        public CreateAssetType()
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
            txtEnterAssetType.Text = "";
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strAssetType;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            int intLength;

            try
            {
                //data validation
                strAssetType = txtEnterAssetType.Text;
                if(strAssetType == "")
                {
                    blnFatalError = true;
                    strErrorMessage = "The Asset Type was not Entered";
                }
                else
                {
                    intLength = strAssetType.Length;

                    if(intLength < 5)
                    {
                        blnFatalError = true;
                        strErrorMessage = "The Asset Type is not Long Enough";
                    }
                    else
                    {
                        TheFindAssetTypeByAssetTypeDataSet = TheAssetClass.FindAssetTypeByAssetType(strAssetType);

                        intRecordsReturned = TheFindAssetTypeByAssetTypeDataSet.FindAssetTypeByAssetType.Rows.Count;

                        if(intRecordsReturned > 0)
                        {
                            blnFatalError = true;
                            strErrorMessage = "The Asset Type is Already Entered";
                        }
                    }
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);

                    return;
                }

                blnFatalError = TheAssetClass.InsertAssetType(strAssetType);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Asset Type has been Entered");

                ResetControls();

                txtEnterAssetType.Focus();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Asset Type // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
