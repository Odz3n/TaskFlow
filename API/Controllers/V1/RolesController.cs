using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.API.Controllers.V1;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    
}
