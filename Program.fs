open System
open System.IO
open System.Net.Http
open System.Net.Http.Headers
open System.Text.Json.Nodes

[<EntryPoint>]
let main (args) =
    let input = args |> String.concat " "

    let configFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".hattie.json")


    if not (File.Exists configFilePath) then
        printfn "%s not found." configFilePath
        1
    else
        let config = JsonObject.Parse(File.ReadAllText(configFilePath))

        use client = new HttpClient()

        client.DefaultRequestHeaders.Authorization <-
            new AuthenticationHeaderValue("Bearer", config["token"].ToString())

        let url = config["url"].ToString()

        let response =
            client.PostAsync($"{url}api/conversation/process", new StringContent($"{{\"text\": \"{input}\"}}"))
            |> Async.AwaitTask
            |> Async.RunSynchronously

        let raw =
            response.Content.ReadAsStringAsync()
            |> Async.AwaitTask
            |> Async.RunSynchronously

        let json = JsonObject.Parse(raw)
        let reply = json["response"].["speech"].["plain"].["speech"].ToString()

        printfn "%s" reply
        0
