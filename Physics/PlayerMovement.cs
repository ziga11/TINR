using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WinApp.Managers;
using WinApp.Sprites;
using WinApp.Sprites.Character;

namespace WinApp.Physics;

public class PlayerMovement {
    private Player _player;
    private readonly float _speed;

    private Sprite[] BGs => _player.Game.LoadStates.GpState.Backgrounds;
    private Sprite[] WbGs => _player.Game.LoadStates.GpState.WalkBackgrounds;
    private readonly Vector2 _searchRadius = new Vector2(15, 15);
    private readonly Vector2 _toFeetTf = new Vector2(116, 512);
    public PlayerMovement(Player player, float speed) {
        _player = player;
        _speed = speed;
    }

    public void Click() {
        MouseState mouseState = Globals.MouseState;
        if (mouseState.LeftButton != ButtonState.Pressed)
            return;
        Vector2 pointerPos = Vector2.Subtract(new Vector2(mouseState.X, mouseState.Y), _toFeetTf);
        Move(_player.SearchObject == null ? pointerPos : Vector2.Subtract(new Vector2(NewX(), NewY()), _toFeetTf));
    }

    private bool IsWalkable(Vector2 position) {
        int bgInd = GetBgIndex(position);
        Sprite walkMap = WbGs[bgInd];

        int ind = (int) (position.X - walkMap.Position.X) + (int) position.Y * walkMap.Rectangle.Width;

        return position.X >= 0 && walkMap.TextureData[ind].A == 0;
    }

    private int GetBgIndex(Vector2 position) {
        return (int) (position.X - BGs[0].Position.X) / 1920;
    }

    private float NewX() {
        float playerPosX = _player.Position.X + _toFeetTf.X;
        float objectPosX = _player.SearchObject.Position.X;
        float objectWidth = _player.SearchObject.BoundingRect.Width;

        if (playerPosX >= objectPosX - _searchRadius.X &&
            playerPosX <= objectPosX + objectWidth + _searchRadius.X) {
            return playerPosX;
        }

        float smallest = float.MaxValue;
        float newX = 0;

        if (Math.Abs(playerPosX - objectPosX - _searchRadius.X) < smallest) {
            smallest = Math.Abs(playerPosX - objectPosX - _searchRadius.X);
            newX = objectPosX - _searchRadius.X;
        }

        if (Math.Abs(playerPosX - (objectPosX + objectWidth + _searchRadius.Y)) < smallest)
            newX = objectPosX + objectWidth + _searchRadius.Y;
        return newX;
    }

    private float NewY() {
        float playerPosY = _player.Position.Y + _toFeetTf.Y;
        float objectPosY = _player.SearchObject.Position.Y;
        float objectHeight = _player.SearchObject.BoundingRect.Height;

        if (playerPosY >= objectPosY - _searchRadius.Y && playerPosY <= objectPosY + objectHeight + _searchRadius.Y)
            return playerPosY;

        float smallest = float.MaxValue;
        float newY = 0;

        if (Math.Abs(playerPosY - objectPosY - _searchRadius.Y) < smallest) {
            smallest = Math.Abs(playerPosY - objectPosY - _searchRadius.Y);
            newY = objectPosY - _searchRadius.Y;
        }

        if (Math.Abs(playerPosY - (objectPosY + objectHeight + _searchRadius.Y)) < smallest)
            newY = objectPosY + objectHeight + _searchRadius.Y;
        return newY;
    }

    private void Move(Vector2 pointer) {
        ref Vector2 playerPos = ref _player.Position;
        float elapsed = (float) Globals.GameTime.ElapsedGameTime.TotalSeconds;

        Vector2 direction = Vector2.Normalize(pointer - playerPos);
        Vector2 movement = direction * _speed * elapsed;

        Vector2 nextPos = Vector2.Distance(playerPos, pointer) > movement.Length() ? Vector2.Add(playerPos, movement) : pointer;
        if (!IsWalkable(Vector2.Add(nextPos, _toFeetTf))) {
            _player.Rectangle = Globals.ObjectDict[$"{(_player.Left ? "Left0" : "Right0")}"].Rect;
            return;
        }

        playerPos = nextPos;

        FinalPos(pointer);
        WalkPos(direction, pointer);
        _player.FrameCount++;
    }

    private void FinalPos(Vector2 pointer) {
        if (_player.Position != pointer)
            return;

        _player.Rectangle = Globals.ObjectDict[_player.Left ? "Left0" : "Right0"].Rect;
        _player.Start = true;

        if (_player.SearchObject is not { Searched: false })
            return;

        if (_player.SearchStart == 0)
            _player.SearchStart = (float) Globals.GameTime.TotalGameTime.TotalMilliseconds;
        else if (_player.SearchTime >= 500.0 && _player.BattleMonsters.Any(a => a.Health > 0)) {
            _player.SearchObject.Searched = true;
            _player.SearchObject = null;
            Globals.Game.LoadStates.GpState.Timespan = MediaPlayer.PlayPosition;
            AudioManager.StopSong();
            Globals.Game.SetGameState("Battle");
        }
        _player.SearchTime = (float) Globals.GameTime.TotalGameTime.TotalMilliseconds - _player.SearchStart;
    }

    private void WalkPos(Vector2 dir, Vector2 pointer) {
        if (float.IsNaN(dir.X) || float.IsNaN(dir.Y) || dir is { X: 0, Y: 0 } || pointer == _player.Position)
            return;

        ref int index = ref _player.Index;
        ref bool start = ref _player.Start;
        ref bool left = ref _player.Left;
        List<int> itrList = _player.ItrList;
        left = dir.X < 0;

        _player.Rectangle = Globals.ObjectDict[$"{(left ? "Left" : "Right")}{itrList[index]}"].Rect;

        if (_player.FrameCount % 5 != 0)
            return;

        index++;
        if ((start && index >= itrList.Count) || (!_player.Start && index >= itrList.Count)) {
            index = 0;
            start = false;
        }

        _player.Rectangle = Globals.ObjectDict[$"{(left ? "Left" : "Right")}{itrList[index]}"].Rect;
    }
}