/* Title:           Create Help Desk Problem Type
 * Date:            7-13-20
 * Author:          Terry Holmes
 * 
 * Description:     This is the window to create help desk problem types */

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
using HelpDeskDLL;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for CreateHelpDeskProblemType.xaml
    /// </summary>
    public partial class CreateHelpDeskProblemType : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up data
        FindHelpDeskProblemTypeByProblemTypeDataSet TheFindHelpDeskProblemTypeByProblemTypeDataSet = new FindHelpDeskProblemTypeByProblemTypeDataSet();

        public CreateHelpDeskProblemType()
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
            txtProblemType.Text = "";
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txtProblemType.Text = "";
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            string strProblemType;
            bool blnFatalError;
            int intRecordsReturned;

            try
            {
                strProblemType = txtProblemType.Text;
                if(strProblemType.Length < 5)
                {
                    TheMessagesClass.ErrorMessage("The Problem Type is to short");
                    return;
                }

                TheFindHelpDeskProblemTypeByProblemTypeDataSet = TheHelpDeskClass.FindHelpDeskProblemTypeByProblemType(strProblemType);

                intRecordsReturned = TheFindHelpDeskProblemTypeByProblemTypeDataSet.FindHelpDeskProblemTypeByProblemType.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Problem Type is Already Entered");
                    return;
                }

                blnFatalError = TheHelpDeskClass.InsertHelpDeskProblemType(strProblemType);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Problem Type Was Entered");

                txtProblemType.Text = "";
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Create Help Desk Problem Type // Process Button " + Ex.Message);

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
