using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameProject4;
using SharpDX.Direct3D9;

namespace Tool.Sprites
{

    public enum Directions
    {
        Up, Right, Down, Left
    }
    public class DirectionalStaticSprite
    {
        /// <summary>
        /// Sprite's texture
        /// </summary>
        private Texture2D _textureUp;
        /// <summary>
        /// Sprite's texture
        /// </summary>
        private Texture2D _textureLeft;
        /// <summary>
        /// Position of Sprite
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// Scale of the Sprite's Rendering
        /// </summary>
        public float Scale = 1;
        /// <summary>
        /// The string for loading the static sprite's texture.
        /// </summary>
        public string TextureNameUp;
        /// <summary>
        /// The string for loading the static sprite's texture.
        /// </summary>
        public string TextureNameLeft;

        public Directions Direction = Directions.Up;

        public DirectionalStaticSprite(string textureUp, string textureLeft)
        {
            TextureNameUp = textureUp;
            TextureNameLeft = textureLeft;
        }

        public DirectionalStaticSprite(string textureUp, string textureLeft, Vector2 position, float scale)
        {
            TextureNameUp = textureUp;
            TextureNameLeft = textureLeft;
            Position = position;
            Scale = scale * Game1.GlobalScalingFactor.X;
        }

        /// <summary>
        /// Loads the Sprite's sprite texture
        /// </summary>
        /// <param name="content">The content manager</param>
        public void LoadContent(ContentManager content)
        {
            _textureUp = content.Load<Texture2D>(TextureNameUp);
            _textureLeft = content.Load<Texture2D>(TextureNameLeft);

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Draw(gameTime, spriteBatch, Color.White);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            switch (Direction)
            {
                case Directions.Up:
                    spriteBatch.Draw(_textureUp, Position, null, color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
                    break;
                case Directions.Right:
                    spriteBatch.Draw(_textureLeft, Position, null, color, 0f, Vector2.Zero, Scale, SpriteEffects.FlipHorizontally, 0);
                    break;
                case Directions.Down:
                    spriteBatch.Draw(_textureUp, Position, null, color, 0f, Vector2.Zero, Scale, SpriteEffects.FlipVertically, 0);
                    break;
                case Directions.Left:
                    spriteBatch.Draw(_textureLeft, Position, null, color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
                    break;
            }
        }

        public virtual void DrawWithWorldOffset(GameTime gameTime, SpriteBatch spriteBatch)
        {
            switch (Direction)
            {
                case Directions.Up:
                    spriteBatch.Draw(_textureUp, Position-Game1.WorldOffset, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
                    break;
                case Directions.Right:
                    spriteBatch.Draw(_textureLeft, Position - Game1.WorldOffset, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.FlipHorizontally, 0);
                    break;
                case Directions.Down:
                    spriteBatch.Draw(_textureUp, Position - Game1.WorldOffset, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.FlipVertically, 0);
                    break;
                case Directions.Left:
                    spriteBatch.Draw(_textureLeft, Position - Game1.WorldOffset, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
                    break;
            }
        }

        public void RotateClockwise()
        {
            Direction = (Directions)((((int)Direction) + 1) % 4);
        }
        public void RotateCounterClockwise()
        {
            Direction = (Directions)((((int)Direction) + 1) % 4);
        }
    }
}
