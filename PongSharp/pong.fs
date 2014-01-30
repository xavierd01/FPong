namespace Games2d.Pong
//
//-------------------------------------------------------------------------- 
// Pong: the core algorithms
//--------------------------------------------------------------------------  
 
// NOTE: This uses 'light' syntax.  This means whitespace 
// is signficant. 

module Game =
    
    open System.Windows
    open PongIO

    /// some vocabulary definitions
//    type Position =
//    | X of float
//    | Y of float
//
//    type Velocity =
//    | DX of float
//    | DY of float

    type xpos = float
    type ypos = float
    type width = float
    type height = float
    type dx = float
    type dy = float
    type SimpleRect =
        { X: xpos
          Y: ypos
          Width: width
          Height: height }
    //type Wall = SimpleRect
    type score = int

    /// our game objects
    type GameEntity =
        { Body: SimpleRect
          Velocity: dx * dy }

    type Wall = GameEntity
    type Ball = GameEntity
    type Paddle = GameEntity
          
    type Boundary = 
        { PlayArea: SimpleRect
          TopBorder: Wall
          BottomBorder: Wall
          LeftBorder: Wall
          RightBorder: Wall }
    
    type GameState =
        { Board: Boundary
          LeftPaddle: Paddle
          RightPaddle: Paddle
          Ball: Ball
          LPadScore: score
          RPadScore: score }

    type Direction =
        | Up
        | Down
        | Left
        | Right

    let newBall gameBoard w h xDir : Ball =
        let dy = System.Math.Floor(System.Random().NextDouble() * 8.0 - 4.0)
        let dx = 
            let mag = 8.0 - System.Math.Abs(dy)
            match xDir with
            | Left -> -mag
            | Right -> mag
            | _ -> mag

        { Body = { X = gameBoard.PlayArea.Width / 2.0
                   Y = gameBoard.PlayArea.Height / 2.0
                   Width = w
                   Height = h }
              
          Velocity = dx, dy }
    
    /// Movement, collision dection, etc.
    let move pos vel = 
        pos + vel
        
    let moveEntity (ge: GameEntity) =
        let x, y = ge.Body.X, ge.Body.Y
        let dx, dy = ge.Velocity
        let x' = move x dx
        let y' = move y dy
        { Body = 
            { X = x'
              Y = y'
              Width = ge.Body.Width
              Height = ge.Body.Height }
          Velocity = ge.Velocity }

    let colliding (r1: SimpleRect) (r2: SimpleRect) =
        let r1X = r1.X + r1.Width / 2.0
        let r1Y = r1.Y + r1.Height / 2.0
        let r2X = r2.X + r2.Width / 2.0
        let r2Y = r2.Y + r2.Height / 2.0
        // 
        (abs (r1X - r2X) * 2.0 <= (r1.Width + r2.Width)) && 
            (abs (r1Y - r2Y) * 2.0 <= (r1.Height + r2.Height))

    /// Main algorithms
    let checkCollision onCollide onNoCollide (ge1: GameEntity) (ge2: GameEntity) =
        match colliding ge1.Body ge2.Body with
        | false -> onNoCollide ge1 ge2
        | true -> onCollide ge1 ge2

    let checkAnyWallCollision collisionBehavior noCollisionBehavior (ge: GameEntity) (walls: Wall list) =
        let entityCollide = colliding ge.Body
        match walls |> List.exists (fun w -> entityCollide w.Body) with
        | false -> noCollisionBehavior ge
        | true -> collisionBehavior ge

    let handleGameInput (input: Keys) (gs: GameState) =
        let paddleSpeed = 10.0
        let lpad = gs.LeftPaddle
        let rpad = gs.RightPaddle

        let movePaddle paddle (up: paddleAction) (down: paddleAction) : Paddle =
            let moveUp = (fun p ->
                            { Body = p.Body
                              Velocity = 0.0, -paddleSpeed } )
            let moveDown = (fun p ->
                                { Body = p.Body
                                  Velocity = 0.0, paddleSpeed } )
            let stop = (fun p ->
                            { Body = p.Body
                              Velocity = 0.0, 0.0 } )

            let paddle' = moveEntity paddle
            let detectTopCollision = checkAnyWallCollision stop moveUp paddle'
            let detectBotCollision = checkAnyWallCollision stop moveDown paddle'
            
            match input.Down up with 
            | true ->
                [gs.Board.TopBorder] |> detectTopCollision

            | false ->
                match input.Down down with
                | true ->
                    [gs.Board.BottomBorder] |> detectBotCollision

                | false ->
                    stop paddle'
        
        { Board = gs.Board
          LeftPaddle = movePaddle lpad LPad_Up LPad_Down
          RightPaddle = movePaddle rpad RPad_Up RPad_Down
          Ball = gs.Ball
          LPadScore = gs.LPadScore
          RPadScore = gs.RPadScore }

    // Handles all logic related to the ball.
    // Movement, collision with walls and paddles, scoring, etc.
    let handleBall (gs: GameState) =
        let ball = gs.Ball
        let moveBall = moveEntity
        
        let checkPaddleCollision (ball: Ball) =
            let lpad = gs.LeftPaddle
            let rpad = gs.RightPaddle

            let calculateBallAngle ball paddle = 
                let midY ge = ge.Body.Y + ge.Body.Height / 2.0
                6.0 * ( (midY ball - midY paddle) / paddle.Body.Height)
        
            let reflect reflectDirection paddle ball =
                let pongSound = new System.Media.SoundPlayer("pongblipf4.wav")
                pongSound.Play();
                let dx, dy = ball.Velocity
                let dx' =
                    match dx, reflectDirection with
                    | dx, dir when dx < 0.0 && dir = Right -> -dx
                    | dx, dir when dx > 0.0 && dir = Left -> -dx
                    | _ -> dx

                let dy' = 
                    let _, Pdy = paddle.Velocity
                    let angle = calculateBallAngle ball paddle
                    let getDirection dy =
                        if dy < 0.0 then Some Up 
                        elif dy > 0.0 then Some Down
                        else None
                    
                    match getDirection Pdy, getDirection dy with
                    | Some Up, Some Up -> angle * 2.5
                    | Some Down, Some Down -> angle * 2.5
                    | Some Up, Some Down -> angle * 1.5
                    | Some Down, Some Up -> angle * 1.5
                    | _, _ -> angle

                { Body = { X = ball.Body.X
                           Y = ball.Body.Y
                           Width = ball.Body.Width
                           Height = ball.Body.Height }
                  
                  Velocity = dx', dy' }

            let reflectOffLPad = reflect Right 
            let reflectOffRPad = reflect Left
            let noCollision = (fun _ b -> b)
            let lpadCollision = checkCollision reflectOffLPad noCollision lpad 
            let rpadCollision = checkCollision reflectOffRPad noCollision rpad
            ball |> lpadCollision |> rpadCollision

        let checkWallCollision (ball: Ball) : Ball =
            let onWallCollide = (fun b ->
                                    let pongSound = new System.Media.SoundPlayer("pongblipf4.wav")
                                    pongSound.Play();
                                    let dx, dy = b.Velocity
                                    { Body = { X = b.Body.X
                                               Y = b.Body.Y
                                               Width = b.Body.Width
                                               Height = b.Body.Height }
                  
                                      Velocity = dx, -dy })
            let onNoWallCollide = (fun b -> b)
            // check collision against top and bottom walls
            let ballCollision = checkAnyWallCollision onWallCollide onNoWallCollide ball
            [gs.Board.TopBorder; gs.Board.BottomBorder] 
            |> ballCollision

        let checkForScore (ball: Ball) =
            let checkGoal (ball: Ball) =
                let player1Goal = colliding gs.Board.RightBorder.Body
                let player2Goal = colliding gs.Board.LeftBorder.Body
                (player1Goal ball.Body, player2Goal ball.Body)

            let handleScore score =
                score + 1
            
            let serveBall (ball: Ball) = newBall gs.Board ball.Body.Width ball.Body.Height

            let p1scored, p2scored = checkGoal ball

            { Board = gs.Board
              LeftPaddle = gs.LeftPaddle
              RightPaddle = gs.RightPaddle
              Ball = 
                if not (p1scored || p2scored) then 
                    ball 
                else 
                    let dir = if p1scored then Right else Left
                    serveBall ball dir

              LPadScore = if p1scored then handleScore gs.LPadScore else gs.LPadScore
              RPadScore = if p2scored then handleScore gs.RPadScore else gs.RPadScore }

        ball |> moveBall |> checkPaddleCollision |> checkWallCollision |> checkForScore