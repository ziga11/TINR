using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Sprites;

namespace WinApp.Animation;

public class AnimationManager {
    private readonly Dictionary<string, Animation> _animationDict = new Dictionary<string, Animation>();

    public Animation AddAnimation(string name, List<Sprite> sprites, List<Vector2> positions, float duration) {
        Animation a = new Animation(sprites, positions, duration);
        _animationDict[name] = a;
        
        return a;
    }

    public Animation Animate(string animation) {
        _animationDict[animation].Animate = true;
        
        return _animationDict[animation];
    }

    public void Update(GameTime gameTime) {
        foreach (Animation animation in _animationDict.Values.ToList())
            animation.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        foreach (Animation animation in _animationDict.Values.ToList())
            animation.Draw(gameTime, spriteBatch);
    }
}