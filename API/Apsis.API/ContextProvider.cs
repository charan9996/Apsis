using Apsis.Abstractions;
using Apsis.Models.Authorization;
using Microsoft.AspNetCore.Http;

namespace Apsis.API
{
    public class ContextProvider : IContextProvider
    {
        /// <summary>
        /// Http Context accesor to be used to read the http context for authorization
        /// </summary>
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Http context object
        /// </summary>
        private HttpContext _context => _httpContextAccessor.HttpContext;

        public ContextProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Application context
        /// </summary>
        public ApplicationContext Context
        {
            get
            {
                if (_context != null)
                {
                    var _appContext = _context.Items["ApplicationContext"];

                    if (_appContext != null)
                    {
                        return _appContext as ApplicationContext;
                    }
                }

                return null;
            }
        }
    }
}
