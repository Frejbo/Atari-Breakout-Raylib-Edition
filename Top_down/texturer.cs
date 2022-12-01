using Raylib_cs;

namespace textures {
    class Texturer {
        public Texture2D hardness = Raylib.LoadTexture("Assets/Blocks/Hardness 1.png");
        public List<Texture2D> full_health = new(){
            Raylib.LoadTexture("Assets/Lifebar/life2.png"),
            Raylib.LoadTexture("Assets/Lifebar/life1.png"),
            Raylib.LoadTexture("Assets/Lifebar/life0.png")};
    }
}