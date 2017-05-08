module UserController

open System.Web.Http
open StoreAgent
open Result

type UserController (getForUser : (string -> Async<Result<Item list>>)) =
    inherit ApiController ()

    let getItemsForUser = getForUser

    [<HttpGetAttribute>]
    [<RouteAttribute("api/v1/user/{username}")>]
    member this.Get (username : string) : string[] =
        let result = username |> getItemsForUser |> Async.RunSynchronously
        match result with
        | Success x ->
            x |> List.map (fun (Item item) -> item) |> Array.ofList
        | Failure f -> [|f|]