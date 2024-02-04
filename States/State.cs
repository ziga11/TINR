using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.States; 

public abstract class State {
    protected readonly Game1 Game1;
    protected ContentManager _content;

    protected State(Game1 game1, ContentManager content) {
        Game1 = game1;
        _content = content;
    }

    public abstract void LoadContent();

    public abstract void Update(GameTime gameTime);

    public abstract void PostUpdate(GameTime gameTime);

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}