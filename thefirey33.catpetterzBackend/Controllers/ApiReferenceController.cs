using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace thefirey33.catpetterzBackend.Controllers;

[Route("/api/[controller]")]
public class ApiReferenceController(
    ILogger<ApiReferenceController> logger,
    IActionDescriptorCollectionProvider actionDescriptorProvider) : Controller
{
    /// <summary>
    ///     This will get the specified API reference for CatPetterz.
    /// </summary>
    public IActionResult GetHtml()
    {
        var result = actionDescriptorProvider.ActionDescriptors
            .Items
            .OfType<ControllerActionDescriptor>()
            .Select(d =>
                {
                    var str = new StringBuilder();

                    // The base information about the route.
                    str.Append(
                        $"""
                         <h3>CONTROLLER {d.ControllerName}: ACTION {d.ActionName}</h3>
                         <p>
                             Route: <strong>{d.AttributeRouteInfo?.Template}</strong>
                         </p>
                         """
                    );

                    // Each parameter that the route has, will be displayed here.
                    foreach (var parameterDescriptor in d.Parameters)
                        str.Append(
                            $"<em>Parameter: {parameterDescriptor.Name}, type: <strong>{parameterDescriptor.ParameterType.Name}</strong></em>");

                    str.Append("<hr/>");

                    return str.ToString();
                }
            )
            .ToList();

        result.Insert(0, "<title>CatPetterz API Reference</title>");
        return Content(string.Join("\n", result), "text/html");
    }
}