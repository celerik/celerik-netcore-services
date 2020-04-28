using Celerik.NetCore.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Celerik.NetCore.Services.Test
{
    public class ServiceBaseTest : BaseTest
    {
        protected override void AddServices(IServiceCollection services)
        {
            var config = GetService<IConfiguration>();
            config["ServiceType"] = ApiServiceType.ServiceMock.GetDescription();

            services.AddCoreServices<ServiceBaseTest>(config)
                .AddAutomapper()
                .AddValidators(() =>
                {
                    services.AddValidator<PaginationRequest, PaginationRequestValidator<PaginationRequest>>();
                    services.AddValidator<PaginationRequest<Cat>, PaginationRequestValidator<PaginationRequest<Cat>, Cat>>();
                })
                .AddBusinesServices(config =>
                {
                    //services.AddTransient<ICalculatorService, CalculatorService>();
                });
        }
    }
}
