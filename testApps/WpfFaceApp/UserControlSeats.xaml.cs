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

namespace WpfFaceApp
{
    /// <summary>
    /// Interaction logic for UserControlSeats.xaml
    /// </summary>
    public partial class UserControlSeats : UserControl
    {
        public UserControlSeats() { }
        public void Init(BlockParser parser)
        {
            InitializeComponent();
            HorizontalAlignment[] blkHorAlighment = new HorizontalAlignment[4]
            {
                HorizontalAlignment.Right, HorizontalAlignment.Right,
                HorizontalAlignment.Left, HorizontalAlignment.Left,
            };
            for (int i = 0; i < parser.blocks.Count; i++)
            {
                var blkInfp = parser.blocks[i];
                Grid blkGrid = new Grid();
                gridMain.Children.Add(blkGrid);
                blkGrid.SetValue(Grid.ColumnProperty, i);                
                for (int row = 0; row < blkInfp.rows.Count; row++)
                {
                    blkGrid.RowDefinitions.Add(new RowDefinition());
                    StackPanel sp = new StackPanel();
                    sp.SetValue(Grid.RowProperty, row);
                    sp.SetValue(StackPanel.HorizontalAlignmentProperty, blkHorAlighment[i]);
                    sp.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                    blkGrid.Children.Add(sp);
                    var cellInfos = blkInfp.rows[row];
                    for(int cellCnt = 0; cellCnt < cellInfos.Length; cellCnt++)
                    {
                        var cell = cellInfos[cellCnt];
                        if (cell != null && cell.type != ' ')
                        {
                            Button b = new Button();
                            b.Content = "" + cell.x + "_" + cell.y;
                            sp.Children.Add(b);
                        }
                    }
                }
            }
        }
    }
}
