using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour, IDataPersistence<PlayerSettings>
{
    [SerializeField] private Settings settings;

    [Space(12)]
    [SerializeField] private GameObject _volumeSlider;

    [Space(12)]
    [SerializeField] private GameObject _BGCDropdown;

    [Space(12)]
    [SerializeField] private GameObject _contrastButton;
    [SerializeField] private Image[] _cards;
    [SerializeField] private CardDefinition[] _cardSprites;

    private void Start()
    {
        var dropdown = _BGCDropdown.GetComponent<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(settings.BGColours.GetNameList());
           
    }

    private void Update()
    {
        bool hc = _contrastButton.GetComponent<Toggle>().isOn;
        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].sprite = hc ? _cardSprites[i].highContrast : _cardSprites[i].lowContrast;
        }
    }

    public void LoadData(PlayerSettings data)
    {
        _volumeSlider.GetComponent<Slider>().value = data.volume;

        var dropdown = _BGCDropdown.GetComponent<Dropdown>();
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == data.backgroundId)
            {
                dropdown.value = i;
                break;
            }
        }

        _contrastButton.GetComponent<Toggle>().isOn = data.highConstrast;
    }

    public void SaveData(ref PlayerSettings data)
    {
        data.volume = _volumeSlider.GetComponent<Slider>().value;
        data.backgroundId = _BGCDropdown.GetComponent<Dropdown>().options[
            _BGCDropdown.GetComponent<Dropdown>().value].text;
        data.highConstrast = _contrastButton.GetComponent<Toggle>().isOn;
    }
}
