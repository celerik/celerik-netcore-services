using AutoMapper;
using Celerik.NetCore.Util;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Celerik.NetCore.Services.Test
{
    public class ServiceBaseTest : BaseTest
    {
        protected override void AddServices(IServiceCollection services)
        {
            services.AddBaseServices();
            services.AddSingleton(new MapperConfiguration(c => { }).CreateMapper());
            services.AddTransient<ApiServiceArgs<ServiceBaseTest>>();
            services.AddTransient<ICalculatorService, CalculatorService>();
            services.AddTransient<IValidator<PaginationRequest>, PaginationRequestValidator<PaginationRequest>>();
        }
    }
}
