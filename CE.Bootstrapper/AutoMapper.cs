using AutoMapper;
using CE.Bootstrapper.Mapper;
using System.Linq;

namespace CE.Bootstrapper
{
    public static class AutoMapper
    {
        public static void Configure(IMapperConfigurationExpression config)
        {
            // Create mapping between ENTITY and DTO
            // Service layer should care only about DTO, not ENTITY. AutoMapper will be the middle man that do the mapping between them
            // *** NOTES: 
            // - All mapping from ENTITY to DTO will be used on EF query. Unrecoginzied C# functions must not be used on mapping expressions.
            // - The mapping should be created manually, not auto-mapped by property/field with the same name (default AutoMapper's behaviour). We should use IgnoreAll method and then manually map the properties we want

            AdministrationMapper.Configure(config);
            MasterDataMapper.Configure(config);
            LogMapper.Configure(config);
        }

        public static IMappingExpression<TSource, TDestination> IgnoreAll<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            foreach (var property in typeof(TDestination).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetProperty).Where(x => x.CanWrite))
            {
                expression.ForMember(property.Name, opt => opt.Ignore());
            }
            return expression;
        }
    }
}

