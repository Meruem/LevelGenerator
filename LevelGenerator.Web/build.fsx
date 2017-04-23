// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#I "../packages"
#I "../../repository/packages"
 
#r "FAKE/tools/FakeLib.dll"
#r "Suave/lib/net40/Suave.dll"
 
open System
open System.IO
open Fake
  
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
  
// Step 2. Use the packages
#load "app.fsx"

open Suave 
open Suave.Web
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