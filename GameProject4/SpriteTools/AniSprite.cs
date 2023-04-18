using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tool.Sprites;

namespace GameProject4.SpriteTools
{
    public class AniSprite: StaticSprite
    {
        private int _textureWidth;
        private int _textureHeight;
        private int _frames;
        private int _framesPerRow;
        private int _currentFrame;
        private float _frameDuration;
        private double _timer;

        public AniSprite(string texture, int textureWidth, int textureHeight, int frames, int framesPerRow, float frameDuration) : base(texture)
        {
            _textureWidth = textureWidth;
            _textureHeight = textureHeight;
            _frames = frames;
            _framesPerRow = framesPerRow;
            _frameDuration = frameDuration;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            _timer += gameTime.TotalGameTime.TotalSeconds;
            if(_timer >= _frameDuration)
            {
                _currentFrame = (_currentFrame + 1) % _frames;
            }
            spriteBatch.Draw(GetTexture(), Position, new Rectangle(_textureWidth * (_currentFrame % _framesPerRow), _textureHeight * (_currentFrame / _framesPerRow), _textureWidth, _textureHeight), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Draw(gameTime, spriteBatch, Color.White);
        }
    }
}
