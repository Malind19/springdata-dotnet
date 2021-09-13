using Microsoft.AspNetCore.Mvc;
using System;

namespace springdata_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PulseController : ControllerBase
    {
        public string Get()
        {
            return DateTime.Now.ToString();
        }
    }
}
