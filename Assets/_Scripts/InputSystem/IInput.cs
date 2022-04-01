using UnityEngine;

public interface IInput
{
    public Vector3 PositionClick();
    public Vector3 PositionHold();
    public bool BackClick();
}