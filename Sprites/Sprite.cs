using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.Sprites;

public class Sprite : Component {
    public float Layer = 0f;
    public float Rotation = 0f;
    public bool Visible = true;
    public Vector2 Origin;
    public Vector2 Position;
    public Vector2 Scale = Vector2.One;
    public SpriteEffects SpriteEffects = SpriteEffects.None;
    public Rectangle Rectangle;
    public Rectangle IntersectArea;
    

    public Texture2D Texture;
    public Color Color = Color.White;
    public Color[] TextureData;

    public Sprite(Texture2D texture) {
        Rectangle = new Rectangle(0, 0, 1920, 1080);

        Texture = texture;
        IntersectArea = Rectangle;
    }

    public Sprite(Texture2D texture, Rectangle rectangle, Vector2 position) {
        Rectangle = rectangle;
        Position = position;

        Texture = texture;
        IntersectArea = new Rectangle((int)Position.X, (int)Position.Y, rectangle.Width, rectangle.Height);
    }
    
    public Sprite(Texture2D texture, Rectangle rectangle, Vector2 position, Vector2 scale) {
        Rectangle = rectangle;
        Position = position;
        Scale = scale;

        Texture = texture;
        IntersectArea = new Rectangle((int)Position.X, (int)Position.Y, (int) (scale.X * rectangle.Width), (int) (scale.Y * rectangle.Height));
    }
    public void SetTextureData() {
        TextureData = new Color[Texture.Width * Texture.Height];
        Texture.GetData(TextureData);
    }
    private void Hover() {
        OnHover?.Invoke(this, EventArgs.Empty);
    }
    private void Leave() {
        OnLeave?.Invoke(this, EventArgs.Empty);
    }
    
    public EventHandler OnHover;
    public EventHandler OnLeave;
    
    public override void Update(GameTime gameTime) {
        if (!Visible)
            return;
        
        if (Globals.MousePos.Intersects(IntersectArea))
            Hover();
        else if (Globals.PrevMousePos.Intersects(IntersectArea))
            Leave();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Texture != null && Visible)
            spriteBatch.Draw(Texture, Position, Rectangle, Color, Rotation, Origin, Scale, SpriteEffects, Layer);
    }
}
