// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake

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
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
    -- "*.zip"
    |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

Target "DeployAzure" (fun _ ->
    CopyDir (wwwRoot "LevelGenerator.Web") "LevelGenerator.Web" allFiles
    CopyDir (wwwRoot "build") "build" allFiles
    CopyDir (wwwRoot "packages") "packages" allFiles
    CopyFile @"d:\home\site\wwwroot" @"LevelGenerator.Web\web.config" )

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
