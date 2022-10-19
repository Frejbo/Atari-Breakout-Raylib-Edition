using Raylib_cs;
using System.Numerics;

Vector2 ball_velocity = new Vector2(0, -1);
const int ball_speed_base = 5;
float ball_speed = ball_speed_base;

float ball_acceleration = (float)0.1; // ball_speed / ball_acceleration måste vara möjligt.
float actual_ball_speed = 1; // Ändra inte den här!

Vector2 screen_size = new Vector2(1280, 720);
Rectangle screen_rect = new Rectangle(0, 0, screen_size.X, screen_size.Y);
Raylib.InitWindow((int)screen_size.X, (int)screen_size.Y, "Atari Breakout");
Raylib.SetTargetFPS(60);

Texture2D bollBild = Raylib.LoadTexture("Assets/Ball.png");
Texture2D game_over_screen = Raylib.LoadTexture("Assets/Backgrounds/Game over.png");
Texture2D success_screen = Raylib.LoadTexture("Assets/Backgrounds/Success.png");



List<Texture2D> block_texturer = new List<Texture2D>();
block_texturer.Add(Raylib.LoadTexture("Assets/Blocks/Brown Block.png"));
block_texturer.Add(Raylib.LoadTexture("Assets/Blocks/Dark Blue Block.png"));
block_texturer.Add(Raylib.LoadTexture("Assets/Blocks/Green Block.png"));
block_texturer.Add(Raylib.LoadTexture("Assets/Blocks/Orange Block.png"));
block_texturer.Add(Raylib.LoadTexture("Assets/Blocks/Pink Block.png"));
block_texturer.Add(Raylib.LoadTexture("Assets/Blocks/Red Block.png"));
block_texturer.Add(Raylib.LoadTexture("Assets/Blocks/Turquise Block.png"));
block_texturer.Add(Raylib.LoadTexture("Assets/Blocks/Yellow Block.png"));

List<Powerup> powerups = new List<Powerup>();

List<Texture2D> health = new List<Texture2D>();
health.Add(Raylib.LoadTexture("Assets/Lifebar/life2.png"));
health.Add(Raylib.LoadTexture("Assets/Lifebar/life1.png"));
health.Add(Raylib.LoadTexture("Assets/Lifebar/life0.png"));

// Vector2 platta_size = new Vector2(long_platta.width, long_platta.height);
// Rectangle platta = new Rectangle((screen_size.X/2)-platta_size.X/2, screen_size.Y - (int)(screen_size.Y/10), platta_size.X, platta_size.Y);

Vector2 ball = new Vector2(screen_size.X/2, screen_size.Y/2);

List<Texture2D> user_blocks_texture = new List<Texture2D>();
List<Rectangle> blocks = new List<Rectangle>();
int blocks_gap = 12;
Vector2 block_size = new Vector2((float)(screen_size.X/11-blocks_gap), ((screen_size.X / 11 - blocks_gap) / 150) * 60); // Blockens res är 150x60.


Random rand = new Random();


for (int y = 0; y < 5; y++) {
    for (int x = 0; x < 10; x++) {
        if (rand.Next(0, 2) >= 1) { // Ju högre det andra värdet i Next är desto högre chans att block spawnar. 1=100%, 2=50%
            // Add block
            blocks.Add(new Rectangle((((block_size.X + blocks_gap)*x)+block_size.X/2)+blocks_gap, ((block_size.Y + blocks_gap) * y)+blocks_gap, block_size.X, block_size.Y));
            user_blocks_texture.Add(block_texturer[rand.Next(0, block_texturer.Count)]);
        }
    }
}

Platta platta = new Platta();
platta.init_platta();
platta.speed = 10;

while (!Raylib.WindowShouldClose()) {
    if (health.Count > 0 && blocks.Count > 0) {
        bool bounced_x = false;
        bool bounced_y = false;

        platta.tick_platta_size();


        // accelerera om bollhastigheten är under den angivna
        if (actual_ball_speed < ball_speed) {actual_ball_speed += ball_acceleration;}
        if (actual_ball_speed > ball_speed) {actual_ball_speed = ball_speed;}

        // ball
        // studsa på kanter och hörn
        if ((ball.X < 1 || ball.X > (screen_size.X - bollBild.width)) && (ball.Y < 1 || ball.Y > (screen_size.Y - bollBild.height))) {
            ball_velocity = -ball_velocity;
        } else if(ball.X-bollBild.width/2 < 1 || ball.X > (screen_size.X - bollBild.width/2)) {
            ball_velocity.X -= (ball_velocity.X*2);
        } else if (ball.Y < 1) {
            ball_velocity.Y -= (ball_velocity.Y*2);
        } else if (ball.Y > (screen_size.Y - bollBild.height/2)) { // hits bottom
            health.RemoveAt(0);
            if (health.Count == 0) {continue;}
            ball.X = screen_size.X/2;
            ball.Y = screen_size.Y/2;
            ball_velocity = new Vector2(0, 1);
            actual_ball_speed = 1;
            ball_speed = ball_speed_base; // !! ball speeds need rework
        } else if (Raylib.CheckCollisionCircleRec(ball, bollBild.width/2, platta.rect)) { // träffar platta
            if (ball.Y > platta.position.Y-(bollBild.height/2)) {
                ball.Y = platta.position.Y-(bollBild.height/2); // sets the boll on top om den är under plattan tpy
            }
            ball_velocity.Y -= (ball_velocity.Y*2);
            ball_velocity.X = ball_velocity.X + ((ball.X - platta.position.X - (platta.width/2))/platta.width); // skickar bollens x velocity beroende på var man träffar plattan.
        }


        // studsa på block
        List<int> remove_blocks = new List<int>();
        int index = 0;
        foreach (Rectangle block in blocks) {
            if (!Raylib.CheckCollisionCircleRec(ball, bollBild.width/2, block)) {index++; continue;}
            remove_blocks.Add(index);                
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
            ball_speed += (float)0.1;



            // Spawn powerups
            if (rand.Next(0, 101) <= 100) { // second argument: percentage of blocks that spawn powerups.
                Powerup powerup = new Powerup();
                powerup.position = new Vector2(block.x, block.y);
                string[] alla_powerups = Directory.GetFiles("Assets/Powerups/");
                powerup.name = alla_powerups[rand.Next(0, alla_powerups.Length)].Replace("Assets/Powerups/", "").Replace(".png", "");
                powerup.texture = Raylib.LoadTexture($"Assets/Powerups/{powerup.name}.png");
                powerups.Add(powerup);
                System.Console.WriteLine(powerup.texture);
            }
        }
        foreach (Powerup powerup in powerups) {powerup.position.Y += powerup.speed;}
        
        // Checking if powerup should be taken
        List<Powerup> remove_powerups = new List<Powerup>();
        foreach (Powerup powerup in powerups) {
            if (!Raylib.CheckCollisionRecs(new Rectangle(powerup.position.X, powerup.position.Y, powerup.size.X, powerup.size.Y), platta.rect)) {continue;}
            // Om powerupen ska aktiveras & tas bort:
            if (powerup.name == "Longer Plate") {
                platta.increase_platta_size();
            } else if (powerup.name == "Shorter Plate") {
                platta.decrease_platta_size();
            } else if (powerup.name == "speed down") {
                ball_speed -= 2;
                if (ball_speed < 1) {ball_speed = 1;} // så inte bollen kan råka åka baklänges hehe
            } else if (powerup.name == "speed up") {
                ball_speed += 2;
            } else {
                System.Console.WriteLine($"Tog upp {powerup.name}, men den fungerar inte i nuläget.");
            }
            remove_powerups.Add(powerup);
        }
        foreach (Powerup powerup in remove_powerups) {
            powerups.Remove(powerup);
        }


        
        remove_blocks.Reverse();
        foreach (int block in remove_blocks) {
            blocks.RemoveAt(block);
            user_blocks_texture.RemoveAt(block);
        }

        
        // calculating direction of ball
        ball.X += ball_velocity.X * actual_ball_speed;
        ball.Y += ball_velocity.Y * actual_ball_speed;

        draw_game();
    }
    else if (health.Count == 0) // Death
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

    int index = 0;
    foreach (Rectangle block in blocks) { // Ritar alla block
        Texture2D block_textur = block_texturer[rand.Next(0, block_texturer.Count)];

        // Raylib.DrawRectangleRec(block, Color.WHITE);
        Raylib.DrawTexturePro(user_blocks_texture[index], new Rectangle(0, 0, block_textur.width, block_textur.height), new Rectangle(block.x, block.y, block.width, block.height), new Vector2(0, 0), 0, Color.WHITE);
        // Raylib.DrawText(index.ToString(), (int)block.x, (int)block.y, 32, Color.WHITE);
        index++;
    }

    foreach (Powerup powerup in powerups) {
        Rectangle source_size = new Rectangle(0, 0, powerup.texture.width, powerup.texture.height);
        Rectangle dest_size = new Rectangle(powerup.position.X, powerup.position.Y, powerup.size.X, powerup.size.Y);
        Raylib.DrawTexturePro(powerup.texture, source_size, dest_size, new Vector2(0,0), 0, Color.WHITE);
    }


    // Raylib.DrawRectangleRec(platta, Color.WHITE);
    Console.WriteLine(platta.color_tint);
    Raylib.DrawTexture(platta.texture, (int)platta.position.X, (int)platta.position.Y, platta.color_tint);
    // Raylib.DrawCircleV(ball, 16, white);
    Raylib.DrawTexture(bollBild, (int)ball.X-bollBild.width/2, (int)ball.Y-bollBild.height/2, Color.WHITE);

    Raylib.DrawTexture(health[0], ((int)screen_size.X - health[0].width) - 20, 20, new Color(255, 255, 255, 200));

    Raylib.EndDrawing();
}



class Powerup {
    public Vector2 position;
    public Texture2D texture;
    public int speed = 2;
    public Vector2 size = new Vector2(40, 40);
    public string name = "";
}

class Platta {
    public int speed;
    public int size = 1;
    public float size_change_timer = 0f;
    public Color color_tint;
    Texture2D short_platta = Raylib.LoadTexture("Assets/Short Plate.png");
    Texture2D medium_platta = Raylib.LoadTexture("Assets/Medium Plate.png");
    Texture2D long_platta = Raylib.LoadTexture("Assets/Long Plate.png");
    public Vector2 position;
    public Rectangle rect;
    public int width;
    public int height;
    public Texture2D texture;

    public void increase_platta_size() {
        size += 1;
        size_change_timer += 60*10; // En powerup varar i 10 sekunder
    }
    public void decrease_platta_size() {
        if (size == 1) {
        }
        size -= 1;
        size_change_timer += 60*10; // En powerup varar i 10 sekunder
    }
    public void tick_platta_size() {
        if (size > 1) {
            if (texture.width == medium_platta.width) {
                position.X -= (long_platta.width-medium_platta.width)/2;
            }
            texture = long_platta;
        }
        if (size == 1) {
            if (texture.width == short_platta.width) {
                position.X -= (medium_platta.width-short_platta.width)/2;
            } else if (texture.width == long_platta.width) {
                position.X += (long_platta.width-medium_platta.width)/2;
            }
            texture = medium_platta;
        }
        if (size < 1) {
            if (texture.width == medium_platta.width) {
                position.X += (medium_platta.width-short_platta.width)/2;
            }
            texture = short_platta;
        }


        if (Raylib.IsKeyDown(KeyboardKey.KEY_A) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) {
            if (position.X < 1) {
                position.X = 0;
            } else {
                position.X -= speed;
            }
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) {
            if (position.X + texture.width >= Raylib.GetScreenWidth()) {
                position.X = Raylib.GetScreenWidth() - texture.width;
            } else {
                position.X += speed;
            }
        }
        rect = new Rectangle(position.X, position.Y, texture.width, texture.height);
        width = texture.width;
        height = texture.height;


        if (size_change_timer > 0) {size_change_timer -= 1;}

        // (55*sin(x*0.1)+200);
        if (size_change_timer <= 60*4 && size_change_timer != 0) { // Blinka att plattan ändrar storlek
            color_tint = new Color(255, 255, 255, (int)(System.Math.Sin(size_change_timer*0.1)*100)+155);
        }
        if (size_change_timer == 0) {
            if (size < 1) {size++;}
            if (size > 1) {size--;}
            color_tint = Color.WHITE;
        }
        // System.Math.Sin()
    }
    public void init_platta() {
        texture = medium_platta;
        color_tint = Color.WHITE;
        position = new Vector2((Raylib.GetScreenWidth()/2)-texture.width/2, Raylib.GetScreenHeight() - (int)(Raylib.GetScreenHeight()/10));
    }
}