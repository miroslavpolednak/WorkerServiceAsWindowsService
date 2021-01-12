using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csob.Project.WindowsService.Helpers
{
    public static class DiHelper
    {
        public static List<Type> GetAllTypesThatImplement(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                             .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                             .Select(x => x).ToList();
        }
    }
}
