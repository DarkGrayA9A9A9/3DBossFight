using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public bool option;
    public int windowMode;

    public Slider BGMSlider;
    public Slider SESlider;

    public GameObject titleCanvas;
    public GameObject optionCanvas;

    AudioSource audio;

    public static TitleManager instance;
    
    void Awake()
    {
        audio = GetComponent<AudioSource>();

        if (TitleManager.instance == null)
            TitleManager.instance = this;
    }

    void Start()
    {
        LoadSetting();
    }

    void Update()
    {
        if (option)
        {
            SaveSetting();

            if (Input.GetKeyDown(KeyCode.Escape))
                OptionExit();
        }
    }

    void LateUpdate()
    {
        audio.volume = BGMSlider.value / 2;
    }

    void SaveSetting()
    {
        PlayerPrefs.SetFloat("BGM", BGMSlider.value);
        PlayerPrefs.SetFloat("SE", SESlider.value);
        PlayerPrefs.SetInt("Windowed", windowMode);
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
        windowMode = windowed;

    }

    public void Option()
    {
        option = true;
        titleCanvas.SetActive(false);
        optionCanvas.SetActive(true);
    }

    public void OptionExit()
    {
        option = false;
        optionCanvas.SetActive(false);
        titleCanvas.SetActive(true);
    }

    public void SetWindow()
    {
        if (windowMode == 1)
        {
            windowMode = 0;
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            windowMode = 1;
            Screen.SetResolution(1280, 720, false);
        }
    }
}
