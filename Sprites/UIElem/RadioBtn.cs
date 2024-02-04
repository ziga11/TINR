using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.Sprites.UIElem;

public class RadioBtn : Sprite {
    public bool Checked = true;
    private Rectangle _intersectRect;
    
    public RadioBtn(Texture2D texture, Rectangle rectangle, Vector2 position) : base(texture, rectangle, position) {
        _intersectRect = new Rectangle((int) Position.X, (int) Position.Y, Rectangle.Width, Rectangle.Height);
    }
    public RadioBtn(Texture2D texture, Rectangle rectangle, Vector2 position, Vector2 scale) : base(texture, rectangle, position, scale) {
        _intersectRect = new Rectangle((int) Position.X, (int) Position.Y, (int) scale.X * Rectangle.Width, (int) scale.Y * Rectangle.Height);
    }

    public EventHandler OnClick;
    public EventHandler OnHover;
    public EventHandler OnLeave;

    private void Click() {
        OnClick?.Invoke(this, EventArgs.Empty);
    }
    
    private void Hover() {
        OnHover?.Invoke(this, EventArgs.Empty);
    }
    
    private void Leave() {
        OnLeave?.Invoke(this, EventArgs.Empty);
    }
     
    public override void Update(GameTime gameTime) {
        if (!Visible)
            return;
        
        if (Globals.MousePos.Intersects(_intersectRect)) {
            Hover();
            if (!Globals.Clicked)
                return;
            Checked = !Checked;
            Click();
        }
        else if (Globals.PrevMousePos.Intersects(_intersectRect))
            Leave();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (Texture == null || !Visible)
            return;
        spriteBatch.Draw(Texture, Position, Rectangle, Checked ? Color.White : Color.DarkSlateGray, Rotation, Origin, Scale, (Checked ? SpriteEffects.None : SpriteEffects.FlipHorizontally), Layer);
    }
}