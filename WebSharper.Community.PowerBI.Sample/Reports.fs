namespace WebSharper.Community.PowerBI.Sample

module Reports = 
    open System
    open System.Configuration
    open Microsoft.PowerBI.Api.V1.Models
    open Microsoft.PowerBI.Api.V1
    open Microsoft.PowerBI.Security
    open Microsoft.Rest

    let workspaceCollection = ConfigurationManager.AppSettings.["powerbi:WorkspaceCollection"]
    let workspaceId = ConfigurationManager.AppSettings.["powerbi:WorkspaceId"]
    let accessKey = ConfigurationManager.AppSettings.["powerbi:AccessKey"]
    let apiUrl = ConfigurationManager.AppSettings.["powerbi:ApiUrl"]
    let roles = ConfigurationManager.AppSettings.["powerbi:Roles"]
    
    let expiration = ConfigurationManager.AppSettings.["powerbi:Expiration"] //miutes

    let createRoles () = 
        seq {yield roles}

    let createExpiration() = 
        TimeSpan.FromMinutes (float expiration)

    let powerBiClient () =
        let  credentials = new TokenCredentials(accessKey, "AppKey");
        let client = new PowerBIClient(credentials)
        client.BaseUri <- Uri(apiUrl)
        client


    let Reports() =         
        use client = powerBiClient()                    
        let reportsResponse = client.Reports.GetReports(workspaceCollection, workspaceId)
        reportsResponse.Value

    type ReportType = {
        ReportId : string
        EmbedUrl : string
        AccessToken : string
    }

    let Report (reportId:string) (clienthash:string) = async {
        use client = powerBiClient()   
        let! reports = client.Reports.GetReportsAsync(workspaceCollection, workspaceId) |> Async.AwaitTask
        let report =   
            reports.Value |> Seq.tryFind (fun elem -> elem.Id = reportId)
        //let embedToken = PowerBIToken.CreateReportEmbedToken(workspaceCollection, workspaceId, report.Value.Id)
        let embedToken = 
            PowerBIToken.CreateReportEmbedToken(
                workspaceCollection, workspaceId, report.Value.Id, 
                createExpiration(),
                clienthash, createRoles())
        let accessToken = embedToken.Generate(accessKey)
        return {ReportId = report.Value.Id; EmbedUrl = report.Value.EmbedUrl; AccessToken = accessToken}}

    let ReportTest (client:string) = async {
        let reports = Reports()
        let report_to_show = reports.[reports.Count - 1]//take last report 
        let! report_data = Report report_to_show .Id client
        return report_data
    }


