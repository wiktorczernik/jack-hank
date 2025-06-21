using System;
using UnityEngine.Serialization;

namespace AccountManagement
{
    // Jest to po prostu kontener zawierający wszystkie dane konta gracza. Służy do zapisywania i odczytywania danych
    // z pliku JSON. Po załadowaniu konta zawierają jedynie nieaktualne dane poziomów — aktualne dane można uzyskać
    // z obiektów LevelInfo.
    [Serializable]
    public class PlayerAccountData : ICloneable
    {
        public int bountyPoints;
        public bool hasWatchedIntroCutscene;
        public LevelStatistics[] openedLevels;
        public int savedPassengers;
        public int playTimeTimestamp;
        [NonSerialized] public string accountName;

        public object Clone()
        {
            var clone = new PlayerAccountData();

            clone.accountName = accountName;
            clone.bountyPoints = bountyPoints;
            clone.savedPassengers = savedPassengers;
            clone.playTimeTimestamp = playTimeTimestamp;
            clone.openedLevels = new LevelStatistics[openedLevels?.Length ?? 0];
            
            if (openedLevels == null) return clone;

            for (var i = 0; i < openedLevels.Length; i++)
                clone.openedLevels[i] = openedLevels[i].Clone() as LevelStatistics;

            return clone;
        }
    }
}