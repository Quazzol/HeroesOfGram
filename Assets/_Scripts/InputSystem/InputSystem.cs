using System;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public event Action<Vector3> PositionClicked;
    public event Action<Vector3> PositionHolded;

    public static InputSystem System => _system;

    private IInput _input;
    private static InputSystem _system = null;

    private void Awake()
    {
        if (_system != null)
        {
            Destroy(this);
            return;
        }
            
        _system = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        _input = InputFactory.CreateInput();
    }

    private void Update()
    {
        var clickPos = _input.PositionClick();
        if (IsValidPosition(clickPos))
        {
            PositionClicked?.Invoke(clickPos);
            return;
        }

        var holdPos = _input.PositionHold();
        if (IsValidPosition(holdPos))
        {
            PositionHolded?.Invoke(holdPos);
        }

        if (_input.BackClick())
        {
            Application.Quit();
        }
    }

    private bool IsValidPosition(Vector3 vector)
    {
        return Vector3.Distance(vector, Vector3.positiveInfinity) > float.Epsilon;
    }
}