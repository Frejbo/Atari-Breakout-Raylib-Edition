using Raylib_cs;

Raylib.InitWindow(1024, 1024, "TopDownGame");
Raylib.SetTargetFPS(60);

Rectangle character = new Rectangle(0, 0, 30, 30);

while (!Raylib.WindowShouldClose()) {
    if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) {
        character.x = 512;
        character.y = 512;
    }


    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.BLACK);
    Raylib.EndDrawing();

    Raylib.DrawRectangleRec(character, new Color(255, 255, 255, 255));
}