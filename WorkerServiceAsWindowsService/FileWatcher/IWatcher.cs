using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService.FileWatcher
{
    public interface IWatcher
    {
        Task StartWatching();
    }
}
