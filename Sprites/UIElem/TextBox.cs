using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.Sprites.UIElem;

public class TextBox : TextElem {
    public Vector2 Bounds;
    public Color RectColor;
    public Texture2D RectTexture;

    public TextBox(string text, Vector2 position, Color textColor, Vector2 bounds, Color rectColor) : base(text, position, textColor) {
        Bounds = bounds;
        RectColor = rectColor;
        RectTexture = new Texture2D(Globals.GraphicsDevice, (int) bounds.X, (int) bounds.Y);
        RectTexture.SetData(new [] {rectColor});
    }

    public TextBox(string text, Vector2 position, Color textColor, Vector2 bounds, Texture2D texture, Rectangle texBounds) : base(text, position, textColor) {
        Bounds = bounds;
        RectTexture = texture;
    }

    public void Draw(SpriteBatch spriteBatch) {
        if (!Visible)
            return;
        
        spriteBatch.Draw(RectTexture, Position, Color.White);
        spriteBatch.DrawString(Font, Text, Position, TextColor, Rotation, Vector2.Zero, Scale, SpriteEffects.None, Layer);
    }
}
