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
using RentalTrackingDLL;
using InspectionsDLL;
using System.Security.Policy;
using System.Runtime.CompilerServices;
using ProjectMatrixDLL;
using DateSearchDLL;

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
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the public classes
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();
        public static FindWIPBySessionIDDataSet TheFindWIPBySessionIDDataSet = new FindWIPBySessionIDDataSet();
        public static FindSessionByEmployeeIDDataSet TheFindSessionByEmployeeIDDataSet = new FindSessionByEmployeeIDDataSet();
        public static FindRentalTrackingTransactionsByPONumberDataSet TheFindRentalTrackingTransactionsByPONumberDataSet = new FindRentalTrackingTransactionsByPONumberDataSet();
        public static FindRentalTransactionByProjectIDDataSet TheFindRentalTransactionByProjectIDDataSet = new FindRentalTransactionByProjectIDDataSet();
        ProjectLastDateDataSet TheProjectLastDateDataSet = new ProjectLastDateDataSet();
        FindProjectMatrixByGreaterDateDataSet TheFindProjectMatrixByGreaterDateDataSet = new FindProjectMatrixByGreaterDateDataSet();
        public static VerifyEmployeeDataSet TheVerifyEmployeeDataSet = new VerifyEmployeeDataSet();

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
        public static int gintTrailerID;
        public static string gstrFirstName;
        public static string gstrLastName;
        public static int gintWorkTaskID;
        public static string gstrWorkTask;
        public static bool gblnKeepNewEmployee;
        public static int gintTransactionID;
        public static string gstrLaborCode;

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
        public static ProjectShopAnalysis ProjectShopAnalysisWindow = new ProjectShopAnalysis();
        public static ImportInventory ImportInventoryWindow = new ImportInventory();
        public static UpdateTrailerProblems UpdateTrailerProblemsWindow = new UpdateTrailerProblems();
        public static AddDepartment AddDepartmentWindow = new AddDepartment();
        public static CreateToolProblem CreateToolProblemWindow = new CreateToolProblem();
        public static UpdateToolProblem UpdateToolProblemWindow = new UpdateToolProblem();
        public static ImportPrices ImportPricesWindow = new ImportPrices();
        public static JSIProjectReports JSIProjectReportWindow = new JSIProjectReports();
        public static VoidInventoryTransaction VoidInventoryTransactionWindow = new VoidInventoryTransaction();
        public static EmployeeRoster EmployeeRosterWindow = new EmployeeRoster();
        public static EmployeeLookup EmployeeLookupWindow = new EmployeeLookup();
        public static AddProjectLabor AddProjectLaborWindow = new AddProjectLabor();
        public static AddEmployee AddEmployeeWindow = new AddEmployee();
        public static AddEmployeeGroup AddEmployeeGroupWindow = new AddEmployeeGroup();
        public static EditEmployee EditEmployeeWindow = new EditEmployee();
        public static AddEmployeeLaborRate AddEmployeeLaborRateWindow = new AddEmployeeLaborRate();
        public static ImportEmployeeHours ImportEmployeeHoursWindow = new ImportEmployeeHours();
        public static ImportEmployeePunches ImportEmployeePunchesWindow = new ImportEmployeePunches();
        public static ITCreateHelpDeskTicket ITCreateHelpDeskTicketWindow = new ITCreateHelpDeskTicket();
        public static DepartmentProjectOpenList DepartmentProjectOpenListWindow = new DepartmentProjectOpenList();
        public static UpdateProject UpdateProjectWindow = new UpdateProject();
        public static OpenProjectDashboard OpenProjectDashboardWindow = new OpenProjectDashboard();
        public static ImportProductionCodes ImportProductionCodesWindow = new ImportProductionCodes();
        public static OverdueProjectDashboard OverdueProjectDashboardWindow = new OverdueProjectDashboard();
        public static OverdueProjectReport OverdueProjectReportWindow = new OverdueProjectReport();
        public static ProjectManagementReport ProjectManagementReportWindow = new ProjectManagementReport();
        public static InvoicedProjectReports InvoicedProjectReportsWindow = new InvoicedProjectReports();
        public static UpdateEmployeeVehicleActive UpdateEmployeeVehicleActiveWindow = new UpdateEmployeeVehicleActive();
        public static AddAdminProductivity AddAdminProductivityWindow = new AddAdminProductivity();
        public static ImportNonProductionTask ImportNonProductionTaskWindow = new ImportNonProductionTask();
        public static NonProductionEmployeeProductivityReport NonProductionEmployeeProductivityReportWindow = new NonProductionEmployeeProductivityReport();
        public static EmployeeOvertimeReport EmployeeOvertimeReportWindow = new EmployeeOvertimeReport();
        public static EmployeeDoubleHours EmployeeDoubleHoursWindow = new EmployeeDoubleHours();
        public static AddProductivityWorkTask AddProductivityWorkTaskWindow = new AddProductivityWorkTask();
        public static ImportEditedWorkTasks ImportEditedWorkTaskWindow = new ImportEditedWorkTasks();
        public static EditWorkTask EditWorkTaskWindow = new EditWorkTask();
        public static ImportCodesForSheets ImportCodesForSheetsWindow = new ImportCodesForSheets();
        public static CreateProductionSheet CreateProductionSheetWindow = new CreateProductionSheet();
        public static AssignWorkTaskBusinessLine AssignWorkTaskBusinessLineWindow = new AssignWorkTaskBusinessLine();
        public static AddWorkTask AddWorkTaskWindow = new AddWorkTask();
        public static EmployeePunchedVsProductionHours EmployeePunchedVsProductionHoursWindow = new EmployeePunchedVsProductionHours();
        public static ManagerProductivityPunched ManagerProductivityPunchedWindow = new ManagerProductivityPunched();

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
            ProjectShopAnalysisWindow.Visibility = Visibility.Hidden;
            ImportInventoryWindow.Visibility = Visibility.Hidden;
            UpdateTrailerProblemsWindow.Visibility = Visibility.Hidden;
            AddDepartmentWindow.Visibility = Visibility.Hidden;
            CreateToolProblemWindow.Visibility = Visibility.Hidden;
            UpdateToolProblemWindow.Visibility = Visibility.Hidden;
            ImportPricesWindow.Visibility = Visibility.Hidden;
            JSIProjectReportWindow.Visibility = Visibility.Hidden;
            VoidInventoryTransactionWindow.Visibility = Visibility.Hidden;
            EmployeeRosterWindow.Visibility = Visibility.Hidden;
            EmployeeLookupWindow.Visibility = Visibility.Hidden;
            AddProjectLaborWindow.Visibility = Visibility.Hidden;
            AddEmployeeWindow.Visibility = Visibility.Hidden;
            AddEmployeeGroupWindow.Visibility = Visibility.Hidden;
            EditEmployeeWindow.Visibility = Visibility.Hidden;
            AddEmployeeLaborRateWindow.Visibility = Visibility.Hidden;
            ImportEmployeeHoursWindow.Visibility = Visibility.Hidden;
            ImportEmployeePunchesWindow.Visibility = Visibility.Hidden;
            ITCreateHelpDeskTicketWindow.Visibility = Visibility.Hidden;
            DepartmentProjectOpenListWindow.Visibility = Visibility.Hidden;
            UpdateProjectWindow.Visibility = Visibility.Hidden;
            OpenProjectDashboardWindow.Visibility = Visibility.Hidden;
            ImportProductionCodesWindow.Visibility = Visibility.Hidden;
            OverdueProjectDashboardWindow.Visibility = Visibility.Hidden;
            OverdueProjectReportWindow.Visibility = Visibility.Hidden;
            ProjectManagementReportWindow.Visibility = Visibility.Hidden;
            InvoicedProjectReportsWindow.Visibility = Visibility.Hidden;
            UpdateEmployeeVehicleActiveWindow.Visibility = Visibility.Hidden;
            AddAdminProductivityWindow.Visibility = Visibility.Hidden;
            ImportNonProductionTaskWindow.Visibility = Visibility.Hidden;
            NonProductionEmployeeProductivityReportWindow.Visibility = Visibility.Hidden;
            EmployeeOvertimeReportWindow.Visibility = Visibility.Hidden;
            EmployeeDoubleHoursWindow.Visibility = Visibility.Hidden;
            AddProductivityWorkTaskWindow.Visibility = Visibility.Hidden;
            ImportEditedWorkTaskWindow.Visibility = Visibility.Hidden;
            EditWorkTaskWindow.Visibility = Visibility.Hidden;
            ImportCodesForSheetsWindow.Visibility = Visibility.Hidden;
            CreateProductionSheetWindow.Visibility = Visibility.Hidden;
            AssignWorkTaskBusinessLineWindow.Visibility = Visibility.Hidden;
            AddWorkTaskWindow.Visibility = Visibility.Hidden;
            EmployeePunchedVsProductionHoursWindow.Visibility = Visibility.Hidden;
            ManagerProductivityPunchedWindow.Visibility = Visibility.Hidden;
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
            expJSIReports.IsExpanded = false;
        }

        private void expProjectReports_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
            expJSIReports.IsExpanded = false;
        }

        private void expProjectAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
            expJSIReports.IsExpanded = false;
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
            SendNewProjectReport();

            EmployeeSignsIn();
        }
        private void SendNewProjectReport()
        {
            DateTime datTodaysDate = DateTime.Now;
            DateTime datLastDate;
            int intCounter;
            int intNumberOfRecords;
            string strEmailAddress = "newprojectnotificationdll@bluejaycommunications.com";
            string strHeader = "New Projects Created";
            string strMessage;
            string strManager;
            DateTime datTransactionDate;
            string strCustomerProjectID;
            string strAssignedProjectID;
            string strProjectName;
            string strDepartment;
            bool blnFatalError = false;
            int intTransactionID;

            try
            {
                TheProjectLastDateDataSet = TheProjectMatrixClass.GetProjectLastDateInfo();

                datLastDate = TheProjectLastDateDataSet.projectlastdate[0].LastDate;
                intTransactionID = TheProjectLastDateDataSet.projectlastdate[0].TransactionID;

                datLastDate = TheDateSearchClass.RemoveTime(datLastDate);

                datTodaysDate = TheDateSearchClass.RemoveTime(datTodaysDate);

                if((datTodaysDate.DayOfWeek != DayOfWeek.Saturday) || (datTodaysDate.DayOfWeek != DayOfWeek.Sunday))
                {
                    if(datLastDate < datTodaysDate)
                    {
                        strMessage = "<h1>New Projects Created</h1>";
                        strMessage += "<table>";
                        strMessage += "<tr>";
                        strMessage += "<td><b>Date</b></td>";
                        strMessage += "<td><b>Customer Project ID</b></td>";
                        strMessage += "<td><b>Assigned Project ID</b></td>";
                        strMessage += "<td><b>Project Name</b></td>";
                        strMessage += "<td><b>Department</b></td>";
                        strMessage += "<td><b>Assigned Manager</b></td>";
                        strMessage += "</tr>";
                        strMessage += "<p>               </p>";

                        TheFindProjectMatrixByGreaterDateDataSet = TheProjectMatrixClass.FindProjectMatrixByGreaterDate(datLastDate);

                        intNumberOfRecords = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate.Rows.Count;

                        if(intNumberOfRecords > 0)
                        {
                            for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                            {
                                datTransactionDate = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].TransactionDate;
                                strCustomerProjectID = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].CustomerAssignedID;
                                strAssignedProjectID = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].AssignedProjectID;
                                strProjectName = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].ProjectName;
                                strDepartment = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].Department;
                                strManager = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].FirstName + " ";
                                strManager += TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].LastName;

                                strMessage += "<tr>";
                                strMessage += "<td>" + Convert.ToString(datTransactionDate) + "</td>";
                                strMessage += "<td>" + strCustomerProjectID + "</td>";
                                strMessage += "<td>" + strAssignedProjectID + "</td>";
                                strMessage += "<td>" + strProjectName + "</td>";
                                strMessage += "<td>" + strDepartment + "</td>";
                                strMessage += "<td>" + strManager + "</td>";
                                strMessage += "</tr>";
                            }
                        }

                        strMessage += "</table>";

                        blnFatalError = !(TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage));

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = TheProjectMatrixClass.UpdateProjectLastDate(intTransactionID, datTodaysDate);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Main Window // Send New Project Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
            expEmployeeReports.IsEnabled = true;
            expEditProject.IsEnabled = true;
            expAddAdminProductivity.IsEnabled = true;
            expProjectManagementReport.IsEnabled = true;
            expProjectInvoiceReport.IsEnabled = true;
            expEmployeeProjectLaborReport.IsExpanded = true;
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
                    expProjectReports.IsEnabled = false;
                    expEmployeeReports.IsEnabled = false;
                    expEmployeeProjectLaborReport.IsEnabled = false;
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
                    expProjectManagementReport.IsEnabled = false;
                    expProjectInvoiceReport.IsEnabled = false;
                    expEmployeeProjectLaborReport.IsExpanded = false;
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
                    expProjectReports.IsEnabled = false;
                    expEmployeeReports.IsEnabled = false;
                    expAddAdminProductivity.IsEnabled = false;
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
            expJSIReports.IsExpanded = false;
        }

        private void expCompanyFootages_Expanded(object sender, RoutedEventArgs e)
        {
            CompanyProjectFootagesWindows.Visibility = Visibility.Visible;
            SetProjectReportsExpanders();
        }

        private void expProjectProductivityReport_Expanded(object sender, RoutedEventArgs e)
        {
            ProjectProductivityReportWindow.Visibility = Visibility.Visible;
            SetProjectReportsExpanders();
        }

        private void expDepartmentProductionEmail_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
            DepartmentProductionEmailWindow.Visibility = Visibility.Visible;            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void expAddNewProject_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectDataEntryExpanders();
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
            ResetEmployeeReportExpanders();
            EmployeeHoursPunchedWindow.Visibility = Visibility.Visible;
        }

        private void expManagerHourlyDailyReport_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeReportExpanders();
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
            SetProjectReportsExpanders();
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
            expJSIReports.IsExpanded = false;
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
            ResetEmployeeAdministration();
            CreateFuelCardNumberWindow.Visibility = Visibility.Visible;
        }

        private void expEditFuelCard_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            EditFuelCardWindow.Visibility = Visibility.Visible;
        }

        private void expFuelCardPINReport_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            FuelCardPINReportWindow.Visibility = Visibility.Visible;
        }

        private void expManuallAddFuelPin_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
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
            ResetProjectDataEntryExpanders();
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
            ResetITDataEntryExpanders();
            CreateHelpDeskProblemTypeWindow.Visibility = Visibility.Visible;
        }

        private void expUpdateHelpDeskTickets_Expanded(object sender, RoutedEventArgs e)
        {
            ResetITDataEntryExpanders();
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
            SetProjectReportsExpanders();
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
            ResetProjectDataEntryExpanders();
            EditProjectsWindow.Visibility = Visibility.Visible;
        }

        private void expRemoveDuplicateProjectMatrix_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
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
            SetProjectReportsExpanders();
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

        private void expProjectShopAnalysis_Expanded(object sender, RoutedEventArgs e)
        {
            SetProjectReportsExpanders();
            ProjectShopAnalysisWindow.Visibility = Visibility.Visible;
        }
        private void SetProjectReportsExpanders()
        {
            expProjects.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expCompanyFootages.IsExpanded = false;
            expProjectShopAnalysis.IsExpanded = false;
            expProjectProductivityReport.IsExpanded = false;
            expEmployeeProjectLaborReport.IsExpanded = false;
            expAllProjectProductivityCosting.IsExpanded = false;
            expSortedWorkTasksReport.IsExpanded = false;
            expDepartmentProjectOpenList.IsExpanded = false;
            expOverdueProjectReport.IsExpanded = false;
            expProjectManagementReport.IsExpanded = false;
            expProjectInvoiceReport.IsExpanded = false;
            expCreateProductionSheet.IsExpanded = false;
        }

        private void expInventoryImport_Expanded(object sender, RoutedEventArgs e)
        {
            ResetInventoryAdministrationExpanders();
            ImportInventoryWindow.Visibility = Visibility.Visible;
        }
        private void ResetInventoryAdministrationExpanders()
        {
            expInventory.IsExpanded = false;
            expInventoryAdministration.IsExpanded = false;
            expInventoryImport.IsExpanded = false;
            expImportPrices.IsExpanded = false;
            expVoidInventoryTransaction.IsExpanded = false;
        }

        private void expUpdateTrailerProblem_Expanded(object sender, RoutedEventArgs e)
        {
            SetTrailerDataEntryExpanders();
            UpdateTrailerProblemsWindow.Visibility = Visibility.Visible;
        }
        private void SetTrailerDataEntryExpanders()
        {
            expTrailers.IsExpanded = false;
            expTrailerDataEntry.IsExpanded = false;
            expUpdateTrailerProblem.IsExpanded = false;
        }

        private void expAddDepartment_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            AddDepartmentWindow.Visibility = Visibility.Visible;
        }
        private void ResetEmployeeAdministration()
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
            expUpdateEmployeeVehicleActive.IsExpanded = false;
            expEmployeeDoubleHours.IsExpanded = false;
            expAddProductivityWorkTask.IsExpanded = false;
        }

        private void expCreateToolProblem_Expanded(object sender, RoutedEventArgs e)
        {
            ResetToolProblemExpander();
            CreateToolProblemWindow.Visibility = Visibility.Visible;
        }
        private void ResetToolProblemExpander()
        {
            expTools.IsExpanded = false;
            expToolProblems.IsExpanded = false;
            expCreateToolProblem.IsExpanded = false;
            expUpdateToolProblem.IsExpanded = false;
        }

        private void expUpdateToolProblem_Expanded(object sender, RoutedEventArgs e)
        {
            ResetToolProblemExpander();
            UpdateToolProblemWindow.Visibility = Visibility.Visible;
        }

        private void expImportPrices_Expanded(object sender, RoutedEventArgs e)
        {
            ResetInventoryAdministrationExpanders();
            ImportPricesWindow.Visibility = Visibility.Visible;
        }

        private void expJSIProjectReports_Expanded(object sender, RoutedEventArgs e)
        {
            ResetJSIReportsExpanders();
            JSIProjectReportWindow.Visibility = Visibility.Visible;
        }
        private void ResetJSIReportsExpanders()
        {
            expProjects.IsExpanded = false;
            expJSIReports.IsExpanded = false;
            expJSIProjectReports.IsExpanded = false;
            expJSIDateReports.IsExpanded = false;
            expViewJSIInspection.IsExpanded = false;
        }

        private void expVoidInventoryTransaction_Expanded(object sender, RoutedEventArgs e)
        {
            ResetInventoryAdministrationExpanders();
            VoidInventoryTransactionWindow.Visibility = Visibility.Visible;
        }

        private void expEmployeeRoster_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeReportExpanders();
            EmployeeRosterWindow.Visibility = Visibility.Visible;
        }
        private void ResetEmployeeReportExpanders()
        {
            expCompareEmployeeCrews.IsExpanded = false;
            expDesignEmployeeProductivity.IsExpanded = false;
            expManagerHourlyDailyReport.IsExpanded = false;
            expEmployeeHoursOverDateRange.IsExpanded = false;
            expEmployeeHoursPunched.IsExpanded = false;
            expEmployeeProductivityByDateRange.IsExpanded = false;
            expProductionEmployeeProductivityMeasure.IsExpanded = false;
            expEmployeePunchedVsProductionHours.IsExpanded = false;
            expEmployeeRoster.IsExpanded = false;
            expViewEmployeePunches.IsExpanded = false;
            expEmployeeLookup.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expNonProductionEmployeeProductivity.IsExpanded = false;
            expEmployeeOvertimeReport.IsExpanded = false;
            expManagerProductivityPunchedReport.IsExpanded = false;
        }

        private void expEmployeeLookup_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeReportExpanders();
            EmployeeLookupWindow.Visibility = Visibility.Visible;
        }

        private void expAddProjectLabor_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectDataEntryExpanders();
            AddProjectLaborWindow.Visibility = Visibility.Visible;
        }
        private void ResetProjectDataEntryExpanders()
        {
            expProjects.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expAddNewProject.IsExpanded = false;
            expAddProjectLabor.IsExpanded = false;
            expEditProject.IsExpanded = false;
            expSubmitAfterHoursWork.IsExpanded = false;
            expUpdateProject.IsExpanded = false;
        }

        private void expAddEmployee_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            AddEmployeeWindow.Visibility = Visibility.Visible;
        }

        private void expAddEmployeeGroups_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            AddEmployeeGroupWindow.Visibility = Visibility.Visible;
        }

        private void expEditEmployee_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            EditEmployeeWindow.Visibility = Visibility.Visible;
        }

        private void expAddEmployeeToVehicleEmailList_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            AddEmployeeToVehicleEmailsWindow.Visibility = Visibility.Visible;
        }

        private void expEmployeeLaborRate_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            AddEmployeeLaborRateWindow.Visibility = Visibility.Visible;
        }

        private void expImportEmployeeHours_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            ImportEmployeeHoursWindow.Visibility = Visibility.Visible;
        }

        private void expImportEmployeePunches_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            ImportEmployeePunchesWindow.Visibility = Visibility.Visible;
        }

        private void expITCreateHelpDeskTicket_Expanded(object sender, RoutedEventArgs e)
        {
            ResetITDataEntryExpanders();
            ITCreateHelpDeskTicketWindow.Visibility = Visibility.Visible;
        }
        private void ResetITDataEntryExpanders()
        {
            expInformationTechology.IsExpanded = false;
            expITDataEntry.IsExpanded = false;
            expInspectionDataEntry.IsExpanded = false;
            expCreateHelpDeskProblemType.IsExpanded = false;
            expITCreateHelpDeskTicket.IsExpanded = false;
            expUpdateHelpDeskTickets.IsExpanded = false;
        }

        private void expDepartmentProjectOpenList_Expanded(object sender, RoutedEventArgs e)
        {
            SetProjectReportsExpanders();
            DepartmentProjectOpenListWindow.Visibility = Visibility.Visible;
        }

        private void expUpdateProject_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectDataEntryExpanders();
            UpdateProjectWindow.Visibility = Visibility.Visible;
        }

        private void expOpenProjectsDashboard_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectDashboardExpanders();
            OpenProjectDashboardWindow.Visibility = Visibility.Visible;
        }
        private void ResetProjectDashboardExpanders()
        {
            expProjects.IsExpanded = false;
            expProjectDashboards.IsExpanded = false;
            expOpenProjectsDashboard.IsExpanded = false;
            expOverdueProjectDashbord.IsExpanded = false;
        }

        private void expImportProductionCodes_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
            ImportProductionCodesWindow.Visibility = Visibility.Visible;
        }
        private void ResetProjectAdministrationExpanders()
        {
            expProjects.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expDepartmentProductionEmail.IsExpanded = false;
            expImportProductionCodes.IsExpanded = false;
            expRemoveDuplicateProjectMatrix.IsExpanded = false;
            expAddNonProductionTask.IsExpanded = false;
            expImportNonProductionTask.IsExpanded = false;
            expImportEditedProductionCodes.IsExpanded = false;
            expEditWorkTask.IsExpanded = false;
            expImportProductionCodesForSheets.IsExpanded = false;
            expAssignTaskBusinessLine.IsExpanded = false;
            expAddWorkTask.IsExpanded = false;
        }

        private void expJSIReports_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expJSIDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
        }

        private void expOverdueProjectDashbord_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectDashboardExpanders();
            OverdueProjectDashboardWindow.Visibility = Visibility.Visible;
        }

        private void expOverdueProjectReport_Expanded(object sender, RoutedEventArgs e)
        {
            SetProjectReportsExpanders();
            OverdueProjectReportWindow.Visibility = Visibility.Visible;
        }

        private void expProjectManagementReport_Expanded(object sender, RoutedEventArgs e)
        {
            SetProjectReportsExpanders();
            ProjectManagementReportWindow.Visibility = Visibility.Visible;
        }

        private void expProjectInvoiceReport_Expanded(object sender, RoutedEventArgs e)
        {
            SetProjectReportsExpanders();
            InvoicedProjectReportsWindow.Visibility = Visibility.Visible;
        }

        private void expUpdateEmployeeVehicleActive_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            UpdateEmployeeVehicleActiveWindow.Visibility = Visibility.Visible;
        }

        private void expAddAdminProductivity_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeDataEntryExpanders();
            AddAdminProductivityWindow.Visibility = Visibility.Visible;
        }
        private void ResetEmployeeDataEntryExpanders()
        {
            expEmployees.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expAddAdminProductivity.IsExpanded = false;
        }

        private void expImportNonProductionTask_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
            ImportNonProductionTaskWindow.Visibility = Visibility.Visible;
        }

        private void expNonProductionEmployeeProductivity_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeReportExpanders();
            NonProductionEmployeeProductivityReportWindow.Visibility = Visibility.Visible;
        }

        private void expEmployeeOvertimeReport_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeReportExpanders();
            EmployeeOvertimeReportWindow.Visibility = Visibility.Visible;
        }

        private void expEmployeeDoubleHours_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            EmployeeDoubleHoursWindow.Visibility = Visibility.Visible;
        }

        private void expAddProductivityWorkTask_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeAdministration();
            AddProductivityWorkTaskWindow.Visibility = Visibility.Visible;
        }

        private void expImportEditedProductionCodes_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
            ImportEditedWorkTaskWindow.Visibility = Visibility.Visible;
        }

        private void expEditWorkTask_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
            EditWorkTaskWindow.Visibility = Visibility.Visible;
        }

        private void expImportProductionCodesForSheets_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
            ImportCodesForSheetsWindow.Visibility = Visibility.Visible;
        }

        private void expCreateProductionSheet_Expanded(object sender, RoutedEventArgs e)
        {
            SetProjectReportsExpanders();
            CreateProductionSheetWindow.Visibility = Visibility.Visible;
        }

        private void expAssignTaskBusinessLine_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
            AssignWorkTaskBusinessLineWindow.Visibility = Visibility.Visible;
        }

        private void expAddWorkTask_Expanded(object sender, RoutedEventArgs e)
        {
            ResetProjectAdministrationExpanders();
            AddWorkTaskWindow.Visibility = Visibility.Visible;
        }

        private void expEmployeePunchedVsProductionHours_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeReportExpanders();
            EmployeePunchedVsProductionHoursWindow.Visibility = Visibility.Visible;
        }

        private void expManagerProductivityPunchedReport_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeReportExpanders();
            ManagerProductivityPunchedWindow.Visibility = Visibility.Visible;
        }
    }
}