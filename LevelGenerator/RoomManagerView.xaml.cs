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
            RefreshList();
        }

        private void OnOpenFolder(object sender, RoutedEventArgs e)
        {
            RoomManager.Instance.OpenFolder(PathTextBox.Text);
            RefreshList();
        }

        private void OnOpenFiles(object sender, RoutedEventArgs e)
        {
            RoomManager.Instance.OpenFiles();
            RefreshList();
        }

        private void RefreshList()
        {
            RoomsListBox.ItemsSource = RoomManager.Instance.GetRooms().Select(r => r.RoomName).ToList();
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            RoomManager.Instance.Clear();
            RefreshList();
        }
    }
}
