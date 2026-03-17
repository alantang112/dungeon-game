using DungeonGame.Engine.Models;

namespace DungeonGame.Engine.Tests;

public static class PositionExtensions
{
    public static Position Translate(this Position position, int x, int y)
    {
        return new Position(position.X + x, position.Y + y);
    }
}
