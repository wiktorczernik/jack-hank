using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHPManager : MonoBehaviour
{
    public static BossHPManager Instance;

    [SerializeField] GameObject Activation;
    [SerializeField] Image MainBar;
    [SerializeField] Image Background;
    [SerializeField] Image Foreground;
    [SerializeField] Image Mask;
    [SerializeField] Image WhiteBar;
    [SerializeField] TMP_Text BossNameText;
    [SerializeField] TMP_Text BossNameHP;
    [SerializeField] GameObject RearMirror;

    public AnimationCurve mainBarCurve;
    public AnimationCurve damageCurve;
    public float damageBarAnimTime = 1f;
    [Tooltip("Set animation time in % when damage bar hasn't been fully drained but damage was taken again")] 
    public float breakResumeTimePerc = 5f;

    IBossBarApplicable onDisplay;
    float lastHPValue;
    float damageBarTime = 0f;
    float lastFillValue = 0f;
    
    public static void DisplayBoss(IBossBarApplicable boss)
    {
        EndDisplay();
        Instance.RearMirror.SetActive(false);
        Instance.onDisplay = boss;
        boss.Self.onDeath += EndDisplay;

        Instance.Activation.SetActive(true);

        Instance.Background.color = boss.SecondaryColor;
        Instance.Foreground.color = boss.PrimaryColor;
        Instance.BossNameText.text = Instance.onDisplay.BossTitle;

        Instance.lastHPValue = boss.Self.health;
    }

    public static void EndDisplay()
    {
        Instance.RearMirror.SetActive(true);
        Instance.Activation.SetActive(false);

        if (Instance.onDisplay == null) return;

        Instance.onDisplay.Self.onDeath -= EndDisplay;
        Instance.onDisplay = null;
    }

    private void Update()
    {
        if (onDisplay == null) return;

        if (lastHPValue != onDisplay.Self.health)
            RegisterDamage();
        BossNameHP.text = (Mathf.Ceil(onDisplay.Self.health * 10f) / 10f).ToString();

        lastHPValue = onDisplay.Self.health;
    }
    void RegisterDamage()
    {
        StopCoroutine(nameof(UpdateBossbar));

        float breakResumeTime = damageBarAnimTime * breakResumeTimePerc / 100f;
        if (damageBarTime > breakResumeTime) damageBarTime = breakResumeTime;

        StartCoroutine(nameof(UpdateBossbar));
    }
    IEnumerator UpdateBossbar()
    {
        float startFillValue = WhiteBar.fillAmount >= lastFillValue ? WhiteBar.fillAmount : lastFillValue;
        float startMainFillValue = MainBar.fillAmount;
        float targetFillValue = onDisplay.Self.health / onDisplay.Self.maxHealth;

        while (damageBarTime < damageBarAnimTime)
        {
            yield return new WaitForEndOfFrame();
            damageBarTime += Time.deltaTime;

            float process = damageBarTime / damageBarAnimTime;

            MainBar.fillAmount = Mathf.LerpUnclamped(startMainFillValue, targetFillValue, mainBarCurve.Evaluate(process));
            WhiteBar.fillAmount = lastFillValue = Mathf.LerpUnclamped(startFillValue, targetFillValue, damageCurve.Evaluate(process));
        }

        Background.fillAmount = targetFillValue;
        MainBar.fillAmount = targetFillValue;
        damageBarTime = 0f;
        lastFillValue = 0f;
    }

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }
}
