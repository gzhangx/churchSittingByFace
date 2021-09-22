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
        public void Init(BlockParser parser, RecoInfo rsInfo, Action<CellInfo> cellClicked, Action onCancel, List<RecoInfo> recoInfos)
        {
            bool modifyMode = rsInfo == null;
            userControlSeats.Init(parser, modifyMode);
            userControlSeats.cellClicked = cellInfo=>
            {                
                if (!modifyMode)
                {
                    ok = true;
                    cellInfo.occupyedBy = this.txtName.Text;
                    rsInfo.name = this.txtName.Text;
                    cellInfo.occupyedById = rsInfo.Id;
                    cellClicked.Invoke(cellInfo);

                    Console.WriteLine("info." + cellInfo.x + "," + cellInfo.y);
                    this.Close();
                } else
                {
                    if (String.IsNullOrEmpty(cellInfo.occupyedById)) return;
                    this.txtName.Text = cellInfo.occupyedBy;

                    RecoInfo r = recoInfos.Find(rr => rr.Id == cellInfo.occupyedById);
                    if (r != null)
                        this.imgPerson.Source = Util.getImgSrc(r);
                }
            };

            this.Closing += (eve1, eve2) =>
              {
                  if (!ok)onCancel();
              };
            this.btnCancel.Click += (eve1, eve2) =>
              {
                  onCancel();
              };

            if (rsInfo != null)
            {
                if (rsInfo.imageName != null) this.imgPerson.Source = Util.getImgSrc(rsInfo);
                this.txtName.Text = rsInfo.name == null ? "" : rsInfo.name;

            }
            this.Width = userControlSeats.PerferedWidth + 40;
            this.Height = userControlSeats.PerferedHeight + 40;
        }
    }
}
