namespace WebSharper.Community.PowerBI

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let R1 = Resource "PowerBIResource" "powerbi.js"

    let PB = Class "powerbi"

    let PowerBISettings =
        Pattern.Config "PowerBISettings" {
            Required = []
            Optional =
                [
                    "filterPaneEnabled" , T<bool>             
                ]
        }

    let PowerBIConfig =
        Pattern.Config "PowerBIConfig" {
            Required = []
            Optional =
                [
                    "accessToken" , T<string>
                    "embedUrl", T<string>
                    "type", T<string>
                    "id", T<string>  
                    "settings", PowerBISettings.Type
                ]
        }

    let PowerBIClass =
        PB
        |+> Static [
                "embed" => T<Dom.Element>?target * PowerBIConfig?config ^-> T<unit>
            ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Community.PowerBI" [
                PowerBIConfig
                PowerBISettings
                PowerBIClass
            ]
            Namespace "WebSharper.Community.PowerBI.Resources" [                
                R1
                |> fun r -> r.AssemblyWide()
            ] 
        ] |> Requires [R1]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
