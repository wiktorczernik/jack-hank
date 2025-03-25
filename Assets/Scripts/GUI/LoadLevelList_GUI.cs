using System;
using LevelManagement;
using UnityEngine;

public class LoadLevelList_GUI : MonoBehaviour
{
    [SerializeField] private SelectLevelButton_GUI selectLevelBtn;
    
    private void Start()
    {
        var i = 0;
        foreach (var levelInfo in LevelManager.GetLevelsList())
        {
            Instantiate(selectLevelBtn, transform).Initialize(i, levelInfo);
            i++;
        }
    }
}
