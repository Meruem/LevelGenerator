module RoomTypes

type TileType =
    | Wall = 0
    | Room = 1
    | Exit = 2
    | Border = 3

type Position = {X:int; Y:int} 
    
type ExitInfo = Position

type LevelMap =
    {
        Width : int
        Height : int
        Tiles : TileType[,]
    }

type RoomBlueprint = 
    {
        Map : TileType[,]
        Exits : ExitInfo[]
        Width : int
        Height: int
    }

type DynamicRectRoom =
    {
        MinWidth : int
        MaxWidth : int
        MinHeight : int
        MaxHeight : int
        HasBorder : bool
    }

type TileMap =
    | Blueprint of RoomBlueprint
    | DynamicRect of DynamicRectRoom

type Room =
    {
            RoomName : string
            MinExits : int
            MaxExits : int
            CleanDeadEnds : bool
            ProbWeight : int
            RoomMap : TileMap
    }

