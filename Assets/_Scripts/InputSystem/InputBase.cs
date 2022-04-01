using UnityEngine;

public abstract class InputBase : IInput
{
    protected bool _clicked;
    protected float _clickDuration;
    protected const float MinClickDuration = 3f;

    public abstract Vector3 PositionClick();
    public abstract Vector3 PositionHold();

    public bool BackClick()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}