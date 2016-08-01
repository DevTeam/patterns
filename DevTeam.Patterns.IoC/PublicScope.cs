namespace DevTeam.Patterns.IoC
{
    internal class PublicScope: IScope
    {
        public bool Satisfy(IResolver resolver)
        {
            return true;
        }
    }
}
