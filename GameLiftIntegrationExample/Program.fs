// F# の詳細については、http://fsharp.org を参照してください
// 詳細については、'F# チュートリアル' プロジェクトを参照してください。

open Aws.GameLift.Server
open Aws.GameLift.Server.Model
open System.Diagnostics
open System.Collections.Generic
open System

[<EntryPoint>]
let main argv = 
    try
        let listeningPort = 7777

        let initSDKOutcome = GameLiftServerAPI.InitSDK()
        if initSDKOutcome.Success then
            let onStartGameSession = ProcessParameters.OnStartGameSessionDelegate(fun (gameSession : GameSession) ->
                GameLiftServerAPI.ActivateGameSession() |> ignore
            )

            let onProcessTerminate = ProcessParameters.OnProcessTerminateDelegate(fun () -> 
                GameLiftServerAPI.ProcessEnding() |> ignore
            )
            let onHealthCheck = ProcessParameters.OnHealthCheckDelegate(fun () -> true)

            let processParameters = new ProcessParameters(
                                        onStartGameSession,
                                        onProcessTerminate,
                                        onHealthCheck,
                                        listeningPort,
                                        new LogParameters(new List<string>(["/local/game/logs/myserver.log"])))

            let processReadyOutcome = GameLiftServerAPI.ProcessReady(processParameters)
            if processReadyOutcome.Success then
               printfn "ProcessReadySuccess"
            else
               printfn "ProcessReadyFailure : %A" processReadyOutcome.Error
        else
            printfn "InitSDK failure : %A" initSDKOutcome.Error
        
        Console.Read()

        0 // 整数の終了コードを返します
    finally
        GameLiftServerAPI.Destroy() |> ignore
