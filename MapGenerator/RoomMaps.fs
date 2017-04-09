module RoomMaps

open RoomTypes

let CentralRoomMap =
    {
        Height = 11;
        Width = 11;
        Map = array2D [[TileType.Wall; TileType.Wall; TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall];
                        [TileType.Wall; TileType.Wall; TileType.Wall;TileType.Wall;TileType.Room;TileType.Room;TileType.Room;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall];
                        [TileType.Wall; TileType.Wall; TileType.Wall;TileType.Wall;TileType.Room;TileType.Room;TileType.Room;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall];
                        [TileType.Wall; TileType.Wall; TileType.Wall;TileType.Wall;TileType.Room;TileType.Room;TileType.Room;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall];
                        [TileType.Wall; TileType.Room; TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Wall];
                        [TileType.Wall; TileType.Room; TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Wall];
                        [TileType.Wall; TileType.Room; TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Room;TileType.Wall];
                        [TileType.Wall; TileType.Wall; TileType.Wall;TileType.Wall;TileType.Room;TileType.Room;TileType.Room;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall];
                        [TileType.Wall; TileType.Wall; TileType.Wall;TileType.Wall;TileType.Room;TileType.Room;TileType.Room;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall];
                        [TileType.Wall; TileType.Wall; TileType.Wall;TileType.Wall;TileType.Room;TileType.Room;TileType.Room;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall];
                        [TileType.Wall; TileType.Wall; TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall;TileType.Wall]];
        Exits = [||];
    }

let DefaultRooms =
    [    
        {
            RoomName = "Small Room";
            MinExits = 2;
            MaxExits = 4;
            CleanDeadEnds = true;
            ProbWeight = 1;
            RoomMap = DynamicRect
                {
                    MinWidth = 3;
                    MaxWidth = 5;
                    MinHeight  = 3;
                    MaxHeight = 5;
                    HasBorder = true;
                }
        };
        {
            RoomName = "Large Room";
            MinExits = 3;
            MaxExits = 5;
            CleanDeadEnds = true;
            ProbWeight = 1;
            RoomMap = DynamicRect
                {
                    MinWidth = 6;
                    MaxWidth = 12;
                    MinHeight  = 6;
                    MaxHeight = 12;
                    HasBorder = true;
                }
        };
        {
            RoomName = "Small corridor 1";
            MinExits = 1;
            MaxExits = 2;
            CleanDeadEnds = true;
            ProbWeight = 1;
            RoomMap = DynamicRect
                {
                    MinWidth = 1;
                    MaxWidth = 2;
                    MinHeight  = 4;
                    MaxHeight = 7;
                    HasBorder = true;
                }
        };
        {
            RoomName = "Small corridor 2";
            MinExits = 1;
            MaxExits = 2;
            CleanDeadEnds = true;
            ProbWeight = 1;
            RoomMap = DynamicRect
                {
                    MinWidth = 4;
                    MaxWidth = 7;
                    MinHeight  = 1;
                    MaxHeight = 2;
                    HasBorder = true;
                }
        };
        {
            RoomName = "Large corridor 1";
            MinExits = 1;
            MaxExits = 5;
            CleanDeadEnds = true;
            ProbWeight = 1;
            RoomMap = DynamicRect
                {
                    MinWidth = 3;
                    MaxWidth = 5;
                    MinHeight  = 6;
                    MaxHeight = 12;
                    HasBorder = true;
                }
        };
        {
            RoomName = "Large corridor 2";
            MinExits = 1;
            MaxExits = 5;
            CleanDeadEnds = true;
            ProbWeight = 1;
            RoomMap = DynamicRect
                {
                    MinWidth = 6;
                    MaxWidth = 12;
                    MinHeight  = 3;
                    MaxHeight = 5;
                    HasBorder = true;
                }
        }
    ]
//                new RoomInfo("Circle", 1, 3, new RoomMap { // round room
//                    Height = 7, 
//                    Width = 7, 
//                    Map = new[,]
//                {
//                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Border,TileType.Wall,TileType.Wall,TileType.Wall},
//                    {TileType.Wall, TileType.Wall, TileType.Border,TileType.Room,TileType.Border,TileType.Wall,TileType.Wall},
//                    {TileType.Wall, TileType.Border, TileType.Room,TileType.Room,TileType.Room,TileType.Border,TileType.Wall},
//                    {TileType.Border, TileType.Room, TileType.Room,TileType.Room,TileType.Room,TileType.Room,TileType.Border},
//                    {TileType.Wall, TileType.Border, TileType.Room,TileType.Room,TileType.Room,TileType.Border,TileType.Wall},
//                    {TileType.Wall, TileType.Wall, TileType.Border,TileType.Room,TileType.Border,TileType.Wall,TileType.Wall},
//                    {TileType.Wall, TileType.Wall, TileType.Wall,TileType.Border,TileType.Wall,TileType.Wall,TileType.Wall}
//                }}) {CleanDeadEnds = true}
//            };
//        }
