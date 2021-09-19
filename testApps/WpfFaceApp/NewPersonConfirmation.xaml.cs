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

namespace WpfFaceApp
{
    /// <summary>
    /// Interaction logic for NewPersonConfirmation.xaml
    /// </summary>
    public partial class NewPersonConfirmation : Window
    {
        public NewPersonConfirmation()
        {
            InitializeComponent();
        }

        private Action<String> onOk = null;
        public void SetImage(BitmapImage img, Action<String> okAct)
        {
            imgPerson.Source = img;
            onOk = okAct;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            onOk(txtName.Text);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
