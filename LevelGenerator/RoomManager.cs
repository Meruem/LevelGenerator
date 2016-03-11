using System.Collections.Generic;
using System.IO;
using Assets.Scripts;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LevelGen
{
    public class RoomManager : IRoomQuery
    {
        public static RoomManager Instance
        {
            get { return _instance ?? (_instance = new RoomManager()); }
        }

        private List<MapGenerator.RoomInfo> _results;
        private static RoomManager _instance;

        public List<MapGenerator.RoomInfo> GetRooms()
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

        public List<MapGenerator.RoomInfo> GenerateFromFiles()
        {
            var result = new List<MapGenerator.RoomInfo>();
            var dialog = new OpenFileDialog();
            dialog.Filter = "Room Files | *.room";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() != true) return null;

            foreach (var file in dialog.FileNames)
            {
                var ser = File.ReadAllText(file);
                var room = JsonConvert.DeserializeObject<MapGenerator.RoomInfo>(ser);
                result.Add(room);
              
            }
            return result;
        }

        public List<MapGenerator.RoomInfo> GenerateFromFolder(string folderName)
        {
            var result = new List<MapGenerator.RoomInfo>();
            var d = new DirectoryInfo(folderName);
            FileInfo[] files = d.GetFiles("*.room"); 
            foreach (FileInfo file in files)
            {
                var fileName = file.FullName;
                var ser = File.ReadAllText(fileName);
                var room = JsonConvert.DeserializeObject<MapGenerator.RoomInfo>(ser);
                result.Add(room);
            }
            return result;
        }

    }
}
