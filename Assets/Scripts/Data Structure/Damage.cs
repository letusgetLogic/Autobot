public struct Damage
{
    public int Value;

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
