using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Physics;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace WinApp.Sprites.Character;

public class Player {
    public readonly Game1 Game;

    public bool Left;

    public readonly List<int> ItrList = new List<int> { 1, 2, 3, 2, 1, 4, 5, 6, 5, 4 };

    public int Index;
    public int FrameCount;

    public float SearchStart;
    public float SearchTime;

    public bool Start = true;

    public readonly List<Monster> Inventory;
    public Monster[] BattleMonsters;
    public SearchObject SearchObject;

    private readonly PlayerMovement _playerMovement;

    private readonly Texture2D _searchHud;
    private readonly Vector2 _searchLen = Globals.Font.MeasureString("Searching");
    public Vector2 Position;
    public Rectangle Rectangle;

    public Player(Game1 game1) {
        Game = game1;
        Inventory = new List<Monster> {
            new Monster("Blazer", "Fire", Globals.FireMonsterAtlas, 1)
                { SpriteEffects = SpriteEffects.FlipHorizontally},
            new Monster("AquaVisor", "Water", Globals.WaterMonsterAtlas, 1)
                { SpriteEffects = SpriteEffects.FlipHorizontally},
            new Monster("Alligator", "Water", Globals.WaterMonsterAtlas, 1)
                { SpriteEffects = SpriteEffects.FlipHorizontally},
            new Monster("AquaFluffy", "Water", Globals.WaterMonsterAtlas, 1)
                { SpriteEffects = SpriteEffects.FlipHorizontally },
            new Monster("GreenWolf", "Nature", Globals.NatureMonsterAtlas, 1)
                { SpriteEffects = SpriteEffects.FlipHorizontally },
            new Monster("GreenBunny", "Nature", Globals.NatureMonsterAtlas, 1)
                { SpriteEffects = SpriteEffects.FlipHorizontally },
            new Monster("Aetos", "Wind", Globals.WindMonsterAtlas, 1)
                { SpriteEffects = SpriteEffects.FlipHorizontally },
            new Monster("WindUnicorn", "Wind", Globals.WindMonsterAtlas, 1)
                { SpriteEffects = SpriteEffects.FlipHorizontally },
            new Monster("Bonobo", "Wind", Globals.WindMonsterAtlas, 25)
                { SpriteEffects = SpriteEffects.FlipHorizontally }
        };


        _searchHud = new Texture2D(Game.Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        _searchHud.SetData(new[] { Color.Aqua });

        Position = new Vector2(0, 550);
        BattleMonsters = new Monster[4];
        BattleMonsters[0] = Inventory[0];
        BattleMonsters[1] = Inventory[1];
        BattleMonsters[2] = Inventory[8];
        _playerMovement = new PlayerMovement(this, 400f);
    }

    private void ResetChar() {
        if (Globals.MouseState.LeftButton != ButtonState.Released)
            return;

        Rectangle = Globals.ObjectDict[$"{(Left ? "Left0" : "Right0")}"].Rect;

        SearchStart = 0;
        SearchTime = 0;
        Index = 0;
        FrameCount = 0;
        Start = true;
    }

    public void Update(GameTime gameTime) {
        _playerMovement.Click();
        ResetChar();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Draw(Globals.ObjectAtlas, Position, Rectangle, Color.White, 0f, Vector2.Zero, 1f,
            SpriteEffects.None, 0f);

        if (SearchObject == null || SearchObject is { Searched: true } || SearchTime == 0)
            return;

        int posX = (int) SearchObject.Position.X + (SearchObject.BoundingRect.Width / 2);
        int posY = (int) SearchObject.Position.Y + (SearchObject.BoundingRect.Height / 2);
        Vector2 pos = Vector2.Subtract(new Vector2(posX, posY), _searchLen / 2);

        if (BattleMonsters.Any(monster => monster.Health > 0)) {
            float lineWidth = SearchTime * 10 / _searchLen.X;
            Rectangle rectangle = new Rectangle(posX + (int)lineWidth / 2, posY - 10, (int)lineWidth, 3);
            spriteBatch.Draw(_searchHud, rectangle, null, Color.White, 0f, Vector2.One, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(Globals.Font, "Searching", pos, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None,
                0.6f);
        } 
        else {
            spriteBatch.DrawString(Globals.Font, "All Monsters\nare dead", pos, Color.White, 0f, Vector2.Zero, 1,
                SpriteEffects.None,
                0.6f);
        }
    }
}