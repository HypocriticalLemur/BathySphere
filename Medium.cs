public interface Medium{
    public float DENSITY { get; }
    public float DYNAMIC_VISCOUSITY { get; }
}

public struct Water : Medium
{
    public float DENSITY => 1100f;
    public float DYNAMIC_VISCOUSITY => 1788e-6f;
}
