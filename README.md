# PacGuyGame
A Pacman –style game demo, where the aim of the game is to control the yellow character (Hero) and eat all of the green items (tablets) in the maze and to avoid the ghosts moving around the maze.

The main characteristics: 
- Developed with C# and WPF
- Features two overlaid canvases for generating the maze (lower layer) and the character
  animation / movement (top layer)
- The maze is generated from a text –file, describing the maze / level layout. This allows
  extending the game with multiple levels in the future (and possibly even a level editor).
- The ghosts (foes) are determining the movement pattern / path independently by
  (randomly) deciding the new direction on each crossroad or corner.
- Hero’s animation is done with two GIF –images which are altered every 20th frame to 
  create mouth open / closed animation. The image(s) are placed inside a rectangle to imagebrush and this is then rotated or mirrored, based on the direction.

Controlling the Hero:
- Arrow keys:
  UP    = move up
  DOWN  = move down
  LEFT  = move left
  RIGHT = move right
