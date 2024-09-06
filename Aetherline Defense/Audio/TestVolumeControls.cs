using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TestVolumeControls : MonoBehaviour
{
    AudioVolumeController volumeController;
    private bool isVisible;

    private static class UIClassNames
    {
        public const string VOLUME_SLIDERS = "volume-sliders";
        public const string VOLUME_SLIDERS_HIDDEN = VOLUME_SLIDERS + "--hidden";
    }


    private static class UINames
    {
        public const string MASTER_VOLUME = "MasterVolumeSlider";
        public const string MUSIC_VOLUME = "MusicVolumeSlider";
        public const string SFX_VOLUME = "SFXVolumeSlider";
    }

    public UIDocument uiDocument;
    private VisualElement _root;
    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider sfxVolumeSlider;

    private void Awake()
    {
        volumeController = new();

        _root = uiDocument.rootVisualElement;
        masterVolumeSlider = _root.Q<Slider>(UINames.MASTER_VOLUME);
        musicVolumeSlider = _root.Q<Slider>(UINames.MUSIC_VOLUME);
        sfxVolumeSlider = _root.Q<Slider>(UINames.SFX_VOLUME);
    }

    private void OnEnable()
    {
        volumeController.Init();

        masterVolumeSlider.value = volumeController.masterVolume;
        musicVolumeSlider.value = volumeController.musicVolume;
        sfxVolumeSlider.value = volumeController.sfxVolume;

        masterVolumeSlider.RegisterValueChangedCallback(OnMasterValueChanged);
        musicVolumeSlider.RegisterValueChangedCallback(OnMusicValueChanged);
        sfxVolumeSlider.RegisterValueChangedCallback(OnSFXValueChanged);

        SetVolumeVisibility(false);
    }

    private void OnDisable()
    {
        masterVolumeSlider.UnregisterValueChangedCallback(OnMasterValueChanged);
        musicVolumeSlider.UnregisterValueChangedCallback(OnMusicValueChanged);
        sfxVolumeSlider.UnregisterValueChangedCallback(OnSFXValueChanged);
    }

    private void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            Debug.Log("Hide/Show Volume Sliders");
            isVisible = !isVisible;
            SetVolumeVisibility(isVisible);
        }
    }

    private void OnMasterValueChanged(ChangeEvent<float> evt)
    {
        volumeController.SetVolume(EVolume.Master, evt.newValue);
    }

    private void OnMusicValueChanged(ChangeEvent<float> evt)
    {
        volumeController.SetVolume(EVolume.Music, evt.newValue);
    }

    private void OnSFXValueChanged(ChangeEvent<float> evt)
    {
        volumeController.SetVolume(EVolume.SFX, evt.newValue);
    }

    public void SetVolumeVisibility(bool isVisible)
    {
        _root.EnableInClassList(UIClassNames.VOLUME_SLIDERS_HIDDEN, isVisible);
    }
}
