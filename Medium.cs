public abstract class Medium{
    public abstract const float DENSITY;
    public abstract const float DYNAMIC_VISCOUSITY;
}

public class Water : Medium
{
    public override const float DENSITY = 1100f;
    public override const float DYNAMIC_VISCOUSITY = 1788e-6f;
}
