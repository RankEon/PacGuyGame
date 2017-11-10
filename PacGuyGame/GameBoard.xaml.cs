using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacGuyGame
{
    // Character types
    public enum CharacterType : int
    {
        PlayerControlled = 1,
        AutonumousFoe = 2
    }

    // Directions
    public enum Directions : int
    {
        NA = 0,
        LEFT = 1,
        RIGHT = 2,
        DOWN = 3,
        UP = 4
    }

    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>    
    public partial class GameBoard : Page
    {
        // Constants
        const int BrickWallWidthHeight = 50;
        const int TabletWidthHeight = 10;
        const int GameBoardLayoutRows = 16;
        const int GameBoardLayoutCols = 25;

        // General
        bool gameRunning = false;

        // Gameboard canvas related variables
        private char?[,] gameBoardLayout = new char?[16, 25];
        private Rectangle[,] gameBoardBrickLayout = new Rectangle[16, 25];
        BitmapImage brickBitmap = new BitmapImage();
        Image brickImage = new Image();
        private int numTablets = 0;

        // Character animation canvas related variables

        // Gameloop is run as backgroundworker.
        //Worker gameLoopWorker = new Worker();
        GameLoop gameLoopThread;
        Thread gameLoop;

        // Game characters
        List<Creature> creatures = new List<Creature>();
        Hero pacman;

        /// <summary>
        /// Constructor of the Gameboard
        /// </summary>
        public GameBoard()
        {
            InitializeComponent();
            lblMessageOverlay.Visibility = Visibility.Hidden;
            InitBrickBitmap();
            LoadGameBoard();
            DrawGameBoard();

            creatures.Add(new Creature(@".\Graphics\Ghost2.gif", gameBoardLayout, CharacterType.AutonumousFoe, new Point(50, 50)));
            creatures.Add(new Creature(@".\Graphics\Ghost1.gif", gameBoardLayout, CharacterType.AutonumousFoe, new Point(200, 50)));

            pacman = new Hero(@".\Graphics\Pac_Frame_1.gif", @".\Graphics\Pac_Frame_2.gif", gameBoardLayout, 
                              CharacterType.PlayerControlled, new Point(150, 700));

            gameLoopThread = new GameLoop(this, creatures, pacman);
            gameLoop = new Thread(gameLoopThread.DoWork);
            gameLoop.SetApartmentState(ApartmentState.STA);

#if DEBUG
            btnTestAnimation.Visibility = Visibility.Visible;
#else
            btnTestAnimation_Click(this, new RoutedEventArgs());
            btnTestAnimation.Visibility = Visibility.Hidden;
#endif

        }

        /// <summary>
        /// Constructs and initially draws the gameboard.
        /// </summary>
        public void DrawGameBoard()
        {
            int topShift = 0;

            for (int row = 0; row < GameBoardLayoutRows; row++)
            {
                int leftShift = 0;

                for (int col = 0; col < GameBoardLayoutCols; col++)
                {
                    // Draw bricks and tablets.
                    if (gameBoardLayout[row, col] == 'W')
                    {
                        // Draw brick wall(s)
                        gameBoardBrickLayout[row, col] = new Rectangle();
                        gameBoardBrickLayout[row, col].Height = BrickWallWidthHeight;
                        gameBoardBrickLayout[row, col].Width = BrickWallWidthHeight;

                        ImageBrush brickBrush = new ImageBrush();
                        brickBrush.ImageSource = brickBitmap;
                        brickBrush.Stretch = Stretch.Fill;
                        gameBoardBrickLayout[row, col].Fill = brickBrush;

                        Canvas.SetLeft(gameBoardBrickLayout[row, col], leftShift);
                        Canvas.SetTop(gameBoardBrickLayout[row, col], topShift);
                        gameBoardCanvas.Children.Add(gameBoardBrickLayout[row, col]);
                    }
                    else
                    {
                        // Draw tablet(s)
                        gameBoardBrickLayout[row, col] = new Rectangle();
                        gameBoardBrickLayout[row, col].Height = TabletWidthHeight;
                        gameBoardBrickLayout[row, col].Width = TabletWidthHeight;

                        SolidColorBrush tabletColor = new SolidColorBrush(Colors.DarkGreen);
                        gameBoardBrickLayout[row, col].Fill = tabletColor;

                        Canvas.SetLeft(gameBoardBrickLayout[row, col], leftShift + 20);
                        Canvas.SetTop(gameBoardBrickLayout[row, col], topShift + 20);
                        gameBoardCanvas.Children.Add(gameBoardBrickLayout[row, col]);

                        numTablets++;
                    }

                    leftShift += BrickWallWidthHeight;
                }

                topShift += BrickWallWidthHeight;
            }
        }

        /// <summary>
        /// Get the number of tablets.
        /// </summary>
        /// <returns>Number of tablets</returns>
        public int GetNumTablets()
        {
            return numTablets;
        }

        /// <summary>
        /// Get the current tablet (from given coordinates)
        /// </summary>
        /// <param name="row">Row coordinate</param>
        /// <param name="col">Column coordinate</param>
        /// <returns>Tablet Rectangle</returns>
        public Rectangle GetCurrentTablet(int row, int col)
        {
            return gameBoardBrickLayout[row, col];
        }

        /// <summary>
        /// Removes the tablet from given coordinates
        /// </summary>
        /// <param name="row">Row coordinate</param>
        /// <param name="col">Column coordinate</param>
        public void RemoveTablet(int row, int col)
        {
            gameBoardBrickLayout[row, col] = null;
        }

        /// <summary>
        /// Checks whether current coordinate contains a tablet.
        /// </summary>
        /// <param name="row">Row coordinate</param>
        /// <param name="col">Column coordinate</param>
        /// <returns>
        ///    TRUE = Tablet found.
        ///    FALSE = Tablet not found.
        /// </returns>
        public bool IsCurrentCoordTablet(int row, int col)
        {
            if(gameBoardBrickLayout[row, col] == null)
            {
                return false;
            }

            Type index = gameBoardBrickLayout[row, col].GetType();

            if (index.Equals(typeof(Rectangle)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initializes the brick bitmap.
        /// </summary>
        private void InitBrickBitmap()
        {
            brickBitmap.BeginInit();
            brickBitmap.UriSource = new Uri(@".\Graphics\Brick.png", UriKind.Relative);
            brickBitmap.EndInit();
        }

        /// <summary>
        /// Loads the game board.
        /// </summary>
        private void LoadGameBoard()
        {
            try
            {
                string levelFile = File.ReadAllText(@".\Levels\Level2.txt");

                int row = 0;

                foreach (string line in levelFile.Split('\n'))
                {
                    char[] characters = line.ToCharArray();

                    for (int col = 0; col < GameBoardLayoutCols; col++)
                    {
                        gameBoardLayout[row, col] = characters[col];
                    }
                    row++;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Exception during gameboard load:\n" + e.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Animation / game test button.
        /// </summary>
        private void btnTestAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (!gameRunning)
            {
                btnTestAnimation.Content = "Stop test";
                gameRunning = true;
                gameLoop.Start();
            }
            else
            {
                gameRunning = false;
                gameLoopThread.RequestStop();
                btnTestAnimation.Content = "Run test";
            }
        }

        /// <summary>
        /// Key event handler.
        /// </summary>
        private void GameBoard_KeyDown(object sender, KeyEventArgs e)
        {
            lock (this)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        pacman.SetDirection(Directions.RIGHT);
                        //pacman.Move();
                        break;
                    case Key.Left:
                        pacman.SetDirection(Directions.LEFT);
                        //pacman.Move();
                        break;
                    case Key.Up:
                        pacman.SetDirection(Directions.UP);
                        //pacman.Move();
                        break;
                    case Key.Down:
                        pacman.SetDirection(Directions.DOWN);
                        //pacman.Move();
                        break;
                }
            }
        }
    }
}
