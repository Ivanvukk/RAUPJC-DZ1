using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;

namespace PongV3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()   //constructor
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 900,
                PreferredBackBufferWidth = 500
            };
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Left/Right Wall & Top Bottom Goal
        /// </summary>
        public List<Wall> Walls { get; set; }
        public List<Wall> Goals { get; set; }
        /// <summary>
        /// Bottom paddle object
        /// </summary>
        public Paddle PaddleBottom { get; private set; }

        /// <summary>
        /// Top paddle object
        /// </summary>
        public Paddle PaddleTop { get; private set; }

        /// <summary>
        /// Ball object
        /// </summary>
        public Ball Ball { get; private set; }

        /// <summary>
        /// Background image
        /// </summary>
        public Background Background { get; private set; }

        /// <summary>
        /// Sound when ball hits an obstacle.
        /// SoundEffect is a type defined in Monogame framework
        /// </summary>
        public SoundEffect HitSound { get; private set; }

        /// <summary>
        /// Background music. Song is a type defined in Monogame framework
        /// </summary>
        public Song Music { get; private set; }

        /// <summary>
        /// Generic list that holds Sprites that should be drawn on screen
        /// </summary>
        private IGenericList<Sprite> SpritesForDrawList = new GenericList<Sprite>();

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Screen bounds details. Use this information to set up game objects positions.
            var screenBounds = GraphicsDevice.Viewport.Bounds;

            PaddleBottom = new Paddle(GameConstants.PaddleDefaultWidth, GameConstants.PaddleDefaulHeight, GameConstants.PaddleDefaulSpeed);
            PaddleBottom.X = 150;
            PaddleBottom.Y = 880;

            PaddleTop = new Paddle(GameConstants.PaddleDefaultWidth, GameConstants.PaddleDefaulHeight, GameConstants.PaddleDefaulSpeed);
            PaddleTop.X = 150;
            PaddleTop.Y = 0;

            Ball = new Ball(GameConstants.DefaultBallSize, GameConstants.DefaultInitialBallSpeed, GameConstants.DefaultBallBumpSpeedIncreaseFactor);
            Ball.X = 230;
            Ball.Y = 430;


            Background = new Background(500, 900);

            // Add our game objects to the sprites that should be drawn collection..
            //you’ll see why in a second 
            SpritesForDrawList.Add(Background);
            SpritesForDrawList.Add(PaddleBottom);
            SpritesForDrawList.Add(PaddleTop);
            SpritesForDrawList.Add(Ball);

            Walls = new List<Wall>()
            {
                new Wall(-50,0, GameConstants.WallDefaultSize,900), 
                new Wall(500,0, GameConstants.WallDefaultSize, 900),
            };

            Goals = new List<Wall>()
            {
                new Wall(0,-50, 500, GameConstants.WallDefaultSize),
                new Wall(0,900, 500, GameConstants.WallDefaultSize),
            };


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Set textures 
            Texture2D paddleTexture = Content.Load<Texture2D>("paddle");
            PaddleBottom.Texture = paddleTexture;
            PaddleTop.Texture = paddleTexture;
            Ball.Texture = Content.Load<Texture2D>("ball");
            Background.Texture = Content.Load<Texture2D>("background");

            // Load sounds
            // Start background music
            HitSound = Content.Load<SoundEffect>("hit");
            Music = Content.Load<Song>("music");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Music);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var touchState = Keyboard.GetState();

            //Paddle 
            if (touchState.IsKeyDown(Keys.Left) && PaddleBottom.X > 0)
            {
                PaddleBottom.X = PaddleBottom.X - (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
            }
            if (touchState.IsKeyDown(Keys.Right) && PaddleBottom.X < 300)
            {
                PaddleBottom.X = PaddleBottom.X + (float)(PaddleBottom.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
            }
            if (touchState.IsKeyDown(Keys.A) && PaddleTop.X > 0)
            {
                PaddleTop.X = PaddleTop.X - (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
            }
            if (touchState.IsKeyDown(Keys.D) && PaddleTop.X < 300)
            {
                PaddleTop.X = PaddleTop.X + (float)(PaddleTop.Speed * gameTime.ElapsedGameTime.TotalMilliseconds);
            }

            //Ball
            var ballPositionChange = Ball.Direction * (float)(gameTime.ElapsedGameTime.TotalMilliseconds * Ball.Speed);

            Ball.X += ballPositionChange.X;
            Ball.Y += ballPositionChange.Y;



            // Ball - side walls
            if (Walls.Any(w => CollisionDetector.Overlaps(Ball, w)))
            {
                Ball.Direction = new Vector2(-Ball.Direction.X, Ball.Direction.Y);
                Ball.Speed = Ball.Speed * Ball.BumpSpeedIncreaseFactor;
            }
            // Ball - winning walls
            if (Goals.Any(w => CollisionDetector.Overlaps(Ball, w)))
            {
                Ball.X = 230;
                Ball.Y = 430;
                Ball.Speed = GameConstants.DefaultInitialBallSpeed;
                Ball.Direction = new Vector2(Ball.Direction.X, -Ball.Direction.Y);
                Ball.Speed *= Ball.BumpSpeedIncreaseFactor;
                HitSound.Play();
            }
            // Paddle - ball collision
            if (CollisionDetector.Overlaps(Ball, PaddleTop) && Ball.Direction.Y < 0 || (CollisionDetector.Overlaps(Ball, PaddleBottom) && Ball.Direction.Y > 0))
            {
                Ball.Direction = new Vector2(Ball.Direction.X, -Ball.Direction.Y);
                Ball.Speed *= Ball.BumpSpeedIncreaseFactor;
            }

            //  Speedspeed max limit
            if (Ball.Speed > GameConstants.DefaultMaxBallSpeed)
                Ball.Speed = GameConstants.DefaultMaxBallSpeed;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            for (int i = 0; i < SpritesForDrawList.Count; i++)
            {
                SpritesForDrawList.GetElement(i).DrawSpriteOnScreen(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public abstract class Sprite : IPhysicalObject2D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        /// <summary>
        /// Represents the texture of the Sprite on the screen.
        /// Texture2D is a type defined in Monogame framework.
        /// </summary>
        public Texture2D Texture { get; set; }

        protected Sprite(int width, int height, float x = 0, float y = 0)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }
        /// <summary>
        /// Base draw method
        /// </summary>
        public virtual void DrawSpriteOnScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Vector2(X, Y), new Rectangle(0, 0, Width, Height), Color.White);
        }
    }

    public interface IPhysicalObject2D
    {
        float X { get; set; }
        float Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }

    public class CollisionDetector
    {
        public static bool Overlaps(IPhysicalObject2D a, IPhysicalObject2D b)
        {
            if (a.X >= b.X && a.X <= b.X + b.Width && a.Y >= b.Y && a.Y <= b.Y + b.Height ||
                a.X + a.Width >= b.X && a.X + a.Width <= b.X + b.Width && a.Y >= b.Y && a.Y <= b.Y + b.Height ||
                a.X + a.Width >= b.X && a.X + a.Width <= b.X + b.Width && a.Y + a.Height >= b.Y && a.Y + a.Height <= b.Y + b.Height ||
                a.X >= b.X && a.X <= b.X + b.Width && a.Y + a.Height >= b.Y && a.Y + a.Height <=b.Y + b.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Game background representation
    /// </summary>
        public class Background : Sprite
    {
        public Background(int width, int height) : base(width, height)
        {

        }
    }

    public class Wall : IPhysicalObject2D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Wall(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    /// <summary>
    /// Game ball object representation
    /// </summary>
    public class Ball : Sprite
    {
        /// <summary>
        /// Defines current ball speed in time.
        /// </summary>
        public float Speed { get; set; }
        public float BumpSpeedIncreaseFactor { get; set; }
        /// <summary>
        /// Defines ball direction.
        /// Valid values (-1,-1), (1,1), (1,-1), (-1,1).
        /// Using Vector2 to simplify game calculation. Potentially
        /// dangerous because vector 2 can swallow other values as well.
        /// OPTIONAL TODO: create your own, more suitable type
        /// </summary>
        public Vector2 Direction { get; set; }

        public Ball(int size, float speed, float defaultBallBumpSpeedIncreaseFactor) : base(size, size)
        {
            Speed = speed;
            BumpSpeedIncreaseFactor = defaultBallBumpSpeedIncreaseFactor;
            // Initial direction
            Direction = new Vector2(1, 1);
        }
    }

    public class Paddle : Sprite
    {
        /// <summary>
        /// Current paddle speed in time
        /// </summary>
        public float Speed { get; set; }

        public Paddle(int width, int height, float initialSpeed) : base(width, height)
        {
            Speed = initialSpeed;
        }
        /// <summary>
        /// Overriding draw method. Masking paddle texture with black color.
        /// </summary>
        public override void DrawSpriteOnScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Vector2(X, Y), new Rectangle(0, 0, Width, Height), Color.GhostWhite);
        }
    }

    public class GameConstants
    {
        public const float PaddleDefaulSpeed = 0.9f;
        public const int PaddleDefaultWidth = 200;
        public const int PaddleDefaulHeight = 20;
        public const float DefaultInitialBallSpeed = 0.4f;
        public const float DefaultMaxBallSpeed = 1.0f;
        public const float DefaultBallBumpSpeedIncreaseFactor = 1.04f;
        public const int DefaultBallSize = 40;
        public const int WallDefaultSize = 50;
    }
}
