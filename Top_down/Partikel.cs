using Raylib_cs;
using System.Numerics;

namespace Partikel {
    class Particle {
        int number_of_points = 32;
        public Color color = Color.RED;
        public Vector2 position;
        public bool is_emitting = true;
        List<ParticleDot> dots = new();
        public void Initialize() {
            for (int degree = 0; degree < 360; degree += 360/number_of_points) {
                ParticleDot dot = new ParticleDot();
                Random rand = new Random();
                dot.initial_velocity = (float)rand.NextDouble()*2+1;
                dot.dot_radius = rand.Next(2, 10);
                dot.SetBaseColor(color);

                double radian = (Math.PI / 180) * degree;

                dot.position.X = (float)Math.Cos(radian)*1 + position.X;
                dot.position.Y = (float)Math.Sin(radian)*1 + position.Y;

                dot.velocity = new Vector2(dot.position.X - position.X, dot.position.Y - position.Y);
                dot.velocity *= dot.initial_velocity;

                dots.Add(dot);
            }
        }
        
        // only call update_particle inside Raylib drawing!
        public void UpdateDrawParticle() {
            int index = 0;
            // Skapar en temporär kopia som kan loopas över, eftersom C# inte tillåter att ändra värden som loopas
            List<ParticleDot> temp_copy = new();
            foreach (ParticleDot dot in dots) {
                temp_copy.Add(dot);
            }
            
            foreach (ParticleDot dot in temp_copy) {
                // change position
                dot.TickVelocity();
                if (dot.color.a <= 4) {
                    dots.Remove(dot);
                }
                else {
                    dot.color.a -= 4;
                }
                Raylib.DrawCircle((int)dot.position.X, (int)dot.position.Y, dot.dot_radius, dot.color);
                index++;
            }
        }
    }

    class ParticleDot {
        public Color color = Color.WHITE;
        public Vector2 position;
        public int dot_radius;
        public float initial_velocity;
        public Vector2 velocity;
        Random rand = new Random();
        float GRAVITY = .03f;

        public void SetBaseColor(Color base_color) {
            int tint_range = 25;

            color.r = tint_value(base_color.r);
            color.g = tint_value(base_color.g);
 
            color.b = tint_value(base_color.b);
            
            Byte tint_value(Byte base_val) {
                int value = rand.Next(Convert.ToInt32(base_val)-tint_range, Convert.ToInt32(base_val)+tint_range);
                
                if (value > 255) { value = 255; }
                if (value < 0) { value = 0; }
                return (Convert.ToByte(value));
            }
        }

        public void TickVelocity() {
            float multiplier = 1-(rand.Next(1, 5)*.01f);
            velocity.X *= multiplier;
            velocity.Y *= multiplier;
            velocity.Y += GRAVITY*multiplier;

            position.X += velocity.X;
            position.Y += velocity.Y;
        }
    }
}