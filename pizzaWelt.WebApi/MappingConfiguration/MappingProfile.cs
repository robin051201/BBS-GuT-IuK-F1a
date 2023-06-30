namespace PizzaWelt.MappingConfiguration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ConfigureMappings();
        }

        private void ConfigureMappings()
        {
            var dtoTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(IsMappedDto)
                .ToList();

            foreach (var dtoType in dtoTypes)
            {
                var mappedType = GetMappedType(dtoType);

                if (mappedType != null)
                {
                    CreateMap(mappedType, dtoType).ReverseMap();
                }
            }
        }

        private static bool IsMappedDto(Type type)
        {
            return type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IMappedDto<>)));
        }

        private static Type? GetMappedType(Type dtoType)
        {
            return dtoType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IMappedDto<>)))
                ?.GetGenericArguments()
                .FirstOrDefault();
        }
    }
}
