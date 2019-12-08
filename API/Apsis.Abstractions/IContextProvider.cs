using Apsis.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Abstractions
{
    public interface IContextProvider
    {
        ApplicationContext Context { get; }
    }
}
