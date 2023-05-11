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
using SharpDX.Direct3D11;
using Microsoft.Xna.Framework.Audio;

namespace Tools.StateManagement
{
    public enum LevelState
    {
        AwaitingInput,
        Moving,
        FadingIn
    }
    public class Level: State
    {
        public TileMap TileMap;
        public DirectionalStaticSprite Player;
        public DirectionalStaticSprite[] Reds = new DirectionalStaticSprite[4];
        public List<int> RedsIndexes = new List<int>();
        public List<int> VisionIndexes = new List<int>();
        public List<int> DarknessRevealed = new List<int>();
        public StaticSprite Exit;
        public bool DarknessMod = false;
        public StaticSprite Dark = new StaticSprite("Dark");

        public static bool GodMode = false;

        private KeyboardState _prevKey;
        private int _playerTileIndex = 42;
        private StaticSprite _vision;
        private int _exitIndex;
        public Level NextLevel;
        private StateCommands _command;
        private LevelState LevelState = LevelState.FadingIn;
        private double MovingDuration = 200;
        private double FadingInDuration = 1000;
        private double MovingElapsed = 0;
        private Vector2 MovingDifference;
        private Vector2 InitWorldOffsetMoving;
        private SoundEffect _move;
        private SoundEffect _fail;
        private int respawnIndex;
        private Microsoft.Xna.Framework.Graphics.Texture2D _black;

        public Vector2 WorldOffset = Vector2.Zero;

        public Level(string tileMapName, ContentManager content)
        {
            Construct(tileMapName, content);
        }

        public Level(string tileMapName, ContentManager content, Level target)
        {
            NextLevel = target;
            Construct(tileMapName, content);
        }

        public void Construct(string tileMapName, ContentManager content)
        {
            //GlobalScalingFactor = Vector2.One;
            // TODO: Add your initialization logic here
            TileMap = new TileMap(tileMapName);
            _vision = new StaticSprite("Vision");
            Player = new DirectionalStaticSprite("BlueUp", "BlueLeft");
            Exit = new StaticSprite("Exit");
            _black = content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Black");
            Player.Scale = 2f;
            Player.Position = new Vector2(800 / 2 - 32, 480 / 2 - 32);
            Player.Direction = Directions.Right;
            for (int i = 0; i < Reds.Length; i++)
            {
                Reds[i] = new DirectionalStaticSprite("RedUp", "RedLeft");
                Reds[i].Scale = 2f;
            }
            TileMap.LoadContent(content, this, out RedsIndexes, out _exitIndex);
            for (int i = 0; i < 4; i++)
            {
                Reds[i].Position = 64 * new Vector2(RedsIndexes[i] % TileMap.MapWidth, RedsIndexes[i] / TileMap.MapWidth);
            }
            _playerTileIndex = 2 * TileMap.MapWidth + 2;
            respawnIndex = _playerTileIndex;
            WorldOffset = PosTool.VectorSubtraction(-Player.Position, new Vector2(-128, -128));
        }

        public override void LoadContent(ContentManager content)
        {
            // TODO: use this.Content to load your game content here
            _move = content.Load<SoundEffect>("Move");
            _fail = content.Load<SoundEffect>("Fail");
            _vision.LoadContent(content);
            Player.LoadContent(content);
            Exit.LoadContent(content);
            foreach (DirectionalStaticSprite red in Reds)
            {
                red.LoadContent(content);
            }
            if (DarknessMod)
            {
                Dark.LoadContent(content);
                RevealDarkness(null);
            }
        }

        public void RevealDarkness(Directions? direction)
        {
            if(direction != null)
            {
                switch (direction)
                {
                    case Directions.Up:
                        //ABOVE
                        if(DarknessRevealed.IndexOf(_playerTileIndex - TileMap.MapWidth) == -1) DarknessRevealed.Add(_playerTileIndex - TileMap.MapWidth);
                        //ABOVE RIGHT
                        if (DarknessRevealed.IndexOf(_playerTileIndex - TileMap.MapWidth + 1) == -1) DarknessRevealed.Add(_playerTileIndex - TileMap.MapWidth + 1);
                        //ABOVE LEFT
                        if (DarknessRevealed.IndexOf(_playerTileIndex - TileMap.MapWidth - 1) == -1) DarknessRevealed.Add(_playerTileIndex - TileMap.MapWidth - 1);
                        break;
                    case Directions.Down:
                        //BELOW
                        if (DarknessRevealed.IndexOf(_playerTileIndex + TileMap.MapWidth) == -1) DarknessRevealed.Add(_playerTileIndex + TileMap.MapWidth);
                        //BELOW RIGHT
                        if (DarknessRevealed.IndexOf(_playerTileIndex + TileMap.MapWidth + 1) == -1) DarknessRevealed.Add(_playerTileIndex + TileMap.MapWidth + 1);
                        //BELOW LEFT
                        if (DarknessRevealed.IndexOf(_playerTileIndex + TileMap.MapWidth - 1) == -1) DarknessRevealed.Add(_playerTileIndex + TileMap.MapWidth - 1);
                        break;
                    case Directions.Left:
                        //LEFT
                        if (DarknessRevealed.IndexOf(_playerTileIndex - 1) == -1) DarknessRevealed.Add(_playerTileIndex - 1);
                        //BELOW LEFT
                        if (DarknessRevealed.IndexOf(_playerTileIndex - 1 + TileMap.MapWidth) == -1) DarknessRevealed.Add(_playerTileIndex - 1 + TileMap.MapWidth);
                        //ABOVE LEFT
                        if (DarknessRevealed.IndexOf(_playerTileIndex - 1 - TileMap.MapWidth) == -1) DarknessRevealed.Add(_playerTileIndex - 1 - TileMap.MapWidth);
                        break;
                    case Directions.Right:
                        //RIGHT
                        if (DarknessRevealed.IndexOf(_playerTileIndex + 1) == -1) DarknessRevealed.Add(_playerTileIndex + 1);
                        //BELOW RIGHT
                        if (DarknessRevealed.IndexOf(_playerTileIndex + 1 + TileMap.MapWidth) == -1) DarknessRevealed.Add(_playerTileIndex + 1 + TileMap.MapWidth);
                        //ABOVE RIGHT
                        if (DarknessRevealed.IndexOf(_playerTileIndex + 1 - TileMap.MapWidth) == -1) DarknessRevealed.Add(_playerTileIndex + 1 - TileMap.MapWidth);
                        break;
                }
            }
            else
            {
                //CENTER
                DarknessRevealed.Add(_playerTileIndex);
                //RIGHT
                DarknessRevealed.Add(_playerTileIndex+1);
                //LEFT
                DarknessRevealed.Add(_playerTileIndex - 1);
                //BELOW
                DarknessRevealed.Add(_playerTileIndex + TileMap.MapWidth);
                //ABOVE
                DarknessRevealed.Add(_playerTileIndex - TileMap.MapWidth);
                //BELOW RIGHT
                DarknessRevealed.Add(_playerTileIndex + TileMap.MapWidth + 1);
                //ABOVE RIGHT
                DarknessRevealed.Add(_playerTileIndex - TileMap.MapWidth + 1);
                //BELOW LEFT
                DarknessRevealed.Add(_playerTileIndex + TileMap.MapWidth - 1);
                //ABOVE LEFT
                DarknessRevealed.Add(_playerTileIndex - TileMap.MapWidth - 1);
            }
        }

        public override State GetTargetState()
        {
            return NextLevel;
        }

        public override StateCommands GetStateCommand()
        {
            return _command;
        }

        public override void Initialize()
        {
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

        public override void Update(GameTime gameTime, MouseState mouse, KeyboardState keys)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _command = StateCommands.Clear;
            if(LevelState == LevelState.AwaitingInput)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D) && !_prevKey.IsKeyDown(Keys.D))
                {
                    if (!TileMap.IsWall(_playerTileIndex + 1) && !RedsIndexes.Contains(_playerTileIndex + 1))
                    {
                        _playerTileIndex += 1;
                        if (DarknessMod) RevealDarkness(Directions.Right);
                        Player.Direction = Directions.Right;
                        MovingDifference = new Vector2(64, 0);
                        InitWorldOffsetMoving = WorldOffset;
                        LevelState = LevelState.Moving;
                        _move.Play();
                        UpdateReds();
                        CheckFailure();
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A) && !_prevKey.IsKeyDown(Keys.A))
                {
                    if (!TileMap.IsWall(_playerTileIndex - 1) && !RedsIndexes.Contains(_playerTileIndex - 1))
                    {
                        _playerTileIndex -= 1;
                        if (DarknessMod) RevealDarkness(Directions.Left);
                        Player.Direction = Directions.Left;
                        MovingDifference = new Vector2(-64, 0);
                        InitWorldOffsetMoving = WorldOffset;
                        LevelState = LevelState.Moving;
                        _move.Play();
                        UpdateReds();
                        CheckFailure();
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W) && !_prevKey.IsKeyDown(Keys.W))
                {
                    if (!TileMap.IsWall(_playerTileIndex - TileMap.MapWidth) && !RedsIndexes.Contains(_playerTileIndex - TileMap.MapWidth))
                    {
                        _playerTileIndex -= TileMap.MapWidth;
                        if (DarknessMod) RevealDarkness(Directions.Up);
                        Player.Direction = Directions.Up;
                        MovingDifference = new Vector2(0, -64);
                        InitWorldOffsetMoving = WorldOffset;
                        LevelState = LevelState.Moving;
                        _move.Play();
                        UpdateReds();
                        CheckFailure();
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S) && !_prevKey.IsKeyDown(Keys.S))
                {
                    if (!TileMap.IsWall(_playerTileIndex + TileMap.MapWidth) && !RedsIndexes.Contains(_playerTileIndex + TileMap.MapWidth))
                    {
                        _playerTileIndex += TileMap.MapWidth;
                        if (DarknessMod) RevealDarkness(Directions.Down);
                        Player.Direction = Directions.Down;
                        MovingDifference = new Vector2(0, 64);
                        InitWorldOffsetMoving = WorldOffset;
                        LevelState = LevelState.Moving;
                        _move.Play();
                        UpdateReds();
                        CheckFailure();
                    }
                }
                if (_playerTileIndex == _exitIndex)
                {
                    if (NextLevel != null)
                    {
                        _command = StateCommands.AddTarget;
                    }
                    else
                    {
                        _command = StateCommands.Clear;
                    }
                }
            }
            else if(LevelState == LevelState.Moving)
            {
                MovingElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
                if(MovingElapsed < MovingDuration)
                {
                    WorldOffset = InitWorldOffsetMoving + (float)(MovingElapsed / MovingDuration) * MovingDifference;
                }
                else
                {
                    MovingElapsed = 0f;
                    LevelState = LevelState.AwaitingInput;
                    WorldOffset = InitWorldOffsetMoving + MovingDifference;
                }
            }
            else if (LevelState == LevelState.FadingIn)
            {
                MovingElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
                if(MovingElapsed >= FadingInDuration)
                {
                    MovingElapsed = 0;
                    LevelState = LevelState.AwaitingInput;
                }
            }
            
                // TODO: Add your update logic here
                _prevKey = Keyboard.GetState();
        }

        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin();
            if (!DarknessMod)
            {
                TileMap.Draw(gameTime, spriteBatch, WorldOffset);
                Exit.DrawWithWorldOffset(gameTime, spriteBatch, new Vector2((_exitIndex % TileMap.MapWidth) * 64, (_exitIndex / TileMap.MapWidth) * 64), WorldOffset);
            }
            else
            {
                TileMap.Draw(gameTime, spriteBatch, WorldOffset,DarknessRevealed,Dark);
                if(DarknessRevealed.Contains(_exitIndex)) Exit.DrawWithWorldOffset(gameTime, spriteBatch, new Vector2((_exitIndex % TileMap.MapWidth) * 64, (_exitIndex / TileMap.MapWidth) * 64), WorldOffset);
            }
            Player.Draw(gameTime, spriteBatch);

            if (!DarknessMod)
            {
                foreach (DirectionalStaticSprite red in Reds)
                {
                    red.DrawWithWorldOffset(gameTime, spriteBatch, WorldOffset);
                }
                foreach (int vision in VisionIndexes)
                {
                    _vision.DrawWithWorldOffset(gameTime, spriteBatch, new Vector2((vision % TileMap.MapWidth) * 64, (vision / TileMap.MapWidth) * 64), WorldOffset);
                }
            }
            else
            {
                for(int i = 0; i < RedsIndexes.Count; i++)
                {
                    if (DarknessRevealed.Contains(RedsIndexes[i])) Reds[i].DrawWithWorldOffset(gameTime, spriteBatch, WorldOffset);
                }
                foreach (int vision in VisionIndexes)
                {
                    if (DarknessRevealed.Contains(vision)) _vision.DrawWithWorldOffset(gameTime, spriteBatch, new Vector2((vision % TileMap.MapWidth) * 64, (vision / TileMap.MapWidth) * 64), WorldOffset);
                }
            }
            if(LevelState == LevelState.FadingIn)
            {
                spriteBatch.Draw(_black, new Rectangle(0, 0, 800, 480), null, Color.White * (1 - (float)(MovingElapsed/FadingInDuration)));
            }
            spriteBatch.End();
        }

        public void UpdateReds()
        {
            foreach (DirectionalStaticSprite red in Reds)
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
                        layerDiff = -TileMap.MapWidth;
                        horiDiff = 1;
                        break;
                    case Directions.Down:
                        layerDiff = TileMap.MapWidth;
                        horiDiff = -1;
                        break;
                    case Directions.Left:
                        layerDiff = -1;
                        horiDiff = -TileMap.MapWidth;
                        break;
                    case Directions.Right:
                        layerDiff = 1;
                        horiDiff = TileMap.MapWidth;
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
            if (VisionIndexes.Contains(_playerTileIndex) && !GodMode)
            {
                _playerTileIndex = respawnIndex;
                WorldOffset = PosTool.VectorSubtraction(-Player.Position, new Vector2(-128, -128));
                MovingElapsed = 0;
                LevelState = LevelState.AwaitingInput;
                foreach (DirectionalStaticSprite red in Reds)
                {
                    red.Direction = Directions.Up;
                }
                /*if (DarknessMod)
                {
                    DarknessRevealed.Clear();
                    RevealDarkness(null);
                }*/
                _fail.Play();
            }
        }
    }
}
