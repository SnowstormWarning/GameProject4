using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct2D1;
using SharpDX.Direct3D9;
using System.Collections.Generic;
using Tool.Sprites;
using Tools.Computation;
using Tools.Sprites;
using System;
using Tools.StateManagement;
using System.CodeDom;
using Microsoft.Xna.Framework.Media;
using System.Reflection.Metadata;

namespace GameProject4
{
    public class Game1 : Game //800 x 480
    {
        public static Vector2 GlobalScalingFactor;
        public static Microsoft.Xna.Framework.Graphics.SpriteFont Font;

        private GraphicsDeviceManager _graphics;
        private Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;
        private StateManager _stateManager;
        private Song _song;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _stateManager = new StateManager(GraphicsDevice);
            StateManager.Game = this;
            Vector2 test = GlobalScalingFactor;
            _stateManager.AddUnloadedState(new TitleScreen());
            _stateManager.CurrentState.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _song = Content.Load<Song>("Music");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(_song);
            _spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
            _stateManager.CurrentState.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();
            _stateManager.CurrentState.Update(gameTime, mouse, keyboard);
            switch (_stateManager.CurrentState.GetStateCommand())
            {
                case StateCommands.None:
                    break;
                case StateCommands.AddTarget:
                    _stateManager.AddState(_stateManager.CurrentState.GetTargetState(), Content);
                    break;
                case StateCommands.RemoveSource:
                    _stateManager.RemoveCurrentState();
                    break;
                case StateCommands.Clear:
                    _stateManager.ReturnAndClear();
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _stateManager.CurrentState.Draw(gameTime, _spriteBatch, Font);
            base.Draw(gameTime);
        }

        public void GameExit()
        {
            Exit();
        }
    }
}