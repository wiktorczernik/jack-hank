using UnityEngine;

namespace JackHank.Dialogs
{
    /// <summary>
    /// Reprezentacja postaci dla systemu dialogów
    /// </summary>
    [CreateAssetMenu(fileName = "New Character", menuName = "Jack&Hank/Dialogs/Character", order = 1)]
    public class Character : ScriptableObject
    {
        /// <summary>
        /// Krótka nazwa postaci, takie niby id
        /// </summary>
        public string shortname;
        /// <summary>
        /// Nazwa postaci która mo¿e byæ wyœwietlona np w napisach
        /// </summary>
        public string displayName;
    }
}
