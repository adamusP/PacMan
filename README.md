# PacMan

A simple Pac-Man-style game written in C# with Windows Forms. The player moves through a maze, collects pellets, avoids ghosts, and tries to get the highest score before losing all lives.

## Features

- Classic maze gameplay with pellets, power pellets, ghosts, score, and lives.
- Keyboard control with buffered turns, so Pac-Man can remember the next direction before reaching a corner.
- Four ghosts that chase the player using a breadth-first search pathfinding algorithm mixed with random movement.
- Power-pellet mode where Pac-Man moves faster, ghosts slow down, run away from the player, and can be eaten for bonus points.
- Side tunnel teleporting between the left and right edges of the maze.
- External map and sprite resources in `cshra/Resources`.

## Requirements

- Windows
- .NET 8 SDK

The project targets `net8.0-windows7.0` and uses Windows Forms, so it is intended to run on Windows.

## Installation

1. Clone the repository:

   ```powershell
   git clone <repository-url>
   cd PacMan
   ```

2. Restore the project:

   ```powershell
   dotnet restore cshra/cshra.sln
   ```

3. Build it:

   ```powershell
   dotnet build cshra/cshra.sln
   ```

## Running the Game

Run the Windows Forms application with:

```powershell
dotnet run --project cshra/cshra.csproj
```

When the window opens, click **Start Game**.

## Scoring

- Small pellet: 10 points
- Large pellet: 50 points
- First ghost in one reverse mode: 200 points
- Each additional ghost in the same reverse mode doubles the ghost score: 400, 800, 1600

## Ghost Movement

The ghosts do not copy the original Pac-Man ghost personalities. Each ghost uses the same movement logic:

- At intersections or turns, it sometimes chooses a random valid direction.
- Otherwise, it runs a breadth-first search through the maze and chooses the first step on the shortest path to Pac-Man.
- In reverse mode, the same search is used, but the ghost chooses the opposite direction so it runs away from the player.

This creates movement that is partly unpredictable while still making the ghosts actively pressure the player.

## Project Structure

```text
cshra/
  Form1.cs               Main form, input handling, and game loop timer
  Form1.Designer.cs      Windows Forms layout
  Game.cs                Map, player, ghost, movement, collision, and drawing logic
  Program.cs             Application entry point
  Resources/
    icon.ico             Window icon
    icons.png            Sprite sheet
    map.txt              Maze layout
```

Generated build folders, local user files, and exported documentation files are ignored by Git.
