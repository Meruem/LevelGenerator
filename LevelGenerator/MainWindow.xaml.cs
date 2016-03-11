using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Assets.Scripts;
using LevelGenerator;

namespace LevelGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int _tileWidth = 10;
        private int _tileHeight = 10;

        public void Generate(int width, int height, float ratio)
        {
            var mapGenerator = new MapGenerator(width, height, ratio, RoomManager.Instance, DefaultCheckBox.IsChecked.GetValueOrDefault(false));
            var map = mapGenerator.GenerateMap();

            for (var i = 0; i < mapGenerator.MapWidth; i++)
            {
                for (var j = 0; j < mapGenerator.MapHeight; j++)
                {
                    var tile = new PreviewTile();
                    switch (map[i, j])
                    {
                        case MapGenerator.TileType.Room: tile.TileCanvas.Background = Brushes.Black; break;
                        case MapGenerator.TileType.Wall: tile.TileCanvas.Background = Brushes.Gray; break;
                        case MapGenerator.TileType.Exit: tile.TileCanvas.Background = Brushes.CornflowerBlue; break;
                        default: tile.TileCanvas.Background = Brushes.Black; break;
                    }

                    Canvas.SetLeft(tile, 10 + _tileWidth * j);
                    Canvas.SetTop(tile, 10 + _tileHeight * i);
                    GameCanvas.Children.Add(tile);
                }
            }

            GameCanvas.Width = mapGenerator.MapWidth * _tileWidth + 50;
            GameCanvas.Height = mapGenerator.MapHeight * _tileHeight + 50;
        }

        private void OnDesignerOpen(object sender, RoutedEventArgs e)
        {
            var w = new Designer();
            w.Show();
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            GameCanvas.Children.Clear();
            Generate(int.Parse(WidthTextBox.Text), int.Parse(HeightTextBox.Text), float.Parse(RatioTextBox.Text, CultureInfo.InvariantCulture));
        }

        private void OnRoomManagerOpen(object sender, RoutedEventArgs e)
        {
            var w = new RoomManagerWindow();
            w.Show();
        }
    }
}
