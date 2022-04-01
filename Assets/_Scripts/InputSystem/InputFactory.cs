using UnityEngine;

public class InputFactory
{
    public static IInput CreateInput()
    {
        if (Application.isMobilePlatform)
            return new MobileInput();
        return new PcInput();
    }
}