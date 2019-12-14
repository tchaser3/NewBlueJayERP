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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void BtnEmail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMyTasks_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAssignTask_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetExpandedMenu();
        }
        private void ResetExpandedMenu()
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expEmployees_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expEmployeeDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expEmployeeReports_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expEmployeeAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expProjects_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expProjectDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expProjectReports_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expProjectAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expInventory_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expInventoryDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expInventoryReports_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expInventoryAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expVehicles_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expVehicleDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expVehicleReports_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expVehicleAdminstration_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expTrailers_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expTrailerDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expTrailerReports_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expTrailerAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expTools_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expToolsDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expToolReports_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expToolAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expAssetDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expAssets_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
        }

        private void expAssentReports_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expInformationTechology_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expITDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
        }

        private void expITReports_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
            expToolsDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
        }

        private void expPhoneAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailerAdministration.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expTrailerReports.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            expToolReports.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
            expAssets.IsExpanded = false;
            expToolsDataEntry.IsExpanded = false;
        }
    }
}
