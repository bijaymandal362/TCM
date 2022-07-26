using System;

namespace Infrastructure.Helper.Exceptions
{
    [Serializable]
    public class EmptyListItemException : Exception
    {
        public EmptyListItemException(string name) : base($"{name} not found.")
        {
        }
    }
}