using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PacGuyGame
{
    /// <summary>
    /// Creature Class.
    /// </summary>
    class Creature
    {
        protected const int SpriteCoordOffset = 0; // points

        protected string creatureSpritePath;
        protected Point coordinates = new Point(50, 50);
        protected Point startCoordinates = new Point(50, 50);
        protected int creatureDirection = (int) Directions.RIGHT;
        protected bool allowDirectionChange = false;
        protected Random rand;

        protected Rectangle creatureRectangle = new Rectangle();
        protected BitmapImage creatureBitmap = new BitmapImage();
        protected char?[,] gameBoardLayout;
        protected int characterType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spritePath">Path to the creature sprite</param>
        /// <param name="boardLayout">Copy of game board layout</param>
        /// <param name="type">Creature type</param>
        /// <param name="initialCoordinates">Initial coordinates of the creature</param>
        public Creature(string spritePath, char?[,] boardLayout, CharacterType type, Point initialCoordinates)
        {
            creatureSpritePath = spritePath;
            gameBoardLayout = boardLayout;
            characterType = (int) type;
            coordinates = initialCoordinates;
            startCoordinates = initialCoordinates;
            rand = new Random();
            InitCreatureBitmap();
        }

        /// <summary>
        /// Get creature sprite.
        /// </summary>
        /// <returns>Creature rectangle (sprite)</returns>
        public virtual Rectangle GetCharacter()
        {
            return creatureRectangle;
        }

        /// <summary>
        /// Get creature coordinates.
        /// </summary>
        /// <returns>Coordinates</returns>
        public Point GetCoordinates()
        {
            return coordinates;
        }

        /// <summary>
        /// Get creature type.
        /// </summary>
        /// <returns>Creature type</returns>
        public int GetCharacterType()
        {
            return characterType;
        }

        /// <summary>
        /// Resets the position of the creature to the initial
        /// coordinates.
        /// </summary>
        public void ResetCharacterPosition()
        {
            coordinates = startCoordinates;
        }

        /// <summary>
        /// Handles creature movement.
        /// </summary>
        public virtual void Move()
        {
            CharacterPossibleDirections directions = GetPossibleDirections();

            // Remove illegal moves, i.e. if the creature is going right, do not allow
            // move to left, if there is still possibility to continue. This is to avoid
            // "ping-pong" effect.
            if (1 < AmoutOfPossibleDirections(directions) && allowDirectionChange == true)
            {
                if (creatureDirection == (int)Directions.RIGHT && directions.Left == (int)Directions.LEFT)
                {
                    directions.Left = (int)Directions.NA;
                }
                else if (creatureDirection == (int)Directions.LEFT && directions.Right == (int)Directions.RIGHT)
                {
                    directions.Right = (int)Directions.NA;
                }
                else if (creatureDirection == (int)Directions.UP && directions.Down == (int)Directions.DOWN)
                {
                    directions.Down = (int)Directions.NA;
                }
                else if (creatureDirection == (int)Directions.DOWN && directions.Up == (int)Directions.UP)
                {
                    directions.Up = (int)Directions.NA;
                }
            }

            // If we can move, move the creature and if direction change is 
            // allowed, randomize new direction.
            if (IsCurrentDirectionPossible(creatureDirection, directions))
            {
                if(allowDirectionChange == true)
                {
                    int direction = rand.Next(1, 5);

                    while (!IsCurrentDirectionPossible(direction, directions))
                    {
                        direction = rand.Next(1, 5);
                    }

                    creatureDirection = direction;
                }

                switch (creatureDirection)
                {
                    case (int) Directions.LEFT:
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
            else
            {
                if (allowDirectionChange == true)
                {
                    int direction = rand.Next(1, 5);

                    while (!IsCurrentDirectionPossible(direction, directions))
                    {
                        direction = rand.Next(1, 5);
                    }

                    creatureDirection = direction;
                }

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
        /// Move creature in X -plane (DEPRECATED).
        /// </summary>
        /// <param name="x">amount of pixels to move.</param>
        public void MoveX(int x)
        {
            coordinates.X += x;
        }

        /// <summary>
        /// Get the amount of possible directions.
        /// </summary>
        /// <param name="directions">Direction table</param>
        /// <returns>Possible directions</returns>
        protected int AmoutOfPossibleDirections(CharacterPossibleDirections directions)
        {
            int count = 0;

            if (directions.Down != 0) count++;
            if (directions.Up != 0) count++;
            if (directions.Left != 0) count++;
            if (directions.Right != 0) count++;

            return count;
        }

        /// <summary>
        /// Checks whether current direction is possible
        /// </summary>
        /// <param name="direction">Direction</param>
        /// <param name="possibleDirections">Possible directions</param>
        /// <returns>
        ///    TRUE = Direction possible.
        ///    FALSE = Direction is not possible.
        /// </returns>
        protected bool IsCurrentDirectionPossible(int direction, CharacterPossibleDirections possibleDirections)
        {
            if(direction == (int) possibleDirections.Left)
            {
                return true;
            }
            else if (direction == (int)possibleDirections.Right)
            {
                return true;
            }
            else if (direction == (int)possibleDirections.Down)
            {
                return true;
            }
            else if (direction == (int)possibleDirections.Up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the possible directions, based on the game board layout.
        /// </summary>
        /// <returns>Possible directions.</returns>
        protected CharacterPossibleDirections GetPossibleDirections()
        {
            CharacterPossibleDirections direction = new CharacterPossibleDirections();
            CharacterGameBoardLocation charLocation = GetCharacterGameBoardLocation();

            if(!gameBoardLayout[charLocation.Row, charLocation.Col + 1].Equals('W'))
            {
                direction.Right = (int) Directions.RIGHT;
            }
            if (!gameBoardLayout[charLocation.Row, charLocation.Col - 1].Equals('W'))
            {
                direction.Left = (int)Directions.LEFT;
            }
            if (!gameBoardLayout[charLocation.Row + 1, charLocation.Col].Equals('W'))
            {
                direction.Down = (int)Directions.DOWN;
            }
            if (!gameBoardLayout[charLocation.Row - 1, charLocation.Col].Equals('W'))
            {
                direction.Up = (int)Directions.UP;
            }

            return direction;
        }

        /// <summary>
        /// Get creature game board location coordinates.
        /// </summary>
        /// <returns>Creature location.</returns>
        public CharacterGameBoardLocation GetCharacterGameBoardLocation()
        {
            CharacterGameBoardLocation charLocation = new CharacterGameBoardLocation();

            double row, col, rowRounded, colRounded;

            col = coordinates.X / 50;
            row = coordinates.Y / 50;

            // Direction change is allowed if we are in even grid (with respect to
            // the gameBoard table v.s. canvas coordinates).
            if(col % 1 == 0 && row % 1 == 0)
            {
                allowDirectionChange = true;
            }
            else
            {
                allowDirectionChange = false;
            }

            // If character is going UP or LEFT, we ceil the canvas coordinates, otherwise we will
            // floor the coordinates. This is to align the canvas coordinates (pixels) with the
            // 50 x 50 game board grid blocks.
            if(creatureDirection == (int) Directions.UP || creatureDirection == (int) Directions.LEFT)
            {
                rowRounded = Math.Ceiling(row);
                colRounded = Math.Ceiling(col);
            }
            else
            {
                rowRounded = Math.Floor(row);
                colRounded = Math.Floor(col);
            }

            charLocation.Row = (int)rowRounded;
            charLocation.Col = (int)colRounded;

            return charLocation;
        }

        /// <summary>
        /// Initializes the creature bitmap
        /// </summary>
        private void InitCreatureBitmap()
        {
            if(characterType == (int) CharacterType.PlayerControlled)
            {
                return;
            }

            // Load bitmap
            creatureBitmap.BeginInit();
            creatureBitmap.UriSource = new Uri(creatureSpritePath, UriKind.Relative);
            creatureBitmap.EndInit();

            // Initialize Brush
            ImageBrush creatureBrush = new ImageBrush();
            creatureBrush.ImageSource = creatureBitmap;
            creatureBrush.Stretch = Stretch.Fill;

            // Fill a Rectange with the creature image and stretch.
            creatureRectangle = new Rectangle();
            creatureRectangle.Width = 50;
            creatureRectangle.Height = 50;
            creatureRectangle.Fill = creatureBrush;
        }
    }

    /// <summary>
    /// Character game board location property.
    /// </summary>
    public class CharacterGameBoardLocation
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }

    /// <summary>
    /// Possible directions property.
    /// </summary>
    public class CharacterPossibleDirections
    {
        public int Left { get; set; } = 0;
        public int Right { get; set; } = 0;
        public int Up { get; set; } = 0;
        public int Down { get; set; } = 0;
    }
}
