using System;

public static class MathCalculator
{
    public static int RoundBasedReference(float a, int reference)
    {
        float midPoint = reference * 0.5f;

        // Check if value ends with .5
        bool isHalf = Math.Abs(a % 1f - 0.5f) < 0.0001f;

        if (isHalf)
        {
            if (a >= midPoint)
            {
                // round DOWN
                return (int)Math.Floor(a);
            }
            else
            {
                // round UP
                return (int)Math.Ceiling(a);
            }
        }

        // Normal rounding for non-.5 values
        return (int)Math.Round(a);
    }
    public static int RoundAverageBasedReference(int a, int b, int reference)
    {
        float midPoint = reference * 0.5f;
        float average = (a + b) * 0.5f;

        // Check if value ends with .5
        bool isHalf = Math.Abs(average % 1f - 0.5f) < 0.0001f;

        if (isHalf)
        {
            if (average >= midPoint)
            {
                // round DOWN
                return (int)Math.Floor(average);
            }
            else
            {
                // round UP
                return (int)Math.Ceiling(average);
            }
        }

        // Normal rounding for non-.5 values
        return (int)Math.Round(average);
    }
}
