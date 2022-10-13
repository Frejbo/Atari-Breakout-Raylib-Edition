using Raylib_cs;
using System.Numerics;

int speed = 10;


Raylib.InitWindow(1024, 1024, "TopDownGame");
Raylib.SetTargetFPS(60);

Texture2D bollBild = Raylib.LoadTexture("Assets/Ball.png");
Texture2D short_platta = Raylib.LoadTexture("Assets/Short Plate.png");
Texture2D medium_platta = Raylib.LoadTexture("Assets/Medium Plate.png");
Texture2D long_platta = Raylib.LoadTexture("Assets/Long Plate.png");

Vector2 platta_size = new Vector2(long_platta.width, long_platta.height);
Rectangle platta = new Rectangle(512-platta_size.X/2, 900, platta_size.X, platta_size.Y);

Vector2 ball = new Vector2(512, 512);

List<Rectangle> blocks = new List<Rectangle>();
Vector2 block_size = new Vector2(40, 20);
int blocks_gap = 10;

for (int x = 1; x <= 19; x++) {
    for (int y = 1; y <= 5; y++) {
        blocks.Add(new Rectangle((block_size.X + blocks_gap) * x, (block_size.Y + blocks_gap) * y, block_size.X, block_size.Y));
    }
}

Vector2 ball_velocity = new Vector2(0, 1);
const int ball_speed = 7;


while (!Raylib.WindowShouldClose()) {
    bool bounced_x = false;
    bool bounced_y = false;

    if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) {
        if (platta.x < 1) {
            platta.x = 0;
        } else {
            platta.x -= speed;
        }
    }
    if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) {
        if (platta.x + platta_size.X >= 1024) {
            platta.x = 1024 - platta_size.X;
        } else {
            platta.x += speed;
        }
    }


    // ball
    // studsa på kanter och hörn
    if ((ball.X < 1 || ball.X > (1024 - bollBild.width)) && (ball.Y < 1 || ball.Y > (1024 - bollBild.height))) {
        ball_velocity = -ball_velocity;
    } else if(ball.X-bollBild.width/2 < 1 || ball.X > (1024 - bollBild.width/2)) {
        ball_velocity.X -= (ball_velocity.X*2);
    } else if (ball.Y < 1 || ball.Y > (1024 - bollBild.height/2)) {
        ball_velocity.Y -= (ball_velocity.Y*2);
    }
    if (Raylib.CheckCollisionCircleRec(ball, bollBild.width/2, platta)) {
        ball_velocity.Y -= (ball_velocity.Y*2);
        ball_velocity.X = (ball.X - platta.x - (platta_size.X/2))/platta_size.X; // skickar bollens x velocity beroende på var man träffar plattan.
    }


    // studsa på block
    List<Rectangle> remove_blocks = new List<Rectangle>();
    foreach (Rectangle block in blocks) {
        bool colliding = Raylib.CheckCollisionCircleRec(ball, bollBild.width/2, block);
        if (colliding) {
            remove_blocks.Add(block);
            
            // bounce ball
            Rectangle over_check = new Rectangle(block.x+(bollBild.width/4), block.y-bollBild.height, block_size.X-(bollBild.width/2), 1);
            Rectangle below_check = new Rectangle(block.x+(bollBild.width/4), block.y+block_size.Y+bollBild.height, block_size.X-(bollBild.width/2), 1);
            if (Raylib.CheckCollisionCircleRec(ball, bollBild.height, over_check)) { // Bollen är över blocket
                if (!bounced_y) {ball_velocity.Y = -ball_velocity.Y;}
                bounced_y = true;
            } else if (Raylib.CheckCollisionCircleRec(ball, bollBild.height, below_check)) { // Bollen är under blocket
                if (!bounced_y) {ball_velocity.Y = -ball_velocity.Y;}
                bounced_y = true;
            } else if (block.x <= ball.X) { // Bollen är till höger om blocket
                if (!bounced_x) {ball_velocity.X = -ball_velocity.X;}
                bounced_x = true;
            } else if (block.x >= ball.X) { // Bollen är till vänster om blocket
                if (!bounced_x) {ball_velocity.X = -ball_velocity.X;}
                bounced_x = true;
            }
        }
    }
    foreach (Rectangle block in remove_blocks) {blocks.Remove(block);}


    // calculating direction of ball
    ball.X += ball_velocity.X * ball_speed;
    ball.Y += ball_velocity.Y * ball_speed;



    // Drawing
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    foreach (Rectangle block in blocks) {Raylib.DrawRectangleRec(block, Color.WHITE);}

    // Raylib.DrawRectangleRec(platta, Color.WHITE);
    Raylib.DrawTexture(long_platta, (int)platta.x, (int)platta.y, Color.WHITE);
    // Raylib.DrawCircleV(ball, 16, white);
    Raylib.DrawTexture(bollBild, (int)ball.X-bollBild.width/2, (int)ball.Y-bollBild.height/2, Color.WHITE);
    Raylib.EndDrawing();
}