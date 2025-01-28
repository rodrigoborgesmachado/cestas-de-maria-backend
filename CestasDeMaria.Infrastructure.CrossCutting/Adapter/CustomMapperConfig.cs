using AutoMapper;
using System.Reflection;

namespace CestasDeMaria.Infrastructure.CrossCutting.Adapter
{
    public static class CustomMapperConfig
    {
        private static MapperConfiguration _instance;

        public static MapperConfiguration Instance => _instance ?? (_instance = CreateConfig());

        private static MapperConfiguration CreateConfig()
        {
            var profiles = new List<Type>();

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in a.GetTypes())
                    {
                        if (type.BaseType == typeof(Profile))
                            profiles.Add(type);
                    }
                }
                catch (Exception) { }
            }

            var config = new MapperConfiguration(cfg =>
                profiles.ForEach(p => cfg.AddProfile(Activator.CreateInstance(p) as Profile))
            );

            return config;
        }
    }
}
