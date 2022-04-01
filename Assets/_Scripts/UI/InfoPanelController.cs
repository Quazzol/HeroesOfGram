using UnityEngine;
using TMPro;
using System;

public class InfoPanelController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    public void SetInfo(Hero hero)
    {
        var text = string.Format("Name: {1}{0}Level: {2}{0}Experience: {3}{0}HP: {4}{0}Attak Power: {5}",
                                Environment.NewLine,
                                hero.Name,
                                hero.Level,
                                hero.Experience,
                                hero.Health,
                                hero.AttackPower);

        _text.text = text;
    }
}