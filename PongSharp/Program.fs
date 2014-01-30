namespace Games2d.Pong

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Threading

open PongIO
open Game

module Client =
    
    type PongRender() as prnd =
        inherit UserControl()

        /// Setup game
        // Create a 800 X 600 game space
        let board_width = 800.
        let board_height = 600.
        // offset to give a nice board
        let offset = 10.
        // Our maxim score to win
        let max_score = 10
        
        // Create our game components
        let board = 
            { PlayArea = { X = offset
                           Y = offset
                           Width = board_width 
                           Height = board_height }

              TopBorder = { Body = { X = 0.0
                                     Y = 0.0 
                                     Width = board_width + offset * 2.0
                                     Height = offset }

                            Velocity = 0.0, 0.0 }

              BottomBorder = { Body = { X = 0.0 
                                        Y = board_height + offset
                                        Width = board_width + offset * 2.0
                                        Height = offset }
                               Velocity = 0.0, 0.0 }

              LeftBorder = { Body = { X = 0.0
                                      Y = 0.0
                                      Width = offset
                                      Height = board_height + offset * 2.0 }
                             Velocity = 0.0, 0.0 }

              RightBorder = { Body = { X = board_width + offset
                                       Y = 0.0
                                       Width = offset
                                       Height = board_height + offset * 2.0 }
                              Velocity = 0.0, 0.0 } }

        let ballSize = 5.0
        let ball = newBall board ballSize ballSize Right

        let paddle_height = 75.0
        let paddle_width = 15.0
        let paddle_vel = 0.0, 10.0
        let paddle_ystart = board.PlayArea.Height / 2.0 - paddle_height / 2.0
        let lpaddle =
            { Body = 
                { X = board.PlayArea.X + offset
                  Y = paddle_ystart
                  Width = paddle_width
                  Height = paddle_height }

              Velocity = paddle_vel }

        let rpaddle =
            { Body = 
                { X = board.PlayArea.Width - paddle_width
                  Y = paddle_ystart
                  Width = paddle_width
                  Height = paddle_height }

              Velocity = paddle_vel }

        let mutable game =
            { Board = board
              LeftPaddle = lpaddle
              RightPaddle = rpaddle
              Ball = ball
              LPadScore = 0
              RPadScore = 0 }

        // timer for updating the ball's position
        let fps = 50.
        let timer = new DispatcherTimer(Interval = new TimeSpan(0,0,0,0,(1000 / int fps)))

        // setup for drawing
        let backbrush = new SolidColorBrush(Colors.Black)
        let entitybrush = new SolidColorBrush(Colors.White)
        let entitypen = new Pen(entitybrush, 1.0)

        let mutable disposables = []
        let remember disposable = disposables <- disposable :: disposables
        let forget () = 
            disposables |> List.iter (fun (d:IDisposable) -> d.Dispose())
            disposables <- []

        // Keyboard capture
        let keys = Keys(prnd.KeyDown, prnd.KeyUp, remember)

        let gameLoop gs keys =
            handleGameInput keys gs
            |> handleBall

        // helper function to do some initialization
        let init() =
            // initialize the control properties
            prnd.Width <- board_width + (offset * 2.0)
            prnd.Height <- board_height + (offset * 2.0)
            prnd.Focusable <- true 
            prnd.Focus() |> ignore
            // initialize listener for user input
            keys.StartListening()

            // set up the timer to control game loop
            timer.Tick.Add(fun _ ->

                game <- gameLoop game keys

                //game <- update game (1. / fps) keys
                prnd.InvalidateVisual())
            timer.Start()

        do init()

        // our drawing function
        override prnd.OnRender(dc: DrawingContext) =
            // draw the ball
            let ballx, bally = game.Ball.Body.X, game.Ball.Body.Y
            let ballw = game.Ball.Body.Width
            let ballh = game.Ball.Body.Height
            dc.DrawEllipse(entitybrush, entitypen, new Point(ballx, bally), ballw, ballh)

            // helper for drawing paddles
            let draw_pad pad =
                dc.DrawRectangle(entitybrush, entitypen, new Rect(pad.Body.X, pad.Body.Y, pad.Body.Width, pad.Body.Height))

            // draw left and right paddles
            draw_pad game.LeftPaddle
            draw_pad game.RightPaddle
    
            // Draw scores
            let p1score = sprintf "%i" game.LPadScore
            let p2score = sprintf "%i" game.RPadScore
            dc.DrawText(
                new FormattedText(p1score, 
                    System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    26., entitybrush),
                new Point(prnd.Width / 2.0 - offset * 3.5, offset * 2.0))
            dc.DrawText(
                new FormattedText(p2score, 
                    System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    26., entitybrush),
                new Point(prnd.Width / 2.0 + offset * 2.0, offset * 2.0))

            // Draw border, an outline rectangle
            let clientRect = game.Board.PlayArea 
            dc.DrawRectangle(null, entitypen, new Rect(clientRect.X, clientRect.Y, clientRect.Width, clientRect.Height))
            // Drawer divider
            dc.DrawRectangle(null, entitypen, new Rect(prnd.Width / 2.0, offset, 2.0, prnd.Height - (offset * 2.0)))

//            let testbrush = new SolidColorBrush(Colors.Red)
//            let testpen = new Pen(testbrush, 1.0)
//            let top = game.Board.TopBorder
//            let bot = game.Board.BottomBorder
//            let left = game.Board.LeftBorder
//            let right = game.Board.RightBorder
//            dc.DrawRectangle(null, testpen, new Rect(top.X, top.Y, top.Width, top.Height))
//            dc.DrawRectangle(null, testpen, new Rect(bot.X, bot.Y, bot.Width, bot.Height))
//            dc.DrawRectangle(null, testpen, new Rect(left.X, left.Y, left.Width, left.Height))
//            dc.DrawRectangle(null, testpen, new Rect(right.X, right.Y, right.Width, right.Height))

module Main =
    // create an instance of the new control
    let br = new Client.PongRender()
    // create a window to hold the control
    let win = new Window(Title = "Pong Sharp",
                        Content = br,
                        Width = br.Width + 40.,
                        Height = br.Height + 60.,
                        Background = new SolidColorBrush(Colors.Black))

    let app = new Application()
    [<STAThread>]
    do app.Run win |> ignore