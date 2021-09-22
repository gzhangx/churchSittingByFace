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
        public Action<CellInfo> cellClicked { get; set; }
        public int PerferedWidth = 0;
        public int PerferedHeight = 0;
        public void Init(BlockParser parser, bool modify)
        {
            const int Button_Size = 20;
            const int Button_Bottom_Margin = 4;
            const int Block_Right_Margin = 20;
            InitializeComponent();
            
            HorizontalAlignment[] blkHorAlighment = new HorizontalAlignment[4]
            {
                HorizontalAlignment.Right, HorizontalAlignment.Right,
                HorizontalAlignment.Left, HorizontalAlignment.Left,
            };
            ColumnDefinition[] colDefs = new ColumnDefinition[] { col1, col2, col3, col4 };
            char[] blkNames = new char[] { 'A', 'B', 'C', 'D' };
            for (int i = 0; i < parser.blocks.Count; i++)
            {
                var blkInfp = parser.blocks[i];                
                var curWidth = blkInfp.MaxColumns * Button_Size + Block_Right_Margin;
                colDefs[i].Width = new GridLength(curWidth);
                PerferedWidth += curWidth;
                Grid blkGrid = new Grid();
                gridMain.Children.Add(blkGrid);
                blkGrid.SetValue(Grid.ColumnProperty, i);
                if (PerferedHeight < blkInfp.rows.Count) PerferedHeight = blkInfp.rows.Count * (Button_Size + Button_Bottom_Margin);
                for (int row = 0; row < blkInfp.rows.Count; row++)
                {
                    blkGrid.RowDefinitions.Add(new RowDefinition());
                    StackPanel sp = new StackPanel();
                    sp.Margin = new Thickness(0, 1, Block_Right_Margin, 1);
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
                            if (row % 2 == 0 || row == 11)
                            {
                                b.Content = blkNames[blkInfp.Section] + "" + (row+1).ToString().Last();
                                b.Click += (naa,nab) =>
                                {
                                    cellClicked?.Invoke(cell);
                                };
                            } else
                            {
                                b.IsEnabled = false;
                            }
                            if (cell.occupyedBy != null)
                            {
                                if (!modify)
                                    b.IsEnabled = false;
                                b.Background = Brushes.Red;
                            } else
                            {
                                b.Background = Brushes.LightGreen;
                            }
                            b.Width = Button_Size;
                            b.Height = Button_Size;
                            sp.Children.Add(b);
                        }
                    }
                }
            }

            gridMain.Width = PerferedWidth;
            gridMain.Height = PerferedHeight;
        }

    }
}
