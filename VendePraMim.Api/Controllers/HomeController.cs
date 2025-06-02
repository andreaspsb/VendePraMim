using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Hello World!");
}
