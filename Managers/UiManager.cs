using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Sprites;
using WinApp.Sprites.UIElem;

namespace WinApp.Managers;

public class UiManager {
    public readonly Dictionary<string, Button> BtnDict = new Dictionary<string, Button>();
    public readonly Dictionary<string, RadioBtn> RadioDict = new Dictionary<string, RadioBtn>();
    public readonly Dictionary<string, TextBox> TextBoxDict = new Dictionary<string, TextBox>();
    public readonly Dictionary<string, Slider> SliderDict = new Dictionary<string, Slider>();
    public readonly Dictionary<string, TextElem> TextElemDict = new Dictionary<string, TextElem>();
    public readonly Dictionary<string, Sprite> SpriteDict = new Dictionary<string, Sprite>();

    public Button AddButton(string name, Vector2 position, Texture2D texture, Rectangle texBounds) {
        Button a = new Button(position, texture, texBounds);
        BtnDict[name] = a;
        return a;
    }
    public Button AddButton(string name, Vector2 position, Texture2D texture, Rectangle texBounds, Vector2 scale) {
        Button a = new Button(position, texture, texBounds, scale);
        BtnDict[name] = a;
        return a;
    }
    public void AddButton(string name, Button button) {
        BtnDict[name] = button;
    }
    public Button RemoveButton(string name) {
        Button button = BtnDict[name];
        BtnDict.Remove(name);
        return button;
    }
    public TextElem RemoveTextElem(string name) {
        TextElem textElem = TextElemDict[name];
        TextElemDict.Remove(name);
        return textElem;
    }

    public TextBox AddTextBox(string name, string text, Vector2 position, Color textColor, Vector2 bounds, Color boundsColor) {
        TextBox a = new TextBox(text, position, textColor, bounds, boundsColor);
        TextBoxDict[name] = a;
        return a;
    }
    public TextElem AddTextElem(string name, string text, Vector2 position, Color textColor) {
        TextElem a = new TextElem(text, position, textColor);
        TextElemDict[name] = a;
        return a;
    }

    public void AddTextElem(string name, TextElem textElem) {
        TextElemDict[name] = textElem;
    }

    public Sprite AddSprite(string name, Texture2D texture) {
        Sprite a = new Sprite(texture);
        SpriteDict[name] = a;
        return a;
    }
    public Sprite AddSprite(string name, Texture2D texture, Rectangle rectangle, Vector2 position) {
        Sprite a = new Sprite(texture, rectangle, position);
        SpriteDict[name] = a;
        return a;
    }
    public Sprite AddSprite(string name, Texture2D texture, Rectangle rectangle, Vector2 position, float layer) {
        Sprite a = new Sprite(texture, rectangle, position) { Layer = layer };
        SpriteDict[name] = a;
        return a;
    }
    public Sprite AddSprite(string name, Texture2D texture, Rectangle rectangle, Vector2 position, Vector2 scale) {
        Sprite a = new Sprite(texture, rectangle, position, scale);
        SpriteDict[name] = a;
        return a;
    }
    public Sprite AddSprite(string name, Texture2D texture, Rectangle rectangle, Vector2 position, float layer, Vector2 scale) {
        Sprite a = new Sprite(texture, rectangle, position, scale) { Layer = layer };
        SpriteDict[name] = a;
        return a;
    }
    public Slider AddSlider(string name, Sprite slider, Sprite bounds, bool horizontal) {
        Slider a = new Slider(slider, bounds, horizontal);
        SliderDict[name] = a;
        return a;
    }
    
    public RadioBtn AddRadio(string name, Texture2D texture, Rectangle rectangle, Vector2 position) {
        RadioBtn a = new RadioBtn(texture, rectangle, position);
        RadioDict[name] = a;
        return a;
    }
   
    public void Update(GameTime gameTime) {
        foreach (Button button in BtnDict.Values.ToList())
            button.Update(gameTime);

        foreach (TextBox textBox in TextBoxDict.Values)
            textBox.Update(gameTime);

        foreach (TextElem textElem in TextElemDict.Values)
            textElem.Update(gameTime);

        foreach (Sprite sprite in SpriteDict.Values)
            sprite.Update(gameTime);
        
        foreach (Slider slider in SliderDict.Values)
            slider.Update(gameTime);

        foreach (RadioBtn radioBtn in RadioDict.Values)
            radioBtn.Update(gameTime);
    }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        foreach (Button button in BtnDict.Values)
            button.Draw(gameTime, spriteBatch);
        
        foreach (TextBox textBox in TextBoxDict.Values)
            textBox.Draw(gameTime, spriteBatch);
        
        foreach (TextElem textElem in TextElemDict.Values)
            textElem.Draw(gameTime, spriteBatch);

        foreach (Sprite sprite in SpriteDict.Values)
            sprite.Draw(gameTime, spriteBatch);
    
        foreach (Slider slider in SliderDict.Values)
            slider.Draw(gameTime, spriteBatch);
        
        foreach (RadioBtn radioBtn in RadioDict.Values)
            radioBtn.Draw(gameTime, spriteBatch);
    }
}