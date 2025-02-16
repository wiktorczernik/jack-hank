public class LevelInfo
{
    public readonly int LevelID;
    public LevelStatus Status;
    public bool IsOpened;
    
    private readonly LevelInfo[] _lastLevels;

    public LevelInfo[] LastLevels => _lastLevels.Clone() as LevelInfo[];
    

    public LevelInfo(int levelID, LevelStatus status, LevelInfo[] lastLevels, bool isOpened)
    {
        LevelID = levelID;
        Status = status;
        _lastLevels = lastLevels.Clone() as LevelInfo[];
        IsOpened = isOpened;
    }
}

public enum LevelStatus
{
    Passed, Available, Unavailable
}