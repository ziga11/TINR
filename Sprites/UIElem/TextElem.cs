using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.Sprites.UIElem;

public class TextElem {
    public string Text;
    public SpriteFont Font = Globals.Font;
    public Vector2 Position;
    public Vector2 Scale = Vector2.One;
    public Color TextColor;
    public float Rotation;
    public float Layer = 0f;
    public bool Visible = true;

    public TextElem(string text, Vector2 position, Color textColor) {
        Text = text;
        Position = position;
        TextColor = textColor;
    }

    public void Update(GameTime gameTime) { }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Visible)
            spriteBatch.DrawString(Font, Text, Position, TextColor, Rotation, Vector2.Zero, Scale, SpriteEffects.None, Layer);
    }
}