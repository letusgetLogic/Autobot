/// <summary>
/// Return non-negative value.
/// </summary>
public struct Damage
{
    public int Value;

    /// <summary>
    /// Get the damage value in parameter and return non-negative value.
    /// </summary>
    /// <param name="_value"></param>
    public Damage(int _value)
    {
        if (_value < 0)
        {
            Value = 0;
            return;
        }

        Value = _value;
    }
}
