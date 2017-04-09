using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LevelGen
{
    public class RoomManager
    {
        public static RoomManager Instance => _instance ?? (_instance = new RoomManager());

        private List<RoomTypes.Room> _results = new List<RoomTypes.Room>();
        private static RoomManager _instance;

        public void Clear()
        {
            _results.Clear();
        }

        public List<RoomTypes.Room> GetRooms()
        {
            return _results;
        }

        public void OpenFolder(string folderName)
        {
            _results = GenerateFromFolder(folderName);
        }

        public void OpenFiles()
        {
            _results = GenerateFromFiles();
        }

        public List<RoomTypes.Room> GenerateFromFiles()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Room Files | *.room",
                Multiselect = true
            };
            if (dialog.ShowDialog() != true) return new List<RoomTypes.Room>();

            return dialog.FileNames
                .Select(File.ReadAllText)
                .Select(JsonConvert.DeserializeObject<RoomTypes.Room>)
                .ToList();
        }

        public List<RoomTypes.Room> GenerateFromFolder(string folderName)
        {
            var result = new List<RoomTypes.Room>();
            var d = new DirectoryInfo(folderName);
            var files = d.GetFiles("*.room"); 
            foreach (var file in files)
            {
                var fileName = file.FullName;
                var ser = File.ReadAllText(fileName);
                var room = JsonConvert.DeserializeObject<RoomTypes.Room>(ser);
                result.Add(room);
            }
            return result;
        }
    }
}
