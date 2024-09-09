open System
open System.IO

open FSharp.Data
open Flurl
open Flurl.Http

type Configuration =
    JsonProvider<"""
{
    "url": "asdf",
    "token": "asdf"
}""">

type ConversationResponse =
    JsonProvider<"""
{
    "response": {
        "speech": {
            "plain": {
                "speech": "asdf",
                "extra_data": null
            }
        },
        "card": {},
        "language": "asdf",
        "response_type": "asdf",
        "data": {}
    },
    "conversation_id": null
}""">


[<EntryPoint>]
let main (args) =
    let input = args |> String.concat " "

    let configFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".hattie.json")

    if not (File.Exists configFilePath) then
        printfn "%s not found." configFilePath
        1
    else
        let config = File.ReadAllText(configFilePath) |> Configuration.Parse

        let response =
            config.Url
                .AppendPathSegment("api/conversation/process")
                .WithOAuthBearerToken(config.Token)
                .PostJsonAsync({| text = input |})
                .ReceiveString()
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ConversationResponse.Parse

        printfn "%s" response.Response.Speech.Plain.Speech
        0
