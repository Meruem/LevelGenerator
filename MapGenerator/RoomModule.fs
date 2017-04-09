module RoomModule

open System
open System.Collections.Generic

open RoomTypes
open RoomMaps
    
let random = new Random()

let IsPositionOutOfBounds pos (levelMap : LevelMap) =
    pos.X <= 0 || pos.X >= levelMap.Width - 1 || pos.Y <= 0 || pos.Y >= levelMap.Height - 1

let CreateRoom position roomMap (tileMap_ : TileType[,]) =
    roomMap |> Array2D.iteri (fun i j tile -> 
                                if tile = TileType.Room then 
                                    do tileMap_.[position.X + i, position.Y + j] <- TileType.Room 
                                else ())
    
let AddExitAtPosition pos (tileMap_ : TileType[,]) (queue_ : Queue<Position>) =
    tileMap_.[pos.X, pos.Y] <- TileType.Exit
    queue_.Enqueue pos

let BuildCenterRoom (levelMap : LevelMap) (queue_ : Queue<Position>) =
    CreateRoom { X = levelMap.Width / 2 - 5; Y = levelMap.Height / 2 - 5 } CentralRoomMap.Map levelMap.Tiles
    AddExitAtPosition { X = levelMap.Width / 2 - 5; Y = levelMap.Height / 2 } levelMap.Tiles queue_
    AddExitAtPosition { X = levelMap.Width / 2 + 5; Y = levelMap.Height / 2 } levelMap.Tiles queue_
    AddExitAtPosition { X = levelMap.Width / 2; Y = levelMap.Height / 2 - 5 } levelMap.Tiles queue_
    AddExitAtPosition { X = levelMap.Width / 2; Y = levelMap.Height / 2 + 5 } levelMap.Tiles queue_

let BuildAllRooms (minRatio:float) maxTries (rooms : Room list) levelMap (queue : Queue<Position>) =

    let exitsToCheck = new ResizeArray<Position> ()

    let IsRoomAvailable roomPosition (roomMap : TileType[,]) =
        let rec innerRec n = 
            let w = roomMap.GetLength(0)
            let j = n / w
            let i = n % w
            let newPos = { X = roomPosition.X + i; Y =  roomPosition.Y + j }
            if IsPositionOutOfBounds newPos levelMap then false
            else if (roomMap.[i, j] = TileType.Room || roomMap.[i, j] = TileType.Border) && levelMap.Tiles.[newPos.X, newPos.Y] = TileType.Room then false
                else if n >= (roomMap.Length - 1) then true else innerRec (n + 1)
        innerRec 0

    let rec GetAvailableEntrancePosition i start (entrancePositions : Position[]) pos map =
        let entranceNumber = (start + i) % (entrancePositions.Length)
        let entrance = entrancePositions.[entranceNumber]
        let roomPosition = { X = pos.X - entrance.X; Y = pos.Y - entrance.Y }
        let isRoomAvailable = IsRoomAvailable roomPosition map
        if isRoomAvailable then Some roomPosition
        else if i = (entrancePositions.Length - 1) then None
                else GetAvailableEntrancePosition (i+1) start entrancePositions pos map

    let rec FindAndQueueExit i leftTop start (entrancePositions:Position[]) addForDeadEndCheck =
        let entranceNumber = (start + 0) % (entrancePositions.Length)
        let entrance = entrancePositions.[entranceNumber]
        let entrancePosition = { X = leftTop.X + entrance.X; Y = leftTop.Y + entrance.Y }
        if levelMap.Tiles.[entrancePosition.X, entrancePosition.Y] = TileType.Wall && not(IsPositionOutOfBounds entrancePosition levelMap) then
            AddExitAtPosition entrancePosition levelMap.Tiles queue
            if addForDeadEndCheck then
                exitsToCheck.Add(entrancePosition)
            else ()
        else if i >= entrancePositions.Length then ()
                else FindAndQueueExit (i + 1) leftTop start entrancePositions addForDeadEndCheck


    let FindAndQueueExits number leftTop (entrancePositions:Position[]) addForDeadEndCheck=
        [1..number] |> List.iter (fun _ ->
                let start = random.Next(0, entrancePositions.Length)
                FindAndQueueExit 0 leftTop start  entrancePositions addForDeadEndCheck)

    let TryBuildRoomFromRoomMap pos roomInfo =
        match roomInfo.RoomMap with
        | DynamicRect _ -> false
        | Blueprint blueprint ->
            let start = random.Next(0, blueprint.Exits.Length)
            let roomPosition = GetAvailableEntrancePosition 0 start blueprint.Exits pos blueprint.Map
            match roomPosition with
            | None -> false
            | Some entrancePosition -> 
                CreateRoom entrancePosition blueprint.Map levelMap.Tiles
                let exitCount = random.Next(roomInfo.MinExits, roomInfo.MaxExits + 1)
                FindAndQueueExits exitCount entrancePosition blueprint.Exits roomInfo.CleanDeadEnds
                true

    // Creates tiles map from dynamic room specification
    let BuildRoomMap width height hasBorder =
            let adjWidth = if hasBorder then width + 2 else width
            let adjHeight = if hasBorder then height + 2 else height

            let mutable map = Array2D.create adjWidth adjHeight TileType.Room

            if hasBorder then
                map.[0, 0] <- TileType.Border;
                map.[0, adjHeight - 1] <- TileType.Border;
                map.[adjWidth - 1, 0] <- TileType.Border;
                map.[adjWidth - 1, adjHeight - 1] <- TileType.Border;
            else ()

            let exits1 = [1..adjWidth - 2] |> List.fold (fun acc i ->
                            if hasBorder then
                                map.[i, 0] <- TileType.Border
                                map.[i, adjHeight - 1] <- TileType.Border
                            else ()
                
                            {X = i; Y = 0} :: {X = i; Y = adjHeight - 1} :: acc
                            ) []

            let exits2 = [1..adjHeight - 2] |> List.fold (fun acc i ->
                            if hasBorder then
                                map.[0, i] <- TileType.Border
                                map.[adjWidth - 1, i] <- TileType.Border
                            else ()
                
                            {X = 0; Y = i} :: {X = adjWidth - 1; Y = i} :: acc
                            ) []

            {
                Width = adjWidth;
                Height = adjHeight;
                Map = map;
                Exits = List.toArray (exits1 @ exits2)
            }

    let BuildRoom pos (room:Room) =
        match room.RoomMap with
        | Blueprint _ -> TryBuildRoomFromRoomMap pos room
        | DynamicRect dyn -> 
            let width = random.Next(dyn.MinWidth, dyn.MaxWidth + 1)
            let height = random.Next(dyn.MinHeight, dyn.MaxHeight + 1)
            let blueprint = BuildRoomMap width height dyn.HasBorder
            TryBuildRoomFromRoomMap pos { room with RoomMap = Blueprint blueprint } 

    let rec BuildSingleRoom pos roomInfo count =
        if count = 0 then ()
        else
            let roomBuilt = BuildRoom pos roomInfo
            if roomBuilt then ()
            else BuildSingleRoom pos roomInfo (count - 1)

    let GetFreeTilesCount () =
        let mutable sum = 0
        levelMap.Tiles |> Array2D.iter (fun tile -> 
            if tile = TileType.Room then sum <- (sum + 1) else ())
        sum

    let RoomAroundCount pos =
        let directions = [ {X = 0; Y = 1}; {X = 0; Y = -1}; {X = 1; Y = 0}; {X = -1; Y = 0}]
        directions |> List.fold (fun agg p -> 
            let newPos = {X = p.X + pos.X; Y = p.Y + pos.Y}
            if not (IsPositionOutOfBounds newPos levelMap) 
                && (levelMap.Tiles.[newPos.X, newPos.Y] = TileType.Room
                    || levelMap.Tiles.[newPos.X, newPos.Y] = TileType.Exit) then (agg + 1)
            else agg) 0

    let HasRoomNeighbor pos =
        RoomAroundCount pos > 0

    let AddExtraExit () =
        let x = random.Next(0, levelMap.Width)
        let y = random.Next(0, levelMap.Height)

        let IsTileWall pos =
            not (IsPositionOutOfBounds pos levelMap) && levelMap.Tiles.[pos.X, pos.Y] = TileType.Wall

        let rec AddExtraExitRec x y  =
            let pos = {X = x; Y = y} 
            if HasRoomNeighbor pos  && IsTileWall pos  then
                AddExitAtPosition pos levelMap.Tiles queue 
                exitsToCheck.Add(pos)
            else
                let newX = (x + 1) % levelMap.Width
                let newY = if (newX <> 0) then y else (y + 1) % levelMap.Height
                AddExtraExitRec newX newY 
                exitsToCheck.Add({X = newX; Y = newY;})
        AddExtraExitRec x y 

    let ClearDeadEnds () = 
        List.ofSeq exitsToCheck |> List.iter (fun pos ->
            let roomAround = RoomAroundCount pos 
            if roomAround < 2 then
                levelMap.Tiles.[pos.X, pos.Y] <-TileType.Wall
            else
                ())
        exitsToCheck.Clear()

    let rec BuildAllRoomsRec () = 
        if queue.Count = 0 then ()
        else 
            let pos = queue.Dequeue()
            let choice = random.Next(0, rooms.Length)
            let roomInfo = rooms.[choice]
            BuildSingleRoom pos roomInfo 10 
            BuildAllRoomsRec ()

    let rec BuildAllRoomsN n =
        BuildAllRoomsRec ()
        let minFreeTilesCount = float levelMap.Width * float levelMap.Height * minRatio
        if float (GetFreeTilesCount ()) > minFreeTilesCount then ()
        else
            if n > maxTries then () 
            else
                AddExtraExit ()
                BuildAllRoomsN (n + 1)

    BuildAllRoomsN 0
    ClearDeadEnds ()

