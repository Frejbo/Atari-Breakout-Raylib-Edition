using Raylib_cs;
using System.Numerics;

int speed = 10;
int health = 3;

Vector2 screen_size = new Vector2(1280, 720);
Rectangle screen_rect = new Rectangle(0, 0, screen_size.X, screen_size.Y);
Raylib.InitWindow((int)screen_size.X, (int)screen_size.Y, "Atari Breakout");
Raylib.SetTargetFPS(60);

Texture2D bollBild = Raylib.LoadTexture("Assets/Ball.png");
Texture2D short_platta = Raylib.LoadTexture("Assets/Short Plate.png");
Texture2D medium_platta = Raylib.LoadTexture("Assets/Medium Plate.png");
Texture2D long_platta = Raylib.LoadTexture("Assets/Long Plate.png");
Texture2D game_over_screen = Raylib.LoadTexture("Assets/Backgrounds/Game over.png");
Texture2D success_screen = Raylib.LoadTexture("Assets/Backgrounds/Success.png");

List<Texture2D> life = new List<Texture2D>();
life.Add(Raylib.LoadTexture("Assets/Lifebar/life2.png"));
life.Add(Raylib.LoadTexture("Assets/Lifebar/life1.png"));
life.Add(Raylib.LoadTexture("Assets/Lifebar/life0.png"));

Vector2 platta_size = new Vector2(long_platta.width, long_platta.height);
Rectangle platta = new Rectangle((screen_size.X/2)-platta_size.X/2, screen_size.Y - (int)(screen_size.Y/10), platta_size.X, platta_size.Y);

Vector2 ball = new Vector2(screen_size.X/2, screen_size.Y/2);


List<Rectangle> blocks = new List<Rectangle>();
Vector2 block_size = new Vector2(40, 20);
int blocks_gap = 10;

for (int x = 19; x <= 19; x++) {
    for (int y = 5; y <= 5; y++) {
        blocks.Add(new Rectangle((block_size.X + blocks_gap) * x, (block_size.Y + blocks_gap) * y, block_size.X, block_size.Y));
    }
}

Vector2 ball_velocity = new Vector2(0, 1);
const int ball_speed = 7;

while (!Raylib.WindowShouldClose()) {
    if (life.Count > 0 && blocks.Count > 0) {
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
            if (platta.x + platta_size.X >= screen_size.X) {
                platta.x = screen_size.X - platta_size.X;
            } else {
                platta.x += speed;
            }
        }


        // ball
        // studsa på kanter och hörn
        if ((ball.X < 1 || ball.X > (screen_size.X - bollBild.width)) && (ball.Y < 1 || ball.Y > (screen_size.Y - bollBild.height))) {
            ball_velocity = -ball_velocity;
        } else if(ball.X-bollBild.width/2 < 1 || ball.X > (screen_size.X - bollBild.width/2)) {
            ball_velocity.X -= (ball_velocity.X*2);
        } else if (ball.Y < 1) {
            ball_velocity.Y -= (ball_velocity.Y*2);
        } else if (ball.Y > (screen_size.Y - bollBild.height/2)) { // hits bottom
            life.RemoveAt(0);
            if (life.Count == 0) {continue;}
            ball.X = screen_size.X/2;
            ball.Y = screen_size.Y/2;
            ball_velocity = new Vector2(0, 1);
        } else if (Raylib.CheckCollisionCircleRec(ball, bollBild.width/2, platta)) { // träffar platta
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

        draw_game();
    }
    else if (life.Count == 0) // Death
    {

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    Raylib.DrawTexturePro(game_over_screen, get_texture_rect(game_over_screen), screen_rect, new Vector2(0, 0), 0, Color.WHITE);

    Raylib.EndDrawing();

    }
    else if (blocks.Count == 0) // vinner
    {
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    Raylib.DrawTexturePro(success_screen, get_texture_rect(success_screen), screen_rect, new Vector2(0, 0), 0, Color.WHITE);

    Raylib.EndDrawing();
    }
}
Console.ReadLine();

Rectangle get_texture_rect(Texture2D texture) {return new Rectangle(0, 0, texture.width, texture.height);}


void draw_game() {
    // Drawing
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    foreach (Rectangle block in blocks) {Raylib.DrawRectangleRec(block, Color.WHITE);}

    // Raylib.DrawRectangleRec(platta, Color.WHITE);
    Raylib.DrawTexture(long_platta, (int)platta.x, (int)platta.y, Color.WHITE);
    // Raylib.DrawCircleV(ball, 16, white);
    Raylib.DrawTexture(bollBild, (int)ball.X-bollBild.width/2, (int)ball.Y-bollBild.height/2, Color.WHITE);

    Raylib.DrawTexture(life[0], ((int)screen_size.X - life[0].width) - 20, 20, new Color(255, 255, 255, 200));

    Raylib.EndDrawing();
}