using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace WinApp;

public class TexObject {
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Rectangle Rect => new Rectangle(X, Y, Width, Height);
}

public class Vec2 {
    public int X { get; set; }
    public int Y { get; set; }
}

public class SliderReference {
    public float Min { get; set; }
    public float Max { get; set; }
    public float Value { get; set; }
}