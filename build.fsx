#r "paket:
source https://api.nuget.org/v3/index.json
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget FAKE.IO.Zip
nuget Fake.Core.Target //"
#load ".fake/build.fsx/intellisense.fsx"
#r "netstandard"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

open System

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "examples/**/bin"
    ++ "examples/**/obj"
    |> Shell.cleanDirs 
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    ++ "examples/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "All"

Target.runOrDefault "All"
