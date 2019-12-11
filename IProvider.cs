using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grab
{
    interface IProvider
    {
        IEnumerable<Task<(string, bool, string)>> DownloadPackages(string[] packages);
    }
}