public class ModifierValue
{
    public ModifierValue()
    {
        AdditiveValue = 0;
        MultiplicativeValue = 1;
    }

    public ModifierValue(float additiveValue, float multiplicativeValue)
    {
        AdditiveValue = additiveValue;
        MultiplicativeValue = multiplicativeValue;
    }

    public float AdditiveValue;
    public float MultiplicativeValue;
}
