using System.Collections.Generic;

public class BallSection
{
    public List<Ball> Balls { get; } = new();

    public BallSection()
    {
    }

    public BallSection(IEnumerable<Ball> items)
    {
        Balls.AddRange(items);
    }
}