namespace WebSharper.Community.PowerBI.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
open Reports
open WebSharper.Community.PowerBI

[<JavaScript>]
module Client =

    let Main (report:ReportType) =

        let powerbi_target = divAttr [attr.style "height:720px"] []

        let powerbi_settings = PowerBISettings(FilterPaneEnabled = true)
        
        let powerbi_conf = 
            PowerBIConfig(
                AccessToken = report.AccessToken, 
                Type = "report", 
                EmbedUrl = report.EmbedUrl,
                Id = report.ReportId,
                Settings = powerbi_settings)
        
        let start_embed () : unit = 
            async {
              Powerbi.Embed(powerbi_target.Dom, powerbi_conf)
            } |> Async.Start

        div [
            Doc.Button "Start embed powerbi" [attr.``class`` "btn btn-primary"]  (fun _ -> start_embed())

            powerbi_target
        ]