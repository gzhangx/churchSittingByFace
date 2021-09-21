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
        public void Init(BlockParser parser, RecoInfoWithSeat rsInfo, Action<CellInfo> cellClicked, Action onCancel)
        {
            userControlSeats.Init(parser);
            userControlSeats.cellClicked = info=>
            {
                cellClicked?.Invoke(info);
                Console.WriteLine("info." + info.x + "," + info.y);
                info.occupyedBy = this.txtName.Text;
                info.occupyedById = rsInfo.recoInfo.Id;
                this.Close();
            };

            this.Closing += (eve1, eve2) =>
              {
                  onCancel();
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
