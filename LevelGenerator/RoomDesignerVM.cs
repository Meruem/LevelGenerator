using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LevelGen
{
    public enum TileBrush
    {
        Border,
        Room,
        Wall,
        Exit
    }

    public class RoomDesignerVM : ViewModel
    {
        public RoomDesignerVM()
        {
            Width = 10;
            Height = 10;
            IsRoomSelected = true;
            CleanDeadEnds = true;
            MapRoomSelected = true;
            MinExits = 1;
            MaxExits = 3;
        }


        public static RoomDesignerVM Instance { get; set; }
        
        public RoomDesigner View { get; set; }

        public TileBrush SelectedTileBrush
        {
            get
            {
                if (IsBorderSelected) return TileBrush.Border;
                if (IsRoomSelected) return TileBrush.Room;
                if (IsWallSelected) return TileBrush.Wall;
                if (IsExitSelected) return TileBrush.Exit;
                return TileBrush.Wall;
            }
        }


        private void CreateTiles()
        {
            if (Width <= 0 || Height <= 0) return;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    var tile = new Tile { X = i, Y = j };
                    Canvas.SetLeft(tile, i * 21);
                    Canvas.SetTop(tile, j * 21);
                    View.DesignCanvas.Children.Add(tile);
                }
            }
        }

        private void Load()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Room Files | *.room",
                Multiselect = true
            };
            if (dialog.ShowDialog() != true) return;

            foreach (var fileName in dialog.FileNames)
            {
                var ser = File.ReadAllText(fileName);
                var room = JsonConvert.DeserializeObject<RoomTypes.Room>(ser);
                RoomName = room.RoomName;
                MinExits = room.MinExits;
                MaxExits = room.MaxExits;
                CleanDeadEnds = room.CleanDeadEnds;
                View.DesignCanvas.Children.Clear();

                if (room.RoomMap.IsBlueprint)
                {
                    MapRoomSelected = true;
                    var bluePrint = ((RoomTypes.TileMap.Blueprint)room.RoomMap).Item;
                    Width = bluePrint.Width;
                    Height = bluePrint.Height;
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            var tile = new Tile { X = i, Y = j, TileType = bluePrint.Map[i, j] };
                            if (bluePrint.Exits.Any(e => e.X == i && e.Y == j))
                            {
                                tile.IsExit = true;
                            }
                            Canvas.SetLeft(tile, i * 21);
                            Canvas.SetTop(tile, j * 21);
                            View.DesignCanvas.Children.Add(tile);
                        }
                    }
                }
                else if (room.RoomMap.IsDynamicRect)
                {
                    DynamicRoomSelected = true;
                    var dynamicRext = ((RoomTypes.TileMap.DynamicRect) room.RoomMap).Item;
                    MinWidth = dynamicRext.MinWidth;
                    MaxWidth = dynamicRext.MaxWidth;
                    MinHeight = dynamicRext.MinHeight;
                    MaxHeight = dynamicRext.MaxHeight;
                    HasBorder = dynamicRext.HasBorder;
                }
            }
        }

        private void SaveAs()
        {
            RoomTypes.Room room;

            if (DynamicRoomSelected)
            {
                room = RoomTypes.createDynamicRoom(RoomName, MinExits, MaxExits, CleanDeadEnds,
                    MinWidth, MaxWidth, MinHeight, MaxHeight, HasBorder);
            }
            else
            {
                var tileMap = new RoomTypes.TileType[Width, Height];
                var exits = new List<RoomTypes.Position>();
                foreach (var tile in View.DesignCanvas.Children.Cast<Tile>())
                {
                    tileMap[tile.X, tile.Y] = tile.TileType;
                    if (tile.IsExit)
                    {
                        exits.Add(new RoomTypes.Position(tile.X, tile.Y));
                    }
                }

                room = RoomTypes.createBlooprintRoom(RoomName, MinExits, MaxExits, CleanDeadEnds,
                    Width, Height, tileMap, exits.ToArray());
            }


            var ser = JsonConvert.SerializeObject(room);
            var dialog = new SaveFileDialog
            {
                DefaultExt = "*.room",
                Filter = "Room Files | *.room"
            };

            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                File.WriteAllText(fileName, ser);
            }
        }


        public ICommand SaveAsCommand => _saveAsCommand ?? (_saveAsCommand = new BasicCommand(SaveAs, true));
        public ICommand LoadCommand => _loadCommand ?? (_loadCommand = new BasicCommand(Load, true));
        public ICommand CreateTilesCommand => _createTiles ?? (_createTiles = new BasicCommand(CreateTiles, true));

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        private bool _isWallSelected;

        public bool IsWallSelected
        {
            get { return _isWallSelected; }
            set
            {
                _isWallSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isExitSelected;

        public bool IsExitSelected
        {
            get { return _isExitSelected; }
            set
            {
                _isExitSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isRoomSelected;

        public bool IsRoomSelected
        {
            get { return _isRoomSelected; }
            set
            {
                _isRoomSelected = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isBorderSelected;
        private ICommand _createTiles;
        private int _width;
        private int _height;
        private ICommand _saveAsCommand;
        private int _minExits;
        private int _maxExits;
        private ICommand _loadCommand;
        private string _roomName;
        private bool _dynamicRoomSelected;
        private int _minWidth;
        private int _maxWidth;
        private int _minHeight;
        private int _maxHeight;
        private bool _hasBorder;
        private bool _mapRoomSelected;
        private bool _cleanDeadEnds;

        public bool IsBorderSelected
        {
            get { return _isBorderSelected; }
            set
            {
                _isBorderSelected = value;
                OnPropertyChanged();
            }
        }

        public bool DynamicRoomSelected
        {
            get { return _dynamicRoomSelected; }
            set
            {
                _dynamicRoomSelected = value; 
                OnPropertyChanged();
            }
        }

        public bool MapRoomSelected
        {
            get { return _mapRoomSelected; }
            set
            {
                _mapRoomSelected = value;
                OnPropertyChanged();
            }
        }

        public int MinExits
        {
            get { return _minExits; }
            set
            {
                _minExits = value;
                OnPropertyChanged();
            }
        }

        public int MaxExits
        {
            get { return _maxExits; }
            set
            {
                _maxExits = value;
                OnPropertyChanged();
            }
        }

        public string RoomName
        {
            get { return _roomName; }
            set
            {
                _roomName = value;
                OnPropertyChanged();
            }
        }

        public int MinHeight
        {
            get { return _minHeight; }
            set
            {
                _minHeight = value; 
                OnPropertyChanged();
            }
        }

        public int MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                _maxHeight = value; 
                OnPropertyChanged();
            }
        }

        public int MinWidth
        {
            get { return _minWidth; }
            set
            {
                _minWidth = value;
                OnPropertyChanged();
            }
        }

        public int MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                _maxWidth = value;
                OnPropertyChanged();
            }
        }

        public bool HasBorder
        {
            get { return _hasBorder; }
            set
            {
                _hasBorder = value;
                OnPropertyChanged();
            }
        }

        public bool CleanDeadEnds
        {
            get { return _cleanDeadEnds; }
            set
            {
                _cleanDeadEnds = value;
                OnPropertyChanged();
            }
        }
    }
}
