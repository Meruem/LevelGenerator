using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Assets.Scripts;

namespace LevelGen
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class Tile : UserControl
    {
        private MapGenerator.TileType _tileType;
        private bool _isExit;

        public Tile()
        {
            InitializeComponent();
            TileType = MapGenerator.TileType.Wall;
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

        public MapGenerator.TileType TileType
        {
            get { return _tileType; }
            set
            {
                if (_tileType == value) return;
                _tileType = value;
                switch (_tileType)
                {
                    case MapGenerator.TileType.Border:
                        TileCanvas.Background = Brushes.DarkGreen;
                        break;
                    case MapGenerator.TileType.Wall:
                        TileCanvas.Background = Brushes.Gray;
                        break;
                    case MapGenerator.TileType.Room:
                        TileCanvas.Background = Brushes.Black;
                        break;
                }
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e != null && e.RightButton == MouseButtonState.Pressed)
            {
                TileType = MapGenerator.TileType.Wall;
                return;
            }

            var tileBrush  = RoomDesignerVM.Instance.SelectedTileBrush;

            switch (tileBrush)
            {
                case TileBrush.Exit:
                    IsExit = true;
                    break;
                case TileBrush.Border:
                    TileType = MapGenerator.TileType.Border;
                    break;
                case TileBrush.Room:
                    TileType = MapGenerator.TileType.Room;
                    IsExit = false;
                    break;
                case TileBrush.Wall:
                    TileType = MapGenerator.TileType.Wall;
                    break;
            }

        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) UIElement_OnMouseDown(sender, null);
            if (e.RightButton == MouseButtonState.Pressed) TileType = MapGenerator.TileType.Wall;
        }
    }
}
