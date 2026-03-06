using System;

public static class MathCalculator
{
    /// <summary>
    /// Rounds a float value based on a reference point. If the value ends with .5, 
    /// it rounds down if the value is greater than or equal to the midpoint of the reference, 
    /// and rounds up if it's less than the midpoint. For non-.5 values, it performs normal rounding.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Rounds the average of two integers based on a reference point. If the average ends with .5,
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
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
