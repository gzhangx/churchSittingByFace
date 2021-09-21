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
        private bool ok = false;
        public void Init(BlockParser parser, RecoInfoWithSeat rsInfo, Action cellClicked, Action onCancel)
        {
            userControlSeats.Init(parser);
            userControlSeats.cellClicked = cellInfo=>
            {
                ok = true;                
                cellInfo.occupyedBy = this.txtName.Text;
                rsInfo.recoInfo.name = this.txtName.Text;
                rsInfo.cellInfo = cellInfo;
                cellInfo.occupyedById = rsInfo.recoInfo.Id;
                cellClicked.Invoke();
                Console.WriteLine("info." + cellInfo.x + "," + cellInfo.y);
                this.Close();
            };

            this.Closing += (eve1, eve2) =>
              {
                  if (!ok)onCancel();
              };
            this.btnCancel.Click += (eve1, eve2) =>
              {
                  onCancel();
              };
            this.imgPerson.Source = rsInfo.image;
            this.txtName.Text = rsInfo.recoInfo?.name == null?"": rsInfo.recoInfo?.name;         

            this.Width = userControlSeats.PerferedWidth + 40;
            this.Height = userControlSeats.PerferedHeight + 40;
        }
    }
}
