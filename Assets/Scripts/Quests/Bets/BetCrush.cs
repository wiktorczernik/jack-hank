using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BetCrush : Bet
{
    [SerializeField] private List<Target> targets;
    [SerializeField] private int targetsCounter;

    private void Start()
    {
        targetsCounter = 0;
    }

    private void Update()
    {
        if (!IsCompleted)
        {
            targetsCounter = GetTargets(true).Count();
            if (targetsCounter == targets.Count())
            {
                IsCompleted = true;
                Debug.Log("Brawo!!!");
                //this.enabled = false;
            }
        }
    }

    public List<Target> GetTargets(bool completed)
    {
        return targets.Where(t => t.IsCompleted == completed).ToList();
    }
}
