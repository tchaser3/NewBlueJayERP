﻿using System;
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
using RentalTrackingDLL;
using InspectionsDLL;
using System.Security.Policy;
using System.Runtime.CompilerServices;

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
        public static FindRentalTrackingTransactionsByPONumberDataSet TheFindRentalTrackingTransactionsByPONumberDataSet = new FindRentalTrackingTransactionsByPONumberDataSet();
        public static FindRentalTransactionByProjectIDDataSet TheFindRentalTransactionByProjectIDDataSet = new FindRentalTransactionByProjectIDDataSet();

        //setting global variables
        public static bool gblnLoggedIn;
        public static string gstrEmployeeGroup;
        public static int gintWarehouseID;
        public static bool gblnReceiveMaterial;
        public static bool gblnIssueMaterial;
        public static bool gblnProcessBOM;
        public static int gintSessionID;
        public static string gstrWarehouseName;
        public static int gintEmployeeID;
        public static int gintVendorID;
        public static DateTime gdatTransactionDate;
        public static int gintProjectID;
        public static string gstrAssignedProjectID;
        public static int gintRentalTrackingID;
        public static string gstrAgreementNo;
        public static int gintVehicleID;
        public static string gstrVehicleNumber;
        public static int gintJSITransationID;
        public static int gintDepartmentID;
        public static int gintManagerID;
        public static int gintInspectingEmployeeID;
        public static bool gblnRentalPO;
        public static DateTime gdatInspectionDate;
        public static int gintTicketID;
        public static int gintProblemTypeID;
        public static string gstrTicketStatus;
        public static DateTime gdatStartDate;
        public static DateTime gdatEndDate;
        public static int gintProblemID;
        public static int gintPartID;
        public static int gintCategoryID;
        public static int gintToolKey;

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
        public static CreateJSIEntry CreateJSIEntryWindow = new CreateJSIEntry();
        public static UpdateRental UpdateRentalWindow = new UpdateRental();
        public static CloseRental CloseRentalWindow = new CloseRental();
        public static CreateFuelCardNumber CreateFuelCardNumberWindow = new CreateFuelCardNumber();
        public static EditFuelCard EditFuelCardWindow = new EditFuelCard();
        public static FuelCardPINReport FuelCardPINReportWindow = new FuelCardPINReport();
        public static ManuallyAddFuelPIN ManuallyAddFuelPINWindow = new ManuallyAddFuelPIN();
        public static OpenRentalReport OpenRentalReportWindow = new OpenRentalReport();
        public static ExpiringRentals ExpiringRentalsWindow = new ExpiringRentals();
        public static ViewRental ViewRentalWindow = new ViewRental();
        public static ImportVendors ImportVendorsWindow = new ImportVendors();
        public static SubmitAfterHoursWork SubmitAfterHoursWorkWindow = new SubmitAfterHoursWork();
        public static ImportTowMotors ImportTowMotorWindow = new ImportTowMotors();
        public static ImportAssets ImportAssetsWindow = new ImportAssets();
        public static CreateAssetType CreateAssetTypeWindow = new CreateAssetType();
        public static CreateAsset CreateAssetWindow = new CreateAsset();
        public static CreateHelpDeskProblemType CreateHelpDeskProblemTypeWindow = new CreateHelpDeskProblemType();
        public static UpdateHelpDeskTickets UpdateHelpDeskTicketsWindow = new UpdateHelpDeskTickets();
        public static TowMotorInspection TowMotorInspectionWindow = new TowMotorInspection();
        public static ViewMyOpenHelpDeskTickets ViewMyOpenHelpDeskTicketsWindow = new ViewMyOpenHelpDeskTickets();
        public static ProjectsProductivityCosting ProjectsProductivityCostingWindow = new ProjectsProductivityCosting();
        public static AddEmployeeToVehicleEmails AddEmployeeToVehicleEmailsWindow = new AddEmployeeToVehicleEmails();
        public static EditVehicleProblems EditVehicleProblemsWindow = new EditVehicleProblems();
        public static AddCableReel AddCableReelWindow = new AddCableReel();
        public static RemoveDuplicateProjectMatrix RemoveDuplicateProjectMatrixWindow = new RemoveDuplicateProjectMatrix();
        public static EditProjects EditProjectsWindow = new EditProjects();
        public static HelpDeskTicketReport HelpDeskTicketReportWindow = new HelpDeskTicketReport();
        public static CreateSpectrumReport CreateSpectrumReportWindow = new CreateSpectrumReport();
        public static WarehouseInventoryReport WarehouseInventoryReportWindow = new WarehouseInventoryReport();
        public static AddInventoryLocation AddInventoryLocationWindow = new AddInventoryLocation();
        public static AddNewTool AddNewToolWindow = new AddNewTool();
        public static EditTool EditToolWindow = new EditTool();
        public static PartsList PartsListWindow = new PartsList();
        public static SortedWorkTaskReport SortedWorkTaskReportWindow = new SortedWorkTaskReport();
        public static PartLookup PartLookupWindow = new PartLookup();
        public static ServerAuditLog ServerAuditLogWindow = new ServerAuditLog();

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
            expRentals.IsExpanded = false;
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
            CreateJSIEntryWindow.Visibility = Visibility.Hidden;
            UpdateRentalWindow.Visibility = Visibility.Hidden;
            CloseRentalWindow.Visibility = Visibility.Hidden;
            CreateFuelCardNumberWindow.Visibility = Visibility.Hidden;
            EditFuelCardWindow.Visibility = Visibility.Hidden;
            FuelCardPINReportWindow.Visibility = Visibility.Hidden;
            ManuallyAddFuelPINWindow.Visibility = Visibility.Hidden;
            OpenRentalReportWindow.Visibility = Visibility.Hidden;
            ExpiringRentalsWindow.Visibility = Visibility.Hidden;
            ViewRentalWindow.Visibility = Visibility.Hidden;
            ImportVendorsWindow.Visibility = Visibility.Hidden;
            SubmitAfterHoursWorkWindow.Visibility = Visibility.Hidden;
            ImportTowMotorWindow.Visibility = Visibility.Hidden;
            ImportAssetsWindow.Visibility = Visibility.Hidden;
            CreateAssetTypeWindow.Visibility = Visibility.Hidden;
            CreateAssetWindow.Visibility = Visibility.Hidden;
            CreateHelpDeskProblemTypeWindow.Visibility = Visibility.Hidden;
            UpdateHelpDeskTicketsWindow.Visibility = Visibility.Hidden;
            TowMotorInspectionWindow.Visibility = Visibility.Hidden;
            ViewMyOpenHelpDeskTicketsWindow.Visibility = Visibility.Hidden;
            ProjectsProductivityCostingWindow.Visibility = Visibility.Hidden;
            AddEmployeeToVehicleEmailsWindow.Visibility = Visibility.Hidden;
            EditVehicleProblemsWindow.Visibility = Visibility.Hidden;
            AddCableReelWindow.Visibility = Visibility.Hidden;
            RemoveDuplicateProjectMatrixWindow.Visibility = Visibility.Hidden;
            EditProjectsWindow.Visibility = Visibility.Hidden;
            HelpDeskTicketReportWindow.Visibility = Visibility.Hidden;
            CreateSpectrumReportWindow.Visibility = Visibility.Hidden;
            WarehouseInventoryReportWindow.Visibility = Visibility.Hidden;
            AddInventoryLocationWindow.Visibility = Visibility.Hidden;
            AddNewToolWindow.Visibility = Visibility.Hidden;
            EditToolWindow.Visibility = Visibility.Hidden;
            PartsListWindow.Visibility = Visibility.Hidden;
            SortedWorkTaskReportWindow.Visibility = Visibility.Hidden;
            PartLookupWindow.Visibility = Visibility.Hidden;
            ServerAuditLogWindow.Visibility = Visibility.Hidden;
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
            expRentals.IsExpanded = false;
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
            expRentals.IsExpanded = false;
        }

        private void expProjectDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
        }

        private void expProjectReports_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
        }

        private void expProjectAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
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
            expRentals.IsExpanded = false;
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
            expRentals.IsExpanded = false;
        }

        private void expVehicleDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expVehicleReports_Expanded(object sender, RoutedEventArgs e)
        {
            
        }

        private void expVehicleAdminstration_Expanded(object sender, RoutedEventArgs e)
        {
            expVehicleDataEntry.IsExpanded = false;
            expVehicleReports.IsExpanded = false;
            expInspectionDataEntry.IsExpanded = false;
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
            expRentals.IsExpanded = false;
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
            expRentals.IsExpanded = false;
        }

        private void expToolsDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expToolAdministration.IsExpanded = false;
            expToolProblems.IsExpanded = false;
            expToolReports.IsExpanded = false;
        }

        private void expToolReports_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expToolProblems.IsExpanded = false;
            expToolAdministration.IsExpanded = false;
            
        }

        private void expToolAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expToolsDataEntry.IsExpanded = false;
            expToolProblems.IsExpanded = false;
            expToolReports.IsExpanded = false;
        }

        private void expAssetDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expAssentReports.IsExpanded = false;
            expAssetAdministration.IsExpanded = false;
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
            expRentals.IsExpanded = false;
        }

        private void expAssentReports_Expanded(object sender, RoutedEventArgs e)
        {
            expAssetDataEntry.IsExpanded = false;
            expAssetAdministration.IsExpanded = false;
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
            expRentals.IsExpanded = false;
        }

        private void expITDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expITReports.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
        }

        private void expITReports_Expanded(object sender, RoutedEventArgs e)
        {
            expITDataEntry.IsExpanded = false;
            expPhoneAdministration.IsExpanded = false;
        }

        private void expPhoneAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expITDataEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
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
            expAssetAdministration.IsEnabled = true;
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
            expRentals.IsEnabled = true;
            expEditVehicleProblems.IsEnabled = true;
            expHelpDeskTicketsReport.IsEnabled = true;
            expServerAuditLogReport.IsExpanded = true;
            expToolProblems.IsEnabled = true;
        }
        private void SetEmployeeSecurity()
        {
            try
            {
                if (gstrEmployeeGroup == "USERS")
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
                    expRentals.IsEnabled = false;
                    expEditVehicleProblems.IsEnabled = false;
                    expHelpDeskTicketsReport.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                    expToolProblems.IsEnabled = false;
                }
                else if (gstrEmployeeGroup == "MANAGERS")
                {
                    expAssets.IsEnabled = false;
                    expToolAdministration.IsEnabled = false;
                    expToolsDataEntry.IsEnabled = false;
                    expTrailerAdministration.IsEnabled = false;
                    expVehicleAdminstration.IsEnabled = false;
                    expInventoryAdministration.IsEnabled = false;
                    expToolAdministration.IsEnabled = false;
                    expVehicleAdminstration.IsEnabled = false;
                    expITDataEntry.IsEnabled = false;
                    expAssets.IsEnabled = false;
                    expPhoneAdministration.IsEnabled = false;
                    expProjectAdministration.IsEnabled = false;
                    expEmployeeAdministration.IsEnabled = false;
                    expEditVehicleProblems.IsEnabled = false;
                    expHelpDeskTicketsReport.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                    expToolProblems.IsEnabled = false;
                }
                else if (gstrEmployeeGroup == "OFFICE")
                {
                    expAssets.IsEnabled = false;
                    expToolAdministration.IsEnabled = false;
                    expToolsDataEntry.IsEnabled = false;
                    expTrailerAdministration.IsEnabled = false;
                    expVehicleAdminstration.IsEnabled = false;
                    expInventoryAdministration.IsEnabled = false;
                    expVehicleAdminstration.IsEnabled = false;
                    expITDataEntry.IsEnabled = false;
                    expAssets.IsEnabled = false;
                    expPhoneAdministration.IsEnabled = false;
                    expProjectAdministration.IsEnabled = false;
                    expEmployeeAdministration.IsEnabled = false;
                    expCompanyFootages.IsEnabled = false;
                    expEditVehicleProblems.IsEnabled = false;
                    expHelpDeskTicketsReport.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                    expToolProblems.IsEnabled = false;
                }
                else if (gstrEmployeeGroup == "WAREHOUSE")
                {
                    expAssetAdministration.IsEnabled = false;
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
                    expEditVehicleProblems.IsEnabled = false;
                    expHelpDeskTicketsReport.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                }
                else if (gstrEmployeeGroup == "SUPER USER")
                {
                    expAssetAdministration.IsEnabled = false;
                    expEmployeeAdministration.IsEnabled = false;
                    expProjectAdministration.IsEnabled = false;
                    expInventoryAdministration.IsEnabled = false;
                    expVehicleAdminstration.IsEnabled = false;
                    expTrailerAdministration.IsEnabled = false;
                    expToolAdministration.IsEnabled = false;
                    expPhoneAdministration.IsEnabled = false;
                    expEditVehicleProblems.IsEnabled = false;
                    expHelpDeskTicketsReport.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                }
                else if ((gstrEmployeeGroup == "ADMIN") || (gstrEmployeeGroup == "IT"))
                {
                    TheMessagesClass.InformationMessage("Your are an Administrator of the Program");
                }
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Main Window // Set Employee Security " + Ex.Message);
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
            expRentals.IsExpanded = false;
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
            expRentals.IsExpanded = false;
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
            expJSIDataEntry.IsExpanded = false;
        }

        private void expCompanyFootages_Expanded(object sender, RoutedEventArgs e)
        {
            CompanyProjectFootagesWindows.Visibility = Visibility.Visible;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expAllProjectProductivityCosting.IsExpanded = false;
            expSortedWorkTasksReport.IsExpanded = false;
            expEmployeeProjectLaborReport.IsExpanded = false; 
        }

        private void expProjectProductivityReport_Expanded(object sender, RoutedEventArgs e)
        {
            ProjectProductivityReportWindow.Visibility = Visibility.Visible;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expEmployeeProjectLaborReport.IsExpanded = false;
            expCompanyFootages.IsExpanded = false;
            expAllProjectProductivityCosting.IsExpanded = false;
            expSortedWorkTasksReport.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
        }

        private void expDepartmentProductionEmail_Expanded(object sender, RoutedEventArgs e)
        {
            DepartmentProductionEmailWindow.Visibility = Visibility.Visible;
            expProjectAdministration.IsExpanded = false;
            expProjects.IsExpanded = false;
            expDepartmentProductionEmail.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
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
            expSortedWorkTasksReport.IsExpanded = false;
            expProjects.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expCompanyFootages.IsExpanded = false;
            expProjectProductivityReport.IsExpanded = false;
            expAllProjectProductivityCosting.IsExpanded = false;
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
            
        }

        private void expRentals_Expanded(object sender, RoutedEventArgs e)
        {
            expEmployees.IsExpanded = false;
            expProjects.IsExpanded = false;
            expInventory.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expTrailers.IsExpanded = false;
            expTools.IsExpanded = false;
            expAssets.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expTasks.IsExpanded = false;
            expITReports.IsExpanded = false;
            expHelp.IsExpanded = false;
        }

        private void exRemtalDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expRentalAdministration.IsExpanded = false;
            expRentalReports.IsExpanded = false;
        }

        private void expCreateRental_Expanded_1(object sender, RoutedEventArgs e)
        {
            expCreateRental.IsExpanded = false;
            expUpdateRental.IsExpanded = false;
            exRemtalDataEntry.IsExpanded = false;
            expRentals.IsExpanded = false;
            CreateRentalWindow.Visibility = Visibility.Visible;
        }

        private void expUpdateRental_Expanded(object sender, RoutedEventArgs e)
        {
            expCreateRental.IsExpanded = false;
            expUpdateRental.IsExpanded = false;
            expRentals.IsExpanded = false;
            exRemtalDataEntry.IsExpanded = false;
            UpdateRentalWindow.Visibility = Visibility.Visible;
        }

        private void expAddRentalItems_Expanded(object sender, RoutedEventArgs e)
        {
            expCreateRental.IsExpanded = false;
            expUpdateRental.IsExpanded = false;
        }

        private void expUpdateRentalAgreement_Expanded(object sender, RoutedEventArgs e)
        {
            expCreateRental.IsExpanded = false;
            expUpdateRental.IsExpanded = false;
        }

        private void expRentalReports_Expanded(object sender, RoutedEventArgs e)
        {
            exRemtalDataEntry.IsExpanded = false;
            expRentalAdministration.IsExpanded = false;
        }

        private void expRentalAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            exRemtalDataEntry.IsExpanded = false;
            expRentalReports.IsExpanded = false;
        }

        private void expCreateJSI_Expanded(object sender, RoutedEventArgs e)
        {
            expProjects.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectDashboards.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
            expCreateJSI.IsExpanded = false;
            CreateJSIEntryWindow.Visibility = Visibility.Visible;
        }

        private void expJSIDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectAdministration.IsExpanded = false;
            expProjectDashboards.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
        }

        private void expCloseRental_Expanded(object sender, RoutedEventArgs e)
        {
            expCreateRental.IsExpanded = false;
            expUpdateRental.IsExpanded = false;
            exRemtalDataEntry.IsExpanded = false;
            CloseRentalWindow.Visibility = Visibility.Visible;
            expCloseRental.IsExpanded = false;
            expRentals.IsExpanded = false;
        }

        private void expCreateFuelCardNumber_Expanded(object sender, RoutedEventArgs e)
        {
            expAddDepartment.IsExpanded = false;
            expAddEmployee.IsExpanded = false;
            expAddEmployeeGroups.IsExpanded = false;
            expAddEmployeeToVehicleEmailList.IsExpanded = false;
            expEditEmployee.IsExpanded = false;
            expEmployeeLaborRate.IsExpanded = false;
            expImportEmployeeHours.IsExpanded = false;
            expImportEmployeePunches.IsExpanded = false;
            expImportEmployeeHours.IsExpanded = false;
            expTerminateEmployee.IsExpanded = false;
            expCreateFuelCardNumber.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expEditFuelCard.IsExpanded = false;
            expFuelCardPINReport.IsExpanded = false;
            expManuallAddFuelPin.IsExpanded = false;
            CreateFuelCardNumberWindow.Visibility = Visibility.Visible;
        }

        private void expEditFuelCard_Expanded(object sender, RoutedEventArgs e)
        {
            expAddDepartment.IsExpanded = false;
            expAddEmployee.IsExpanded = false;
            expAddEmployeeGroups.IsExpanded = false;
            expAddEmployeeToVehicleEmailList.IsExpanded = false;
            expEditEmployee.IsExpanded = false;
            expEmployeeLaborRate.IsExpanded = false;
            expImportEmployeeHours.IsExpanded = false;
            expImportEmployeePunches.IsExpanded = false;
            expImportEmployeeHours.IsExpanded = false;
            expTerminateEmployee.IsExpanded = false;
            expCreateFuelCardNumber.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expEditFuelCard.IsExpanded = false;
            expFuelCardPINReport.IsExpanded = false;
            expManuallAddFuelPin.IsExpanded = false;
            EditFuelCardWindow.Visibility = Visibility.Visible;
        }

        private void expFuelCardPINReport_Expanded(object sender, RoutedEventArgs e)
        {
            expAddDepartment.IsExpanded = false;
            expAddEmployee.IsExpanded = false;
            expAddEmployeeGroups.IsExpanded = false;
            expAddEmployeeToVehicleEmailList.IsExpanded = false;
            expEditEmployee.IsExpanded = false;
            expEmployeeLaborRate.IsExpanded = false;
            expImportEmployeeHours.IsExpanded = false;
            expImportEmployeePunches.IsExpanded = false;
            expImportEmployeeHours.IsExpanded = false;
            expTerminateEmployee.IsExpanded = false;
            expCreateFuelCardNumber.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expEditFuelCard.IsExpanded = false;
            expFuelCardPINReport.IsExpanded = false;
            expManuallAddFuelPin.IsExpanded = false;
            FuelCardPINReportWindow.Visibility = Visibility.Visible;
        }

        private void expManuallAddFuelPin_Expanded(object sender, RoutedEventArgs e)
        {
            expAddDepartment.IsExpanded = false;
            expAddEmployee.IsExpanded = false;
            expAddEmployeeGroups.IsExpanded = false;
            expAddEmployeeToVehicleEmailList.IsExpanded = false;
            expEditEmployee.IsExpanded = false;
            expEmployeeLaborRate.IsExpanded = false;
            expImportEmployeeHours.IsExpanded = false;
            expImportEmployeePunches.IsExpanded = false;
            expImportEmployeeHours.IsExpanded = false;
            expTerminateEmployee.IsExpanded = false;
            expCreateFuelCardNumber.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expEditFuelCard.IsExpanded = false;
            expFuelCardPINReport.IsExpanded = false;
            expManuallAddFuelPin.IsExpanded = false;
            ManuallyAddFuelPINWindow.Visibility = Visibility.Visible;
        }

        private void expOpenRentalReport_Expanded(object sender, RoutedEventArgs e)
        {
            expRentalReports.IsExpanded = false;
            expRentals.IsExpanded = false;
            expOpenRentalReport.IsExpanded = false;
            expExpiringRentalReport.IsExpanded = false;
            expViewRental.IsExpanded = false;

            OpenRentalReportWindow.Visibility = Visibility.Visible;
        }

        private void expExpiringRentalReport_Expanded(object sender, RoutedEventArgs e)
        {
            expRentalReports.IsExpanded = false;
            expRentals.IsExpanded = false;
            expOpenRentalReport.IsExpanded = false;
            expExpiringRentalReport.IsExpanded = false;
            expViewRental.IsExpanded = false;

            ExpiringRentalsWindow.Visibility = Visibility.Visible;
        }

        private void expViewRental_Expanded(object sender, RoutedEventArgs e)
        {
            expRentalReports.IsExpanded = false;
            expRentals.IsExpanded = false;
            expOpenRentalReport.IsExpanded = false;
            expExpiringRentalReport.IsExpanded = false;
            expViewRental.IsExpanded = false;

            ViewRentalWindow.Visibility = Visibility.Visible;
        }

        private void expImportVendors_Expanded(object sender, RoutedEventArgs e)
        {
            expRentals.IsExpanded = false;
            expRentalAdministration.IsExpanded = false;
            expImportVendors.IsExpanded = false;

            ImportVendorsWindow.Visibility = Visibility.Visible;
        }

        private void expSubmitAfterHoursWork_Expanded(object sender, RoutedEventArgs e)
        {
            expProjects.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expAddNewProject.IsExpanded = false;
            expEditProject.IsExpanded = false;
            expSubmitAfterHoursWork.IsExpanded = false;

            SubmitAfterHoursWorkWindow.Visibility = Visibility.Visible;
        }

        private void expImportTowMotors_Expanded(object sender, RoutedEventArgs e)
        {
            expVehicles.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expImportTowMotors.IsExpanded = false;
            ImportTowMotorWindow.Visibility = Visibility.Visible;
        }

        private void expAssetAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expAssentReports.IsExpanded = false;
            expAssetDataEntry.IsExpanded = false;
        }

        private void expImportAssets_Expanded(object sender, RoutedEventArgs e)
        {
            expAssets.IsExpanded = false;
            expAssetAdministration.IsExpanded = false;
            expImportAssets.IsExpanded = false;
            ImportAssetsWindow.Visibility = Visibility.Visible;
        }

        private void expCreateAssetType_Expanded(object sender, RoutedEventArgs e)
        {
            expAssets.IsExpanded = false;
            expAssetAdministration.IsExpanded = false;
            expCreateAssetType.IsExpanded = false;
            CreateAssetTypeWindow.Visibility = Visibility.Visible;
        }

        private void expCreateAsset_Expanded(object sender, RoutedEventArgs e)
        {
            expAssets.IsExpanded = false;
            expAssetAdministration.IsExpanded = false;
            expCreateAsset.IsExpanded = false;
            CreateAssetWindow.Visibility = Visibility.Visible;
        }

        private void expCreateHelpDeskProblemType_Expanded(object sender, RoutedEventArgs e)
        {
            expITDataEntry.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expCreateHelpDeskProblemType.IsExpanded = false;
            expUpdateHelpDeskTickets.IsExpanded = false;
            CreateHelpDeskProblemTypeWindow.Visibility = Visibility.Visible;
        }

        private void expUpdateHelpDeskTickets_Expanded(object sender, RoutedEventArgs e)
        {
            expITDataEntry.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expCreateHelpDeskProblemType.IsExpanded = false;
            expUpdateHelpDeskTickets.IsExpanded = false;
            UpdateHelpDeskTicketsWindow.Visibility = Visibility.Visible;
        }

        private void expTowMoterInspection_Expanded(object sender, RoutedEventArgs e)
        {
            expTowMoterInspection.IsExpanded = false;
            TowMotorInspectionWindow.Visibility = Visibility.Visible;
        }

        private void expHelpDesk_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDesk.IsExpanded = false;
            TheMessagesClass.LaunchHelpDeskTickets();

        }

        private void expMyOpenTickets_Expanded(object sender, RoutedEventArgs e)
        {
            expMyOpenTickets.IsExpanded = false;
            expMyTickets.IsExpanded = false;
            expITReports.IsExpanded = false;
            expHelpDeskTicketsReport.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            ViewMyOpenHelpDeskTicketsWindow.Visibility = Visibility.Visible;
        }

        private void expMyTickets_Expanded(object sender, RoutedEventArgs e)
        {
            expHelpDeskTicketsReport.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
            expITReports.IsExpanded = false;
            expMyOpenTickets.IsExpanded = false;
            expMyTickets.IsExpanded = false;
        }

        private void expAllProjectProductivityCosting_Expanded(object sender, RoutedEventArgs e)
        {
            expAllProjectProductivityCosting.IsExpanded = false;
            expCompanyFootages.IsExpanded = false;
            expProjectProductivityReport.IsExpanded = false;
            expEmployeeProjectLaborReport.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expSortedWorkTasksReport.IsExpanded = false;
            ProjectsProductivityCostingWindow.Visibility = Visibility.Visible;
        }

        private void expAddEmployeeToVehicleEmails_Expanded(object sender, RoutedEventArgs e)
        {
            expImportTowMotors.IsExpanded = false;
            expVehicles.IsExpanded = false;
            expVehicleAdminstration.IsExpanded = false;
            expAddEmployeeToVehicleEmailList.IsExpanded = false;
            AddEmployeeToVehicleEmailsWindow.Visibility = Visibility.Visible;
        }

        private void expEditVehicleProblems_Expanded(object sender, RoutedEventArgs e)
        {
            expVehicles.IsExpanded = false;
            expVehicleDataEntry.IsExpanded = false;
            expImportGEOFenceReport.IsExpanded = false;
            expEditVehicleProblems.IsExpanded = false;
            EditVehicleProblemsWindow.Visibility = Visibility.Visible;
        }

        private void expAddCableReel_Expanded(object sender, RoutedEventArgs e)
        {
            expInventory.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expIssueMaterial.IsExpanded = false;
            expReceiveMaterial.IsExpanded = false;
            expProcessBOMMaterial.IsExpanded = false;
            expReturnMaterial.IsExpanded = false;
            expViewCurrentSession.IsExpanded = false;
            AddCableReelWindow.Visibility = Visibility.Visible;
        }

        private void expEditProject_Expanded(object sender, RoutedEventArgs e)
        {
            expProjects.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expAddNewProject.IsExpanded = false;
            expSubmitAfterHoursWork.IsExpanded = false;
            expEditProject.IsExpanded = false;
            EditProjectsWindow.Visibility = Visibility.Visible;
        }

        private void expRemoveDuplicateProjectMatrix_Expanded(object sender, RoutedEventArgs e)
        {
            expProjects.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expDepartmentProductionEmail.IsExpanded = false;
            expRemoveDuplicateProjectMatrix.IsExpanded = false;
            RemoveDuplicateProjectMatrixWindow.Visibility = Visibility.Visible;
        }

        private void expHelpDeskTicketsReport_Expanded(object sender, RoutedEventArgs e)
        {
            expInformationTechology.IsExpanded = false;
            expITReports.IsExpanded = false;
            expMyOpenTickets.IsExpanded = false;
            expMyTickets.IsExpanded = false;
            expHelpDeskTicketsReport.IsExpanded = false;
            HelpDeskTicketReportWindow.Visibility = Visibility.Visible;
        }

        private void expCreateSpectrum_Expanded(object sender, RoutedEventArgs e)
        {
            InventoryReportExpanders();
            CreateSpectrumReportWindow.Visibility = Visibility.Visible;
        }

        private void expWarehouseInventoryReport_Expanded(object sender, RoutedEventArgs e)
        {
            InventoryReportExpanders();
            WarehouseInventoryReportWindow.Visibility = Visibility.Visible;
        }

        private void expAddInventoryLocation_Expanded(object sender, RoutedEventArgs e)
        {
            expInventory.IsExpanded = false;
            expInventoryDataEntry.IsExpanded = false;
            expAddCableReel.IsExpanded = false;
            expIssueMaterial.IsExpanded = false;
            expReceiveMaterial.IsExpanded = false;
            expProcessBOMMaterial.IsExpanded = false;
            expReturnMaterial.IsExpanded = false;
            expViewCurrentSession.IsExpanded = false;
            expAddInventoryLocation.IsExpanded = false;
            AddInventoryLocationWindow.Visibility = Visibility.Visible;
        }

        private void expAddNewTool_Expanded(object sender, RoutedEventArgs e)
        {
            expTools. IsExpanded = false;
            expToolsDataEntry.IsExpanded = false;
            expAddNewTool.IsExpanded = false;
            expEditTool.IsExpanded = false;
            expBulkToolSignIn.IsExpanded = false;
            expBulkToolSignOut.IsExpanded = false;
            expToolAvailability.IsExpanded = false;
            AddNewToolWindow.Visibility = Visibility.Visible;
        }

        private void expEditTool_Expanded(object sender, RoutedEventArgs e)
        {
            expTools.IsExpanded = false;
            expToolsDataEntry.IsExpanded = false;
            expAddNewTool.IsExpanded = false;
            expEditTool.IsExpanded = false;
            expBulkToolSignIn.IsExpanded = false;
            expBulkToolSignOut.IsExpanded = false;
            expToolAvailability.IsExpanded = false;
            EditToolWindow.Visibility = Visibility.Visible;
        }

        private void expPartList_Expanded(object sender, RoutedEventArgs e)
        {
            InventoryReportExpanders();
            PartsListWindow.Visibility = Visibility.Visible;
        }

        private void expSortedWorkTasksReport_Expanded(object sender, RoutedEventArgs e)
        {
            expProjects.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expCompanyFootages.IsExpanded = false;
            expProjectProductivityReport.IsExpanded = false;
            expEmployeeProjectLaborReport.IsExpanded = false;
            expAllProjectProductivityCosting.IsExpanded = false;
            expSortedWorkTasksReport.IsExpanded = false;
            SortedWorkTaskReportWindow.Visibility = Visibility.Visible;
        }

        private void expPartLookup_Expanded(object sender, RoutedEventArgs e)
        {
            InventoryReportExpanders();
            PartLookupWindow.Visibility = Visibility.Visible;
        }
        private void InventoryReportExpanders()
        {
            expInventory.IsExpanded = false;
            expInventoryReports.IsExpanded = false;
            expCreateSpectrum.IsExpanded = false;
            expPartList.IsExpanded = false;
            expPartLookup.IsExpanded = false;
            expWarehouseInventoryReport.IsExpanded = false;
        }

        private void expServerAuditLogReport_Expanded(object sender, RoutedEventArgs e)
        {
            ITReportsExpanders();
            ServerAuditLogWindow.Visibility = Visibility.Visible;
        }
        private void ITReportsExpanders()
        {
            expMyOpenTickets.IsExpanded = false;
            expMyTickets.IsExpanded = false;
            expHelpDeskTicketsReport.IsExpanded = false;
            expServerAuditLogReport.IsExpanded = false;
            expITReports.IsExpanded = false;
            expInformationTechology.IsExpanded = false;
        }
    }
}