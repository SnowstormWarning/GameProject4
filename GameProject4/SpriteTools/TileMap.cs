using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DirectWrite;
using System.Security.Policy;
using GameProject4;

namespace Tools.Sprites
{
    public class TileMap
    {
        /// <summary>
        /// Dimensions of the tiles and map
        /// </summary>
        int _tileWidth, _tileHeight, _mapWidth, _mapHeight;

        /// <summary>
        /// The tileset texture
        /// </summary>
        Texture2D _tilesetTexture;

        /// <summary>
        /// The info in the tileset
        /// </summary>
        Rectangle[] _tiles;

        /// <summary>
        /// the tile map data
        /// </summary>
        int[] _map;

        /// <summary>
        /// The filename of the map file
        /// </summary>
        string _filename;

        public float Scale = 2.0f;

        public TileMap(string filename)
        {
            _filename = filename;
        }

        public void LoadContent(ContentManager content)
        {
            string data = File.ReadAllText(Path.Join(content.RootDirectory, _filename));
            var lines = data.Split('\n');

            var tilesetFilename = lines[0].Trim();
            _tilesetTexture = content.Load<Texture2D>(tilesetFilename);

            var secondLine = lines[1].Split(',');
            _tileWidth = int.Parse(secondLine[0]);
            _tileHeight = int.Parse(secondLine[1]);

            int tilesetColumns = _tilesetTexture.Width / _tileWidth;
            int tilesetRows = _tilesetTexture.Height / _tileHeight;
            _tiles = new Rectangle[tilesetColumns * tilesetRows];

            for (int y = 0; y < tilesetColumns; y++)
            {
                for (int x = 0; x < tilesetRows; x++)
                {
                    int index = y * tilesetRows + x;
                    _tiles[index] = new Rectangle(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);
                }
            }

            var thirdLine = lines[2].Split(',');
            _mapWidth = int.Parse(thirdLine[0]);
            _mapHeight = int.Parse(thirdLine[1]);

            var fourthLine = lines[3].Split(',');
            _map = new int[_mapWidth * _mapHeight];

            for (int i = 0; i < _mapWidth * _mapHeight; i++)
            {
                _map[i] = int.Parse(fourthLine[i]);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    int index = _map[y * _mapWidth + x] - 1;
                    if (index == -1) continue;
                    spriteBatch.Draw(_tilesetTexture, new Vector2(x * _tileWidth * Scale, y * _tileHeight * Scale) - Game1.WorldOffset, _tiles[index], Color.White, 0f, Vector2.Zero,Scale,SpriteEffects.None,0);
                }
            }
        }

        public bool IsWall(int index)
        {
            return _map[index] == 1;
        }
    }
}
