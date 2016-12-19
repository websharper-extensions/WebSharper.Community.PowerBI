namespace WebSharper.Community.PowerBI.Sample

open WebSharper

module Server =

    [<Remote>]
    let DoSomething input =
        let R (s: string) = System.String(Array.rev(s.ToCharArray()))
        async {
            return R input
        }
