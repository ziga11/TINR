using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WinApp.Sprites.UIElem;

public class Slider {
    public bool Visible = true;
    private readonly bool _horizontal;
    public readonly Sprite SliderSprite;
    public readonly Sprite BoundsSprite;
    private bool _pressedIA; /* IA - IntersectArea*/
    public Slider(Sprite sliderSprite, Sprite boundsSprite, bool horizontal) {
        SliderSprite = sliderSprite;
        BoundsSprite = boundsSprite;
        _horizontal = horizontal;
        if (horizontal) {
            SliderSprite.Position.X = boundsSprite.Position.X + boundsSprite.IntersectArea.Width - (float) SliderSprite.IntersectArea.Width / 2;
            SliderSprite.Position.Y = boundsSprite.Position.Y - ((SliderSprite.IntersectArea.Height - (float) boundsSprite.IntersectArea.Height) / 2);
        }
        else {
            SliderSprite.Position.X = boundsSprite.Position.X - ((SliderSprite.IntersectArea.Width - (float) boundsSprite.IntersectArea.Width) / 2);
            SliderSprite.Position.Y = boundsSprite.Position.Y + boundsSprite.IntersectArea.Height;
        }
    }

    private void SetSliderPos(Vector2 position) {
        if (_horizontal) {
            SliderSprite.Position.X = Math.Max(Math.Min(position.X - (float) SliderSprite.IntersectArea.Width / 2, 
                                        BoundsSprite.IntersectArea.X + BoundsSprite.IntersectArea.Width - (float) SliderSprite.IntersectArea.Width / 2),
                                        BoundsSprite.IntersectArea.X - (float) SliderSprite.IntersectArea.Width / 2);
        }
        else
            SliderSprite.Position.Y = Math.Max(Math.Min(position.Y - (float) SliderSprite.IntersectArea.Height / 2, 
                                        BoundsSprite.IntersectArea.Y + BoundsSprite.IntersectArea.Height - (float) SliderSprite.IntersectArea.Height / 2),
                                        BoundsSprite.IntersectArea.Y - (float) SliderSprite.IntersectArea.Height / 2);
    }

    public EventHandler OnMove;
    public EventHandler OnHover;
    public EventHandler OnLeave;

    private void Move() {
        SetSliderPos(new Vector2(Globals.MousePos.X, Globals.MousePos.Y));
        _pressedIA = true;
        OnMove?.Invoke(this, EventArgs.Empty);
    }
    private void Hover() {
        OnHover?.Invoke(this, EventArgs.Empty);
    }
    private void Leave() {
        OnHover?.Invoke(this, EventArgs.Empty);
    }

    public void Update(GameTime gameTime) {
        if (!Visible)
            return;
        if (Globals.MousePos.Intersects(BoundsSprite.IntersectArea)) {
            if (Globals.PrevMousePos != Globals.MousePos && Globals.MouseState.LeftButton == ButtonState.Pressed)
                _pressedIA = true;
            Hover();
        }
        else if (Globals.PrevMousePos.Intersects(BoundsSprite.IntersectArea))
            Leave();
        if (_pressedIA)
            Move();
        if (Globals.MouseState.LeftButton == ButtonState.Released)
            _pressedIA = false;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (!Visible)
            return;
        
        BoundsSprite.Draw(gameTime, spriteBatch);
        SliderSprite.Draw(gameTime, spriteBatch);
    }
}