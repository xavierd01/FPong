namespace Games2d.Pong

module PongIO =

    open System
    open System.Windows.Input

    type paddleAction = Start | LPad_Up | LPad_Down | RPad_Up | RPad_Down 

    /// Used to capture keyboard events
    type Keys (keyDown:IObservable<KeyEventArgs>, keyUp:IObservable<KeyEventArgs>, remember) =
        let mutable actions = set []
        let toAction = function
            | Key.W -> Some LPad_Up
            | Key.S -> Some LPad_Down
            | Key.P -> Some RPad_Up
            | Key.L -> Some RPad_Down
            | Key.Space -> Some Start
            | _ -> None
    
        let listen() =
            keyDown
            |> Observable.choose (fun ke -> toAction ke.Key)
            |> Observable.subscribe (fun action ->
                actions <- Set.add action actions)
            |> remember

            keyUp
            |> Observable.choose (fun ke -> toAction ke.Key)
            |> Observable.subscribe (fun action ->
                actions <- Set.remove action actions)
            |> remember

        member this.StartListening () = listen()
        member this.Down action =
            actions.Contains action