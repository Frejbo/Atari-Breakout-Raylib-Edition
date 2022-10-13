using Raylib_cs;
using System.Numerics;

int speed = 5;


Raylib.InitWindow(1024, 1024, "TopDownGame");
Raylib.SetTargetFPS(60);

Texture2D bollBild = Raylib.LoadTexture("ball.png");
Rectangle platta = new Rectangle(412, 900, 200, 30);

Vector2 ball = new Vector2(512, 512);


// for (int i = 0; i < 10; i++) {
// }

Vector2 ball_velocity = new Vector2(1, 1);
const int ball_speed = 2;

while (!Raylib.WindowShouldClose()) {
    if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) {
        platta.x -= speed;
    }
    if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) {
        platta.x += speed;
    }


    // ball
    if(ball.X < 1 || ball.X > (1024 - bollBild.width)) {
        ball_velocity.X -= (ball_velocity.X*2);
    } else if (ball.Y < 1 || ball.Y > (1024 - bollBild.height)) {
        ball_velocity.Y -= (ball_velocity.Y*2);
    }
    if (Raylib.CheckCollisionCircleRec(ball, bollBild.width, platta)) {
        ball_velocity.Y -= (ball_velocity.Y*2);
    }



    ball.X += ball_velocity.X * ball_speed;
    ball.Y += ball_velocity.Y * ball_speed;


    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.BLACK);

    Color white = new Color(255, 255, 255, 255);

    Raylib.DrawRectangleRec(platta, white);
    // Raylib.DrawCircleV(ball, 20, white);
    Raylib.DrawTexture(bollBild, (int)ball.X, (int)ball.Y, new Color(255, 255, 255, 255));
    Raylib.EndDrawing();
}