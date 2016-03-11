using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public interface IRoomQuery
    {
        List<MapGenerator.RoomInfo> GetRooms();
    }

    public class MapGenerator
    {
        public enum TileType
        {
            Wall = 0,
            Room = 1,
            Exit = 2,
            Border = 3, // special case for room maps
        }

        private struct Position
        {
            public int X;
            public int Y;

            public static Position operator +(Position p1, Position p2)
            {
                return new Position { X = p1.X + p2.X, Y = p1.Y + p2.Y };
            }
        }

        public class RoomMap
        {
            public TileType[,] Map;
            public List<ExitInfo> Exits = new List<ExitInfo>();

            public int Width;
            public int Height;
        }

        public class DynamicRoom
        {
            public int MinWidth;
            public int MaxWidth;
            public int MinHeight;
            public int MaxHeight;
            public bool HasBorder;

            public DynamicRoom(int minWidth, int maxWidth, int minHeight, int maxHeight, bool hasBorder = false)
            {
                MinWidth = minWidth;
                MaxWidth = maxWidth;
                MinHeight = minHeight;
                MaxHeight = maxHeight;
                HasBorder = hasBorder;             
            }
        }

        public class ExitInfo
        {
            public int X;
            public int Y;

            public ExitInfo(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public class RoomInfo
        {
            public string RoomName;
            public int MinExits;
            public int MaxExits;
            public bool CleanDeadEnds;
            public int ProbWeight;

            public RoomMap RoomMap;
            public DynamicRoom DynamicRoom;

            public RoomInfo()
            {
            }

            private RoomInfo(string roomName, int minExits, int maxExits)
            {
                RoomName = roomName;
                MinExits = minExits;
                MaxExits = maxExits;
                ProbWeight = 1;
            }

            public RoomInfo(string roomName, int minExits, int maxExits, RoomMap roomMap)
                :this(roomName, minExits, maxExits)
            {
                RoomMap = roomMap;
            }

            public RoomInfo(string roomName, int minExits, int maxExits, DynamicRoom dynamicRoom)
                : this(roomName, minExits, maxExits)
            {
                DynamicRoom = dynamicRoom;
            }
        }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public IList<RoomInfo> Rooms { get; set; }

        private readonly Random _random = new Random();
        private readonly Queue<Position> _queue = new Queue<Position>();
        private readonly List<Position> _exitsToCheck = new List<Position>();

        private TileType[,] _tileMap;
        private readonly int _maxTriesToBuildNewRoom = 10;
        private readonly int _maxAdditionalExitsCount = 50;
        private readonly float _minFreeTileRatio;
        private readonly bool _useAlsoDefaultRooms;

        private readonly IList<Position> _directions = new List<Position>
        {
            new Position {X = 0, Y = 1},
            new Position {X = 0, Y = -1},
            new Position {X = 1, Y = 0},
            new Position {X = -1, Y = 0},
        };

        public MapGenerator(int width, int height, float minFreeTileRatio, IRoomQuery roomQuery = null, bool useAlsoDefaultRooms = false)
        {
            if (width < 30 || height < 30) throw new ArgumentOutOfRangeException("width and height must be over 30");

            MapWidth = width;
            MapHeight = height;
            _minFreeTileRatio = minFreeTileRatio;
            _useAlsoDefaultRooms = useAlsoDefaultRooms;
            if (roomQuery != null)
            {
                Rooms = roomQuery.GetRooms();
            }
        }

        public TileType[,] GenerateMap()
        {
            if (Rooms == null || Rooms.Count == 0)
            {
                Rooms = GenerateDefaultRooms();
            }
            else if (_useAlsoDefaultRooms)
            {
                var addRooms = GenerateDefaultRooms();
                Rooms = Rooms.Concat(addRooms).ToList();
            }

            if (Rooms == null) return null;

            FillMapWithWalls();

            BuildCenterRoom();

            BuildAllRooms();

            ClearDeadEnds();

            return _tileMap;
        }

        private void BuildAllRooms()
        {
            int freeTilesCount;
            int count = 0;
            var minFreeTilesCount = MapHeight * MapWidth * _minFreeTileRatio;
            do
            {
                while (_queue.Count > 0)
                {
                    var pos = _queue.Dequeue();

                    var choice = _random.Next(0, Rooms.Count);
                    var roomInfo = Rooms[choice];

                    var counter = 0;
                    var roomBuilt = false;
                    while (counter < _maxTriesToBuildNewRoom && roomBuilt == false)
                    {
                        roomBuilt = BuildRoom(pos, roomInfo);
                        counter++;
                    }
                }

                count++;
                freeTilesCount = GetFreeTilesCount();
                if (freeTilesCount < minFreeTilesCount)
                {
                    AddExtraExit();
                }
            }
            while (freeTilesCount < minFreeTilesCount && count < _maxAdditionalExitsCount);
        }

        private void AddExtraExit()
        {
            var x = _random.Next(0, MapWidth);
            var y = _random.Next(0, MapHeight);
            while (!IsTileWall(x, y) || !HasFreeNeighbor(new Position { X = x, Y = y }))
            {
                x++;
                if (x == MapWidth)
                {
                    x = 0;
                    y = (y + 1) % MapHeight;
                }
            }

            var pos = new Position { X = x, Y = y };
            AddExitAtPosition(pos);
            _exitsToCheck.Add(pos);
        }

        private bool HasFreeNeighbor(Position pos)
        {
            var freeAround = _directions
                .Select(dir => pos + dir)
                .Count(newPos => !IsPositionOutOfBounds(newPos) && _tileMap[newPos.X, newPos.Y] == TileType.Room);
            return freeAround > 0;
        }

        private int GetFreeTilesCount()
        {
            return ForEachTile(t => t == TileType.Room ? 1 : 0).Sum();
        }

        private IEnumerable<T> ForEachTile<T>(Func<TileType, T> func)
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    yield return func(_tileMap[i, j]);
                }
            }
        }

        private void FillMapWithWalls()
        {
            _tileMap = new TileType[MapWidth, MapHeight];
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    _tileMap[i, j] = TileType.Wall;
                }
            }
        }

        private void ClearDeadEnds()
        {
            foreach (var exit in _exitsToCheck)
            {
                var wallsAround = _directions
                    .Select(dir => exit + dir)
                    .Count(newPos => !IsPositionOutOfBounds(newPos) && _tileMap[newPos.X, newPos.Y] == TileType.Room);

                if (wallsAround < 2)
                {
                    _tileMap[exit.X, exit.Y] = TileType.Wall;
                }
            }

            _exitsToCheck.Clear();
        }

        private bool BuildRoomFromRoomMap(Position pos, RoomInfo room)
        {
            var map = room.RoomMap;
            var entrancePositions = GetEntrancePositionsFromRoom(room.RoomMap);
            var start = _random.Next(0, entrancePositions.Count);
            for (int i = 0; i < entrancePositions.Count; i++)
            {
                var entranceNumber = (start + i) % (entrancePositions.Count);
                var entrance = entrancePositions[entranceNumber];
                var roomPosition = new Position { X = pos.X - entrance.X, Y = pos.Y - entrance.Y };
                var isRoomAvailable = IsRoomAvailable(roomPosition, map);

                if (isRoomAvailable)
                {
                    CreateRoom(roomPosition, map);
                    FindAndQueueExits(_random.Next(room.MinExits, room.MaxExits + 1), roomPosition, entrancePositions, room.CleanDeadEnds);
                    return true;
                }
            }

            return false;
        }

        private IList<Position> GetEntrancePositionsFromRoom(RoomMap map)
        {
            return map.Exits.Select(ri => new Position {X = ri.X, Y = ri.Y}).ToList();
        }

        private RoomMap BuildRoomMap(int width, int height, bool useBorder)
        {
            var adjWidth = useBorder ? width + 2 : width;
            var adjHeight = useBorder ? height + 2 : height;
            var map = new RoomMap
            {
                Width = adjWidth,
                Height = adjHeight,
                Map = new TileType[adjWidth, adjHeight]
            };

            for (int i = 0; i < adjWidth; i++)
            {
                for (int j = 0; j < adjHeight; j++)
                {
                    map.Map[i, j] = TileType.Room;
                }
            }

            if (useBorder)
            {
                map.Map[0, 0] = TileType.Border;
                map.Map[0, adjHeight - 1] = TileType.Border;
                map.Map[adjWidth - 1, 0] = TileType.Border;
                map.Map[adjWidth - 1, adjHeight - 1] = TileType.Border;
            }

            var exits = new List<ExitInfo>();

            for (int i = 1; i < adjWidth - 1; i++)
            {
                if (useBorder)
                {
                    map.Map[i, 0] = TileType.Border;
                    map.Map[i, adjHeight - 1] = TileType.Border;
                }

                exits.Add(new ExitInfo(i, 0));
                exits.Add(new ExitInfo(i, adjHeight - 1));
            }

            for (int i = 1; i < adjHeight - 1; i++)
            {
                if (useBorder)
                {
                    map.Map[0, i] = TileType.Border;
                    map.Map[adjWidth - 1, i] = TileType.Border;
                }

                exits.Add(new ExitInfo(0, i));
                exits.Add(new ExitInfo(adjWidth - 1, i));
            }

            map.Exits = exits;

            return map;
        }

        private bool BuildRoom(Position pos, RoomInfo room)
        {
            if (room.RoomMap == null && room.DynamicRoom != null)
            {
                var width = _random.Next(room.DynamicRoom.MinWidth, room.DynamicRoom.MaxWidth + 1);
                var height = _random.Next(room.DynamicRoom.MinHeight, room.DynamicRoom.MaxHeight + 1);

                room.RoomMap = BuildRoomMap(width, height, room.DynamicRoom.HasBorder);
            }

            return BuildRoomFromRoomMap(pos, room);
        }

        private void FindAndQueueExits(int number, Position leftTop, IList<Position> entrancePositions, bool addForDeadEndCheck)
        {
            for (var n = 0; n < number; n++)
            {
                var start = _random.Next(0, entrancePositions.Count);

                for (int i = 0; i <= entrancePositions.Count; i++)
                {
                    var entranceNumber = (start + i) % (entrancePositions.Count);
                    var entrance = entrancePositions[entranceNumber];
                    var entrancePosition = leftTop + entrance;
                    if (IsTileWall(entrancePosition))
                    {
                        AddExitAtPosition(entrancePosition);
                        if (addForDeadEndCheck)
                        {
                            _exitsToCheck.Add(entrancePosition);
                        }
                        break;
                    }
                }
            }
        }

        private bool IsRoomAvailable(Position leftTop, RoomMap roomMap)
        {
            for (int i = 0; i < roomMap.Width; i++)
            {
                for (int j = 0; j < roomMap.Height; j++)
                {
                    var newPos = leftTop + new Position { X = i, Y = j };
                    if (IsPositionOutOfBounds(newPos)) return false;
                    if ((roomMap.Map[i, j] == TileType.Room || roomMap.Map[i, j] == TileType.Border) && _tileMap[newPos.X, newPos.Y] == TileType.Room)
                        return false;
                }
            }

            return true;
        }

        private void CreateRoom(Position leftTop, RoomMap roomMap)
        {
            for (int i = leftTop.X; i < leftTop.X + roomMap.Width; i++)
            {
                for (int j = leftTop.Y; j < leftTop.Y + roomMap.Height; j++)
                {
                    if (roomMap.Map[i - leftTop.X, j - leftTop.Y] == TileType.Room)
                    {
                        _tileMap[i, j] = TileType.Room;
                    }
                }
            }
        }

        private void AddExitAtPosition(Position pos)
        {
            _tileMap[pos.X, pos.Y] = TileType.Exit;
            _queue.Enqueue(pos);
        }

        private void BuildCenterRoom()
        {
            CreateRoom(new Position { X = MapWidth / 2 - 5, Y = MapHeight / 2 - 5 }, GenerateCentralRoomMap());

            var exit1 = new Position { X = MapWidth / 2 - 5, Y = MapHeight / 2 };
            var exit2 = new Position { X = MapWidth / 2 + 5, Y = MapHeight / 2 };
            var exit3 = new Position { X = MapWidth / 2, Y = MapHeight / 2 - 5 };
            var exit4 = new Position { X = MapWidth / 2, Y = MapHeight / 2 + 5 };

            AddExitAtPosition(exit1);
            AddExitAtPosition(exit2);
            AddExitAtPosition(exit3);
            AddExitAtPosition(exit4);
        }

        private bool IsTileWall(Position pos)
        {
            return IsTileWall(pos.X, pos.Y);
        }

        private bool IsTileWall(int x, int y)
        {
            return !IsPositionOutOfBounds(x, y) && _tileMap[x, y] == TileType.Wall;
        }

        private bool IsPositionOutOfBounds(int x, int y)
        {
            return x <= 0 || x >= MapWidth - 1 || y <= 0 || y >= MapHeight - 1;
        }

        private bool IsPositionOutOfBounds(Position pos)
        {
            return IsPositionOutOfBounds(pos.X, pos.Y);
        }

        private List<RoomInfo> GenerateDefaultRooms()
        {
            return new List<RoomInfo>
            {
                new RoomInfo("Small Room", 2, 4, new DynamicRoom(3, 5, 3, 5, true)) { CleanDeadEnds = true}, // small room
                new RoomInfo("Large Room", 3, 5, new DynamicRoom(6, 12, 6, 12, true)) { CleanDeadEnds = true}, // large room
                new RoomInfo("Small corridor 1", 1, 2, new DynamicRoom(1, 2, 4, 7, true)) { CleanDeadEnds = true}, // small corridor
                new RoomInfo("Small corridor 2", 1, 2, new DynamicRoom(4, 7, 1, 2, true)) { CleanDeadEnds = true}, // small corridor
                new RoomInfo("Large corridor 1", 1, 5, new DynamicRoom(3, 5, 6, 12, true)) { CleanDeadEnds = true}, // large corridor
                new RoomInfo("Large corridor 2" , 1, 5, new DynamicRoom(6, 12, 3, 5, true)) { CleanDeadEnds = true},  // large corridor
                new RoomInfo("Circle", 1, 3, new RoomMap { // round room
                    Height = 7, 
                    Width = 7, 
                    Map = new[,]
                {
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Border,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Border,TileType.Room,TileType.Border,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Border, TileType.Room,TileType.Room,TileType.Room,TileType.Border,TileType.Wall},
                    {TileType.Border, TileType.Room, TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Border},
                    {TileType.Wall, TileType.Border, TileType.Room,TileType.Room,TileType.Room,TileType.Border,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Border,TileType.Room,TileType.Border,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Border,TileType.Wall,TileType.Wall,TileType.Wall}
                }}) {CleanDeadEnds = true}
            };
        }

        private RoomMap GenerateCentralRoomMap()
        {
            return new RoomMap
            {
                Height = 11,
                Width = 11,
                Map = new[,]
                {
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Room,TileType.Room,TileType.Room,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Room,TileType.Room,TileType.Room,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Room,TileType.Room,TileType.Room,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Room, TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Wall},
                    {TileType.Wall, TileType.Room, TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Wall},
                    {TileType.Wall, TileType.Room, TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Room,TileType.Room,TileType.Room,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Room,TileType.Room,TileType.Room,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Room,TileType.Room,TileType.Room,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall},
                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall,TileType.Wall}
                }
            };
        }
    }
}
