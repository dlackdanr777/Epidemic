public class BuildObject : Barricade
{
    private BuildData _buildData;
    public BuildData BuildData => _buildData;


    public void SetData(BuildData buildData)
    {
        _buildData = buildData;
        _maxHp = buildData.Hp;
        SetHp(_maxHp);
    }


    protected override void OnHpMinEvcent()
    {
        ObjectPoolManager.Instance.DespawnBuildObject(this);
    }
}
