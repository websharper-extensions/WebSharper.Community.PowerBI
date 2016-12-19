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

        let rvInput = Var.Create ""
        let submit = Submitter.CreateOption rvInput.View

        let vReversed =
            submit.View.MapAsync(function
                | None -> async { return "" }
                | Some input -> Server.DoSomething input
            )

        let powerbi_target = divAttr [attr.style "height:720px"] []
        let powerbi_settings = PowerBISettings(FilterPaneEnabled = false)
        let powerbi_conf = 
            PowerBIConfig(
                AccessToken = report.AccessToken, 
                Type = "report", 
//                EmbedUrl = report.EmbedUrl,
                EmbedUrl = "https://embedded.powerbi.com/appTokenReportEmbed",
                Id = report.ReportId,
                Settings = powerbi_settings)
        let start_embed () : unit = 
            async {
              Powerbi.Embed(powerbi_target.Dom, powerbi_conf)
            } |> Async.Start

        div [
            Doc.Input [] rvInput
            Doc.Button "Send" [] submit.Trigger
            hr []
            h4Attr [attr.``class`` "text-muted"] [text "The server responded:"]
            divAttr [attr.``class`` "jumbotron"] [h1 [textView vReversed]]

            Doc.Button "Start embed powerbi" [attr.``class`` "btn btn-primary"]  (fun _ -> start_embed())

            powerbi_target
        ]