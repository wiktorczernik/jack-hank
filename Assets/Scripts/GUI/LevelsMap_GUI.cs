using LevelManagement;
using UnityEngine;

public class LevelsMap_GUI : MonoBehaviour
{
    [SerializeField] private SelectLevelButton_GUI selectLevelBtn;

    public void ShowLevelList()
    {  
        gameObject.SetActive(true);
    }

    public void HideLevelList()
    {
        gameObject.SetActive(false);
    }
    
    private void Start()
    {
        HideLevelList();
        
        var i = 0;
        foreach (var levelInfo in LevelManager.GetLevelsList())
        {
            Instantiate(selectLevelBtn, transform).Initialize(i, levelInfo);
            i++;
        }
    }
}