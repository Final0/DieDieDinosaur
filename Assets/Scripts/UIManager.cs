using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static bool GameFinished, GameStarted;
    
    [SerializeField] private Image meteorImage;
    
    [Header("Timer Settings")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image timerImage;
    [SerializeField] private float gameTimer;

    [Header("Menus")] 
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject howToPlayMenu;
    [SerializeField] private GameObject inGameMenu;

    private PlayerManager _playerManager;

    private float _timer;
    
    #region MenusButtons
    public void Play()
    {
        GameStarted = true;
        mainMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }

    public void Quit() => Application.Quit();

    public void HowToPlay()
    {
        mainMenu.SetActive(false);
        howToPlayMenu.SetActive(true);
    }

    public void Return()
    {
        mainMenu.SetActive(true);
        howToPlayMenu.SetActive(false);
    }
    #endregion

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();

        _timer = gameTimer;
    }

    private void Update()
    {
        if(GameFinished || !GameStarted) return;
        
        Timer();
        MeteorCooldown();
    }

    private void Timer()
    {
        timerImage.fillAmount -= 1 / gameTimer * Time.deltaTime;
        
        _timer -= Time.deltaTime;
        timerText.text = _timer.ToString("##.00");
        
        if (_timer <= 3f) timerText.color = Color.red;

        if (_timer > 0f) return;

        timerText.text = "0";
        GameFinished = true;
    }

    private void MeteorCooldown() => meteorImage.fillAmount += 1 / _playerManager.meteorCooldown * Time.deltaTime;
}