using ChocoLuxAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public ResourceController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        [HttpGet("GetControllers")]
        public IActionResult GetControllers()
        {
            var controllerInfos = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .GroupBy(ad => ad.RouteValues["controller"])
                .Select(g => new ControllerInfoDto
                {
                    ControllerName = g.Key,
                    Actions = g.Select(ad => new ActionInfoDto { ActionName = ad.RouteValues["action"] }).ToList()
                }).ToList();

            return Ok(controllerInfos);
        }
    }
}
