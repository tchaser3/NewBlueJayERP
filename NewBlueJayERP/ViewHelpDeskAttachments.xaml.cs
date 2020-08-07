/* Title:           View Help Desk Attachments
 * Date:            8-6-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used to view attachments */

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
    /// Interaction logic for ViewHelpDeskAttachments.xaml
    /// </summary>
    public partial class ViewHelpDeskAttachments : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the data
        FindHelpDeskTicketDocumentationByTicketIDDataSet TheFindHelpDeskTicketDocumentationByTicketIDDataSet = new FindHelpDeskTicketDocumentationByTicketIDDataSet();

        public ViewHelpDeskAttachments()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindHelpDeskTicketDocumentationByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketDocumentationByTicketID(MainWindow.gintTicketID);

            dgrDocuments.ItemsSource = TheFindHelpDeskTicketDocumentationByTicketIDDataSet.FindHelpDeskTicketDocumentationByTicketID;
        }

        private void dgrDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrDocuments.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrDocuments;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // View Help Desk Attachments // Documentation Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
