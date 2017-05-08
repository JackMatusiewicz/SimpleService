module Construction

open Result
open StoreAgent

let readFile (fileName : string) = seq {
    use streamReader = new System.IO.StreamReader(fileName)
    while streamReader.EndOfStream <> true do
        yield streamReader.ReadLine()
}

//For the time being, this takes the approach of just avoiding lines that we can't match, rather than throwing a fit.
let parseLine (line : string) : Result<Item * (User list)> =
    let itemAndUsers = line.Split([|':'|])
    if itemAndUsers.Length <> 2 then
        Failure "Cannot parse line"
    else
        let users = itemAndUsers.[1].Split([|','|])
                        |> List.ofArray
                        |> List.map (fun a -> User a)
        (Item itemAndUsers.[0], users) |> Success

let addLineToCache (line : Result<Item * (User list)>) (cache : ItemCache) : ItemCache =
    let (ItemCache itemCache) = cache
    match line with
    | Success (item, users) -> (ItemCache (Map.add item users itemCache))
    | Failure _ -> cache

let addToCache = parseLine >> addLineToCache

let constructCacheFromFile fileName =
    readFile fileName
    |> Seq.fold (fun (acc : ItemCache) (elem :string) -> addToCache elem acc) (ItemCache Map.empty)