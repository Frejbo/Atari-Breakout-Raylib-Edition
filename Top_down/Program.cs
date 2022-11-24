using Raylib_cs;
using System.Numerics;
using Objects;


Dictionary<int, string> Banor = new Dictionary<int, string>
{
    {1, "00000300063000630006300063000630006300063000600000"},
    {2, "30303060605000505050005000050005050500050606030303"},
    {3, "01010106010131003030304033040303030013101060101010"},
    {4, "00000044444000041050404054040541050400000444400000"},
    {5, "61010001016101000107610756107500107610100010161010"},
    {6, "00016003100430054000500005000054000043000031000016"},
    {7, "08830388800088308380388300880038880008830388000380"},
    {8, "84000444000444000440004440044400440044400440088000"},
    {9, "36631622632552657752257752577557752255266226336631"},
    {10, "63636305065300530056000000000030056530053050663636"},
    {11, "00600006000606006260625266252606260060600060000600"},
    {12, "00000030000300003300003000030002220224220222000000"},
    {13, "10703050601070305060107030506010703050601070305060"},
    {14, "77000774400744407444004440044407444074447744077000"},
    {15, "50006350603360036050600056000536050336003506050006"},
    {16, "41000006704070540705080700807040705407050067041000"},
    {17, "56666600006066560006566066000660666660005660055666"},
    {18, "60100010031063010263020340203410253105300100350100"}
};
Dictionary<int, string> hardness = new Dictionary<int, string>
{
    {1, "00000000000000000000000000000000000000000000000000"},
    {2, "00000000001000101010001000010001010100010000000000"},
    {3, "01010100010101000000000000000000000010101000101010"},
    {4, "00000000000000000000000000000000000000000000000000"},
    {5, "00000000010001000100010000100000100000100000100000"},
    {6, "00000000000000000000000000000000000000000000000000"},
    {7, "00000000000000000000000000000000000000000000000000"},
    {8, "00000000000000000000000000000000000000000000000000"},
    {9, "00000000000110010010010010100110010011000000000000"},
    {10, "00111000010000100011000000000000011000010000100111"},
    {11, "00100001000101001010100011000101010010100010000100"},
    {12, "00000000000000000000000000000001110110110111000000"},
    {13, "00100000101000101000001000001010001010000010000010"},
    {14, "00000000000000000000000000000000000000000000000000"},
    {15, "00000000000010001010100011000101010001000000000000"},
    {16, "00000000101000110001010100101010001100010001000000"},
    {17, "01111000000011100001000010000110111010000010000011"},
    {18, "10100010011011010011000010000110011101100100110100"}
};



Random rand = new Random();

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
start_button_texture.width /= 4;
start_button_texture.height /= 4;
Texture2D campaign_button_texture = Raylib.LoadTexture("Assets/Buttons/Campaign knapp.png");
campaign_button_texture.width /= 4;
campaign_button_texture.height /= 4;
Texture2D campaign_menu_bg = Raylib.LoadTexture("Assets/Buttons/Campaign menu.png");
Texture2D campaign_back_button = Raylib.LoadTexture("Assets/Buttons/Back knapp.png");
campaign_back_button.width /= 4;
campaign_back_button.height /= 4;
Texture2D left_button = Raylib.LoadTexture("Assets/Buttons/Left.png");
left_button.width /= (int)(left_button.width / 1.5f);
left_button.height = (int)(left_button.height / 1.5f);
Texture2D right_button = Raylib.LoadTexture("Assets/Buttons/Right.png");
right_button.width = (int)(right_button.width / 1.5f);
right_button.height = (int)(right_button.height / 1.5f);


List<Texture2D> levels_preview_texture = new List<Texture2D>();
foreach (int index in Enumerable.Range(0, Directory.GetFiles("Assets/Buttons/Levels").Length)) {
    Texture2D texture = Raylib.LoadTexture($"Assets/Buttons/Levels/Level {index+1}.png");
    levels_preview_texture.Add(texture);
}

Texture2D hardness_texture = Raylib.LoadTexture("Assets/Blocks/Hardness 1.png");
List<Texture2D> block_texturer = new List<Texture2D>();
List<Color> block_color_list = new() {new Color(35, 215, 219, 255), new Color(209, 35, 219, 255), new Color(105, 219, 35, 255), new Color(219, 212, 35, 255), new Color(219, 57, 35, 255), new Color(35, 64, 219, 255), new Color(219, 134, 35, 255), new Color(96, 56, 41, 255)};
foreach (string path in Directory.GetFiles("Assets/Blocks/")) {
    if (path.Contains("Hardness")) {continue;}
    block_texturer.Add(Raylib.LoadTexture(path));
}

List<Powerup> powerups = new List<Powerup>();
List<Texture2D> health = new List<Texture2D>();
List<Ball> balls = new List<Ball>();

int amount_of_balls = 0;
int add_amount_of_new_balls = 0;

List<Texture2D> user_blocks_texture = new List<Texture2D>();
List<Block> blocks = new List<Block>();
int amount_of_blocks_left = 0;

int blocks_gap = 12;
Vector2 block_size = new Vector2((float)(screen_size.X/11-blocks_gap), ((screen_size.X / 11 - blocks_gap) / 150) * 60); // Blockens res är 150x60.

void randomize_block_map() {
    for (int x = 0; x < 10; x++) {
        for (int y = 0; y < 5; y++) {
            if (rand.Next(0, 101) <= 50) { // Andra värdet är block som spawnar, i
                // Add block
                Block block = new Block();
                int i = rand.Next(0, block_texturer.Count);
                block.init_block(new Vector2((((block_size.X + blocks_gap)*x)+block_size.X/2)+blocks_gap, (((block_size.Y + blocks_gap) * y)+blocks_gap)), block_texturer[i]);
                block.particle_color = block_color_list[i];
                blocks.Add(block);
                amount_of_blocks_left++;
            }
        }
    }
}
void load_blocks_map(string block_data, string hardness_data) {
    int index = 0;
    for (int x = 0; x < 10; x++) {
        for (int y = 0; y < 5; y++) {
            int texture_idx = 0;
            if (block_data.ToCharArray().Length < index) {continue;} // in case the map string is too short

            texture_idx = Int16.Parse(block_data[index].ToString());

            if (texture_idx > 0) {
                Block block = new Block();
                block.init_block(new Vector2((((block_size.X + blocks_gap)*x)+block_size.X/2)+blocks_gap, (((block_size.Y + blocks_gap) * y)+blocks_gap)), block_texturer[texture_idx-1]);
                block.particle_color = block_color_list[texture_idx-1];
                if (Int16.Parse(hardness_data[index].ToString()) == 1) {block.hardness = true;}
                blocks.Add(block);
                amount_of_blocks_left++;
            }
            index++;
        }
    }
}

Platta platta = new Platta();
platta.init_platta();
platta.speed = 10;

bool bounced_x = false;
bool bounced_y = false;

bool game_active = false;

List<Particle> partiklar = new();

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
        success();
        continue;
    }

    if (game_active) {
        // play
        bounced_x = false;
        bounced_y = false;

        platta.tick_platta_size();

        bool rerun_loop = false;
        foreach (Ball ball in balls) {
            if (!ball.is_alive) {continue;}
            ball.tick();
            // studsa på kanter och hörn
            if (!bounce_ball(ball)) {rerun_loop = true; continue;}
        }
        if (rerun_loop) {continue;}

        foreach (Ball ball in balls) {
            if (!ball.is_alive) {continue;}
            foreach (Block block in blocks) {
                if (!Raylib.CheckCollisionCircleRec(ball.position, bollBild.width/2, block.rect)) {continue;}
                if (!block.is_alive) {continue;}
                if (block.hardness) {
                    block.hardness = false;
                    Particle partikel = new Particle();
                    partikel.position = new Vector2(block.position.X+(block.rect.width/2), block.position.Y+(block.rect.height/2));
                    partikel.color = new Color(61, 61, 69, 255); // color of hardness
                    partikel.init_particle();
                    partiklar.Add(partikel);
                } else {
                    block.is_alive = false;
                    Particle partikel = new Particle();
                    partikel.position = new Vector2(block.position.X+(block.rect.width/2), block.position.Y+(block.rect.height/2));
                    partikel.color = block.particle_color;
                    partikel.init_particle();
                    partiklar.Add(partikel);
                    amount_of_blocks_left--;
                }

                bounce_ball_on_block(block, ball);

                // Spawn powerups
                if (rand.Next(0, 101) <= 33) { // second argument: percentage of blocks that spawn powerups.
                    Powerup powerup = new Powerup();
                    powerup.position = new Vector2(block.position.X, block.position.Y);
                    string[] alla_powerups = Directory.GetFiles("Assets/Powerups/");
                    powerup.name = alla_powerups[rand.Next(0, alla_powerups.Length)].Replace("Assets/Powerups/", "").Replace(".png", "");
                    powerup.texture = Raylib.LoadTexture($"Assets/Powerups/{powerup.name}.png"); 
                    float width_ratio = (float)powerup.texture.width/powerup.texture.height;
                    powerup.size.X = powerup.size.X * width_ratio;
                    powerups.Add(powerup);
                }
            }
        }
        while (add_amount_of_new_balls>0) {
            balls.Add(new Ball());
            amount_of_balls++;
            add_amount_of_new_balls--;
        }
        balls.Reverse();
        for (int i = balls.Count-1; i>0; i--) {
            if (!balls[i].is_alive) {balls.RemoveAt(i);}
        }
        balls.Reverse();

        foreach (Powerup powerup in powerups) {powerup.position.Y += powerup.speed;}
        
        check_to_pick_up_powerup();
        draw_game();

        // kollar om partiklar kan tas bort
        List<Particle> temp_copy = partiklar;
        foreach (Particle particle in temp_copy) {
            if (!particle.is_emitting) {
                partiklar.Remove(particle);
            }
        }
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
    if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(),  start_button_rect) && !campaign_menu_open) {
            // start button pressed
            Console.WriteLine("Startar");
            restart_game();
        } else if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), campaign_button_rect) && !campaign_menu_open) {
            // open campaign menu
            campaign_menu_open = true;
            campaign_menu_page = 0;
        }
    }
}

void restart_game(string map = "", string hardness_map = "") {
    health.Clear();
    powerups.Clear();
    blocks.Clear();
    balls.Clear();
    if (map == "") {
        randomize_block_map();
    } else {
        load_blocks_map(map, hardness_map);
    }
    health.Add(Raylib.LoadTexture("Assets/Lifebar/life2.png"));
    health.Add(Raylib.LoadTexture("Assets/Lifebar/life1.png"));
    health.Add(Raylib.LoadTexture("Assets/Lifebar/life0.png"));
    balls.Add(new Ball());
    amount_of_balls++;
    screen_tick_length = 120;
    platta.init_platta();
    game_active = true;
}


void draw_campaign_menu(int page = 0) {
    Color left_arrow_color = Color.WHITE;
    Color right_arrow_color = Color.WHITE;


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

    Rectangle left_button_rect = new Rectangle((Raylib.GetScreenWidth()/2)-(left_button.width*1.5f), Raylib.GetScreenHeight()-bg_rect.y-(left_button.height*1.2f), left_button.width, left_button.height);
    Rectangle right_button_rect = new Rectangle((Raylib.GetScreenWidth()/2)+(right_button.width*.5f), Raylib.GetScreenHeight()-bg_rect.y-(right_button.height*1.2f), right_button.width, right_button.height);
    // check arrow color
    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), left_button_rect)) {
        // hovrar över vänster pil
        left_arrow_color = new Color(200, 200, 200, 255);
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT)) {
            left_arrow_color = new Color(150, 150, 150, 255);
        }
    }
    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), right_button_rect)) {
        // hovrar över höger pil
        right_arrow_color = new Color(200, 200, 200, 255);
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT)) {
            right_arrow_color = new Color(150, 150, 150, 255);
        }
    }
    // draw left/right button
    Raylib.DrawTexturePro(
        left_button, get_texture_rect(left_button),
        left_button_rect,
        new Vector2(0, 0),
        0, left_arrow_color
    );
    Raylib.DrawTexturePro(
        right_button, get_texture_rect(right_button),
        right_button_rect,
        new Vector2(0, 0),
        0, right_arrow_color
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
    Dictionary<int, Rectangle> rendered_campaign_buttons = new();
    // List<Rectangle> rendered_campaign_buttons = new List<Rectangle>();
    int index = 0;
    int start_index = campaign_menu_page*9;
    foreach (int y in Enumerable.Range(0, 3)) {
        foreach (int x in Enumerable.Range(0, 3)) {
            Rectangle rect = new Rectangle((bg_rect.x*3.35f)+((bg_rect.width/4)*x), (bg_rect.y*3.6f)+((bg_rect.height/4)*y), bg_rect.width/4.2f, (float)bg_rect.height/4.5f);
            Raylib.DrawTexturePro(
                levels_preview_texture[index+start_index],
                get_texture_rect(levels_preview_texture[index+start_index]),
                rect,
                new Vector2(0, 0),
                0, Color.WHITE
            );
            rendered_campaign_buttons.Add(index+start_index+1, rect);
            index++;
        }
    }


    // Checking clicked campaign buttons
    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
        foreach (int key in rendered_campaign_buttons.Keys) {
            if (!Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rendered_campaign_buttons[key])) {continue;}
            restart_game(Banor[key], hardness[key]);
        }
    }
}


bool bounce_ball(Ball ball) {
    if (ball.position.Y > platta.position.Y) {
        // minska bollens alpha när den faller under plattan
        ball.ball_color_a-=10;
        if (ball.ball_color_a < 0) {ball.ball_color_a = 0;}
    }

    if ((ball.position.X < 1 || ball.position.X > (Raylib.GetScreenWidth() - bollBild.width)) && (ball.position.Y < 1 || ball.position.Y > (Raylib.GetScreenHeight() - bollBild.height))) {
        ball.velocity = -ball.velocity;
    } else if(ball.position.X-bollBild.width/2 < 1 || ball.position.X > (screen_size.X - bollBild.width/2)) {
        ball.velocity.X -= (ball.velocity.X*2);
    } else if ((ball.position.Y-bollBild.height/2) < 1) {
        ball.velocity.Y -= (ball.velocity.Y*2);
    } else if (ball.position.Y > (screen_size.Y - bollBild.height/2)) { // hits bottom
        ball.is_alive = false;
        amount_of_balls--;
        // bool balls_exist = false;
        // foreach (Ball boll_som_kanske_inte_lever in balls) {
        //     if (boll_som_kanske_inte_lever.is_alive) {balls_exist = true;}
        //     Console.WriteLine(boll_som_kanske_inte_lever.is_alive);
        // }
        if (amount_of_balls == 0) { // Ta endast bort hälsa ifall 0 bollar är kvar.
            health.RemoveAt(0);
            if (health.Count == 0) {return (false);}
            add_amount_of_new_balls++;
        }
    } else if (Raylib.CheckCollisionCircleRec(ball.position, bollBild.width/2, platta.rect)) { // träffar platta
        if (ball.position.Y > platta.position.Y-(bollBild.height/2)) {
            ball.position.Y = platta.position.Y-(bollBild.height/2); // sets the boll on top om den är under plattan tpy
        }
        ball.velocity.Y -= (ball.velocity.Y*2);
        ball.velocity.X = ball.velocity.X + ((ball.position.X - platta.position.X - (platta.width/2))/platta.width); // skickar bollens x velocity beroende på var man träffar plattan.
    }
    return (true);
}

void bounce_ball_on_block(Block block, Ball ball) {
    Rectangle over_check = new Rectangle(block.position.X+(bollBild.width/4), block.position.Y-bollBild.height, block.rect.width-(bollBild.width/2), 1);
    Rectangle below_check = new Rectangle(block.position.X+(bollBild.width/4), block.position.Y+block.rect.height+bollBild.height, block.rect.width-(bollBild.width/2), 1);
    if (Raylib.CheckCollisionCircleRec(ball.position, bollBild.height, over_check)) { // Bollen är över blocket
        if (!bounced_y) {ball.velocity.Y = -ball.velocity.Y;}
        bounced_y = true;
    } else if (Raylib.CheckCollisionCircleRec(ball.position, bollBild.height, below_check)) { // Bollen är under blocket
        if (!bounced_y) {ball.velocity.Y = -ball.velocity.Y;}
        bounced_y = true;
    } else if (block.position.X <= ball.position.X) { // Bollen är till höger om blocket
        if (!bounced_x) {ball.velocity.X = -ball.velocity.X;}
        bounced_x = true;
    } else if (block.position.X >= ball.position.X) { // Bollen är till vänster om blocket
        if (!bounced_x) {ball.velocity.X = -ball.velocity.X;}
        bounced_x = true;
    }
    ball.speed += 0.1f;
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
            foreach (Ball ball in balls) {
                ball.speed -= 2;
                if (ball.speed < 2) {ball.speed = 2;} // så inte bollen kan råka åka baklänges hehe
            }
        } else if (powerup.name == "speed up") {
            foreach (Ball ball in balls) {ball.speed += 2;}
        } else if (powerup.name == "+1 ball") {
            Ball ball = new Ball();
            ball.set_position_to_platta(platta);
            balls.Add(ball);
            amount_of_balls++;
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
    Raylib.ClearBackground(new Color(40, 40, 40, 255));

    foreach (Particle particle in partiklar) { particle.update_particle(); } // ritar alla partiklar

    foreach (Block block in blocks) { // Ritar alla block
        Texture2D block_textur = block_texturer[rand.Next(0, block_texturer.Count)];

        // Raylib.DrawRectangleRec(block, Color.WHITE);
        if (block.is_alive) {
            Raylib.DrawTexturePro(block.texture, new Rectangle(0, 0, block_textur.width, block_textur.height), block.rect, new Vector2(0, 0), 0, Color.WHITE);
            if (block.hardness) {
                Rectangle hardness_rect = new Rectangle(block.rect.x-5, block.rect.y-5, block.rect.width+10, block.rect.height+10);
                Raylib.DrawTexturePro(hardness_texture, new Rectangle(0, 0, hardness_texture.width, hardness_texture.height), hardness_rect, new Vector2(0, 0), 0, Color.WHITE);
            }
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
        platta.rect,
        new Vector2(0, 0),
        0, platta.color_tint
    );

    foreach (Ball ball in balls) {
        if (!ball.is_alive) {continue;}
        Raylib.DrawTexture(bollBild, (int)ball.position.X-bollBild.width/2, (int)ball.position.Y-bollBild.height/2, new Color(255, 255, 255, ball.ball_color_a));
    }

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