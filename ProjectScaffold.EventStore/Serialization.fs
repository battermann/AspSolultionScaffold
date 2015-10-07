namespace ProjectScaffold.Data

open System
open Newtonsoft.Json
open Microsoft.FSharp.Reflection

module Serialization =

    let settings = 
        let settings = new JsonSerializerSettings()
        settings.TypeNameHandling <- TypeNameHandling.Auto
        settings

    let deserialize<'a> serializedString = 
        JsonConvert.DeserializeObject<'a>(serializedString, settings)

    let serialize (event:'a)= 
        let serializedEvent = JsonConvert.SerializeObject(event, settings)
        let case,_ = FSharpValue.GetUnionFields(event, typeof<'a>)
        case.Name, serializedEvent

