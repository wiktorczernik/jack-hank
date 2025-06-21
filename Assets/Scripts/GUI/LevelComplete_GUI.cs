using System.Collections;
using TMPro;
using UnityEngine;

public class LevelComplete_GUI : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [Range(0, 1)][SerializeField] private float shakeIntensityOnRank;
    [Min(0)][SerializeField] private float delayBeforeAppearInSeconds = 3;
    [Min(0)][SerializeField] private float delayBetweenStatisticsItemsInSeconds = 0.2f;
    [Min(0)][SerializeField] private float delayBeforeCongratulationsInSeconds = 15f;
    [Min(0)] [SerializeField] private float delayAfterCongratulations = 5;

    [Header("UI Elements")] 
    [SerializeField] private StatisticsListItem_GUI scoreStat;
    [SerializeField] private StatisticsListItem_GUI timeStat;
    [SerializeField] private StatisticsListItem_GUI passengerStat;
    [SerializeField] private StatisticsListItem_GUI rankStat;
    [SerializeField] private TextMeshProUGUI congratulationsText;
    [SerializeField] private TextMeshProUGUI levelCompleteText;

    public bool isBusy { get; private set; }

    private void Start()
    {
        levelCompleteText.gameObject.SetActive(false);
        congratulationsText.gameObject.SetActive(false);
        scoreStat.gameObject.SetActive(false);
        timeStat.gameObject.SetActive(false);
        passengerStat.gameObject.SetActive(false);
        rankStat.gameObject.SetActive(false);
    }

    public void Appear()
    {
        if (isBusy) return;
        isBusy = true;
        
        StartCoroutine(AppearCo());
    }

    private IEnumerator AppearCo()
    {
        var runInfo = GameManager.RunInfo;
        string rank;

        if (runInfo == null)
        {
            runInfo = new GameRunInfo();
            runInfo.Time = 225000;
            runInfo.ChangeBonusBountyBy(33400, PlayerBonusTypes.Drift);
            runInfo.PassengersOnBoard = 4;
            rank = "s";
        }
        else
        {
            rank = GameManager.GetStringMarkByBounty();
        }
        
        yield return new WaitForSeconds(delayBeforeAppearInSeconds);

        var levelCompleteTextAnim = levelCompleteText.gameObject.GetComponent<Animation>();
        levelCompleteText.gameObject.SetActive(true);
        levelCompleteTextAnim.Play();
        yield return new WaitWhile(() => levelCompleteTextAnim.isPlaying);
        yield return new WaitForSeconds(delayBetweenStatisticsItemsInSeconds);
        
        scoreStat.gameObject.SetActive(true);
        yield return scoreStat.StartAppearing(runInfo.AllBountyPoints);
        yield return new WaitForSeconds(delayBetweenStatisticsItemsInSeconds);
        
        passengerStat.gameObject.SetActive(true);
        yield return passengerStat.StartAppearing(runInfo.PassengersOnBoard);
        yield return new WaitForSeconds(delayBetweenStatisticsItemsInSeconds);
        
        timeStat.gameObject.SetActive(true);
        yield return timeStat.StartAppearingInTimeField((int)runInfo.Time);
        yield return new WaitForSeconds(delayBetweenStatisticsItemsInSeconds);

        rankStat.gameObject.SetActive(true);
        yield return rankStat.StartAppearing(rank);
        cameraController.Shake(shakeIntensityOnRank);
        yield return new WaitForSeconds(delayBeforeCongratulationsInSeconds);
        
        levelCompleteTextAnim.Play("TextDisappear");
        StartCoroutine(scoreStat.StartDisappear());
        StartCoroutine(passengerStat.StartDisappear());
        StartCoroutine(timeStat.StartDisappear());
        StartCoroutine(rankStat.StartDisappear());
        
        yield return new WaitWhile(() => levelCompleteTextAnim.isPlaying);
        levelCompleteText.gameObject.SetActive(false);
        
        var congratulationsTextAnim = congratulationsText.gameObject.GetComponent<Animation>();
        congratulationsText.gameObject.SetActive(true);
        congratulationsTextAnim.Play();
        yield return new WaitWhile(() => congratulationsTextAnim.isPlaying);
        yield return new WaitForSeconds(delayAfterCongratulations);

        isBusy = false;
    }
}