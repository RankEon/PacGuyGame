using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PacGuyGame
{
    /// <summary>
    /// The gameloop class is responsible for handling and pacing the character movements
    /// and updating the canvas.
    /// </summary>
    class GameLoop
    {
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 
        private volatile bool _gameRunning;

        private int _updateSpeed = 9; // ms (avg. 55 fps).
        private GameBoard _gameboard;
        private Graphics _graphicsEngine;
        List<Creature> _creatures;
        Hero _pacman;
        private int numTablets = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gb">A copy of game board</param>
        /// <param name="creatures">A copy of creatures list</param>
        /// <param name="pacman">A copy of the player character</param>
        public GameLoop(GameBoard gb, List<Creature> creatures, Hero pacman)
        {
            _gameboard = gb;
            _creatures = creatures;
            _pacman = pacman;
            _graphicsEngine = new Graphics(_gameboard);
            numTablets = _gameboard.GetNumTablets();
        }

        /// <summary>
        /// Worker thread
        /// </summary>
        public async void DoWork()
        {
            _gameRunning = true;

            // Gameloop
            while (_gameRunning)
            {
                _graphicsEngine.Draw(_creatures[0], _creatures[0].GetCoordinates());
                _creatures[0].Move();
                _graphicsEngine.Draw(_creatures[1], _creatures[1].GetCoordinates());
                _creatures[1].Move();
               
                _graphicsEngine.Draw(_pacman, _pacman.GetCoordinates());
                _pacman.Move();

                // Collision detection
                if(_graphicsEngine.DoesHeroCollideWithFoe(_pacman, _creatures))
                {
                    _graphicsEngine.ShowMessageOverlay("Foe caught you!");
                    _graphicsEngine.DrawXCrossOverHero(_pacman.GetCoordinates());
                    _graphicsEngine.UpdateHeroLives(_pacman.SubtractOneLife());

                    // Small delay to freeze action before reset.
                    await Task.Delay(2500);
                    
                    _graphicsEngine.HideMessageOverlay();
                    _graphicsEngine.RemoveXCrossOverHero();
                    
                    // Reset character positions
                    for (int i = 0; i < _creatures.Count; i++)
                    {
                        _creatures[i].ResetCharacterPosition();
                    }

                    _pacman.ResetCharacterPosition();

                    if(_pacman.GetHeroLives() == 0)
                    {
                        _graphicsEngine.ShowMessageOverlay("GAME OVER ! ! !");
                        _gameRunning = false;
                    }
                }

                // Tablet handling
                if(_graphicsEngine.DoesHeroCollideWithTablet(_pacman))
                {
                    numTablets--;
                }
                
                if(numTablets == 0)
                {
                    _graphicsEngine.ShowMessageOverlay("NICE JOB - YOU WON!");
                    _gameRunning = false;
                }

                // http://stackoverflow.com/questions/5424667/alternatives-to-thread-sleep
                await Task.Delay(_updateSpeed);
            }
        }

        /// <summary>
        /// Method to request thread exit.
        /// </summary>
        public void RequestStop()
        {
            _gameRunning = false;
        }

        /// <summary>
        /// Method to stop thread.
        /// </summary>
        public void Stop()
        {
            _gameRunning = false;
        }

    }
}
