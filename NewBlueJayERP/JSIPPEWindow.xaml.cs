/* Title:           JSI PPE
 * Date:            5-13-20
 * Author:          Terry Holmes
 * 
 * Description:     This is used for JSI PPE */

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
using JSIMainDLL;

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
    }
}
