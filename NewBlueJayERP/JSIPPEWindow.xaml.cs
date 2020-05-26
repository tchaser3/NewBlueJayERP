/* Title:           JSI PPE
 * Date:            5-13-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for JSI PPE */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JSIMainDLL;
using Microsoft.Office.Core;
using NewEventLogDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for JSIPPEWindow.xaml
    /// </summary>
    public partial class JSIPPEWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        JSIMainClass TheJSIMainClass = new JSIMainClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting global variables
        string gstrSafetyVest;
        string gstrGlassesGloves;
        string gstrFootGear;
        string gstrFallProtection;
        string gstrHardHat;
        string gstrExtensionLadder;
        string gstrSmallerLadder;
        string gstrRatingStickers;
        string gstrCompanyLadders;
        string gstrLadderLocks;
        bool gblnMagnets;
        bool gblnCleanliness;
        bool gblnSecure;
        bool gblnMaintenance;
        bool gblnExtingisher;
        bool gblnUniform;
        bool gblnLicense;
        bool gblnBlueJayLogo;
        bool gblnIDBadge;

        public JSIPPEWindow()
        {
            InitializeComponent();
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
            //settting up combo boxes
            cboSafetyVest.Items.Add("Select Safety Vest");
            cboSafetyVest.Items.Add("Pass");
            cboSafetyVest.Items.Add("NA");
            cboSafetyVest.Items.Add("Fail");
            cboSafetyVest.SelectedIndex = 0;

            cboGlassGloves.Items.Add("Select Glasses and Gloves");
            cboGlassGloves.Items.Add("Pass");
            cboGlassGloves.Items.Add("NA");
            cboGlassGloves.Items.Add("Fail");
            cboGlassGloves.SelectedIndex = 0;

            cboSafetyFootGear.Items.Add("Select Safety Foot Gear");
            cboSafetyFootGear.Items.Add("Pass");
            cboSafetyFootGear.Items.Add("NA");
            cboSafetyFootGear.Items.Add("Fail");
            cboSafetyFootGear.SelectedIndex = 0;

            cboFallProtection.Items.Add("Select Fall Protection");
            cboFallProtection.Items.Add("Pass");
            cboFallProtection.Items.Add("NA");
            cboFallProtection.Items.Add("Fail");
            cboFallProtection.SelectedIndex = 0;

            cboHardHat.Items.Add("Select Hard Hat");
            cboHardHat.Items.Add("Pass");
            cboHardHat.Items.Add("NA");
            cboHardHat.Items.Add("Fail");
            cboHardHat.SelectedIndex = 0;

            cboExtensionLadder.Items.Add("Select Extension Ladder");
            cboExtensionLadder.Items.Add("Pass");
            cboExtensionLadder.Items.Add("NA");
            cboExtensionLadder.Items.Add("Fail");
            cboExtensionLadder.SelectedIndex = 0;

            cboSmallerLadder.Items.Add("Select Smaller Ladder");
            cboSmallerLadder.Items.Add("Pass");
            cboSmallerLadder.Items.Add("NA");
            cboSmallerLadder.Items.Add("Fail");
            cboSmallerLadder.SelectedIndex = 0;

            cboRatingStickers.Items.Add("Select Rating Stickers");
            cboRatingStickers.Items.Add("Pass");
            cboRatingStickers.Items.Add("NA");
            cboRatingStickers.Items.Add("Fail");
            cboRatingStickers.SelectedIndex = 0;

            cboCompanyLadders.Items.Add("Select Company Ladders");
            cboCompanyLadders.Items.Add("Pass");
            cboCompanyLadders.Items.Add("NA");
            cboCompanyLadders.Items.Add("Fail");
            cboCompanyLadders.SelectedIndex = 0;

            cboLadderLocks.Items.Add("Select Ladder Locks");
            cboLadderLocks.Items.Add("Pass");
            cboLadderLocks.Items.Add("NA");
            cboLadderLocks.Items.Add("Fail");
            cboLadderLocks.SelectedIndex = 0;

            rdoBadgeFalse.IsChecked = true;
            rdoCleanlinessBad.IsEnabled = true;
            rdoSecureFalse.IsEnabled = true;
            rdoMaintenanceFalse.IsChecked = true;
            rdoFirstAidFalse.IsChecked = true;
            rdoUniformFalse.IsChecked = true;
            rdoValidFalse.IsChecked = true;
            rdoLogoFalse.IsChecked = true;
            rdoBadgeFalse.IsChecked = true;

            EnablePPE(true);
            EnableVehicle(false);
            EnableLadder(false);
            EnableUniform(false);
        }

        private void cboSafetyVest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSafetyVest.SelectedIndex > 0)
            {
                gstrSafetyVest = cboSafetyVest.SelectedItem.ToString();

                gstrSafetyVest = gstrSafetyVest.ToUpper();
            }
        }

        private void cboGlassGloves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboGlassGloves.SelectedIndex > 0)
            {
                gstrGlassesGloves = cboGlassGloves.SelectedItem.ToString();

                gstrGlassesGloves = gstrGlassesGloves.ToUpper();
            }
        }

        private void cboSafetyFootGear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSafetyFootGear.SelectedIndex > 0)
            {
                gstrFootGear = cboSafetyFootGear.SelectedItem.ToString();

                gstrFootGear = gstrFootGear.ToUpper();
            }
        }

        private void cboFallProtection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboFallProtection.SelectedIndex > 0)
            {
                gstrFallProtection = cboFallProtection.SelectedItem.ToString();

                gstrFallProtection = gstrFallProtection.ToUpper();
            }
        }

        private void cboHardHat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboHardHat.SelectedIndex > 0)
            {
                gstrHardHat = cboHardHat.SelectedItem.ToString();

                gstrHardHat = gstrHardHat.ToUpper();
            }
        }

        private void cboExtensionLadder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboExtensionLadder.SelectedIndex > 0)
            {
                gstrExtensionLadder = cboExtensionLadder.SelectedItem.ToString();

                gstrExtensionLadder = gstrExtensionLadder.ToUpper();
            }
        }

        private void cboSmallerLadder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboSmallerLadder.SelectedIndex > 0)
            {
                gstrSmallerLadder = cboSmallerLadder.SelectedItem.ToString();

                gstrSmallerLadder = gstrSmallerLadder.ToUpper();
            }
        }

        private void cboRatingStickers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboRatingStickers.SelectedIndex > 0)
            {
                gstrRatingStickers = cboRatingStickers.SelectedItem.ToString();

                gstrRatingStickers = gstrRatingStickers.ToUpper();
            }
        }

        private void cboCompanyLadders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboCompanyLadders.SelectedIndex > 0)
            {
                gstrCompanyLadders = cboCompanyLadders.SelectedItem.ToString();

                gstrCompanyLadders = gstrCompanyLadders.ToUpper();
            }
        }

        private void cboLadderLocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboLadderLocks.SelectedIndex > 0)
            {
                gstrLadderLocks = cboLadderLocks.SelectedItem.ToString();

                gstrLadderLocks = gstrLadderLocks.ToUpper();
            }
        }

        private void rdoMagnetTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnMagnets = true;
        }

        private void rdoMagnetFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnMagnets = false;
        }

        private void rdoCleanlinessGood_Checked(object sender, RoutedEventArgs e)
        {
            gblnCleanliness = true;
        }

        private void rdoCleanlinessBad_Checked(object sender, RoutedEventArgs e)
        {
            gblnCleanliness = false;
        }

        private void rdoSecureTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnSecure = true;
        }

        private void rdoSecureFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnSecure = false;
        }

        private void rdoMaintenanceTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnMaintenance = true;
        }

        private void rdoMaintenanceFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnMaintenance = false;
        }

        private void rdoFirstAidTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnExtingisher = true;
        }

        private void rdoFirstAidFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnExtingisher = false;
        }

        private void rdoUniformTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnUniform = true;
        }

        private void rdoUniformFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnUniform = false;
        }

        private void rdoValidTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnLicense = true;
        }

        private void rdoValidFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnLicense = false;
        }

        private void rdoLogoFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnBlueJayLogo = false;
        }

        private void rdoLogoTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnBlueJayLogo = true;
        }

        private void rdoBadgeFalse_Checked(object sender, RoutedEventArgs e)
        {
            gblnIDBadge = true;
        }

        private void rdoBadgeTrue_Checked(object sender, RoutedEventArgs e)
        {
            gblnIDBadge = false;
        }
        private void EnablePPE(bool blnValueBoolean)
        {
            cboSafetyVest.IsEnabled = blnValueBoolean;
            cboGlassGloves.IsEnabled = blnValueBoolean;
            cboSafetyFootGear.IsEnabled = blnValueBoolean;
            cboFallProtection.IsEnabled = blnValueBoolean;
            cboHardHat.IsEnabled = blnValueBoolean;
            btnProcessPPE.IsEnabled = blnValueBoolean;
        }
        private void EnableVehicle(bool blnValueBoolean)
        {
            rdoMagnetFalse.IsEnabled = blnValueBoolean;
            rdoMagnetTrue.IsEnabled = blnValueBoolean;
            rdoCleanlinessBad.IsEnabled = blnValueBoolean;
            rdoCleanlinessGood.IsEnabled = blnValueBoolean;
            rdoSecureFalse.IsEnabled = blnValueBoolean;
            rdoSecureTrue.IsEnabled = blnValueBoolean;
            rdoMaintenanceFalse.IsEnabled = blnValueBoolean;
            rdoMaintenanceTrue.IsEnabled = blnValueBoolean;
            rdoFirstAidFalse.IsEnabled = blnValueBoolean;
            rdoFirstAidTrue.IsEnabled = blnValueBoolean;
            btnProcessVehicle.IsEnabled = blnValueBoolean;
        }
        private void EnableLadder(bool blnValueBoolean)
        {
            cboExtensionLadder.IsEnabled = blnValueBoolean;
            cboSmallerLadder.IsEnabled = blnValueBoolean;
            cboRatingStickers.IsEnabled = blnValueBoolean;
            cboCompanyLadders.IsEnabled = blnValueBoolean;
            cboLadderLocks.IsEnabled = blnValueBoolean;
            btnProcessLadders.IsEnabled = blnValueBoolean;
        }
        private void EnableUniform(bool blnValueBoolean)
        {
            rdoUniformFalse.IsEnabled = blnValueBoolean;
            rdoUniformTrue.IsEnabled = blnValueBoolean;
            rdoValidFalse.IsEnabled = blnValueBoolean;
            rdoValidTrue.IsEnabled = blnValueBoolean;
            rdoLogoFalse.IsEnabled = blnValueBoolean;
            rdoLogoTrue.IsEnabled = blnValueBoolean;
            rdoBadgeFalse.IsEnabled = blnValueBoolean;
            rdoBadgeTrue.IsEnabled = blnValueBoolean;
            btnProcessUniform.IsEnabled = blnValueBoolean;
        }

        private void btnProcessPPE_Click(object sender, RoutedEventArgs e)
        {
            //setting up varlables
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                if(cboSafetyVest.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Safety Vest was not Selected\n";
                }
                if(cboGlassGloves.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Glasses and Gloves were not Selected\n";
                }
                if(cboSafetyFootGear.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Safety Foot Gear Was Not Selected\n";
                }
                if (cboFallProtection.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Fall Protection was not Selected\n";
                }
                if (cboHardHat.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Hard Hat was nto Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheJSIMainClass.InsertJSIPPE(MainWindow.gintJSITransationID, gstrSafetyVest, gstrGlassesGloves, gstrFootGear, gstrFallProtection, gstrHardHat);

                if (blnFatalError == true)
                    throw new Exception();

                EnableVehicle(true);
                EnablePPE(false);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // JSI PPE Window // Process PPE Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcessVehicle_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheJSIMainClass.InsertJSIVehicle(MainWindow.gintJSITransationID, gblnMagnets, gblnCleanliness, gblnSecure, gblnMaintenance, gblnExtingisher, MainWindow.gintVehicleID);

                if (blnFatalError == true)
                    throw new Exception();

                EnableLadder(true);
                EnableVehicle(false);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // JSI PPE Window // Process Vehicle Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcessLadders_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strErrorMessage = "";

            try
            {
                if(cboExtensionLadder.SelectedIndex < 1)
                {
                    blnFatalError = true;

                    strErrorMessage += "The Extension Ladder is not Selected\n";
                }
                if(cboSmallerLadder.SelectedIndex < 1)
                {
                    blnFatalError = true;

                    strErrorMessage += "The Smaller Ladder is not Selected\n";
                }
                if(cboRatingStickers.SelectedIndex < 1)
                {
                    blnFatalError = true;

                    strErrorMessage += "The Rating Stickers is not Selected\n";
                }
                if(cboCompanyLadders.SelectedIndex < 1)
                {
                    blnFatalError = true;

                    strErrorMessage += "The Company Ladders is not Selected\n";
                }
                if(cboLadderLocks.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Ladder Racks is not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheJSIMainClass.InsertJSILadderInspection(MainWindow.gintJSITransationID, gstrExtensionLadder, gstrSmallerLadder, gstrRatingStickers, gstrCompanyLadders, gstrLadderLocks);

                if (blnFatalError == true)
                    throw new Exception();

                EnableUniform(true);
                EnableLadder(false);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // JSI PPE Window // Process Ladders Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcessUniform_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheJSIMainClass.InsertJSIUniform(MainWindow.gintJSITransationID, gblnUniform, gblnLicense, gblnBlueJayLogo, gblnIDBadge);

                if (blnFatalError == true)
                    throw new Exception();

                JSIFinalWindow JSIFinalWindow = new JSIFinalWindow();
                JSIFinalWindow.ShowDialog();

                this.Close();

            }
            catch (Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // JSI PPE Window // Process Uniform Button " + ex.Message);

                TheMessagesClass.ErrorMessage(ex.ToString());
            }
        }
    }
}
