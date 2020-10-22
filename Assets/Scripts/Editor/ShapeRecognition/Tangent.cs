public struct Tangent
{
    public Tangent(int first, int second)
    {
        this.first = first;
        this.second = second;
    }

    private int first, second;

    public bool TryGetFirst(out int index)
    {
        if (first != -1)
        {
            index = first;
            return true;
        }

        index = -1;
        return false;
    }
    public bool TryGetSecond(out int index)
    {
        if (second != -1)
        {
            index = second;
            return true;
        }

        index = -1;
        return false;
    }

    public override string ToString() => $"{first} / {second}";
}