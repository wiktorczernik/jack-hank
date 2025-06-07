using LevelTask;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelManagement
{
    // LevelDefinition to projektowa reprezentacja poziomu w grze.
    [CreateAssetMenu(menuName = "Level Definition", fileName = "New Definition")]
    public class LevelDefinition : ScriptableObject
    {
        [SerializeField] private int levelID;
        [SerializeField] private string sceneName;
        [SerializeField] private LevelDefinition[] lastLevels;
        [SerializeField] private LevelTaskDefinition[] levelTasks;
        [Header("Max values for scale levels: ")]
        [SerializeField] private int e;
        [SerializeField] private int d;
        [SerializeField] private int c;
        [SerializeField] private int b;
        [SerializeField] private int a;
        [SerializeField] private int s;
        public int E => e;
        public int D => d;
        public int C => c;
        public int B => b;
        public int A => a;
        public int S => s;

        public int LevelID => levelID;
       
        // Nazwa sceny, na której znajduję sie poziom
        public string SceneName => sceneName;
        
        // Lista pozimów poprzednich. Od tych poziomów zalerzy czy będzie dostępny ten poziom
        public LevelDefinition[] LastLevels => lastLevels.Clone() as LevelDefinition[];

        public LevelTaskDefinition[] LevelTasks => levelTasks.Clone() as LevelTaskDefinition[];
    }
}
