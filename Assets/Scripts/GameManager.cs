using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public float lastpaused;
    public bool paused;
    public bool option;

    public bool clear;

    public Slider BGMSlider;
    public Slider SESlider;

    public GameObject pausedCanvas;
    public GameObject optionCanvas;
    public GameObject clearCanvas;
    public GameObject gameOverCanvas;

    public AudioClip clearBGM;
    public AudioClip gameOverBGM;
    AudioSource audio;

    public static GameManager instance;

    void Awake()
    {
        audio = GetComponent<AudioSource>();

        if (GameManager.instance == null)
            GameManager.instance = this;
    }

    void Start()
    {
        LoadSetting();
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();

        if (option)
            SaveSetting();

        if (audio.clip == gameOverBGM)
            audio.volume = BGMSlider.value * 2;
        else if (audio.clip == clearBGM)
            audio.volume = BGMSlider.value / 1.5f;
        else
            audio.volume = BGMSlider.value / 3;
    }

    void LateUpdate()
    {
        lastpaused += Time.unscaledDeltaTime;
    }

    void SaveSetting()
    {
        PlayerPrefs.SetFloat("BGM", BGMSlider.value);
        PlayerPrefs.SetFloat("SE", SESlider.value);
        PlayerPrefs.SetInt("Windowed", TitleManager.instance.windowMode);
    }

    void LoadSetting()
    {
        if (!PlayerPrefs.HasKey("BGM"))
            return;

        float bgm = PlayerPrefs.GetFloat("BGM");
        float se = PlayerPrefs.GetFloat("SE");
        int windowed = PlayerPrefs.GetInt("Windowed");

        BGMSlider.value = bgm;
        SESlider.value = se;
        TitleManager.instance.windowMode = windowed;
    }

    void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && lastpaused > 0.1f)
        {
            lastpaused = 0;

            if (!option)
            {
                if (!paused)
                    PausedMenu();
                else
                    PausedMenuExit();
            }
            else
            {
                OptionExit();
            }
        }
    }

    public void PausedMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        paused = true;
        Time.timeScale = 0;
        pausedCanvas.SetActive(true);
    }

    public void PausedMenuExit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        paused = false;
        Time.timeScale = 1;
        pausedCanvas.SetActive(false);
    }

    public void Option()
    {
        option = true;
        pausedCanvas.SetActive(false);
        optionCanvas.SetActive(true);
    }
    
    public void OptionExit()
    {
        option = false;
        optionCanvas.SetActive(false);
        pausedCanvas.SetActive(true);
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }

    public IEnumerator Clear()
    {
        yield return new WaitForSeconds(1f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        audio.Stop();
        audio.clip = clearBGM;
        audio.Play();
        clearCanvas.SetActive(true);

        yield return null;
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        audio.Stop();
        audio.clip = gameOverBGM;
        audio.Play();
        gameOverCanvas.SetActive(true);

        yield return null;
    }
}
