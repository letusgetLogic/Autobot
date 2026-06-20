using TMPro;
using UnityEngine;

public class CatalogDescriptionLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ability;
    [SerializeField] private GameObject energyIcon;
    [SerializeField] private TextMeshProUGUI consumedEnergy;
    [SerializeField] private TextMeshProUGUI recycle0;

    // Recycle Full DG
    [SerializeField] private GameObject recycleNut;
    [SerializeField] private TextMeshProUGUI recycle3;
    // Recycle Full DG
    [SerializeField] private GameObject recycleCurrency;
    [SerializeField] private TextMeshProUGUI recycle3nut;
    [SerializeField] private TextMeshProUGUI recycle3tool;

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
        var index3 = SoTradingCurrency.ConvertToIndex1D(3, _currencyData.LevelAmount, _index, true);
        recycle0.text = "+" + sell[index0].Nut.ToString();

        if (sell[index3].Tool == 0)
        {
            recycleCurrency.SetActive(false);
            recycleNut.SetActive(true);
            recycle3.text = "+" + sell[index3].Nut.ToString();
        }
        else 
        {
            recycleNut.SetActive(false);
            recycleCurrency.SetActive(true);

            recycle3nut.text = "+" + sell[index3].Nut.ToString();
            recycle3tool.text = "+" + sell[index3].Tool.ToString();
        }
    }
}
