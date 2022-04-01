using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HeroSelectionController : MonoBehaviour
{
    [SerializeField]
    private Camera _uiCamera;
    [SerializeField]
    private GameObject _selectionPanel;
    [SerializeField]
    private InfoPanelController _infoPanel;
    [SerializeField]
    private Button _startButton;
    [SerializeField]
    private List<HeroSlot> _heroSlots;
    

    private HeroFactory _heroFactory;
    private Player _player;
    private List<HeroSlot> _selectedSlots;
    private TextMeshProUGUI _startButtonText;
    private const int BATTLE_READY_HERO_COUNT = 3;

    private void Awake()
    {
        _heroFactory = GetComponent<HeroFactory>();
        _selectedSlots = new List<HeroSlot>();
    }

    private void Start()
    {
        var progress = JsonDataSaver.Load<GameProgress>("game_progress");
        if (progress != null && progress.MatchInProgress)
        {
            SceneManager.LoadScene(SceneNames.Battle);
            return;
        }

        _player = new Player(BattleSides.BlueSide);
        _player.SetHeroFactory(_heroFactory);
        if (!_player.HasHeroes)
        {
            _player.GivePlayerNewHeroes(BATTLE_READY_HERO_COUNT);
        }
        
        _startButtonText = _startButton.GetComponentInChildren<TextMeshProUGUI>();
        
        FillHeroSlots();
        UpdateStartButton();

        InputSystem.System.PositionClicked += OnPositionClicked;
        InputSystem.System.PositionHolded += OnPositionHolded;
    }

    private void OnDisable()
    {
        InputSystem.System.PositionClicked -= OnPositionClicked;
        InputSystem.System.PositionHolded -= OnPositionHolded;
    }

    public void OnStartClicked()
    {
        if (_selectedSlots.Count != BATTLE_READY_HERO_COUNT)
            return;

        foreach (var slot in _selectedSlots)
        {
            _player.AddSelectedHero(slot.Hero.Info);
        }

        _player.SaveSelectedHeroes();
        SceneManager.LoadScene(SceneNames.Battle);
    }

    private void FillHeroSlots()
    {
        for (int i = 0; i < _heroSlots.Count; i++)
        {
            var hero = _player.GetHeroFromInventory(i);
            if (hero == null)
                continue;
                
            _heroSlots[i].SetHero(hero);
        }
    }

    private void UpdateStartButton()
    {
        _startButtonText.text = _selectedSlots.Count == BATTLE_READY_HERO_COUNT ? "Start" : $"{BATTLE_READY_HERO_COUNT - _selectedSlots.Count} Hero(es) Required";
        _startButton.interactable = _selectedSlots.Count == BATTLE_READY_HERO_COUNT;
    }

    private void OnPositionHolded(Vector3 position)
    {
        HideInfoPanel();

        var heroSlot = CheckIfHeroSlotHit(position);

        if (heroSlot != null)
            OnSlotHold(heroSlot, position);
    }

    private void OnPositionClicked(Vector3 position)
    {
        HideInfoPanel();

        var heroSlot = CheckIfHeroSlotHit(position);

        if (heroSlot != null)
            OnSlotClicked(heroSlot);
    }

    private HeroSlot CheckIfHeroSlotHit(Vector2 position)
    {
        var pointerEventData = new PointerEventData(EventSystem.current){ position = position};
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        if(raycastResults.Count > 0)
        {
            foreach(var result in raycastResults)
            {
                if (result.gameObject.TryGetComponent<HeroSlot>(out var heroSlot))
                {
                    return heroSlot;
                }
            }
        }

        return null;
    }

    private void OnSlotClicked(HeroSlot selectedSlot)
    {
        if (selectedSlot.Status == HeroSlotStatus.Filled)
        {
            if (_selectedSlots.Count == BATTLE_READY_HERO_COUNT)
            {
                _selectedSlots[0].ChangeStatus(HeroSlotStatus.Filled);
                _selectedSlots.RemoveAt(0);
            }

            selectedSlot.ChangeStatus(HeroSlotStatus.Selected);
            _selectedSlots.Add(selectedSlot);
        }
        else if (selectedSlot.Status == HeroSlotStatus.Selected)
        {
            selectedSlot.ChangeStatus(HeroSlotStatus.Filled);
            _selectedSlots.Remove(selectedSlot);
        }

        UpdateStartButton();
    }

    private void OnSlotHold(HeroSlot selectedSlot, Vector3 position)
    {
        ShowInfoPanel(selectedSlot.Hero, position);
    }

    private void ShowInfoPanel(Hero selectedHero, Vector3 position)
    {
        _infoPanel.gameObject.SetActive(true);
        _infoPanel.SetInfo(selectedHero);
        
        var screenPosition = _uiCamera.ScreenToWorldPoint(position);
        screenPosition.z = 0;
        _infoPanel.transform.position = screenPosition;
    }

    private void HideInfoPanel()
    {
        _infoPanel.gameObject.SetActive(false);
    }
}