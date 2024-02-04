using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using WinApp.Sprites.Character;
using WinApp.Sprites.Skill;
using WinApp.States;


namespace WinApp {
    public class Game1 : Game {
        public readonly GraphicsDeviceManager Graphics;
        private SpriteBatch _spriteBatch;

        private const int ScreenWidth = 1920;
        private const int ScreenHeight = 1080;

        private State _currentState;
        private State _nextState;

        public LoadStates LoadStates { get; private set; }

        public Game1() {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            Window.AllowUserResizing = true;
            /*
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width
             */
            Graphics.PreferredBackBufferWidth = ScreenWidth;
            Graphics.PreferredBackBufferHeight = ScreenHeight;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();


            IsMouseVisible = true;

            LoadStates = new LoadStates(this, Content);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.GraphicsDevice = GraphicsDevice;
            Globals.Game = this;
            Globals.Content = Content;

            LoadTextures();
            LoadData();
            _currentState = LoadStates.GameplayState();
            _nextState = null;
        }

        private void LoadTextures() {
            Globals.ObjectAtlas = Content.Load<Texture2D>("Objects/Atlas");
            Globals.HoveredAtlas = Content.Load<Texture2D>("Objects/AtlasHovered");
            Globals.BattleBackground = Content.Load<Texture2D>("Backgrounds/BattleBackground");
            Globals.GameBg1 = Content.Load<Texture2D>("Backgrounds/BG1");
            Globals.GameBg2 = Content.Load<Texture2D>("Backgrounds/BG2");
            Globals.GameBg3 = Content.Load<Texture2D>("Backgrounds/BG3");
            Globals.WalkBg1 = Content.Load<Texture2D>("Backgrounds/WalkMap1");
            Globals.WalkBg2 = Content.Load<Texture2D>("Backgrounds/WalkMap2");
            Globals.WalkBg3 = Content.Load<Texture2D>("Backgrounds/WalkMap3");
            Globals.SkillAtlas = Content.Load<Texture2D>("Animations/SkillAnimations");
            Globals.FireMonsterAtlas = Content.Load<Texture2D>("Mobs/FireMonsters");
            Globals.WaterMonsterAtlas = Content.Load<Texture2D>("Mobs/WaterMonsters");
            Globals.NatureMonsterAtlas = Content.Load<Texture2D>("Mobs/NatureMonsters");
            Globals.WindMonsterAtlas = Content.Load<Texture2D>("Mobs/WindMonsters");
            Globals.HudAtlas = Content.Load<Texture2D>("HUD/HUDAtlas");
            Globals.Font = Content.Load<SpriteFont>("Fonts/File");
            Globals.BoldFont = Content.Load<SpriteFont>("Fonts/Bold");

            Globals.AudioManager.AddSoundEffect("Flame", Content.Load<SoundEffect>("Sounds/Flame"));
            Globals.AudioManager.AddSoundEffect("Weaken", Content.Load<SoundEffect>("Sounds/Weaken"));
            Globals.AudioManager.AddSoundEffect("FireBall", Content.Load<SoundEffect>("Sounds/FireBall"));
            Globals.AudioManager.AddSoundEffect("Physical", Content.Load<SoundEffect>("Sounds/Physical"));
            Globals.AudioManager.AddSoundEffect("Sharpen", Content.Load<SoundEffect>("Sounds/Sharpen"));
            Globals.AudioManager.AddSoundEffect("Tornado", Content.Load<SoundEffect>("Sounds/Tornado"));

            Globals.AudioManager.AddSong("Battle", Content.Load<Song>("Songs/Battle"));
            Globals.AudioManager.AddSong("Gameplay", Content.Load<Song>("Songs/Gameplay"));
        }

        static private void LoadData() {
            const string mobUrl = "Content/Data/Monsters.json";
            string mobData = File.ReadAllText(mobUrl);
            const string objectUrl = "Content/Data/Objects.json";
            string objectData = File.ReadAllText(objectUrl);
            const string skillUrl = "Content/Data/Skills.json";
            string skillData = File.ReadAllText(skillUrl);
            const string hudUrl = "Content/Data/HUD.json";
            string hudData = File.ReadAllText(hudUrl);

            Globals.MonsterDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, MonsterAttr>>>(mobData);
            Globals.ObjectDict = JsonConvert.DeserializeObject<Dictionary<string, TexObject>>(objectData);
            Globals.HudDict = JsonConvert.DeserializeObject<Dictionary<string, TexObject>>(hudData);
            Globals.SkillDict = JsonConvert.DeserializeObject<Dictionary<string, Skill>>(skillData);

            LoadSettings();
            Globals.MenuState = new MenuState(Globals.Game, Globals.Content);
        }

        public void SetState(State state) {
            _nextState = state;
        }

        public void SetGameState(string state) {
            switch (state) {
                case "Gameplay":
                    _nextState = LoadStates.GameplayState();
                    break;
                case "Battle":
                    _nextState = LoadStates.BattleState();
                    break;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (_nextState != null) {
                _currentState = _nextState;

                _nextState = null;
            }

            _currentState.Update(gameTime);

            _currentState.PostUpdate(gameTime);

            Globals.Update(gameTime);
            base.Update(gameTime);
        }

        static private void LoadSettings() {
            string settingsData = File.ReadAllText("Content/Data/Settings.json");
            Settings settings = JsonConvert.DeserializeObject<Settings>(settingsData);

            Settings.Instance.General = settings.General;
            Settings.Instance.Battle = settings.Battle;
            Settings.Instance.Skill = settings.Skill;

            Settings.Instance.NotMutedAll = settings.NotMutedAll;
            Settings.Instance.NotMutedBattle = settings.NotMutedBattle;
            Settings.Instance.NotMutedSkill = settings.NotMutedSkill;
        }

        static private void SaveSettings() {
            string serialized = JsonConvert.SerializeObject(Settings.Instance);
            File.WriteAllText("Content/Data/Settings.json", serialized);
        }

        protected override void OnExiting(object sender, EventArgs args) {
            base.OnExiting(sender, args);
            SaveSettings();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(new Color(255, 255, 255));

            _currentState.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}