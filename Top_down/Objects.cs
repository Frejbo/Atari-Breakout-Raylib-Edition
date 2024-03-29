using System.Numerics;
using Raylib_cs;
using textures;

namespace Objects {
    class Block {
        public float blocks_gap = 12f;
        Vector2 block_size;
        static Texture2D hardness_texture = new textures.Texturer().hardness;
        public Vector2 position;
        public bool is_alive;
        public Texture2D texture;
        public Rectangle rect;
        public bool hardness = false;
        public Color particle_color;

        public void Initialize(Vector2 pos, Texture2D textur) {
            block_size = new Vector2((float)(Raylib.GetScreenWidth()/11-blocks_gap), ((Raylib.GetScreenWidth() / 11 - blocks_gap) / 150) * 60); // Blockens res är 150x60.
            position = pos;
            texture = textur;
            is_alive = true;
            rect = new Rectangle(position.X, position.Y, block_size.X, block_size.Y);
        }
        public void Draw() {
            if (!is_alive) { return; }
            Raylib.DrawTexturePro(texture, new Rectangle(0, 0, texture.width, texture.height), rect, new Vector2(0, 0), 0, Color.WHITE);
            if (hardness) {
                Rectangle hardness_rect = new Rectangle(rect.x-5, rect.y-5, rect.width+10, rect.height+10);
                Raylib.DrawTexturePro(hardness_texture, new Rectangle(0, 0, hardness_texture.width, hardness_texture.height), hardness_rect, new Vector2(0, 0), 0, Color.WHITE);
            }
        }
    }



    class Ball {
        public bool is_alive = true;
        public Vector2 velocity = new Vector2((float)(new Random().NextDouble()-.5), 1);
        public float actual_ball_speed = 1; // Ändra inte den här!
        public int ball_color_a = 255;
        public Vector2 position = new Vector2(Raylib.GetScreenWidth()/2, Raylib.GetScreenHeight()/2);
        public float speed = 5;
        float ball_acceleration = 0.1f;


        public void Tick() {
            // calculating direction of ball
            position.X += velocity.X * actual_ball_speed;
            position.Y += velocity.Y * actual_ball_speed;

            // accelerera om bollhastigheten är under den angivna
            if (actual_ball_speed < speed) {actual_ball_speed += ball_acceleration;}
            if (actual_ball_speed > speed) {actual_ball_speed = speed;}
        }
        
        public void SetPositionToPlatta(Platta platta) {
            position = new Vector2(platta.position.X+platta.width/2, Raylib.GetScreenHeight()/2);
        }
    }


    class Platta {
        public int speed;
        public int size = 1;
        public float size_change_timer = 0f;
        public Color color_tint;
        static Texture2D short_platta = Raylib.LoadTexture("Assets/Short Plate.png");
        static Texture2D medium_platta = Raylib.LoadTexture("Assets/Medium Plate.png");
        static Texture2D long_platta = Raylib.LoadTexture("Assets/Long Plate.png");
        public Vector2 position;
        public Rectangle rect;
        public int width;
        public int height;
        public Texture2D texture;

        public void IncreasePlattaSize() {
            size += 1;
            size_change_timer += 60*10; // En powerup varar i 10 sekunder
        }
        public void DecreasePlattaSize() {
            if (size == 1) {
            }
            size -= 1;
            size_change_timer += 60*10; // En powerup varar i 10 sekunder
        }
        public void TickPlattaSize() {
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
                if (position.X + ((int)texture.width) >= Raylib.GetScreenWidth()) {
                    position.X = Raylib.GetScreenWidth() - (int)(texture.width);
                } else {
                    position.X += speed;
                }
            }
            width = texture.width*(int)1.5; // Plattan är 1.5x större än dens textur... borde göra om plattan till rätt res men orkar inte
            height = texture.height*(int)1.5;
            rect = new Rectangle(position.X, position.Y, width, height);

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
        }
        public void Initialize() {
            texture = medium_platta;
            size = 1;
            color_tint = Color.WHITE;
            position = new Vector2((Raylib.GetScreenWidth()/2)-texture.width/2, Raylib.GetScreenHeight() - (int)(Raylib.GetScreenHeight()/10));
        }
    }



    class Powerup {
        public Vector2 position;
        public Texture2D texture;
        public int speed = 2;
        public Vector2 size = new Vector2(30, 30);
        public string name = "";
        public void Draw() {
            Rectangle source_size = new Rectangle(0, 0, texture.width, texture.height);
            Rectangle dest_size = new Rectangle(position.X, position.Y, size.X, size.Y);
            Raylib.DrawTexturePro(texture, source_size, dest_size, new Vector2(0,0), 0, Color.WHITE);
        }
    }
}