using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LevelGen
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class Tile : UserControl
    {
        private RoomTypes.TileType _tileType;
        private bool _isExit;

        public Tile()
        {
            InitializeComponent();
            TileType = RoomTypes.TileType.Wall;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public bool IsExit
        {
            get { return _isExit; }
            set
            {
                _isExit = value;
                if (_isExit)
                {
                    TileCanvas.Children.Add(new Label {Content = "X"});
                }
                else
                {
                    TileCanvas.Children.Clear();
                }
            }
        }

        public RoomTypes.TileType TileType
        {
            get { return _tileType; }
            set
            {
                if (_tileType == value) return;
                _tileType = value;
                switch (_tileType)
                {
                    case RoomTypes.TileType.Border:
                        TileCanvas.Background = Brushes.DarkGreen;
                        break;
                    case RoomTypes.TileType.Wall:
                        TileCanvas.Background = Brushes.Gray;
                        break;
                    case RoomTypes.TileType.Room:
                        TileCanvas.Background = Brushes.Black;
                        break;
                }
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e != null && e.RightButton == MouseButtonState.Pressed)
            {
                TileType = RoomTypes.TileType.Wall;
                return;
            }

            var tileBrush  = RoomDesignerVM.Instance.SelectedTileBrush;

            switch (tileBrush)
            {
                case TileBrush.Exit:
                    IsExit = true;
                    break;
                case TileBrush.Border:
                    TileType = RoomTypes.TileType.Border;
                    break;
                case TileBrush.Room:
                    TileType = RoomTypes.TileType.Room;
                    IsExit = false;
                    break;
                case TileBrush.Wall:
                    TileType = RoomTypes.TileType.Wall;
                    break;
            }

        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) UIElement_OnMouseDown(sender, null);
            if (e.RightButton == MouseButtonState.Pressed) TileType = RoomTypes.TileType.Wall;
        }
    }
}
