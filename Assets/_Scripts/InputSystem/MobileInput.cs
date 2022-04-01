using UnityEngine;

public class MobileInput : InputBase
{
    public override Vector3 PositionClick()
    {
        if (Input.touchCount >= 1)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                _clicked = true;
                _clickDuration = 0f;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended)
            {
                _clicked = false;
                if (_clickDuration < MinClickDuration)
                    return Input.touches[0].position;
            }
            else
            {
                _clickDuration += Time.deltaTime;
            }
        }
        
        return Vector3.positiveInfinity;
    }

    public override Vector3 PositionHold()
    {
        if (_clicked && _clickDuration >= MinClickDuration)
        {
            return Input.touches[0].position;
        }

        return Vector3.positiveInfinity;
    }
}