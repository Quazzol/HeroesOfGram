using System;
using System.Collections.Generic;

[Serializable]
public class BattleSave
{
    public BattleState CurrentState;
    public BattleState NextState;
    public HeroStatuses BlueSideStatus;
    public HeroStatuses RedSideStatus;
}


[Serializable]
public class HeroStatus
{
    public string Name;
    public float Health;
}

[Serializable]
public class HeroStatuses
{
    public List<HeroStatus> Status;
}