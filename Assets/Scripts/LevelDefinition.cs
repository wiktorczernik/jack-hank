using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Definition", fileName = "New Definition")]
public class LevelDefinition : ScriptableObject
{
    [SerializeField] private int levelID;
    [SerializeField] private SceneAsset usedScene;
    [SerializeField] private LevelDefinition[] lastLevels;

    public int LevelID => levelID;
    public LevelDefinition[] LastLevels => lastLevels.Clone() as LevelDefinition[];
    public SceneAsset UsedScene => usedScene;
}