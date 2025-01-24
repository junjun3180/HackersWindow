using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_6_Control : MonoBehaviour
{
    private static UI_6_Control instance = null;

    // UI Window
    public GameObject UI_W_Control = null;

    // Detail
    public Dropdown screenModeDropdown;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public Button MasterButton;
    public GameObject MasterVolumeBaseImage;
    public GameObject MasterVolumeMuteImage;
    public Text MasterVolumeText;

    public Button BGMButton;
    public GameObject BGMVolumeBaseImage;
    public GameObject BGMVolumeMuteImage;
    public Text BGMVolumeText;

    public Button SFXButton;
    public GameObject SFXVolumeBaseImage;
    public GameObject SFXVolumeMuteImage;
    public Text SFXVolumeText;

    // Manager
    private SoundManager soundManager = null;

    public static UI_6_Control Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_6_Control>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_6_Control).Name);
                    instance = singletonObject.AddComponent<UI_6_Control>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        soundManager = SoundManager.Instance;

        // ControllOptionUI Setting
        screenModeDropdown.onValueChanged.AddListener(delegate { ChangeScreenMode(screenModeDropdown.value); });
        resolutionDropdown.onValueChanged.AddListener(delegate { ChangeResolution(resolutionDropdown.value); });
        qualityDropdown.onValueChanged.AddListener(delegate { ChangeQuality(qualityDropdown.value); });

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        MasterButton.onClick.AddListener(FMasterButton);
        MasterVolumeMuteImage.SetActive(false);

        BGMButton.onClick.AddListener(FBGMButton);
        BGMVolumeMuteImage.SetActive(false);

        SFXButton.onClick.AddListener(FSFXButton);
        SFXVolumeMuteImage.SetActive(false);
    }

    public void OpenUI()
    {
        if (UI_W_Control != null)
        {
            UI_W_Control.SetActive(true);
            // Debug.Log("OpenUI : UI_6_Control");
        }
    }

    public void CloseUI()
    {
        if (UI_W_Control != null)
        {
            UI_W_Control.SetActive(false);
            // Debug.Log("CloseUI : UI_6_Control");
        }
    }

    // ScreenMode
    public void ChangeScreenMode(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Debug.Log("전체화면 모드");
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Debug.Log("창모드");
                break;
            default:
                Debug.LogWarning("Error");
                break;
        }
    }

    // Resolution
    public void ChangeResolution(int index)
    {
        switch (index)
        {
            case 0: // 1920 x 1080
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 1: // 1600 x 900
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                break;
            case 2: // 1280 x 720
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
            default:
                break;
        }
    }

    public void ChangeQuality(int index)
    {
        switch (index)
        {
            case 0: // 좋음
                QualitySettings.SetQualityLevel(5, true);
                break;
            case 1: // 중간
                QualitySettings.SetQualityLevel(3, true);
                break;
            case 2: // 낮음
                QualitySettings.SetQualityLevel(1, true);
                break;
            default:
                break;
        }
    }

    // Audio
    public void SetMasterVolume(float volume)
    {
        soundManager.SetMasterVolume(volume);
        float VolumeText = ((volume + 40) / 40) * 100;
        MasterVolumeText.text = $"{VolumeText:F0}";
    }

    public void SetBGMVolume(float volume)
    {
        soundManager.SetBGMVolume(volume);
        float VolumeText = ((volume + 40) / 40) * 100;
        BGMVolumeText.text = $"{VolumeText:F0}";
    }

    public void SetSFXVolume(float volume)
    {
        soundManager.SetSFXVolume(volume);
        float VolumeText = ((volume + 40) / 40) * 100;
        SFXVolumeText.text = $"{VolumeText:F0}";
    }

    public void FMasterButton()
    {
        if (!soundManager.MasterVolumeMute) // 음소거 상태 아닐때
        {
            MasterVolumeBaseImage.SetActive(false);
            MasterVolumeMuteImage.SetActive(true);
        }
        else
        {
            MasterVolumeBaseImage.SetActive(true);
            MasterVolumeMuteImage.SetActive(false);
        }
        soundManager.MasterVolumeMute = !soundManager.MasterVolumeMute;
        SetMasterVolume(soundManager.MasterVolume);
    }

    public void FBGMButton()
    {
        if (!soundManager.BGMVolumeMute) // 음소거 상태 아닐때
        {
            BGMVolumeBaseImage.SetActive(false);
            BGMVolumeMuteImage.SetActive(true);
        }
        else
        {
            BGMVolumeBaseImage.SetActive(true);
            BGMVolumeMuteImage.SetActive(false);
        }
        soundManager.BGMVolumeMute = !soundManager.BGMVolumeMute;
        SetBGMVolume(soundManager.BGMVolume);
    }

    public void FSFXButton()
    {
        if (!soundManager.SFXVolumeMute) // 음소거 상태 아닐때
        {
            SFXVolumeBaseImage.SetActive(false);
            SFXVolumeMuteImage.SetActive(true);
        }
        else
        {
            SFXVolumeBaseImage.SetActive(true);
            SFXVolumeMuteImage.SetActive(false);
        }
        soundManager.SFXVolumeMute = !soundManager.SFXVolumeMute;
        SetSFXVolume(soundManager.BGMVolume);
    }
}
