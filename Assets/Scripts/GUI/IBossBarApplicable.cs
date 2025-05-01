using UnityEngine;

public interface IBossBarApplicable
{
    public Color PrimaryColor { get; }
    public Color SecondaryColor { get; }
    public string BossTitle { get; }
    public GameEntity Self { get; }
}
