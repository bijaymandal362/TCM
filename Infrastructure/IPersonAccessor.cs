namespace Infrastructure
{
    public interface IPersonAccessor
    {
         int GetPersonId();
         bool IsPersonAuthenticated();
    }
}