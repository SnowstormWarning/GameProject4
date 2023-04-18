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

namespace GameProject4
{
    public class Game1 : Game
    {
        public static Vector2 GlobalScalingFactor;
        public static Microsoft.Xna.Framework.Graphics.SpriteFont Font;

        public TileMap TileMap;
        public static DirectionalStaticSprite Player;
        public DirectionalStaticSprite[] Reds = new DirectionalStaticSprite[4];
        public List<int> RedsIndexes = new List<int>();
        public List<int> VisionIndexes = new List<int>();
        public StaticSprite Exit;

        private GraphicsDeviceManager _graphics;
        private Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;
        private KeyboardState _prevKey;
        private int _playerTileIndex = 42;
        private StaticSprite _vision;
        private int _exitIndex = 458;

        public static Vector2 WorldOffset = Vector2.Zero;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GlobalScalingFactor = Vector2.One;
            // TODO: Add your initialization logic here
            TileMap = new TileMap("Map.txt");
            _vision = new StaticSprite("Vision");
            Player = new DirectionalStaticSprite("BlueUp", "BlueLeft");
            Exit = new StaticSprite("Exit");
            Player.Scale = 2f;
            Player.Position = new Vector2(GraphicsDevice.Viewport.Width/2-32,GraphicsDevice.Viewport.Height/2-32);
            Player.Direction = Directions.Right;
            for(int i = 0; i < Reds.Length; i++)
            {
                Reds[i] = new DirectionalStaticSprite("RedUp", "RedLeft");
                Reds[i].Scale = 2f;
            }
            Reds[0].Position = new Vector2(576, 512);
            Reds[1].Position = new Vector2(1088, 896);
            Reds[2].Position = new Vector2(704, 1344);
            Reds[3].Position = new Vector2(64, 1024);
            RedsIndexes.Add(169);
            RedsIndexes.Add(297);
            RedsIndexes.Add(321);
            RedsIndexes.Add(431);
            WorldOffset = PosTool.VectorSubtraction(-Player.Position,new Vector2(-128,-128));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _vision.LoadContent(Content);
            TileMap.LoadContent(Content);
            Player.LoadContent(Content);
            Exit.LoadContent(Content);
            foreach(DirectionalStaticSprite red in Reds)
            {
                red.LoadContent(Content);
            }
        }

        public bool PlaceVision(int index)
        {
            if (!TileMap.IsWall(index))
            {
                VisionIndexes.Add(index);
                return true;
            }
            return false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.D) && !_prevKey.IsKeyDown(Keys.D))
            {
                if (!TileMap.IsWall(_playerTileIndex + 1) && !RedsIndexes.Contains(_playerTileIndex + 1))
                {
                    _playerTileIndex += 1;
                    Player.Direction = Directions.Right;
                    WorldOffset += new Vector2(64, 0);
                    UpdateReds();
                    CheckFailure();
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A) && !_prevKey.IsKeyDown(Keys.A))
            {
                if(!TileMap.IsWall(_playerTileIndex - 1) && !RedsIndexes.Contains(_playerTileIndex - 1))
                {
                    _playerTileIndex -= 1;
                    Player.Direction = Directions.Left;
                    WorldOffset += new Vector2(-64, 0);
                    UpdateReds();
                    CheckFailure();
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W) && !_prevKey.IsKeyDown(Keys.W))
            {
                if(!TileMap.IsWall(_playerTileIndex - 20) && !RedsIndexes.Contains(_playerTileIndex - 20))
                {
                    _playerTileIndex -= 20;
                    Player.Direction = Directions.Up;
                    WorldOffset += new Vector2(0, -64);
                    UpdateReds();
                    CheckFailure();
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S) && !_prevKey.IsKeyDown(Keys.S))
            {
                if(!TileMap.IsWall(_playerTileIndex + 20) && !RedsIndexes.Contains(_playerTileIndex + 20))
                {
                    _playerTileIndex += 20;
                    Player.Direction = Directions.Down;
                    WorldOffset += new Vector2(0, 64);
                    UpdateReds();
                    CheckFailure();
                }
            }
            if (_playerTileIndex == _exitIndex) Exit();
            // TODO: Add your update logic here
            _prevKey = Keyboard.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            TileMap.Draw(gameTime,_spriteBatch);
            Exit.DrawWithWorldOffset(gameTime, _spriteBatch, new Vector2((_exitIndex % 20) * 64, (_exitIndex / 20) * 64));
            Player.Draw(gameTime, _spriteBatch);
            foreach(DirectionalStaticSprite red in Reds)
            {
                red.DrawWithWorldOffset(gameTime, _spriteBatch);
            }
            foreach(int vision in VisionIndexes)
            {
                _vision.DrawWithWorldOffset(gameTime, _spriteBatch, new Vector2((vision % 20) * 64, (vision / 20) * 64));
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void UpdateReds()
        {
            foreach(DirectionalStaticSprite red in Reds)
            {
                red.RotateClockwise();
            }
            UpdateVision();
        }

        public void UpdateVision()
        {
            VisionIndexes.Clear();
            for (int i = 0; i < Reds.Length; i++)
            {
                int layerDiff = 0;
                int horiDiff = 0;
                switch (Reds[i].Direction)
                {
                    case Directions.Up:
                        layerDiff = -20;
                        horiDiff = 1;
                        break;
                    case Directions.Down:
                        layerDiff = 20;
                        horiDiff = -1;
                        break;
                    case Directions.Left:
                        layerDiff = -1;
                        horiDiff = -20;
                        break;
                    case Directions.Right:
                        layerDiff = 1;
                        horiDiff = 20;
                        break;
                }
                if (PlaceVision(RedsIndexes[i] + layerDiff))
                {
                    if (PlaceVision(RedsIndexes[i] + 2 * layerDiff))
                    {
                        PlaceVision(RedsIndexes[i] + 3 * layerDiff);
                    }
                }
                if (PlaceVision(RedsIndexes[i] + layerDiff + horiDiff))
                {
                    if (PlaceVision(RedsIndexes[i] + 2 * layerDiff + horiDiff))
                    {
                        PlaceVision(RedsIndexes[i] + 3 * layerDiff + horiDiff);
                    }
                }
                if (PlaceVision(RedsIndexes[i] + layerDiff - horiDiff))
                {
                    if (PlaceVision(RedsIndexes[i] + 2 * layerDiff - horiDiff))
                    {
                        PlaceVision(RedsIndexes[i] + 3 * layerDiff - horiDiff);
                    }
                }
            }
        }

        public void CheckFailure()
        {
            if (VisionIndexes.Contains(_playerTileIndex))
            {
                _playerTileIndex = 42;
                WorldOffset = PosTool.VectorSubtraction(-Player.Position, new Vector2(-128, -128));
                foreach(DirectionalStaticSprite red in Reds)
                {
                    red.Direction = Directions.Up;
                }
            }
        }
    }
}