namespace WebSharper.Community.PowerBI.Sample

module Reports = 
    open System
    open System.Configuration
    open Microsoft.PowerBI.Api.V1.Models
    open Microsoft.PowerBI.Api.V1
    open Microsoft.PowerBI.Security
    open Microsoft.Rest
    //workspaceCollection .. roles are for real apps (to generate new real accessToken
    //code for real apps look  at ReportTest() not ReportSample()
    let workspaceCollection = ConfigurationManager.AppSettings.["powerbi:WorkspaceCollection"]
    let workspaceId = ConfigurationManager.AppSettings.["powerbi:WorkspaceId"]
    let accessKey = ConfigurationManager.AppSettings.["powerbi:AccessKey"]
    let apiUrl = ConfigurationManager.AppSettings.["powerbi:ApiUrl"]
    let roles = ConfigurationManager.AppSettings.["powerbi:Roles"]

    //look at description in App.conf how to get fresh sample and ready token - look at ReportSample()
    let sampleAccessToken = ConfigurationManager.AppSettings.["powerbi:AccessToken"]
    
    let expiration = ConfigurationManager.AppSettings.["powerbi:Expiration"] //minutes

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

    let Report (reportId:string) (clientname:string) = async {
        use client = powerBiClient()   
        let! reports = client.Reports.GetReportsAsync(workspaceCollection, workspaceId) |> Async.AwaitTask
        let report =   
            reports.Value |> Seq.tryFind (fun elem -> elem.Id = reportId)
        //let embedToken = PowerBIToken.CreateReportEmbedToken(workspaceCollection, workspaceId, report.Value.Id)
        let embedToken = 
            PowerBIToken.CreateReportEmbedToken(
                workspaceCollection, workspaceId, report.Value.Id, 
                createExpiration(),
                clientname, createRoles())
        let accessToken = embedToken.Generate(accessKey)

        return {ReportId = report.Value.Id; EmbedUrl = report.Value.EmbedUrl; AccessToken = accessToken}}
    ///clientname username() used in PowerBI Desktop to identify user and his/her 
    let ReportTest (clientname:string) = async {
        let reports = Reports()
        let report_to_show = reports.[reports.Count - 1]//take last report 
        let! report_data = Report report_to_show.Id clientname
        return report_data
    }


    let ReportSample () = async {
        let accessToken = sampleAccessToken
        return {ReportId = "c52af8ab-0468-4165-92af-dc39858d66ad"
                EmbedUrl = "https://embedded.powerbi.com/appTokenReportEmbed"
//                EmbedUrl = "https://embedded.powerbi.com/appTokenReportEmbed?reportId=c52af8ab-0468-4165-92af-dc39858d66ad"
                AccessToken = accessToken}}

