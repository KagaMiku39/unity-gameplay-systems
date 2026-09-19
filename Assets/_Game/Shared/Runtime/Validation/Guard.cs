using System;

public static class Guard
{
    public static int Positive(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "参数必须大于 0。"
            );
        }

        return value;
    }

    public static int NonNegative(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "参数必须大于等于 0。"
            );
        }

        return value;
    }
}
