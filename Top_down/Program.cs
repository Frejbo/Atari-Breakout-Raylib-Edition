using Raylib_cs;
using System.Numerics;

Random rand = new Random();

Vector2 ball_velocity = new Vector2((float)(rand.NextDouble()-.5), 1);
const int ball_speed_base = 5;
float ball_speed = ball_speed_base;

float ball_acceleration = (float)0.1; // ball_speed / ball_acceleration måste vara möjligt.
float actual_ball_speed = 1; // Ändra inte den här!
int ball_color_a = 255;


// rör inte
int screen_tick_length = 0;
bool campaign_menu_open = false;
int campaign_menu_page = 0;

Vector2 screen_size = new Vector2(1280, 720);
Rectangle screen_rect = new Rectangle(0, 0, screen_size.X, screen_size.Y);
Raylib.InitWindow((int)screen_size.X, (int)screen_size.Y, "Atari Breakout");
Raylib.SetTargetFPS(60);

Texture2D bollBild = Raylib.LoadTexture("Assets/Ball.png");
Texture2D game_over_screen = Raylib.LoadTexture("Assets/Backgrounds/Game over.png");
Texture2D success_screen = Raylib.LoadTexture("Assets/Backgrounds/Success.png");
Texture2D bg = Raylib.LoadTexture("Assets/Backgrounds/Meny.png");
Texture2D death_taggar = Raylib.LoadTexture("Assets/Death.png");

Texture2D start_button_texture = Raylib.LoadTexture("Assets/Buttons/Starta knapp.png");
start_button_texture.width = start_button_texture.width / 4;
start_button_texture.height = start_button_texture.height / 4;
Texture2D campaign_button_texture = Raylib.LoadTexture("Assets/Buttons/Campaign knapp.png");
campaign_button_texture.width = campaign_button_texture.width / 4;
campaign_button_texture.height = campaign_button_texture.height / 4;
Texture2D campaign_menu_bg = Raylib.LoadTexture("Assets/Buttons/Campaign menu.png");
Texture2D campaign_back_button = Raylib.LoadTexture("Assets/Buttons/Back knapp.png");
campaign_back_button.width = campaign_back_button.width / 4;
campaign_back_button.height = campaign_back_button.height / 4;
Texture2D left_button = Raylib.LoadTexture("Assets/Buttons/Left.png");
left_button.width = (int)(left_button.width / 1.5f);
left_button.height = (int)(left_button.height / 1.5f);
Texture2D right_button = Raylib.LoadTexture("Assets/Buttons/Right.png");
right_button.width = (int)(right_button.width / 1.5f);
right_button.height = (int)(right_button.height / 1.5f);

List<Texture2D> levels_preview_texture = new List<Texture2D>();
foreach (int index in Enumerable.Range(0, Directory.GetFiles("Assets/Buttons/Levels").Length)) {
    Texture2D texture = Raylib.LoadTexture($"Assets/Buttons/Levels/Level {index+1}.png");
    levels_preview_texture.Add(texture);
}

List<Texture2D> block_texturer = new List<Texture2D>();
foreach (string path in Directory.GetFiles("Assets/Blocks/")) {
    if (path.Contains("Hardness")) {continue;}
    block_texturer.Add(Raylib.LoadTexture(path));
}

List<Powerup> powerups = new List<Powerup>();

List<Texture2D> health = new List<Texture2D>();

Vector2 ball = new Vector2(screen_size.X/2, screen_size.Y/2);

List<Texture2D> user_blocks_texture = new List<Texture2D>();
List<Block> blocks = new List<Block>();
int amount_of_blocks_left = 0;

int blocks_gap = 12;
Vector2 block_size = new Vector2((float)(screen_size.X/11-blocks_gap), ((screen_size.X / 11 - blocks_gap) / 150) * 60); // Blockens res är 150x60.

void randomize_block_map() {
    for (int y = 0; y < 5; y++) {
        for (int x = 0; x < 10; x++) {
            if (rand.Next(0, 101) <= 50) { // Andra värdet är block som spawnar, i
                // Add block
                Block block = new Block();
                block.init_block(new Vector2((((block_size.X + blocks_gap)*x)+block_size.X/2)+blocks_gap, (((block_size.Y + blocks_gap) * y)+blocks_gap)), block_texturer[rand.Next(0, block_texturer.Count)]);
                blocks.Add(block);
                amount_of_blocks_left++;
            }
        }
    }
}

Platta platta = new Platta();
platta.init_platta();
platta.speed = 10;

bool bounced_x = false;
bool bounced_y = false;

bool game_active = false;

while (!Raylib.WindowShouldClose()) {
    if (!game_active) {
        main_menu();
        continue;
    }

    if (health.Count <= 0) {
        game_over();
        continue;
    } else if (amount_of_blocks_left <= 0) {
        // vinner
        // game_active = false;
        success();
        continue;
    }

    if (game_active) {
        // play
        bounced_x = false;
        bounced_y = false;

        platta.tick_platta_size();


        // accelerera om bollhastigheten är under den angivna
        if (actual_ball_speed < ball_speed) {actual_ball_speed += ball_acceleration;}
        if (actual_ball_speed > ball_speed) {actual_ball_speed = ball_speed;}


        // studsa på kanter och hörn
        if (!bounce_ball()) {continue;}


        foreach (Block block in blocks) {
            if (!Raylib.CheckCollisionCircleRec(ball, bollBild.width/2, block.rect)) {continue;}
            if (!block.is_alive) {continue;}
            block.is_alive = false;
            amount_of_blocks_left--;

            bounce_ball_on_block(block);

            // Spawn powerups
            if (rand.Next(0, 101) <= 33) { // second argument: percentage of blocks that spawn powerups.
                Powerup powerup = new Powerup();
                powerup.position = new Vector2(block.position.X, block.position.Y);
                string[] alla_powerups = Directory.GetFiles("Assets/Powerups/");
                powerup.name = alla_powerups[rand.Next(0, alla_powerups.Length)].Replace("Assets/Powerups/", "").Replace(".png", "");
                powerup.texture = Raylib.LoadTexture($"Assets/Powerups/{powerup.name}.png");
                powerups.Add(powerup);
            }
        }
        foreach (Powerup powerup in powerups) {powerup.position.Y += powerup.speed;}
        
        check_to_pick_up_powerup();
        
        // calculating direction of ball
        ball.X += ball_velocity.X * actual_ball_speed;
        ball.Y += ball_velocity.Y * actual_ball_speed;

        draw_game();
    }

}

void game_over() {
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    Raylib.DrawTexturePro(game_over_screen, get_texture_rect(game_over_screen), screen_rect, new Vector2(0, 0), 0, Color.WHITE);

    Raylib.EndDrawing();
    screen_tick_length--;
    if (screen_tick_length <= 0) {
        game_active = false;
    }
}
void success() {
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    Raylib.DrawTexturePro(success_screen, get_texture_rect(success_screen), screen_rect, new Vector2(0, 0), 0, Color.WHITE);

    Raylib.EndDrawing();
    screen_tick_length--;
    if (screen_tick_length <= 0) {
        game_active = false;
    }
}




void main_menu() {
    Raylib.BeginDrawing();
    
    Rectangle dest = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
    Raylib.DrawTexturePro(bg, get_texture_rect(bg), dest, new Vector2(0, 0), 0, Color.WHITE);


    Rectangle start_button_rect = new Rectangle((Raylib.GetScreenWidth()/2)-(start_button_texture.width/2), Raylib.GetScreenHeight()/2, start_button_texture.width, start_button_texture.height);
    Raylib.DrawTexturePro(start_button_texture, // start button
        get_texture_rect(start_button_texture),
        start_button_rect,
        new Vector2(0,0),
        0, Color.WHITE
    );

    Rectangle campaign_button_rect = new Rectangle((Raylib.GetScreenWidth()/2)-(campaign_button_texture.width/2), (Raylib.GetScreenHeight()/3)*2, campaign_button_texture.width, campaign_button_texture.height);
    Raylib.DrawTexturePro(campaign_button_texture,
        get_texture_rect(campaign_button_texture),
        campaign_button_rect,
        new Vector2(0, 0),
        0, Color.WHITE
    );

    if (campaign_menu_open) {draw_campaign_menu(campaign_menu_page);}

    Raylib.EndDrawing();


    // logic
    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(),  start_button_rect) && !campaign_menu_open) {
            // start button pressed
            Console.WriteLine("Startar");
            game_active = true;
            health.Clear();
            powerups.Clear();
            blocks.Clear();
            health.Add(Raylib.LoadTexture("Assets/Lifebar/life2.png"));
            health.Add(Raylib.LoadTexture("Assets/Lifebar/life1.png"));
            health.Add(Raylib.LoadTexture("Assets/Lifebar/life0.png"));
            randomize_block_map();
            ball.X = screen_size.X/2;
            ball.Y = screen_size.Y/2;
            ball_velocity = new Vector2((float)(rand.NextDouble()-.5), 1);
            actual_ball_speed = 1;
            ball_color_a = 255;
            ball_speed = ball_speed_base; // !! ball speeds need rework
            screen_tick_length = 120;
            platta.init_platta();
        } else if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), campaign_button_rect) && !campaign_menu_open) {
            // open campaign menu
            campaign_menu_open = true;
            campaign_menu_page = 0;
        }
    }
}


void draw_campaign_menu(int page = 0) {
    // Raylib is already drawing.
    float difference = (Raylib.GetScreenHeight()/10)*9 / (float)campaign_menu_bg.height; // covers 9/10 of screen height
    Rectangle bg_rect = new Rectangle((Raylib.GetScreenWidth()/2)-((campaign_menu_bg.width*difference)/2), (Raylib.GetScreenHeight()/2)-((campaign_menu_bg.height*difference)/2), campaign_menu_bg.width*difference, campaign_menu_bg.height*difference);
    Raylib.DrawTexturePro(campaign_menu_bg,
        get_texture_rect(campaign_menu_bg),
        bg_rect,
        new Vector2(0,0),
        0, Color.WHITE
    );
    Rectangle back_button_rect = new Rectangle(bg_rect.x+30+(campaign_back_button.width/2), bg_rect.y+20, campaign_back_button.width, campaign_back_button.height);
    Raylib.DrawTexture(campaign_back_button, (int)back_button_rect.x, (int)back_button_rect.y, Color.WHITE);
    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), back_button_rect)) {
        campaign_menu_open = false;
    }

    // draw left/right button
    Rectangle left_button_rect = new Rectangle((Raylib.GetScreenWidth()/2)-(left_button.width*1.5f), Raylib.GetScreenHeight()-bg_rect.y-(left_button.height*1.2f), left_button.width, left_button.height);
    Raylib.DrawTexturePro(
        left_button, get_texture_rect(left_button),
        left_button_rect,
        new Vector2(0, 0),
        0, Color.WHITE
    );
    Rectangle right_button_rect = new Rectangle((Raylib.GetScreenWidth()/2)+(right_button.width*.5f), Raylib.GetScreenHeight()-bg_rect.y-(right_button.height*1.2f), right_button.width, right_button.height);
    Raylib.DrawTexturePro(
        right_button, get_texture_rect(right_button),
        right_button_rect,
        new Vector2(0, 0),
        0, Color.WHITE
    );
    // check arrows clicked
    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), left_button_rect)) {
        campaign_menu_page--;
        if (campaign_menu_page < 0) {campaign_menu_page = 0;}
    }
    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), right_button_rect)) {
        campaign_menu_page++;
        if (campaign_menu_page > 1) {campaign_menu_page = 1;} // max 2 sidor i nuläget
    }


    // draw level previews
    int index = 0;
    int start_index = campaign_menu_page*9;
    foreach (int y in Enumerable.Range(0, 3)) {
        foreach (int x in Enumerable.Range(0, 3)) {
            Raylib.DrawTexturePro(
                levels_preview_texture[index+start_index],
                get_texture_rect(levels_preview_texture[index+start_index]),
                new Rectangle((bg_rect.x*3.35f)+((bg_rect.width/4)*x), (bg_rect.y*3.6f)+((bg_rect.height/4)*y), bg_rect.width/4.2f, (float)bg_rect.height/4.5f),
                new Vector2(0, 0),
                0, Color.WHITE
            );
            index++;
        }
    }
}


bool bounce_ball() {
    if (ball.Y > platta.position.Y) {
        // minska bollens alpha när den faller under plattan
        ball_color_a-=10;
        if (ball_color_a < 0) {ball_color_a = 0;}
    }

    if ((ball.X < 1 || ball.X > (screen_size.X - bollBild.width)) && (ball.Y < 1 || ball.Y > (screen_size.Y - bollBild.height))) {
        ball_velocity = -ball_velocity;
    } else if(ball.X-bollBild.width/2 < 1 || ball.X > (screen_size.X - bollBild.width/2)) {
        ball_velocity.X -= (ball_velocity.X*2);
    } else if ((ball.Y-bollBild.height/2) < 1) {
        ball_velocity.Y -= (ball_velocity.Y*2);
    } else if (ball.Y > (screen_size.Y - bollBild.height/2)) { // hits bottom
        ball_color_a = 255;
        health.RemoveAt(0);
        if (health.Count == 0) {return (false);}
        ball.X = platta.position.X+(platta.width/2);
        ball.Y = screen_size.Y/2;
        ball_velocity = new Vector2((float)(rand.NextDouble()-.5), 1);
        actual_ball_speed = 1;
        ball_speed = ball_speed_base; // !! ball speeds need rework
    } else if (Raylib.CheckCollisionCircleRec(ball, bollBild.width/2, platta.rect)) { // träffar platta
        if (ball.Y > platta.position.Y-(bollBild.height/2)) {
            ball.Y = platta.position.Y-(bollBild.height/2); // sets the boll on top om den är under plattan tpy
        }
        ball_velocity.Y -= (ball_velocity.Y*2);
        ball_velocity.X = ball_velocity.X + ((ball.X - platta.position.X - (platta.width/2))/platta.width); // skickar bollens x velocity beroende på var man träffar plattan.
    }
    return (true);
}

void bounce_ball_on_block(Block block) {
    Rectangle over_check = new Rectangle(block.position.X+(bollBild.width/4), block.position.Y-bollBild.height, block.rect.width-(bollBild.width/2), 1);
    Rectangle below_check = new Rectangle(block.position.X+(bollBild.width/4), block.position.Y+block.rect.height+bollBild.height, block.rect.width-(bollBild.width/2), 1);
    if (Raylib.CheckCollisionCircleRec(ball, bollBild.height, over_check)) { // Bollen är över blocket
        if (!bounced_y) {ball_velocity.Y = -ball_velocity.Y;}
        bounced_y = true;
    } else if (Raylib.CheckCollisionCircleRec(ball, bollBild.height, below_check)) { // Bollen är under blocket
        if (!bounced_y) {ball_velocity.Y = -ball_velocity.Y;}
        bounced_y = true;
    } else if (block.position.X <= ball.X) { // Bollen är till höger om blocket
        if (!bounced_x) {ball_velocity.X = -ball_velocity.X;}
        bounced_x = true;
    } else if (block.position.X >= ball.X) { // Bollen är till vänster om blocket
        if (!bounced_x) {ball_velocity.X = -ball_velocity.X;}
        bounced_x = true;
    }
    ball_speed += (float)0.1;
}

void check_to_pick_up_powerup() {
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
}

Rectangle get_texture_rect(Texture2D texture) {return new Rectangle(0, 0, texture.width, texture.height);}


void draw_game() {
    // Drawing
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);

    foreach (Block block in blocks) { // Ritar alla block
        Texture2D block_textur = block_texturer[rand.Next(0, block_texturer.Count)];

        // Raylib.DrawRectangleRec(block, Color.WHITE);
        if (block.is_alive) {
            Raylib.DrawTexturePro(block.texture, new Rectangle(0, 0, block_textur.width, block_textur.height), block.rect, new Vector2(0, 0), 0, Color.WHITE);
        }
        // Raylib.DrawText(index.ToString(), (int)block.x, (int)block.y, 32, Color.WHITE);
    }

    foreach (Powerup powerup in powerups) {
        Rectangle source_size = new Rectangle(0, 0, powerup.texture.width, powerup.texture.height);
        Rectangle dest_size = new Rectangle(powerup.position.X, powerup.position.Y, powerup.size.X, powerup.size.Y);
        Raylib.DrawTexturePro(powerup.texture, source_size, dest_size, new Vector2(0,0), 0, Color.WHITE);
    }


    Raylib.DrawTexturePro(
        platta.texture,
        get_texture_rect(platta.texture),
        new Rectangle(platta.position.X, platta.position.Y, (int)(platta.width*1.5), (int)(platta.height*1.5)),
        new Vector2(0, 0),
        0, Color.WHITE
    );
    // Raylib.DrawTexture(platta.texture, (int)platta.position.X, (int)platta.position.Y, platta.color_tint);

    Raylib.DrawTexture(bollBild, (int)ball.X-bollBild.width/2, (int)ball.Y-bollBild.height/2, new Color(255, 255, 255, ball_color_a));

    Raylib.DrawTexture(health[0], ((int)screen_size.X - health[0].width) - 20, 20, new Color(255, 255, 255, 200));

    float difference = Raylib.GetScreenWidth() / (float)death_taggar.width;
    Raylib.DrawTexturePro(
        death_taggar,
        get_texture_rect(death_taggar),
        new Rectangle(0, Raylib.GetScreenHeight()-death_taggar.height*difference, death_taggar.width*difference, death_taggar.height*difference),
        new Vector2(0, 0),
        0, Color.WHITE
    );

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
            if (position.X + ((int)texture.width*1.5) >= Raylib.GetScreenWidth()) {
                position.X = Raylib.GetScreenWidth() - (int)(texture.width*1.5);
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
        else {color_tint = Color.WHITE;}

        if (size_change_timer == 0) {
            if (size < 1) {size++;}
            if (size > 1) {size--;}
            color_tint = Color.WHITE;
        }
        // System.Math.Sin()
    }
    public void init_platta() {
        texture = medium_platta;
        size = 1;
        color_tint = Color.WHITE;
        position = new Vector2((Raylib.GetScreenWidth()/2)-texture.width/2, Raylib.GetScreenHeight() - (int)(Raylib.GetScreenHeight()/10));
    }
}

class Block {
    public float blocks_gap = 12f;
    Vector2 block_size;

    public Vector2 position;
    public bool is_alive;
    public Texture2D texture;
    public Rectangle rect;

    public void init_block(Vector2 pos, Texture2D textur) {
        block_size = new Vector2((float)(Raylib.GetScreenWidth()/11-blocks_gap), ((Raylib.GetScreenWidth() / 11 - blocks_gap) / 150) * 60); // Blockens res är 150x60.
        position = pos;
        texture = textur;
        is_alive = true;
        rect = new Rectangle(position.X, position.Y, block_size.X, block_size.Y);
        Console.WriteLine(textur);
    }
}