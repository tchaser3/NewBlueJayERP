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
using NewEmployeeDLL;
using NewEventLogDLL;
using InventoryWIPDLL;

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        //setting up the public classes
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();
        public static FindWIPBySessionIDDataSet TheFindWIPBySessionIDDataSet = new FindWIPBySessionIDDataSet();
        public static FindSessionByEmployeeIDDataSet TheFindSessionByEmployeeIDDataSet = new FindSessionByEmployeeIDDataSet();

        //setting global variables
        public static bool gblnLoggedIn;
        public static string gstrEmployeeGroup;
        public static int gintWarehouseID;
        public static bool gblnReceiveMaterial;
        public static bool gblnIssueMaterial;
        public static bool gblnProcessBOM;
        public static int gintSessionID;
        public static string gstrWarehouseName;

        //setting up global variables for windows
        public static CompanyProjectFootages CompanyProjectFootagesWindows = new CompanyProjectFootages();
        public static ProjectProductivityReport ProjectProductivityReportWindow = new ProjectProductivityReport();
        public static DepartmentProductionEmail DepartmentProductionEmailWindow = new DepartmentProductionEmail();
        public static AddProject AddProjectWindow = new AddProject();
        public static CreatePurchaseRequest CreateSearchRequestWindow = new CreatePurchaseRequest();
        public static EmployeeHoursPunched EmployeeHoursPunchedWindow = new EmployeeHoursPunched();
        public static ManagerHourlyDailyReport ManagerHourlyDailyReportWindow = new ManagerHourlyDailyReport();
        public static ImportGEOFenceReport ImportGEOFenceReportWindow = new ImportGEOFenceReport();
        public static VehicleUsageReport VehicleUsageReportWindow = new VehicleUsageReport();
        public static EmployeeProjectLaborReport EmployeeProjectLaborReportWindow = new EmployeeProjectLaborReport();
        public static SelectWarehouse SelectWarehouseWindow = new SelectWarehouse();
        public static EnterInventory EnterInventoryWindow = new EnterInventory();
        public static VehicleRoster VehicleRosterWindow = new VehicleRoster();
        public static CreateRental CreateRentalWindow = new CreateRental();

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
        
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetExpandedMenu();
        }
        private void ResetExpandedMenu()
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }
        private void ResetWindows()
        {
            CompanyProjectFootagesWindows.Visibility = Visibility.Hidden;
            ProjectProductivityReportWindow.Visibility = Visibility.Hidden;
            DepartmentProductionEmailWindow.Visibility = Visibility.Hidden;
            AddProjectWindow.Visibility = Visibility.Hidden;
            CreateSearchRequestWindow.Visibility = Visibility.Hidden;
            EmployeeHoursPunchedWindow.Visibility = Visibility.Hidden;
            ManagerHourlyDailyReportWindow.Visibility = Visibility.Hidden;
            ImportGEOFenceReportWindow.Visibility = Visibility.Hidden;
            VehicleUsageReportWindow.Visibility = Visibility.Hidden;
            EmployeeProjectLaborReportWindow.Visibility = Visibility.Hidden;
            SelectWarehouseWindow.Visibility = Visibility.Hidden;
            EnterInventoryWindow.Visibility = Visibility.Hidden;
            VehicleRosterWindow.Visibility = Visibility.Hidden;
            CreateRentalWindow.Visibility = Visibility.Hidden;
        }
        private void expEmployees_Expanded(object sender, RoutedEventArgs e)
        {
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expEmployeeDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployeeAdministration.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
        }

        private void expEmployeeReports_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
        }

        private void expEmployeeAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
        }

        private void expProjects_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expProjectDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
        }

        private void expProjectReports_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
        }

        private void expProjectAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
        }

        private void expInventory_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expInventoryDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expInventoryReports_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expInventoryAdministration_Expanded(object sender, RoutedEventArgs e)
        {
                       
        }

        private void expVehicles_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expVehicleDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expVehicleReports_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expVehicleAdminstration_Expanded(object sender, RoutedEventArgs e)
        {
           
        }

        private void expTrailers_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expTrailerDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expTrailerReports_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expTrailerAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expTools_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expToolsDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expToolReports_Expanded(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void expToolAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expAssetDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expAssets_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expAssentReports_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expInformationTechology_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expTasks.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expITDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expITReports_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expPhoneAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EmployeeSignsIn();
        }
        private void EmployeeSignsIn()
        {
            ResetSecurity();
            ResetWindows();

            EmployeeLogin EmployeeLogin = new EmployeeLogin();
            EmployeeLogin.ShowDialog();

            SetEmployeeSecurity();
        }
        private void ResetSecurity()
        {
            expAssentReports.IsEnabled = true;
            expAssetDataEntry.IsEnabled = true;
            expAssets.IsEnabled = true;
            expEmployeeAdministration.IsEnabled = true;
            expEmployeeDataEntry.IsEnabled = true;
            expEmployeeReports.IsEnabled = true;
            expEmployees.IsEnabled = true;
            expProjects.IsEnabled = true;
            expProjectAdministration.IsEnabled = true;
            expProjectDataEntry.IsEnabled = true;
            expProjectReports.IsEnabled = true;
            expInventory.IsEnabled = true;
            expInventoryAdministration.IsEnabled = true;
            expInventoryDataEntry.IsEnabled = true;
            expInventoryReports.IsEnabled = true;
            expVehicleAdminstration.IsEnabled = true;
            expVehicleDataEntry.IsEnabled = true;
            expVehicleReports.IsEnabled = true;
            expVehicles.IsEnabled = true;
            expToolAdministration.IsEnabled = true;
            expToolReports.IsEnabled = true;
            expToolsDataEntry.IsEnabled = true;
            expTools.IsEnabled = true;
            expInformationTechology.IsEnabled = true;
            expITDataEntry.IsEnabled = true;
            expITReports.IsEnabled = true;
            expCompanyFootages.IsEnabled = true;
        }
        private void SetEmployeeSecurity()
        {
            if(gstrEmployeeGroup == "USERS")
            {
                expInformationTechology.IsEnabled = false;
                expAssets.IsEnabled = false;
                expToolAdministration.IsEnabled = false;
                expToolsDataEntry.IsEnabled = false;
                expTrailerAdministration.IsEnabled = false;
                expTrailerDataEntry.IsEnabled = false;
                expVehicleAdminstration.IsEnabled = false;
                expVehicleDataEntry.IsEnabled = false;
                expInventory.IsEnabled = false;
                expEmployees.IsEnabled = false;
                expProjects.IsEnabled = false;
                expAssets.IsEnabled = false;
                expCompanyFootages.IsEnabled = false;
            }
            else if(gstrEmployeeGroup == "MANAGERS")
            {
                expAssets.IsEnabled = false;
                expToolAdministration.IsEnabled = false;
                expToolsDataEntry.IsEnabled = false;
                expTrailerAdministration.IsEnabled = false;
                expVehicleAdminstration.IsEnabled = false;
                expInventoryDataEntry.IsEnabled = false;
                expInventoryAdministration.IsEnabled = false;
                expToolAdministration.IsEnabled = false;
                expVehicleAdminstration.IsEnabled = false;
                expITDataEntry.IsEnabled = false;
                expAssets.IsEnabled = false;
                expPhoneAdministration.IsEnabled = false;
                expProjectAdministration.IsEnabled = false;
                expEmployeeAdministration.IsEnabled = false;
            }
            else if(gstrEmployeeGroup == "OFFICE")
            {
                expAssets.IsEnabled = false;
                expToolAdministration.IsEnabled = false;
                expToolsDataEntry.IsEnabled = false;
                expTrailerAdministration.IsEnabled = false;
                expVehicleAdminstration.IsEnabled = false;
                expInventoryDataEntry.IsEnabled = false;
                expInventoryAdministration.IsEnabled = false;
                expToolAdministration.IsEnabled = false;
                expVehicleAdminstration.IsEnabled = false;
                expITDataEntry.IsEnabled = false;
                expAssets.IsEnabled = false;
                expPhoneAdministration.IsEnabled = false;
                expProjectAdministration.IsEnabled = false;
                expEmployeeAdministration.IsEnabled = false;
                expCompanyFootages.IsEnabled = false;
            }
            else if(gstrEmployeeGroup == "WAREHOUSE")
            {
                expEmployees.IsEnabled = false;
                expProjects.IsEnabled = false;
                expToolAdministration.IsEnabled = false;
                expTrailerAdministration.IsEnabled = false;
                expVehicleAdminstration.IsEnabled = false;
                expInventoryAdministration.IsEnabled = false;
                expToolAdministration.IsEnabled = false;
                expVehicleAdminstration.IsEnabled = false;
                expITDataEntry.IsEnabled = false;
                expPhoneAdministration.IsEnabled = false;
                expProjectAdministration.IsEnabled = false;
                expEmployeeAdministration.IsEnabled = false;
                expCompanyFootages.IsEnabled = false;
            }
            else if(gstrEmployeeGroup == "SUPER USER")
            {
                expEmployeeAdministration.IsEnabled = false;
                expProjectAdministration.IsEnabled = false;
                expInventoryAdministration.IsEnabled = false;
                expVehicleAdminstration.IsEnabled = false;
                expTrailerAdministration.IsEnabled = false;
                expToolAdministration.IsEnabled = false;
                expPhoneAdministration.IsEnabled = false;                
            }
            else if((gstrEmployeeGroup == "ADMIN") || (gstrEmployeeGroup == "IT"))
            {
                TheMessagesClass.InformationMessage("Your are an Administrator of the Program");
            }
            else
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There Has Been an illegal entry into the Blue Jay ERP Program");

                TheMessagesClass.ErrorMessage("You Have Failed Trying To Break In The Program, IT Has Been Alerted");

                Application.Current.Shutdown();
            }
        }

        private void expClose_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void expTasks_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expTasks.IsExpanded = false;
        }

        private void expSignOut_Expanded(object sender, RoutedEventArgs e)
        {
            EmployeeSignsIn();
        }

        private void expHelpSite_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
            ResetExpandedMenu();
        }

        private void expCreateHelpDeskTicket_Expanded(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
            ResetExpandedMenu();
        }

        private void expProjectDashboards_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
        }

        private void expCompanyFootages_Expanded(object sender, RoutedEventArgs e)
        {
            CompanyProjectFootagesWindows.Visibility = Visibility.Visible;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void expProjectProductivityReport_Expanded(object sender, RoutedEventArgs e)
        {
            ProjectProductivityReportWindow.Visibility = Visibility.Visible;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void expDepartmentProductionEmail_Expanded(object sender, RoutedEventArgs e)
        {
            DepartmentProductionEmailWindow.Visibility = Visibility.Visible;
            expProjectAdministration.IsExpanded = false;
            expProjects.IsExpanded = false;
            expDepartmentProductionEmail.IsExpanded = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void expAddNewProject_Expanded(object sender, RoutedEventArgs e)
        {
            expAddNewProject.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
            AddProjectWindow.Visibility = Visibility.Visible;
        }

        private void expCreatedPurchaseRequest_Expanded(object sender, RoutedEventArgs e)
        {
            expInventory.IsExpanded = false;
            expPurchasing.IsExpanded = false;
            expCreatedPurchaseRequest.IsExpanded = false;
            CreateSearchRequestWindow.Visibility = Visibility.Visible;
        }

        private void expEmployeeHoursPunched_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployeeHoursPunched.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            EmployeeHoursPunchedWindow.Visibility = Visibility.Visible;
        }

        private void expManagerHourlyDailyReport_Expanded(object sender, RoutedEventArgs e)
        {
            expManagerHourlyDailyReport.IsExpanded = false;
            expEmployeeRoster.IsExpanded = false;
            expEmployees.IsExpanded = false;
            ManagerHourlyDailyReportWindow.Visibility = Visibility.Visible;
        }

        private void expImportGEOFenceReport_Expanded(object sender, RoutedEventArgs e)
        {
            expVehicleReports.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expImportGEOFenceReport.IsExpanded = false;
            ImportGEOFenceReportWindow.Visibility = Visibility.Visible;
        }

        private void expVehicleUsageReport_Expanded(object sender, RoutedEventArgs e)
        {
            expVehicles.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expVehicleUsageReport.IsExpanded = false;
            VehicleUsageReportWindow.Visibility = Visibility.Visible;
        }

        private void expEmployeeProjectLaborReport_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployeeProjectLaborReport.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            EmployeeProjectLaborReportWindow.Visibility = Visibility.Visible;
        }

        private void expIssueMaterial_Expanded(object sender, RoutedEventArgs e)
        {
            gblnIssueMaterial = true;
            gblnProcessBOM = false;
            gblnReceiveMaterial = false;
            expIssueMaterial.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            SelectWarehouseWindow.Visibility = Visibility.Visible;
        }

        private void expReceiveMaterial_Expanded(object sender, RoutedEventArgs e)
        {
            gblnIssueMaterial = false;
            gblnProcessBOM = false;
            gblnReceiveMaterial = true;
            expReceiveMaterial.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            SelectWarehouseWindow.Visibility = Visibility.Visible;
        }

        private void expProcessBOMMaterial_Expanded(object sender, RoutedEventArgs e)
        {
            gblnIssueMaterial = false;
            gblnProcessBOM = true;
            gblnReceiveMaterial = false;
            expProcessBOMMaterial.IsExpanded = false;
            expInventory.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            SelectWarehouseWindow.Visibility = Visibility.Visible;
        }

        private void expVehicledRoster_Expanded(object sender, RoutedEventArgs e)
        {
            expVehicles.IsExpanded = false;
            expVehicledRoster.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            VehicleRosterWindow.Visibility = Visibility.Visible;
        }

        private void expCreateRental_Expanded(object sender, RoutedEventArgs e)
        {
            expInventory.IsExpanded = false;
            expPurchasing.IsExpanded = false;
            expCreateRental.IsExpanded = false;
            CreateRentalWindow.Visibility = Visibility.Visible;
        }
    }
}
