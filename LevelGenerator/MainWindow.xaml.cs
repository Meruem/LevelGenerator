using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LevelGenerator;
using Microsoft.FSharp.Collections;

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
            var list1 = RoomManager.Instance.GetRooms();
            var l2 = ListModule.OfSeq(list1.Concat(RoomMaps.DefaultRooms));
            var mapGenerator = new MapGeneratorF.MapGeneratorF(width, height, l2);
            var map = mapGenerator.GenerateMap();

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var tile = new PreviewTile();
                    switch (map[i, j])
                    {
                        case RoomTypes.TileType.Room: tile.TileCanvas.Background = Brushes.Black; break;
                        case RoomTypes.TileType.Wall: tile.TileCanvas.Background = Brushes.Gray; break;
                        case RoomTypes.TileType.Exit: tile.TileCanvas.Background = Brushes.CornflowerBlue; break;
                        default: tile.TileCanvas.Background = Brushes.Black; break;
                    }

                    Canvas.SetLeft(tile, 10 + _tileWidth * i);
                    Canvas.SetTop(tile, 10 + _tileHeight * j);
                    GameCanvas.Children.Add(tile);
                }
            }

            GameCanvas.Width = width * _tileWidth + 50;
            GameCanvas.Height = height * _tileHeight + 50;
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
