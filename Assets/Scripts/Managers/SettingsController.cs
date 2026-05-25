using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Settings settings;

    [Space(12)]
    [SerializeField] private GameObject _volumeSlider;
    private Slider slider;

    [Space(12)]
    [SerializeField] private GameObject _BGCDropdown;
    private Dropdown dropdown;

    [Space(12)]
    [SerializeField] private GameObject _contrastButton;
    private Toggle toggle;

    [SerializeField] private Image[] _cards;
    [SerializeField] private CardDefinition[] _cardSprites;

    private void Start()
    {
        dropdown = _BGCDropdown.GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(settings.BGColours.GetNameList());

        slider = _volumeSlider.GetComponent<Slider>();

        toggle = _contrastButton.GetComponent<Toggle>();
    }

    private void Update()
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].sprite = toggle.isOn ? _cardSprites[i].highContrast : _cardSprites[i].lowContrast;
        }
    }

    public void LoadData(GameData data)
    {
        slider.value = data.volume;

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == data.backgroundId)
            {
                dropdown.value = i;
                break;
            }
        }

        toggle.isOn = data.highContrast;
    }

    public void SaveData(ref GameData data)
    {
        data.volume = slider.value;
        data.backgroundId = dropdown.options[
            dropdown.value].text;
        data.highContrast = toggle.isOn;
    }
}
