using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qckdev.AspNetCore.Identity.Services
{
    static class CustomizableActionHelper
    {

        public static IEnumerable<ICustomizableAction> GetCustomizers<TParameter, TValue>(IServiceProvider services)
        {
            return GetCustomizers<TParameter, TParameter, TValue>(services);
        }

        public static IEnumerable<ICustomizableAction> GetCustomizers<TParameter, TParameterBase, TValue>(IServiceProvider services)
            where TParameter : TParameterBase
        {
            return GetCustomizers<TParameter, TParameterBase>(services, typeof(TValue));
        }

        public static IEnumerable<ICustomizableAction> GetCustomizers<TParameter>(IServiceProvider services, Type valueType)
        {
            return GetCustomizers<TParameter, TParameter>(services, valueType);
        }

        public static IEnumerable<ICustomizableAction> GetCustomizers<TParameter, TParameterBase>(IServiceProvider services, Type valueType)
            where TParameter : TParameterBase
        {
            var types = new List<Type>();
            var lastType = typeof(TParameter);
            var customizationServices = new List<ICustomizableAction>();

            while (lastType != null && typeof(TParameterBase).IsAssignableFrom(lastType))
            {
                types.Add(lastType);
                lastType = lastType.BaseType;
            }
            types.Reverse();

            foreach (var type in types)
            {
                customizationServices.AddRange(
                    services
                        .GetServices(
                            typeof(ICustomizableAction<,>).MakeGenericType(type, valueType))
                        .Select(x => (ICustomizableAction)x)
                );
            }
            return customizationServices;
        }

    }
}
