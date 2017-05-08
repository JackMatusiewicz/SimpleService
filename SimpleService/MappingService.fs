module MappingService

open System.ServiceProcess
open System.ComponentModel
open System.Configuration.Install

type MappingService() as this =
    inherit ServiceBase()
    do
        this.ServiceName <- "My Windows Service"
        this.EventLog.Log <- "Application"
    override this.OnStart(args:string[]) =
        base.OnStart(args)
    override this.OnStop() =
        base.OnStop()