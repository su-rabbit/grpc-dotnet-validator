using System;
using System.Linq;
using FluentValidation;
using Grpc.AspNetCore.FluentValidation.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.AspNetCore.FluentValidation
{
    public static class ServiceCollectionHelper
    {
        public static IServiceCollection AddValidatorLocator(this IServiceCollection services)
        {
            return services.AddScoped<IValidatorLocator>(provider =>
                new ServiceCollectionValidationProvider(provider));
        }

        public static IServiceCollection AddValidator<TValidator>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Scoped) where TValidator : class
        {
            var implementationType = typeof(TValidator);
            var validatorType = implementationType.GetInterfaces().FirstOrDefault(t =>
                t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>));

            if (validatorType == null)
                throw new AggregateException(implementationType.Name + "is not implement with IValidator<>.");

            var messageType = validatorType.GetGenericArguments().First();
            var serviceType = typeof(IValidator<>).MakeGenericType(messageType);

            services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            return services;
        }
    }
}