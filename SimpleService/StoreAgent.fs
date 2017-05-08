module StoreAgent

open Result

type User = User of string
type Item = Item of string
type ItemCache = ItemCache of Map<Item, User list>

type StoreQuery =
    | UserQuery of User * AsyncReplyChannel<Result<Item list>>
    | ItemQuery of Item * AsyncReplyChannel<Result<User list>>
    | FullDataQuery of AsyncReplyChannel<ItemCache>
    | UpdateCache of ItemCache

let getAllUsers (cache : ItemCache) (item : Item) : Result<User list> =
    let (ItemCache itemCache) = cache
    if Map.containsKey item itemCache then
        Map.find item itemCache |> Success
    else
        Failure "Unable to find item in cache"

let getAllItems (cache : ItemCache) (user : User) : Result<Item list> =
    let userHasAccessToItem (itemListing : Item * (User list)) =
        let (item, users) = itemListing
        List.contains user users

    let (ItemCache itemCache) = cache
    itemCache
        |> Map.toList
        |> List.filter userHasAccessToItem
        |> List.map fst
        |> Success

type StoreAgent () =
    let agent = MailboxProcessor.Start(fun inbox ->
        let rec loop (cache : ItemCache) = async {
            let! message = inbox.Receive()

            match message with
            | UserQuery (user, replyChannel) ->
                let result = getAllItems cache user
                replyChannel.Reply(result)
                return! loop cache
            | ItemQuery (item, replyChannel) ->
                let result = getAllUsers cache item
                replyChannel.Reply(result)
                return! loop cache
            | FullDataQuery replyChannel ->
                replyChannel.Reply(cache)
                return! loop cache
            | UpdateCache newCache ->
                return! loop newCache
        }
        loop (ItemCache Map.empty)
    )

    member this.FindItemsForUser (user : string) =
        agent.PostAndAsyncReply(fun ch -> UserQuery (User user,ch))

    member this.FindUsersForItem (item : string) =
        agent.PostAndAsyncReply(fun ch -> ItemQuery (Item item,ch))

    member this.GetData () =
        agent.PostAndAsyncReply(fun ch -> FullDataQuery ch)

    member this.UpdateCache(cache : ItemCache) =
        agent.Post((UpdateCache cache))