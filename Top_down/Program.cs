using Raylib_cs;
using System.Numerics;

int speed = 10;


Raylib.InitWindow(1024, 1024, "TopDownGame");
Raylib.SetTargetFPS(60);

Texture2D bollBild = Raylib.LoadTexture("ball.png");
Rectangle platta = new Rectangle(412, 900, 200, 30);

Vector2 ball = new Vector2(512, 512);

List<Rectangle> blocks = new List<Rectangle>();
Vector2 block_size = new Vector2(40, 20);
int blocks_gap = 10;

for (int x = 1; x <= 20; x++) {
    for (int y = 1; y <= 5; y++) {
        blocks.Add(new Rectangle((block_size.X + blocks_gap) * x, (block_size.Y + blocks_gap) * y, block_size.X, block_size.Y));
    }
}

Vector2 ball_velocity = new Vector2(1, 1);
const int ball_speed = 5;

Vector2 prev_ball_pos;

while (!Raylib.WindowShouldClose()) {
    if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) {
        platta.x -= speed;
    }
    if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) {
        platta.x += speed;
    }


    // ball
    if ((ball.X < 1 || ball.X > (1024 - bollBild.width)) && (ball.Y < 1 || ball.Y > (1024 - bollBild.height))) {
        ball_velocity = -ball_velocity;
    } else if(ball.X < 1 || ball.X > (1024 - bollBild.width)) {
        ball_velocity.X -= (ball_velocity.X*2);
    } else if (ball.Y < 1 || ball.Y > (1024 - bollBild.height)) {
        ball_velocity.Y -= (ball_velocity.Y*2);
    }
    if (Raylib.CheckCollisionCircleRec(ball, bollBild.width, platta)) {
        ball_velocity.Y -= (ball_velocity.Y*2);
    }

    List<Rectangle> remove_blocks = new List<Rectangle>();
    foreach (Rectangle block in blocks) {
        bool colliding = Raylib.CheckCollisionCircleRec(ball, bollBild.width, block);
        if (colliding) {
            remove_blocks.Add(block);
            // bounce
            
        }
    }
    foreach (Rectangle block in remove_blocks) {blocks.Remove(block);}



    ball.X += ball_velocity.X * ball_speed;
    ball.Y += ball_velocity.Y * ball_speed;

    prev_ball_pos = ball;


    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.BLACK);

    Color white = new Color(255, 255, 255, 255);
    
    foreach (Rectangle block in blocks) {
        Raylib.DrawRectangleRec(block, white);
    }

    Raylib.DrawRectangleRec(platta, white);
    // Raylib.DrawCircleV(ball, 20, white);
    Raylib.DrawTexture(bollBild, (int)ball.X, (int)ball.Y, new Color(255, 255, 255, 255));
    Raylib.EndDrawing();
}