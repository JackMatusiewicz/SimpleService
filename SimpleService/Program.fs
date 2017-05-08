open MappingService
open System.ServiceProcess
open System.ComponentModel
open System.Configuration.Install
open Startup
open Microsoft.Owin.Hosting
open ControllerActivator
open StoreAgent
open System.Threading
open Construction

let updateItemCache (agent : StoreAgent) =
    fun () ->
        let (newItemCache : ItemCache) = "Store.txt" |> constructCacheFromFile
        agent.UpdateCache newItemCache

[<EntryPoint>]
let main argv =
    let storeAgent = StoreAgent()
    let updateCacheCallback = updateItemCache storeAgent

    let timer = new System.Threading.Timer((fun s -> updateCacheCallback()), null, 5000, 10000)

    let baseAddress = "http://localhost:8880"
    let startup = Startup.Startup(
                    new ControllerActivator(storeAgent.FindItemsForUser,
                        storeAgent.FindUsersForItem))
    let app = WebApp.Start(baseAddress, fun x -> startup.Configuration(x))
    printfn "Starting web service at %s" baseAddress |> ignore
    printfn "Press enter to close." |> ignore
    System.Console.ReadLine() |> ignore
    0
