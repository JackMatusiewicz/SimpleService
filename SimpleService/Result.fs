module Result

type Result<'T> = | Success of 'T | Failure of string

let returnResult (x : 'a) : Result<'a> = Success x

let returnFromResult (x : Result<'a>) = x

let mapResult (f : 'a -> 'b) (a : Result<'a>) : Result<'b> =
    match a with
    | Success x -> f x |> Success
    | Failure f -> Failure f

let bindResult (f : 'a -> Result<'b>) (a : Result<'a>) =
    match a with
    | Failure f -> Failure f
    | Success x -> f x

let (>=>) f g = fun x -> bindResult g (f x)