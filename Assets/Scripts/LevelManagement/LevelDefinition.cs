using UnityEngine;

namespace LevelManagement
{
    [CreateAssetMenu(menuName = "Level Definition", fileName = "New Definition")]
    public class LevelDefinition : ScriptableObject
    {
        [SerializeField] private int levelID;
        [SerializeField] private string sceneName;
        [SerializeField] private LevelDefinition[] lastLevels;

        public int LevelID => levelID;
        public LevelDefinition[] LastLevels => lastLevels.Clone() as LevelDefinition[];
        public string SceneName => sceneName;
    }
}
