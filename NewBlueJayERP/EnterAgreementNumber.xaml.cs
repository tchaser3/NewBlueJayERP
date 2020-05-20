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

namespace NewBlueJayERP
{
    /// <summary>
    /// Interaction logic for EnterAgreementNumber.xaml
    /// </summary>
    public partial class EnterAgreementNumber : Window
    {
        //settting up the class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public EnterAgreementNumber()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnAddAgreementNumber_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.gstrAgreementNo = txtAgreementNumber.Text;

            if(MainWindow.gstrAgreementNo == "")
            {
                TheMessagesClass.ErrorMessage("Agreement Number Was Not Entered");
                return;
            }

            this.Close();
        }
    }
}
