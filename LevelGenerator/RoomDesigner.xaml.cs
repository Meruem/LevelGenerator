using System.Windows.Controls;

namespace LevelGen
{
    /// <summary>
    /// Interaction logic for RoomDesigner.xaml
    /// </summary>
    public partial class RoomDesigner : UserControl
    {
        public RoomDesigner()
        {
            InitializeComponent();
            var rd = new RoomDesignerVM();
            rd.View = this;
            RoomDesignerVM.Instance = rd;
            DataContext = rd;
        }
    }
}
