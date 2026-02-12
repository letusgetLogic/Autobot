public struct Damage
{
    public int Value;

    /// <summary>
    /// Get the damage value in parameter and return the valid value.
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
