// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open NpmHelper

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"

let wwwRootDir = @"d:\home\site\wwwroot"

let wwwRoot dir = 
    wwwRootDir + @"\" + dir

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir; wwwRootDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

Target "NpmUpdate" (fun _ ->
    Npm (fun p ->
        { p with 
            Command = Install Standard
        })
)

Target "DeployAzure" (fun _ ->
    CopyDir (wwwRoot "LevelGenerator.Web") "LevelGenerator.Web" allFiles
    CopyFile @"d:\home\site\wwwroot" @"LevelGenerator.Web\web.config" )

// Build order
"Clean"
  ==> "NpmUpdate"  
  ==> "Build"
  ==> "DeployAzure"


// start build
RunTargetOrDefault "Build"
