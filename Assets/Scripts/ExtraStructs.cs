[System.Serializable]
public struct WorldSize
{
    public float x;
    public float z;
    public WorldSize(float x = 0, float z = 0)
    {
        this.x = x;
        this.z = z;
    }
}