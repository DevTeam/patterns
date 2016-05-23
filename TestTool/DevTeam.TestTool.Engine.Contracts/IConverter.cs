namespace DevTeam.TestTool.Engine.Contracts
{
    public interface IConverter<in TSource, out TDestination>
    {
        TDestination Convert(TSource source);
    }
}
