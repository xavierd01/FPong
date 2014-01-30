namespace Games2d.Pong
////
////-------------------------------------------------------------------------- 
//// Pong: client GUI 
////--------------------------------------------------------------------------  
// 
//// NOTE: This uses 'light' syntax.  This means whitespace 
//// is signficant. 
//
//module Client
// 
//#nowarn "40" 
//
//open System 
//open System.Windows.Forms 
//open System.IO 
//open System.Drawing 
//open System.Drawing.Drawing2D 
//open System.Drawing.Imaging 
//open System.ComponentModel 
//
////-------------------------------------------------------------------------- 
// 
//type DoubleBufferedForm() =  
//    inherit Form() 
//    do base.DoubleBuffered<-true 
//
///// A bitmap is used to store the visual state 
//type Client() =  
//    
//    // our color scheme is black and white
//    let entityColor = Color.White
//    let backgroundColor = Color.Black
//    let backgroundBrush = new SolidBrush(backgroundColor)
//
//    // dimensions for our client
//    let gameWidth = 800
//    let gameHeight = 600
//    let offset = 20
//
//    let bitmap = new Bitmap(gameWidth, gameHeight, PixelFormat.Format24bppRgb)
//    let getGraphic bmp = Graphics.FromImage(bmp)
//    let clearBitMap (bmp: Bitmap) =
//        let gfx = getGraphic bmp
//        gfx.FillRectangle(backgroundBrush, 0, 0, bmp.Width, bmp.Height)
//
//    /// Create the worker object and its thread.  The worker performs 
//    /// computations and fires events back on the GUI thread. 
//    let form = new DoubleBufferedForm(Width = gameWidth + offset, 
//                                      Height = gameHeight + offset,  
//                                      Text = "Pong") 
// 
//    do form.ResizeEnd.Add(fun _ -> form.Invalidate()) 
//    do form.Closing.Add(fun _ -> Application.Exit()) 
//
//    // Add the operations to redraw the GUI at various points 
//    let guiRefresh(graphics: Graphics) =  
//        let region = new Rectangle(0,0,gameWidth,gameHeight)  
//        let cliprect2 = form.ClientRectangle  
//        lock graphics (fun () -> graphics.DrawImage(bitmap,cliprect2,region,GraphicsUnit.Pixel)) 
// 
//    do form.Paint.Add(fun e -> guiRefresh e.Graphics)
//
//    do form.KeyDown
//        |> Event.filter (fun e -> e.KeyCode = Keys.W)
//        |> Event.filter (fun e -> e.KeyCode = Keys.S)
//        |> Event.filter (fun e -> e.KeyCode = Keys.P)
//        |> Event.filter (fun e -> e.KeyCode = Keys.L)
//        |> Event.add (fun evArgs ->
//            