namespace MapGeneratorF

open System
open System.Collections.Generic

open RoomTypes
open RoomMaps
open RoomModule

type MapGeneratorF(width, height, rooms) =

    let _queue = new Queue<Position>()
    let (_tileMap : TileType[,]) = Array2D.create width height TileType.Wall

    member this.GenerateMap () = 
        let levelMap = { Width = width; Height = height; Tiles = _tileMap}
        buildCenterRoom levelMap _queue
        buildAllRooms 0.8 50 rooms levelMap _queue
        _tileMap
