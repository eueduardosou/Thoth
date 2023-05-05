using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Thoth.Core.Interfaces;
using Thoth.Core.Models.Entities;

namespace Thoth.Dashboard.Api;

public static class RoutesCollection
{
    public static IApplicationBuilder InjectThothDashboardRoutes(
        this IApplicationBuilder app,
        ThothDashboardOptions thothDashboardOptions)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            var basePath = $"{thothDashboardOptions.RoutePrefix}-api/FeatureFlag";
            var featureManagementService = endpoints.ServiceProvider.GetRequiredService<IThothFeatureManager>();
            var logger = endpoints.ServiceProvider.GetRequiredService<ILogger<FeatureManagerController>>();
            var controller = new FeatureManagerController(featureManagementService, logger, thothDashboardOptions);
            
            #region GET

            endpoints.MapGet(basePath, async () =>
            {
                return await controller.GetAll();
            });

            endpoints.MapGet(basePath+ "/{name}", async (string name) =>
                await controller.GetByName(name));

            #endregion

            #region POST

            endpoints.MapPost(basePath, async ([FromBody] FeatureManager featureFlag) =>
                await controller.Create(featureFlag));

            #endregion

            #region PUT

            endpoints.MapPut(basePath, async ([FromBody] FeatureManager featureFlag) =>
                await controller.Update(featureFlag));

            #endregion

            #region DELELTE

            endpoints.MapDelete(basePath + "/{name}", async (string name) =>
                await controller.Delete(name));

            #endregion
        });

        return app;
    }
}