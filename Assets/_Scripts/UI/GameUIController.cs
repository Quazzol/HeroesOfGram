using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtGameState;
    [SerializeField]
    private InfoPanelController _infoPanel;
    [SerializeField]
    private Button _btnGoBack;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;

        InputSystem.System.PositionClicked += OnPositionClicked;
        InputSystem.System.PositionHolded += OnPositionHolded;
    }

    private void OnDisable()
    {
        InputSystem.System.PositionClicked -= OnPositionClicked;
        InputSystem.System.PositionHolded -= OnPositionHolded;
    }

    public void UpdateText(string text)
    {
        _txtGameState.text = text;
    }

    public void ShowBackButton()
    {
        _btnGoBack.gameObject.SetActive(true);
    }

    public void OnGoBackClicked()
    {
        SceneManager.LoadScene(SceneNames.HeroSelection);
    }

    private void OnPositionHolded(Vector3 position)
    {
        var hero = CheckIfHeroHit(position);

        if (hero != null)
        {
            ShowInfoPanel(hero, position);
        }
    }
    
    private void OnPositionClicked(Vector3 position)
    {
        HideInfoPanel();
    }

    private Hero CheckIfHeroHit(Vector2 position)
    {
        var ray = _mainCamera.ScreenPointToRay(position);
        if(Physics.Raycast(ray, out var hit))
        {
            if (hit.transform.TryGetComponent<Hero>(out var hero))
            {
                return hero;
            }
        }

        return null;
    }

    private void ShowInfoPanel(Hero selectedHero, Vector3 position)
    {
        _infoPanel.gameObject.SetActive(true);
        _infoPanel.SetInfo(selectedHero);
        
        position.z = 0;
        _infoPanel.transform.position = position;
    }

    private void HideInfoPanel()
    {
        _infoPanel.gameObject.SetActive(false);
    }
}