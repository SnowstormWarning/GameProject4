using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Sprites;
using Tools.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Tools.Computation;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;
using GameProject4;
using SharpDX.Direct2D1;
using static System.Net.Mime.MediaTypeNames;

namespace Tools.StateManagement
{
    public class TitleScreen : State
    {
        public StaticSprite SpotlightTitle;
        public Button Button;
        private StateCommands _command;
        private State _targetState;
        private ContentManager _content;
        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, SpriteFont font)
        {
            // Any "End Point" or Active State should start and end with a spritebatch call.
            spriteBatch.Begin();
            SpotlightTitle.Draw(gameTime, spriteBatch);
            Button.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public override StateCommands GetStateCommand()
        {
            return _command;
        }

        public override State GetTargetState()
        {
            return _targetState;
        }

        public override void Initialize()
        {
            SpotlightTitle = new StaticSprite("Spotlight");
            Button = new Button("Start");
            Button.ChangeClickColor(Color.Gold);
            Button.ChangeHoverColor(Color.Yellow);
            
            Button.Position = new Vector2(350, 400);
            
        }

        public override void LoadContent(ContentManager content)
        {
            SpotlightTitle.LoadContent(content);
            Button.LoadContent(content);
            _content = content;
        }

        public override void Update(GameTime gameTime, MouseState mouse, KeyboardState keys)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                StateManager.CloseGame = true;
            Button.Update(mouse);
            if (_command != StateCommands.None) _command = StateCommands.None;
            if (Button.IsClickedOn)
            {
                _targetState = new Level("Map.txt",_content,new Level("Map2.txt", _content));
                _command = StateCommands.AddTarget;
                StateManager.CloseGame = true;
            }
        }
    }
}
