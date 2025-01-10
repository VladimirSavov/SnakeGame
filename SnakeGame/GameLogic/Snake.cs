namespace SnakeGame.GameLogic;

public class Snake
{
    public Position Head => Body?.First?.Value ?? throw new ArgumentNullException(nameof(Body.First.Value));
    public Position Tail => Body?.Last?.Value ?? throw new ArgumentNullException(nameof(Body.Last.Value));
    public LinkedList<Position> Body = new LinkedList<Position>();
    public Snake()
    {
    }

    public void AddHead(Position pos)
    {
        Body.AddFirst(pos);
    }

    public void RemoveTail()
    {
        Body.RemoveLast();
    }
}

