#r "paket:
source https://api.nuget.org/v3/index.json
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget FAKE.IO.Zip
nuget Fake.Core.Target
nuget FSharp.Json //"
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
    ++ "sample/**/bin"
    ++ "sample/**/obj"
    |> Shell.cleanDirs 
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    ++ "sample/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

open FSharp.Json

Target.create "Download" (fun _ ->
  let commitId = "febf00ee6bb92002a0de6132b26fc0a699f9923f"

  let token = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
  let url = @"https://api.github.com/repos/altseed/altseed2-csharp/actions/artifacts"

  let outputPath = @"lib/Altseed2"

  use client = new Net.Http.HttpClient()
  client.DefaultRequestHeaders.UserAgent.ParseAdd("wraikny.RouteTiles")
  client.DefaultRequestHeaders.Authorization <- Net.Http.Headers.AuthenticationHeaderValue("Bearer", token)

  let downloadName = sprintf "Altseed2-%s" commitId

  let rec getArchiveUrl page = async {
    Trace.tracefn "page %d" page
    let! data = client.GetStringAsync(sprintf "%s?page=%d" url page) |> Async.AwaitTask

    let artifacts =
      data
      |> Json.deserialize<{| artifacts: {| name: string; archive_download_url: string; expired: bool |} [] |}>

    if artifacts.artifacts |> Array.isEmpty then
      failwithf "'%s' is not found" downloadName
    
    match
      artifacts.artifacts
      |> Seq.tryFind(fun x -> x.name = downloadName) with
    | Some x when x.expired -> return failwithf "'%s' is expired" downloadName
    | Some x -> return x.archive_download_url
    | None -> return! getArchiveUrl (page + 1)
  }

  let outputFilePath = sprintf "%s.zip" outputPath

  async {
    let! archiveUrl = getArchiveUrl 1

    let! res =
      client.GetAsync(archiveUrl, Net.Http.HttpCompletionOption.ResponseHeadersRead)
      |> Async.AwaitTask

    use fileStream = IO.File.Create(outputFilePath)
    use! httpStream = res.Content.ReadAsStreamAsync() |> Async.AwaitTask
    do! httpStream.CopyToAsync(fileStream) |> Async.AwaitTask
    do! fileStream.FlushAsync() |> Async.AwaitTask
  } |> Async.RunSynchronously

  Zip.unzip outputPath outputFilePath
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "All"

Target.runOrDefault "All"
