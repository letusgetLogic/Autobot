using TMPro;
using UnityEngine;

public class CatalogDescriptionLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ability;
    [SerializeField] private GameObject energyIcon;
    [SerializeField] private TextMeshProUGUI consumedEnergy;
    [SerializeField] private TextMeshProUGUI recycle0;
    [SerializeField] private TextMeshProUGUI recycle1;

    public void SetData(SoUnit _soUnit, SoTradingCurrency _currencyData, int _index)
    {
        // ablity
        ability.text = _soUnit.Levels[_index].Description;

        // consumed energy
        if (_soUnit.Levels[_index].ConsumedEnergy && _soUnit.Levels[_index].ConsumedEnergy.Value < 0)
        {
            energyIcon.SetActive(true);
            consumedEnergy.text = _soUnit.Levels[_index].ConsumedEnergy.Value.ToString();
        }
        else
        {
            energyIcon.SetActive(false);
        }

        // recycle
        var sell = _currencyData.Sell;
        var index0 = SoTradingCurrency.ConvertToIndex1D(0, _currencyData.LevelAmount, _index, true);
        var index1 = SoTradingCurrency.ConvertToIndex1D(1, _currencyData.LevelAmount, _index, true);
        recycle0.text = "+" + sell[index0].Nut.ToString();
        recycle1.text = "+" + sell[index1].Nut.ToString();
    }
}
