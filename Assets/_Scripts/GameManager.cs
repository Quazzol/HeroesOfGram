using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera _mainCamera;
    private BattleController _battleController;
    private GameUIController _uiController;
    private GameProgress _progress;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _battleController = GetComponent<BattleController>();
        _uiController = GetComponent<GameUIController>();

        _battleController.StateChanged += OnBattleStateChanged;
    }

    private void Start()
    {
        _battleController.Initialize(new Player(BattleSides.BlueSide), new Enemy(BattleSides.RedSide));
        
        LoadProgress();
        if (!LoadBattle())
            _battleController.StartNewBattle();
        _progress.MatchInProgress = true;
    }

    private void OnDisable()
    {
        _battleController.StateChanged -= OnBattleStateChanged;
        SaveProgress();
        SaveBattle();
    }

    private void LoadProgress()
    {
        _progress = JsonDataSaver.Load<GameProgress>("game_progress");
        if (_progress == null)
        {
            _progress = new GameProgress();
        }
    }

    private void SaveProgress()
    {
        JsonDataSaver.Save<GameProgress>(_progress, "game_progress");
    }

    private bool LoadBattle()
    {
        if (!_progress.MatchInProgress)
            return false;

        var progress = JsonDataSaver.Load<BattleSave>("battle_save");
        if (progress == null)
            return false;

        _battleController.LoadBattle(progress);
        return true;
    }

    private void SaveBattle()
    {
        if (!_progress.MatchInProgress)
            return;

        var battleStatus = new BattleSave();
        battleStatus.CurrentState = _battleController.CurrentState;
        battleStatus.NextState = _battleController.NextState;
        battleStatus.BlueSideStatus = new HeroStatuses();
        battleStatus.BlueSideStatus.Status = new List<HeroStatus>();
        battleStatus.RedSideStatus = new HeroStatuses();
        battleStatus.RedSideStatus.Status = new List<HeroStatus>();

        var blueSide = _battleController.BlueSide;
        for (int i = 0; i < blueSide.BattleHeroCount(); i++)
        {
            var hero = blueSide.GetBattleHero(i);
            battleStatus.BlueSideStatus.Status.Add(new HeroStatus() { Name = hero.Name, Health = hero.Health } );
        }

        var redSide = _battleController.RedSide;
        for (int i = 0; i < redSide.BattleHeroCount(); i++)
        {
            var hero = redSide.GetBattleHero(i);
            battleStatus.RedSideStatus.Status.Add(new HeroStatus() { Name = hero.Name, Health = hero.Health } );
        }

        JsonDataSaver.Save<BattleSave>(battleStatus, "battle_save");
    }

    private void OnBattleStateChanged(BattleState gameState)
    {
        var text = string.Empty;

        switch (gameState)
        {
            case BattleState.Start: text = "Ready!"; break;
            case BattleState.BlueSideTurn: text = "Choose a hero to attack"; break;
            case BattleState.RedSideTurn: text = "Enemy retaliates"; break;
            case BattleState.Victory: text = "You WON"; EndBattle(); break;
            case BattleState.Lose: text = "You LOST"; EndBattle(); break;
            default: return;
        }

        _uiController.UpdateText(text);
    }

    private void EndBattle()
    {
        _progress.MatchMade++;
        _progress.MatchInProgress = false;

        SaveProgress();
        if (_progress.MatchMade % 5 == 0)
        {
            ((Player) _battleController.BlueSide).GivePlayerNewHeroes(1);
        }
        _uiController.ShowBackButton();
    }

}