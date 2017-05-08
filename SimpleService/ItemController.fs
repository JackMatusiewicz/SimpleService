module ItemController

open System.Web.Http
open StoreAgent
open Result

type ItemController (getUsersForItem : (string -> Async<Result<User list>>)) =
    inherit ApiController ()

    let getUsersForItem = getUsersForItem

    [<HttpGetAttribute>]
    [<RouteAttribute("api/v1/item/{itemName}")>]
    member this.Get (itemName : string) : string[] =
        let result = itemName |> getUsersForItem |> Async.RunSynchronously
        match result with
        | Success x ->
            x |> List.map (fun (User user) -> user) |> Array.ofList
        | Failure f -> [|f|]