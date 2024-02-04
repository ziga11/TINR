using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WinApp.States;

public class LoadStates {
    private readonly ContentManager _content;
    private readonly Game1 _game;

    public GameplayState GpState { get; private set; }

    public LoadStates(Game1 game, ContentManager content) {
        _content = content;
        _game = game;
    }

    public GameplayState GameplayState() {
        GpState = new GameplayState(_game, _content);
        return GpState;
    }

    public BattleState BattleState() {
        Texture2D[] backgrounds = { Globals.BattleBackground };

        BattleState battleState = new BattleState(_game, _content, backgrounds);
        return battleState;
    }
}


