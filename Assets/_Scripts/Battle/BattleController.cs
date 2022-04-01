using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleController : MonoBehaviour
{
#region "Events"
    public event Action<BattleState> StateChanged;
#endregion

#region "Properties"
    public ISideOfBattle BlueSide => _blueSide;
    public ISideOfBattle RedSide => _redSide;
    public BattleState CurrentState => _currentState == BattleState.Wait ? BattleState.CheckEndGame : _currentState;
    public BattleState NextState => _nextState;
#endregion

#region "Private Members"
    [SerializeField]
    private List<SpawnPosition> _spawnPoints;
    private BattleState _currentState;
    private BattleState _nextState;
    private Camera _mainCamera;
    private HeroFactory _heroFactory;
    private ISideOfBattle _blueSide;
    private ISideOfBattle _redSide;
    private BattleSave _save;
#endregion

#region "MonoBehaviour Methods"
    private void Awake()
    {
        _mainCamera = Camera.main;
        _heroFactory = GetComponent<HeroFactory>();
    }

    private void OnDisable()
    {
        _blueSide.AttackDone -= OnBlueSideAttackDone;
        _redSide.AttackDone -= OnRedSideAttakDone;
    }
#endregion

#region "Public Methods"
    public void Initialize(ISideOfBattle blueSide, ISideOfBattle redSide)
    {
        _blueSide = blueSide;
        _redSide = redSide;

        SetBattleSideValues();
    }
    public void StartNewBattle()
    {
        ChangeState(BattleState.Start);
    }

    public void LoadBattle(BattleSave save)
    {
        _save = save;
        ChangeState(BattleState.Load);
    }
#endregion

#region "States"
    private void OnStateChanged()
    {
        switch (_currentState)
        {
            case BattleState.Wait:
                break;
            case BattleState.Load:
                OnGameLoad();
                break;
            case BattleState.Start:
                OnGameStart();
                break;
            case BattleState.BlueSideTurn:
                OnBlueSideTurn();
                break;
            case BattleState.RedSideTurn:
                OnRedSideTurn();
                break;
            case BattleState.CheckEndGame:
                OnCheckEndGame();
                break;
            case BattleState.Victory:
                OnVictory();
                break;
            case BattleState.Lose:
                OnLose();
                break;
        }
    }

    private void OnGameLoad()
    {
        LoadBattleHeroes();
        _currentState = _save.CurrentState;
        _nextState = _save.NextState;
        ChangeState(_save.CurrentState);
    }

    private void OnGameStart()
    {
        CreateNewBattleHeroes();
        ChangeState(BattleState.BlueSideTurn);
    }

    private void OnBlueSideTurn()
    {
        _blueSide.NotifyForAttack();
    }

    private void OnRedSideTurn()
    {
        _redSide.NotifyForAttack();
    }

    private void OnCheckEndGame()
    {
        if (!_blueSide.HasAliveHeroes())
        {
            ChangeState(BattleState.Lose);
            return;
        }
        
        if (!_redSide.HasAliveHeroes())
        {
            ChangeState(BattleState.Victory);
            return;
        }

        ChangeState(_nextState);
    }

    private void OnVictory()
    {
        _blueSide.BattleEnded(true);
        _redSide.BattleEnded(false);
        ChangeState(BattleState.Wait);
    }

    private void OnLose()
    {
        _blueSide.BattleEnded(false);
        _redSide.BattleEnded(true);
        ChangeState(BattleState.Wait);
    }

    private void ChangeState(BattleState state)
    {
        StateChanged?.Invoke(state);
        _currentState = state;
        OnStateChanged();
    }
#endregion

#region "Private Methods"
    private void SetBattleSideValues()
    {
        _blueSide.SetHeroFactory(_heroFactory);
        _blueSide.SetAttackableTargets(_redSide.GetAliveHeroes);

        _redSide.SetHeroFactory(_heroFactory);
        _redSide.SetAttackableTargets(_blueSide.GetAliveHeroes);

        _blueSide.AttackStarted += OnBlueSideAttackStarted;
        _blueSide.AttackDone += OnBlueSideAttackDone;
        
        _redSide.AttackStarted += OnRedSideAttackStarted;
        _redSide.AttackDone += OnRedSideAttakDone;
    }

    private void LoadBattleHeroes()
    {
        _redSide.LoadBattleHeroes(_save.RedSideStatus.Status);
        _blueSide.LoadBattleHeroes(_save.BlueSideStatus.Status);
        CreateHeroes();
    }

    private void CreateNewBattleHeroes()
    {
        _redSide.CreateBattleHeroes();
        _blueSide.CreateBattleHeroes();
        CreateHeroes();
    }

    private void CreateHeroes()
    {
        for (int i = 0; i < _blueSide.BattleHeroCount(); i++)
        {
            var hero = _blueSide.GetBattleHero(i);
            var position = GetUnusedSpawnPoint(BattleSides.BlueSide).transform.position;
            position.y = 0.5f;
            hero.transform.position = position;
        }

        for (int i = 0; i < _redSide.BattleHeroCount(); i++)
        {
            var hero = _redSide.GetBattleHero(i);
            var position = GetUnusedSpawnPoint(BattleSides.RedSide).transform.position;
            position.y = 0.5f;
            hero.transform.position = position;
        }
    }

    private SpawnPosition GetUnusedSpawnPoint(BattleSides side)
    {
        var point = _spawnPoints.FirstOrDefault(q => q.Side == side && !q.IsFilled);
        point.IsFilled = true;
        return point;
    }

    private void OnBlueSideAttackStarted()
    {
        _nextState = BattleState.RedSideTurn;
        ChangeState(BattleState.Wait);
    }

    private void OnBlueSideAttackDone()
    {
        StartCoroutine(BlueSideAttackDone());
    }

    private IEnumerator BlueSideAttackDone()
    {
        yield return new WaitForSeconds(.5f);
        ChangeState(BattleState.CheckEndGame);
    }

    private void OnRedSideAttackStarted()
    {
        _nextState = BattleState.BlueSideTurn;
        ChangeState(BattleState.Wait);
    }

    private void OnRedSideAttakDone()
    {
        StartCoroutine(RedSideAttackDone());
    }

    private IEnumerator RedSideAttackDone()
    {
        yield return new WaitForSeconds(.5f);
        ChangeState(BattleState.CheckEndGame);
    }
#endregion
}