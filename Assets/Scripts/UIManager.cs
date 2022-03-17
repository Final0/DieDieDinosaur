using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static bool GameFinished, GameStarted, StopSpawn;
    
    [SerializeField] private Image meteorImage;
    [SerializeField] private TMP_Text endText;
    [SerializeField] private GameObject DinosaursParent;

    [Header("Timer Settings")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image timerImage;
    [SerializeField] private float gameTimer;

    [Header("Menus")] 
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject howToPlayMenu;
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject endMenu;

    private PlayerManager _playerManager;

    private float _timer;

    private const string WinText = "You win !", LooseText = "You loose !";

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

    public void ReturnMain() => SceneManager.LoadScene(0);
    #endregion

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        
        GameStarted = false;
        GameFinished = false;
        
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
        
        if (_timer > 5f) return;
        
        timerText.color = Color.red;
        StopSpawn = true;

        if (_timer > 0f) return;

        timerText.text = "0";
        GameFinished = true;
        
        DisplayEndMenu();
    }

    private void DisplayEndMenu()
    {
        mainMenu.SetActive(false);
        howToPlayMenu.SetActive(false);
        inGameMenu.SetActive(false);
        endMenu.SetActive(true);

        endText.text = DinosaursParent.transform.childCount > 0 ? LooseText : WinText;
    }

    private void MeteorCooldown() => meteorImage.fillAmount += 1 / _playerManager.meteorCooldown * Time.deltaTime;
}