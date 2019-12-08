using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apsis.Abstractions;
using Apsis.Models.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Apsis.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        IContextProvider _contextProvidor;
        public UserController(IContextProvider contextProvidor)
        {
            _contextProvidor = contextProvidor;
        }

        [HttpGet]
        [Route("current")]
        public IActionResult GetCurrentUser()
        {
            ApplicationContext context = _contextProvidor.Context;
            if (context != null)
            {
                return StatusCode(200, context.CurrentUser);
            }

            return StatusCode(200, null);
        }
    }
}
