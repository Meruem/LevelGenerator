// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------
 
#r @"../packages/FAKE/tools/FakeLib.dll"
 
open System
open System.IO
open Fake
  
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
  
// Step 2. Use the packages
  
#r "../packages/Suave/lib/net40/Suave.dll"
#load "app.fsx"

open Suave // always open suave
open Suave.Web // for config
open Suave.Successful

open System.Net
open App

Target "run" (fun _ ->
    let port = Sockets.Port.Parse <| getBuildParamOrDefault "port" "8083"
    let serverConfig = 
        { defaultConfig with
            bindings = [ HttpBinding.create HTTP IPAddress.Loopback port ]
        }
    startWebServer serverConfig app)

RunTargetOrDefault "run"