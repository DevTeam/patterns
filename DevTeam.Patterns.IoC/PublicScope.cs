﻿namespace DevTeam.Patterns.IoC
{
    internal class PublicScope: IScope
    {
        public bool ReadyToRegister(bool isRoot)
        {
            return true;
        }

        public bool ReadyToResolve(bool isRoot, IResolver resolver)
        {
            return true;
        }
    }
}
