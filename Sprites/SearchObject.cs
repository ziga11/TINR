using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.Sprites;

public class SearchObject {
    public string Name;
    public Vector2 Position;
    public float Scale = 1f;
    public Boolean Searched = false;
    public Rectangle TexCoords;
    public Rectangle BoundingRect;
    public Color[] TextureData = new Color[Globals.ObjectAtlas.Width * Globals.ObjectAtlas.Height];
    public Texture2D Texture2D = Globals.ObjectAtlas;

    public SearchObject(string name, Vector2 position) {
        Name = name;
        Position = position;
        TexCoords = Globals.ObjectDict[Name].Rect;
        BoundingRect = new Rectangle((int) Position.X, (int) Position.Y, TexCoords.Width, TexCoords.Height);
        Globals.ObjectAtlas.GetData(TextureData);
    }

    public SearchObject(string name, Vector2 position, float scale) {
        Name = name;
        Position = position;
        Scale = scale;
        TexCoords = Globals.ObjectDict[Name].Rect;
        BoundingRect = new Rectangle((int) Position.X, (int) Position.Y, TexCoords.Width, TexCoords.Height);
        Globals.ObjectAtlas.GetData(TextureData);
    }

    public void UpdatePos(int x, int y) {
        Position = new Vector2(x, y);
        BoundingRect = new Rectangle(x, y, BoundingRect.Width, BoundingRect.Height);
    }

    public void Update(GameTime gameTime) {
        if (Globals.MousePos.Intersects(BoundingRect) && !Searched && !Transparency())
            Texture2D = Globals.HoveredAtlas;
        else
            Texture2D = Globals.ObjectAtlas;
    }

    public Boolean Transparency() {
        Vector2 imgPos = new Vector2(Globals.MousePos.X - Position.X, Globals.MousePos.Y - Position.Y);
        Vector2 imgPosTex = new Vector2(TexCoords.X + imgPos.X, TexCoords.Y + imgPos.Y);

        int ind = (int) imgPosTex.X + (int) imgPosTex.Y * Globals.ObjectAtlas.Width;

        return TextureData[ind].A == 0;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Draw(Texture2D, Position, TexCoords, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0.5f);
    }
}