using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.Sprites.UIElem;

public class Button {
    public Vector2 Position;
    public Rectangle IntersectArea;
    private readonly Rectangle _texBounds;
    private readonly Vector2 _scale = Vector2.One;
    private readonly Texture2D _texture;
    public Color Shade = Color.White;
    public SpriteEffects SpriteEffects = SpriteEffects.None;
    public bool Visible = true;
    public float Layer = 0f;

    public Button(Vector2 position, Texture2D texture, Rectangle texBounds) {
        _texture = texture;
        Position = position;
        _texBounds = texBounds;
        SetIntersectArea();
    }
    public Button(Vector2 position, Texture2D texture, Rectangle texBounds, Vector2 scale) {
        _texture = texture;
        Position = position;
        _texBounds = texBounds;
        _scale = scale;
        SetIntersectArea();
    }
    
    
    public void SetIntersectArea() {
        IntersectArea = new Rectangle((int) Position.X, (int) Position.Y, (int) (_texBounds.Width * _scale.X), (int) (_texBounds.Height * _scale.Y));
    }

    public void Update(GameTime gameTime) {
        if (!Visible)
            return;
        if (Globals.MousePos.Intersects(IntersectArea)) {
            Hover();
            if (Globals.Clicked)
                Click();
        }
        else if (Globals.PrevMousePos.Intersects(IntersectArea))
            Leave();
    }

    private void Click() {
        OnClick?.Invoke(this, EventArgs.Empty);
    }
    private void Hover() {
        OnHover?.Invoke(this, EventArgs.Empty);
    }
    private void Leave() {
        OnLeave?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler OnClick;
    public event EventHandler OnHover;
    public event EventHandler OnLeave;

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (!Visible)
            return;
        spriteBatch.Draw(_texture, Position, _texBounds, Shade, 0f, new Vector2(0, 0), _scale, SpriteEffects, Layer);
    }
}