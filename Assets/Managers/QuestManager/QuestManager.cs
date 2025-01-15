using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    [SerializeField] private QuestShift shift;
    [SerializeField] private List<Quest> betting;

    private void Awake()
    {
        Instance = this;
    }

    public List<Quest> GetBetting(bool option)
    {
        return betting.Where(q => q.IsCompleted == option).ToList();
    }

    public void AddBet(Quest q)
    {
        betting.Add(q);
    }

    public void SetShift(QuestShift questShift)
    {
        if(shift == null)
        {
            shift = questShift;
        }
    }
}
