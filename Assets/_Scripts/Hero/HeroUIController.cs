using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroUIController : MonoBehaviour
{
    [SerializeField]
    private Image _healthBar;
    [SerializeField]
    private TextMeshProUGUI _damageText;
    private float _targetAmount = 1f;

    public void UpdateHealthBar(float amount)
    {
        _targetAmount = amount;
        StartCoroutine(UpdateHealthUI());
    }

    public void ShowDamageTaken(float damageAmount)
    {
        ShowFloatingText(string.Format("{0:0.00}", damageAmount), Color.yellow);
    }

    public void ShowXpGained()
    {
        ShowFloatingText("+XP", Color.blue);
    }

    private void ShowFloatingText(string text, Color textColor)
    {
        _damageText.gameObject.SetActive(true);
        _damageText.text = text;
        _damageText.color = UpdateTextColorAlpha(textColor, 1f);
        StartCoroutine(UpdateDamageUI());
    }

    private IEnumerator UpdateHealthUI()
    {
        while (!_healthBar.fillAmount.AreEqual(_targetAmount))
        {
            _healthBar.fillAmount = Mathf.MoveTowards(_healthBar.fillAmount, _targetAmount, Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator UpdateDamageUI()
    {
        yield return new WaitForSeconds(.5f);

        while (_damageText.color.a > float.Epsilon)
        {
            _damageText.color = UpdateTextColorAlpha(_damageText.color, Mathf.MoveTowards(_damageText.color.a, 0, Time.deltaTime));
            yield return null;
        }

        _damageText.gameObject.SetActive(false);
    }

    private Color UpdateTextColorAlpha(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }

}