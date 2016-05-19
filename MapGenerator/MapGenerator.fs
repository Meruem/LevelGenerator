﻿namespace MapGeneratorF

open System
open System.Collections.Generic

open RoomTypes
open RoomMaps
open RoomModule

type MapGeneratorF(width, height, rooms) =

    let _queue = new Queue<Position>()
    let (_tileMap : TileType[,]) = Array2D.create width height TileType.Wall
    let (_exitsToCheck : List<Position>) = new List<Position> ()

    member this.GenerateMap () = 
        let levelMap = { Width = width; Height = height; Tiles = _tileMap}
        BuildCenterRoom levelMap _queue
        BuildAllRooms 0.8 50 rooms levelMap _queue _exitsToCheck
        ClearDeadEnds _exitsToCheck levelMap
        _tileMap
