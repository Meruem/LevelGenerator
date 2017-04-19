#r "../packages/Suave/lib/net40/Suave.dll"
#r "../build/MapGenerator.dll"
#r "../packages/Newtonsoft.Json/lib/net40/Newtonsoft.Json.dll"

open System
open System.Threading
open System.Text
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open MapGeneratorF
open RoomMaps
open RoomTypes
open Newtonsoft.Json

let viewPath file = "LevelGenerator.Web/public/" + file  
let scriptPath file = "LevelGenerator.Web/public/scripts/" + file

let getTextResult (tileMap : TileType[,]) =
  let sb = StringBuilder()
  for i in [0 .. (tileMap.GetLength 0) - 1] do
    for j in [0 .. (tileMap.GetLength 1) - 1] do 
      match tileMap.[i, j] with
      | TileType.Wall -> sb.Append "." |> ignore
      | TileType.Room -> sb.Append "#" |> ignore
      | _ -> sb.Append "X" |> ignore
    sb.AppendLine "" |> ignore
  sb.ToString()

let app =
    choose
      [ GET >=> choose
          [ path "/" >=> Files.file (viewPath "index.html")
            path "/scripts/app.js" >=> Files.file (scriptPath "app.js")
            path "/3rdParty/vue.js" >=> Redirection.redirect "https://unpkg.com/vue" ]// Files.file "LevelGenerator.Web/node_modules/vue/dist/vue.js" ]
        GET >=>  pathScan "/generate/%d/%d" (fun (width, height) -> 
          let mg = MapGeneratorF(width, height, DefaultRooms)
          let result = mg.GenerateMap ()
          OK (JsonConvert.SerializeObject result)) ]

