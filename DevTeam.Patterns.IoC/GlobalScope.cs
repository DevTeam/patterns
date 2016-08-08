namespace DevTeam.Patterns.IoC
{
    internal class GlobalScope: IScope
    {
        public bool ReadyToRegister(bool isRoot)
        {
            return isRoot;
        }

        public bool ReadyToResolve(bool isRoot, IResolver resolver)
        {
            return isRoot;
        }
    }
}
