using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Steganography.Services
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync(string viewPath, object model);
    }

    public class RazorViewToStringRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider) : IRazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine = viewEngine;
        private readonly ITempDataProvider _tempDataProvider = tempDataProvider;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task<string> RenderViewToStringAsync(string viewPath, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var viewEngineResult = _viewEngine.GetView(null, viewPath, false);
            if (!viewEngineResult.Success)
                throw new InvalidOperationException($"Couldn't find view: {viewPath}");

            var view = viewEngineResult.View;

            using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(httpContext, _tempDataProvider),
                output,
                new HtmlHelperOptions()
            );

            await view.RenderAsync(viewContext);
            return output.ToString();
        }
    }
}
