/* Title:           JSI Final Window
 * Date:            5-26-20
 * Author:          Terry Holmes
 * 
 * Description:     This is the final window */

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
using JSIMainDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for JSIFinalWindow.xaml
    /// </summary>
    public partial class JSIFinalWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        JSIMainClass TheJSIMainClass = new JSIMainClass();

        //setting up variables
        string gstrCones;
        string gstrFlagger;
        string gstrJSACompleted;
        string gstrSigns;
        string gstrOverall;
        bool gblnDocumentAttached;
        bool gblnSafetyConcerns;
        string gstrInspectionNotes;

        public JSIFinalWindow()
        {
            InitializeComponent();
        }

        private void expSendEmail_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchEmail();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load up the controls
            cboCones.Items.Add("Select Cones");
            cboCones.Items.Add("Pass");
            cboCones.Items.Add("NA");
            cboCones.Items.Add("Fail");
            cboCones.SelectedIndex = 0;

            cboFlagger.Items.Add("Select Flagger");
            cboFlagger.Items.Add("Pass");
            cboFlagger.Items.Add("NA");
            cboFlagger.Items.Add("Fail");
            cboFlagger.SelectedIndex = 0;

            cboJSACompleted.Items.Add("Select JSA Completed");
            cboJSACompleted.Items.Add("Pass");
            cboJSACompleted.Items.Add("NA");
            cboJSACompleted.Items.Add("Fail");
            cboJSACompleted.SelectedIndex = 0;

            cboSigns.Items.Add("Select Signs");
            cboSigns.Items.Add("Pass");
            cboSigns.Items.Add("NA");
            cboSigns.Items.Add("Fail");
            cboSigns.SelectedIndex = 0;

            cboOverall.Items.Add("Select Overall");
            cboOverall.Items.Add("Pass");
            cboOverall.Items.Add("Pass with Minor Issues");
            cboOverall.Items.Add("Fail");
            cboOverall.SelectedIndex = 0;

            gblnDocumentAttached = false;
            gblnSafetyConcerns = false;
            rdoConcersFalse.IsChecked = true;
            gstrInspectionNotes = "";
        }

        private void cboCones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboCones.SelectedIndex > 0)
            {
                gstrCones = cboCones.SelectedIndex.ToString();

                gstrCones = gstrCones.ToUpper();
            }    
        }

        private void cboFlagger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboFlagger.SelectedIndex > 0)
            {
                gstrFlagger = cboFlagger.SelectedItem.ToString();

                gstrFlagger = gstrFlagger.ToUpper();
            }
        }

        private void cboJSACompleted_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboJSACompleted.SelectedIndex > 0)
            {
                gstrJSACompleted = cboJSACompleted.SelectedItem.ToString();

                gstrJSACompleted = gstrJSACompleted.ToUpper();
            }
        }

        private void cboSigns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboSigns.SelectedIndex > 0)
            {
                gstrSigns = cboSigns.SelectedItem.ToString();

                gstrSigns = gstrSigns.ToUpper();
            }
        }

        private void cboOverall_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboOverall.SelectedIndex > 0)
            {
                gstrOverall = cboOverall.SelectedItem.ToString();

                gstrOverall = gstrOverall.ToUpper();
            }
        }

        private void expAddDocument_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            int intNumberOfRecords;
            int intCounter;
            string strDocumentPath;

            try
            {
                expAddDocument.IsExpanded = false;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();

                            gstrInspectionNotes = txtInspectionNotes.Text;

                            blnFatalError = TheJSIMainClass.InsertJSIDocumentation(MainWindow.gintJSITransationID, DateTime.Now, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strDocumentPath, gstrInspectionNotes); 

                            if (blnFatalError == true)
                                throw new Exception();

                            gblnDocumentAttached = true;
                        }
                    }
                }
                else
                {
                    return;
                }

                TheMessagesClass.InformationMessage("The Inspect has been Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // JSI Final Window // Add Document Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            string strErrorMessage = "";
            bool blnFatalError = false;

            try
            {
                if(cboCones.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Cones was not Selected\n";
                }
                if(cboFlagger.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Flags was not Selected\n";
                }
                if(cboJSACompleted.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "JSA Completed was not Selected\n";
                }
                if(cboSigns.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Signs Was Not Selected\n";
                }
                gstrInspectionNotes = txtInspectionNotes.Text;
                if(gstrInspectionNotes == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Inspection Note was not Entered\n";
                }
                if (gblnDocumentAttached == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Inspection Document was not Added\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheJSIMainClass.InsertJSISiteInspection(MainWindow.gintJSITransationID, gstrCones, gstrFlagger, gstrJSACompleted, gstrSigns);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheJSIMainClass.InsertJSIOverall(MainWindow.gintJSITransationID, gstrOverall, gblnSafetyConcerns, MainWindow.gintInspectingEmployeeID, MainWindow.gdatInspectionDate);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("JSI Inspection Completed");

                this.Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // JSI Final Window // Process Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void rdoConcernsTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnSafetyConcerns = true;
        }

        private void rdoConcersFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnSafetyConcerns = false;
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }
    }
}
