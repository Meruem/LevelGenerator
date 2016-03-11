using System.Linq;
using System.Windows;

namespace LevelGen
{
    /// <summary>
    /// Interaction logic for RoomManagerWindow.xaml
    /// </summary>
    public partial class RoomManagerWindow : Window
    {
        public RoomManagerWindow()
        {
            InitializeComponent();
        }

        private void OnOpenFolder(object sender, RoutedEventArgs e)
        {
            RoomManager.Instance.OpenFolder(PathTextBox.Text);
            RoomsListBox.ItemsSource = RoomManager.Instance.GetRooms().Select(r => r.RoomName).ToList();
        }

        private void OnOpenFiles(object sender, RoutedEventArgs e)
        {
            RoomManager.Instance.OpenFiles();
            RoomsListBox.ItemsSource = RoomManager.Instance.GetRooms().Select(r => r.RoomName).ToList();
        }
    }
}
