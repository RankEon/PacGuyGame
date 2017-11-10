using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PacGuyGame
{
    /// <summary>
    /// Handles drawing of graphics during the game(loop).
    /// </summary>
    class Graphics
    {
        GameBoard gameBoard;
        Line line1 = new Line();
        Line line2 = new Line();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gb">Copy of game board -object.</param>
        public Graphics(GameBoard gb)
        {
            gameBoard = gb;
        }

        /// <summary>
        /// Draws creature.
        /// </summary>
        /// <param name="character">Copy of creature</param>
        /// <param name="coordinates">Creature coordinates.</param>
        public void Draw(Creature character, Point coordinates)
        {
            try
            {
                Point coord = coordinates;
                Creature crea = character;

                gameBoard.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Rectangle creature = crea.GetCharacter();

                    if (gameBoard.gameAnimationCanvas.Children.Contains(creature))
                    {
                        gameBoard.gameAnimationCanvas.Children.Remove(creature);
                    }

                    Canvas.SetLeft(creature, coordinates.X);
                    Canvas.SetTop(creature, coordinates.Y);
                    gameBoard.gameAnimationCanvas.Children.Add(creature);
                }), DispatcherPriority.Normal, null);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception during draw:\n" + e.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Draws Hero.
        /// </summary>
        /// <param name="character">Copy of Hero.</param>
        /// <param name="coordinates">Hero coordinates.</param>
        public void Draw(Hero character, Point coordinates)
        {
            try
            {               
                Point coord = coordinates;
                Hero crea = character;

                gameBoard.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Rectangle creature = crea.GetCharacter();

                    if (gameBoard.gameAnimationCanvas.Children.Contains(creature))
                    {
                        gameBoard.gameAnimationCanvas.Children.Remove(creature);
                    }
                    else if(gameBoard.gameAnimationCanvas.Children.Contains(crea.GetPreviousFrame()))
                    {
                        gameBoard.gameAnimationCanvas.Children.Remove(crea.GetPreviousFrame());
                    }

                    crea.SetCreatureOrientation();

                    Canvas.SetLeft(creature, coordinates.X);
                    Canvas.SetTop(creature, coordinates.Y);
                    gameBoard.gameAnimationCanvas.Children.Add(creature);


                }), DispatcherPriority.Normal, null);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception during draw:\n" + e.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates hero's lives to the UI.
        /// </summary>
        /// <param name="lives">Number of lives</param>
        public void UpdateHeroLives(int lives)
        {
            try
            {
                gameBoard.Dispatcher.BeginInvoke((Action)(() =>
                {
                    gameBoard.lblHeroLives.Content = Convert.ToString(lives) + " UP";
                }), DispatcherPriority.Normal, null);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception during draw:\n" + e.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Removes the "X" over hero (when collided with a foe).
        /// </summary>
        public void RemoveXCrossOverHero()
        {
            try
            {
                gameBoard.Dispatcher.BeginInvoke((Action)(() =>
                {
                    gameBoard.gameAnimationCanvas.Children.Remove(line1);
                    gameBoard.gameAnimationCanvas.Children.Remove(line2);
                }), DispatcherPriority.Normal, null);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception during draw:\n" + e.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Draws "X" over hero when collided with a foe.
        /// </summary>
        /// <param name="coordinates">Coordinates where "X" is drawn.</param>
        public void DrawXCrossOverHero(Point coordinates)
        {
            try
            {
                gameBoard.Dispatcher.BeginInvoke((Action)(() =>
                {
                    line1.Stroke = Brushes.DarkRed;
                    line2.Stroke = Brushes.DarkRed;

                    line1.StrokeThickness = 8;
                    line2.StrokeThickness = 8;

                    line1.X1 = coordinates.X;
                    line1.X2 = coordinates.X + 50;
                    line1.Y1 = coordinates.Y;
                    line1.Y2 = coordinates.Y + 50;

                    line2.X1 = coordinates.X + 50;
                    line2.X2 = coordinates.X;
                    line2.Y1 = coordinates.Y;
                    line2.Y2 = coordinates.Y + 50;

                    gameBoard.gameAnimationCanvas.Children.Add(line1);
                    gameBoard.gameAnimationCanvas.Children.Add(line2);
                }), DispatcherPriority.Normal, null);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception during draw:\n" + e.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Shows message overlay in the UI.
        /// </summary>
        /// <param name="text">Text to show.</param>
        public void ShowMessageOverlay(string text)
        {            
            try
            {
                gameBoard.Dispatcher.BeginInvoke((Action)(() =>
                {
                    gameBoard.lblMessageOverlay.VerticalAlignment = VerticalAlignment.Top;
                    gameBoard.lblMessageOverlay.Content = text;
                    gameBoard.lblMessageOverlay.Visibility = Visibility.Visible;
                }), DispatcherPriority.Normal, null);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception during draw:\n" + e.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Hides the message overlay from the UI
        /// </summary>
        public void HideMessageOverlay()
        {
            try
            {
                gameBoard.Dispatcher.BeginInvoke((Action)(() =>
                {
                    gameBoard.lblMessageOverlay.Visibility = Visibility.Hidden;
                }), DispatcherPriority.Normal, null);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception during draw:\n" + e.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Checks whether the Hero collides with the foe.
        /// </summary>
        /// <param name="hero">Copy of the Hero</param>
        /// <param name="foes">Copy of the list of foes</param>
        /// <returns></returns>
        public bool DoesHeroCollideWithFoe(Hero hero, List<Creature> foes)
        {
            Point heroCoords = hero.GetCoordinates();

            foreach (Creature foe in foes)
            {
                Point foeCoords = foe.GetCoordinates();

                double x = Math.Abs(heroCoords.X - foeCoords.X);
                double y = Math.Abs(heroCoords.Y - foeCoords.Y);

                if (x <= 51 && y <= 51)
                {
                    if ((heroCoords.X <= foeCoords.X + 50) ||
                       (heroCoords.X + 50 >= foeCoords.X) ||
                       (heroCoords.Y <= foeCoords.Y + 50) ||
                       (heroCoords.Y + 50 >= foeCoords.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether the Hero collides with the tablet (eats the tablet) and
        /// updates score.
        /// </summary>
        /// <param name="pacman">Copy of the Hero.</param>
        /// <returns>
        ///    TRUE = Hero collided with a tablet.
        ///    FALSE = Hero didn't collide with a tablet.
        /// </returns>
        public bool DoesHeroCollideWithTablet(Hero pacman)
        {
            CharacterGameBoardLocation heroLocation = pacman.GetCharacterGameBoardLocation();

            if(gameBoard.IsCurrentCoordTablet(heroLocation.Row, heroLocation.Col))
            {
                gameBoard.Dispatcher.BeginInvoke((Action)(() =>
                {
                    int score;
                    int.TryParse(gameBoard.labelScore.Content.ToString(), out score);
                    gameBoard.labelScore.Content = score + 1;

                    Rectangle tabletToRemove = gameBoard.GetCurrentTablet(heroLocation.Row, heroLocation.Col);
                    gameBoard.gameBoardCanvas.Children.Remove(tabletToRemove);
                    gameBoard.RemoveTablet(heroLocation.Row, heroLocation.Col);
                }), DispatcherPriority.Normal, null);

                return true;
            }
            
            return false;
        }
    }
}
