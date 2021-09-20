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
    /// Interaction logic for WindowSeats.xaml
    /// </summary>
    public partial class WindowSeats : Window
    {
        public WindowSeats()
        {
            InitializeComponent();
        }
        public void Init(BlockParser parser)
        {
            userControlSeats.Init(parser);
        }
    }
}
