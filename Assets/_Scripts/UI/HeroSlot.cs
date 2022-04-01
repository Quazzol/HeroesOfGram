using UnityEngine;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour
{
    public Hero Hero => _hero;
    public HeroSlotStatus Status => _status;

    [SerializeField]
    private Transform _heroParent;
    private Hero _hero;
    private Image _imageBG;
    private HeroSlotStatus _status = HeroSlotStatus.Empty;
    private Color EMPTY_COLOR = Color.red;
    private Color FILLED_COLOR = Color.gray;
    private Color SELECTED_COLOR = Color.green;

    private void Awake()
    {
        _imageBG = GetComponent<Image>();
    }

    private void Start()
    {
        SetColor();
    }

    public void SetHero(Hero hero)
    {
        _hero = hero;
        _hero.transform.SetParent(_heroParent);
        _hero.transform.localPosition = Vector3.zero;
        _hero.transform.localRotation = Quaternion.identity;
        _hero.transform.localScale = Vector3.one * 80;
        ChangeStatus(HeroSlotStatus.Filled);
    }

    public Vector3 GetSlotSize()
    {
        return new Vector3(_imageBG.rectTransform.rect.width, _imageBG.rectTransform.rect.height, 0);
    }

    public void ChangeStatus(HeroSlotStatus status)
    {
        if (_hero == null)
            return;

        _status = status;
        SetColor();
    }

    private void SetColor()
    {
        Color color = EMPTY_COLOR;
        switch (_status)
        {
            case HeroSlotStatus.Selected: color = SELECTED_COLOR; break;
            case HeroSlotStatus.Filled: color = FILLED_COLOR; break;
        }

        _imageBG.color = color;
    }

}
