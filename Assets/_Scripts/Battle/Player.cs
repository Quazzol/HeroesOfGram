using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : SideOfBattle
{
#region "Properties"
    public override event Action AttackStarted;
    public bool HasHeroes => _allHeroes.Count() > 0;
    public HeroInfoList AllHeroes => _allHeroes;
    public HeroInfoList SelectedHeroes => _selectedHeroes;
#endregion

#region "Members"
    private const int MAX_NUMBER_OF_HEROES = 10;
    private const int BATTLE_READY_HERO_COUNT = 3;
    private HeroInfoList _allHeroes;
    private HeroInfoList _selectedHeroes;
    private Camera _mainCamera;
#endregion
    
    public Player(BattleSides side) : base(side)
    {
        _allHeroes = JsonDataSaver.Load<HeroInfoList>("all_heroes");
        _selectedHeroes = new HeroInfoList();
        _mainCamera = Camera.main;

        if (_allHeroes == null || _allHeroes.Count() == 0)
        {
            _allHeroes = new HeroInfoList();
        }
    }

#region "Public Methods"
    public override void CreateBattleHeroes()
    {
        LoadSelectedHeroes();
        foreach (var heroInfo in _selectedHeroes.Heroes)
        {
            AddBattleHero(_heroFactory.CreateHero(heroInfo, Side));
        }
    }

    public override void LoadBattleHeroes(List<HeroStatus> statuses)
    {
        foreach (var status in statuses)
        {
            var info = _allHeroes.Heroes.First(q => q.Name.Equals(status.Name));
            var hero = _heroFactory.CreateHero(info, Side);
            hero.LoadState(status);
            AddBattleHero(hero);
        }
    }

    public override void NotifyForAttack()
    {
        InputSystem.System.PositionClicked += OnPositionClicked;
    }

    public override void BattleEnded(bool isVictor)
    {
        if (!isVictor)
            return;

        IncreaseExperience(1);
        SaveHeroInventory();
    }

    public Hero GetHeroFromInventory(int index)
    {
        if (index < 0 || index >= _allHeroes.Count())
            return null;
        return _heroFactory.CreateHero(_allHeroes.Heroes[index], Side);
    }

    public void AddSelectedHero(HeroInfo hero)
    {
        if (IsBattleReady())
            return;

        _selectedHeroes.Heroes.Add(hero);
    }

    public void SaveSelectedHeroes()
    {
        JsonDataSaver.Save<HeroInfoList>(_selectedHeroes, "selected_heroes");
    }

    public void GivePlayerNewHeroes(int heroesToGive)
    {
        for (int i = 0; i < heroesToGive; i++)
        {
            AddHeroInventory(_heroFactory.GetRandomHeroInfoWithExclusion(_allHeroes.Heroes));
        }
        SaveHeroInventory();
    }
#endregion

#region "Private Methods"
    private bool IsBattleReady()
    {
        return _selectedHeroes.Count() == BATTLE_READY_HERO_COUNT;
    }

    private void SaveHeroInventory()
    {
        UpdateHeroInventory();
        JsonDataSaver.Save<HeroInfoList>(_allHeroes, "all_heroes");
    }

    private void LoadSelectedHeroes()
    {
        _selectedHeroes = JsonDataSaver.Load<HeroInfoList>("selected_heroes");
    }

    private void IncreaseExperience(int gainedXP)
    {
        var aliveHeroes = GetAliveHeroes();
        foreach (var hero in aliveHeroes)
        {
            hero.IncreaseXP(gainedXP);
        }
    }

    private void AddHeroInventory(HeroInfo hero)
    {
        if (_allHeroes.Heroes.Count >= MAX_NUMBER_OF_HEROES)
            return;
        _allHeroes.Heroes.Add(hero);
    }

    private void OnPositionClicked(Vector3 position)
    {
        var ray = _mainCamera.ScreenPointToRay(position);
        if(Physics.Raycast(ray, out var hit))
        {
            if (hit.transform.TryGetComponent<Hero>(out var hero))
            {
                if (hero.Side != Side || !hero.IsAlive)
                    return;

                AttackStarted?.Invoke();
                hero.Attack(GetRandomTarget());
                InputSystem.System.PositionClicked -= OnPositionClicked;
            }
        }
    }

    private Hero GetRandomTarget()
    {
        var aliveTargets = _targets();
        if (aliveTargets.Count == 0)
        {
            throw new Exception("No alive hero to target");
        }

        return aliveTargets[UnityEngine.Random.Range(0, aliveTargets.Count)];
    }

    private void UpdateHeroInventory()
    {
        foreach (var battleHero in _heroesToBattle)
        {
            var hero = _allHeroes.Heroes.First(q => q.Name.Equals(battleHero.Name));
            hero.Experience = battleHero.Experience;
        }
    }
#endregion
}
