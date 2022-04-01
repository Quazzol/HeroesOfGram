using UnityEngine;

public class PcInput : InputBase
{    
    public override Vector3 PositionClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _clicked = true;
            _clickDuration = 0f;
        }

        if (Input.GetMouseButton(0))
        {
            _clickDuration += Time.deltaTime;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            _clicked = false;
            if (_clickDuration < MinClickDuration)
                return Input.mousePosition;
        }

        return Vector3.positiveInfinity;
    }

    public override Vector3 PositionHold()
    {
        if (_clicked && _clickDuration >= MinClickDuration)
        {
            return Input.mousePosition;
        }

        return Vector3.positiveInfinity;
    }
}