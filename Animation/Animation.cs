using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WinApp.Sprites;

namespace WinApp.Animation;

public class Animation {
    private readonly List<Sprite> _sprites;
    private readonly List<Vector2> _positions;
    private readonly List<int> _spriteOrder;
    private List<double> _posInterpValues;
    private List<double> _spriteInterpValues;
    private readonly float _duration;
    
    public bool Animate;
    private bool _start = true;
    private int _posIndex;
    private int _spriteIndex;

    /* Specifying the curr sprite pos is necessary */
    public Animation(List<Sprite> sprites, List<Vector2> positions, List<int> spriteOrder, float duration) {
        _sprites = sprites;
        _positions = positions;
        _spriteOrder = spriteOrder;
        _duration = duration;
    }

    public Animation(List<Sprite> sprites, List<Vector2> positions, float duration) {
        _sprites = sprites;
        _positions = positions;
        _spriteOrder = Enumerable.Range(0, _sprites.Count).ToList();
        _duration = duration;
    }

    private void GetInterpValues() {
        double posStepTime = _duration / _positions.Count;
        double spriteStepTime = _duration / _spriteOrder.Count;
        
        _posInterpValues = new List<double> {Globals.GameTime.TotalGameTime.TotalMilliseconds};
        _spriteInterpValues = new List<double> {Globals.GameTime.TotalGameTime.TotalMilliseconds};

        for (int i = 0; i < _positions.Count; i++)
            _posInterpValues.Add(_posInterpValues[i] + posStepTime);
        
        for (int i = 0; i < _spriteOrder.Count; i++)
            _spriteInterpValues.Add(_spriteInterpValues[i] + spriteStepTime);
    }

    public EventHandler AnimationEnd;

    private void EndOfAnimation() {
        AnimationEnd?.Invoke(this, EventArgs.Empty);
    }

    public void Update(GameTime gameTime) {
        if (!Animate)
            return;
        if (_start) {
            _start = false;
            GetInterpValues();
        }

        double time = Globals.GameTime.TotalGameTime.TotalMilliseconds;

        double posT = Math.Min((time - _posInterpValues[_posIndex]) / (_posInterpValues[_posIndex + 1] - _posInterpValues[_posIndex]), 1);
        Vector2 endPos = Vector2.Lerp(_positions[_posIndex], _positions[_posIndex + 1], (float) posT);
        _sprites[_spriteIndex].Position = endPos;
        if (_spriteIndex < _spriteOrder.Count - 1)
            _sprites[_spriteIndex + 1].Position = endPos;
        
        double spriteT = Math.Min((time - _spriteInterpValues[_spriteIndex]) / (_spriteInterpValues[_spriteIndex + 1] - _spriteInterpValues[_spriteIndex]), 1);

        if (spriteT >= 1)
            _spriteIndex++;
        
        if (posT < 1)
            return;
        
        _posIndex++;
        Animate = _posIndex < _positions.Count - 1;

        if (Animate)
            return;

        _start = true;
        _posIndex = 0;
        _spriteIndex = 0;
        _posInterpValues.Clear();
        _spriteInterpValues.Clear();
        EndOfAnimation();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        if (!Animate)
            return;
        
        _sprites[_spriteIndex].Draw(gameTime, spriteBatch);
    }
}