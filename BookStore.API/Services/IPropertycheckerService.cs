namespace BookStore.API.Services
{
    public interface IPropertycheckerService
    {
        bool TypeHasProperties<T>(string fields);
    }
}