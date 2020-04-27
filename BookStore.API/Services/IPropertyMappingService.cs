using System.Collections.Generic;

namespace BookStore.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ValidMappingExistingFor<TSource, TDestination>(string fields);
    }
}