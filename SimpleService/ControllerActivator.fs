module ControllerActivator
open System
open System.Net.Http
open System.Web.Http.Controllers
open System.Web.Http.Dispatcher
open UserController
open ItemController
open StoreAgent
open Result

type ControllerActivator (itemsForUser : string -> Async<Result<Item list>>,
                          usersForItem : string -> Async<Result<User list>>) =
    let itemsForUser = itemsForUser
    let usersForItem = usersForItem

    interface IHttpControllerActivator with
        member this.Create(
                            message:HttpRequestMessage,
                            descriptor:HttpControllerDescriptor,
                            controllerType:Type) : IHttpController =
            if controllerType = typeof<UserController> then
                new UserController(itemsForUser) :> IHttpController
            else if controllerType = typeof<ItemController> then
                new ItemController(usersForItem) :> IHttpController
            else
                failwith "Couldn't find the correct controller"
