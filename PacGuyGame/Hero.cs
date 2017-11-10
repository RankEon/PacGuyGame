using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PacGuyGame
{
    /// <summary>
    /// Hero -class, derived from the Creature.
    /// </summary>
    class Hero : Creature
    {
        private Rectangle[] heroRectangle;
        private BitmapImage[] heroBitmap;
        private string[] heroSpritePath = new string[2];
        private int currentFrame = 0;
        private int framecounter = 0;
        double rotationAngle = 90;
        private int creatureDirectionPref = 0;
        private int heroLives = 3;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spritePathFrame1">Frame 1 sprite path</param>
        /// <param name="spritePathFrame2">Frame 2 sprite path</param>
        /// <param name="boardLayout">Game board layout</param>
        /// <param name="type">Character type</param>
        /// <param name="initialCoordinates">Initial coordinates</param>
        public Hero(string spritePathFrame1, string spritePathFrame2, char?[,] boardLayout, CharacterType type, Point initialCoordinates)
            : base(spritePathFrame1, boardLayout, type, initialCoordinates)
        {
            heroLives = 3;
            heroSpritePath[0] = spritePathFrame1.ToString();
            heroSpritePath[1] = spritePathFrame2.ToString();
            InitHeroBitmap();
            
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyUpEvent, new KeyEventHandler(KeyEventsHandler), true);
        }

        /// <summary>
        /// Gets character sprite (rectangle).
        /// </summary>
        /// <returns>Character sprite (rectangle)</returns>
        public override Rectangle GetCharacter()
        {
            framecounter++;

            if (framecounter < 20)
            {
                currentFrame = 1;
            }
            else if(framecounter >= 20 && framecounter < 40)
            {
                currentFrame = 0;
            }
            else if(framecounter >= 40)
            {
                framecounter = 0;
            }

            return heroRectangle[currentFrame];
        }

        /// <summary>
        /// Subtracts one life from the Hero lives.
        /// </summary>
        /// <returns>Current lives</returns>
        public int SubtractOneLife()
        {
            heroLives = (heroLives > 0) ? --heroLives : 0;
            return heroLives;
        }

        /// <summary>
        /// Get Hero lives.
        /// </summary>
        /// <returns>Number of lives left</returns>
        public int GetHeroLives()
        {
            return heroLives;
        }

        /// <summary>
        /// Sets Hero orientation.
        /// </summary>
        public void SetCreatureOrientation()
        {
            // Set rotation point
            heroRectangle[0].RenderTransformOrigin = new Point(0.5, 0.5);
            heroRectangle[1].RenderTransformOrigin = new Point(0.5, 0.5);

            // Reset
            heroRectangle[0].RenderTransform = new ScaleTransform(1, 1);
            heroRectangle[1].LayoutTransform = new ScaleTransform(1, 1);
            heroRectangle[0].LayoutTransform = new RotateTransform(0);
            heroRectangle[1].LayoutTransform = new RotateTransform(0);

            switch (creatureDirection)
            {
                case (int)Directions.LEFT:
                    heroRectangle[0].RenderTransform = new ScaleTransform(-1, 1);
                    heroRectangle[1].LayoutTransform = new ScaleTransform(-1, 1);
                    break;
                case (int)Directions.RIGHT:
                    heroRectangle[0].LayoutTransform = new RotateTransform(0);
                    heroRectangle[1].LayoutTransform = new RotateTransform(0);
                    break;
                case (int)Directions.DOWN:
                    heroRectangle[0].LayoutTransform = new RotateTransform(90);
                    heroRectangle[1].LayoutTransform = new RotateTransform(90);
                    break;
                case (int)Directions.UP:
                    heroRectangle[0].LayoutTransform = new RotateTransform(270);
                    heroRectangle[1].LayoutTransform = new RotateTransform(270);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Set Hero direction.
        /// </summary>
        /// <param name="directions">Directions property</param>
        public void SetDirection(Directions directions)
        {
            creatureDirectionPref = (int)directions;
        }

        /// <summary>
        /// Gets the previous frame (to allow removing it from the canvas).
        /// </summary>
        /// <returns>Previous frame</returns>
        public Rectangle GetPreviousFrame()
        {
            return (currentFrame == 1) ? heroRectangle[0] : heroRectangle[1];
        }

        /// <summary>
        /// Handles Hero movement (own handler for the Hero).
        /// </summary>
        public override void Move()
        {
            CharacterPossibleDirections directions = GetPossibleDirections();

            if(creatureDirectionPref != (int) Directions.NA &&
               IsCurrentDirectionPossible(creatureDirectionPref, directions) &&
               allowDirectionChange == true)
            {
                creatureDirection = creatureDirectionPref;
                creatureDirectionPref = (int)Directions.NA;
            }

            if (IsCurrentDirectionPossible(creatureDirection, directions))
            {
                switch (creatureDirection)
                {
                    case (int)Directions.LEFT:
                        coordinates.X -= 2;
                        break;
                    case (int)Directions.RIGHT:
                        coordinates.X += 2;
                        break;
                    case (int)Directions.DOWN:
                        coordinates.Y += 2;
                        break;
                    case (int)Directions.UP:
                        coordinates.Y -= 2;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Hero Key events handler
        /// 
        /// Needed to register class handler for keyevents:
        /// social.msdn.microsoft.com/Forums/vstudio/en-US/cf884a91-c135-447d-b16b-214d2d9e9972/capture-all-keyboard-input-regardless-of-what-control-has-focus?forum=wpf
        /// </summary>
        private void KeyEventsHandler(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Right:
                    SetDirection(Directions.RIGHT);
                    //pacman.Move();
                    break;
                case Key.Left:
                    SetDirection(Directions.LEFT);
                    //pacman.Move();
                    break;
                case Key.Up:
                    SetDirection(Directions.UP);
                    //pacman.Move();
                    break;
                case Key.Down:
                    SetDirection(Directions.DOWN);
                    //pacman.Move();
                    break;
            }
        }

        /// <summary>
        /// Initializes the Hero bitmap.
        /// </summary>
        private void InitHeroBitmap()
        {
            heroBitmap = new BitmapImage[2];
            heroRectangle = new Rectangle[2];
            ImageBrush[] heroBrush = new ImageBrush[2];
   
            for (int i = 0; i < heroSpritePath.Length; i++)
            {
                // Load bitmap
                heroBitmap[i] = new BitmapImage();
                heroBitmap[i].BeginInit();
                heroBitmap[i].UriSource = new Uri(heroSpritePath[i], UriKind.Relative);
                heroBitmap[i].EndInit();

                // Initialize Brush
                heroBrush[i] = new ImageBrush();
                heroBrush[i].ImageSource = heroBitmap[i];
                heroBrush[i].Stretch = Stretch.Fill;

                // Fill a Rectange with the creature image and stretch.
                heroRectangle[i] = new Rectangle();
                heroRectangle[i].Width = 50;
                heroRectangle[i].Height = 50;
                heroRectangle[i].Fill = heroBrush[i];

                // heroRectangle[i].LayoutTransform = new RotateTransform(90);
            }
        }
    }
}
