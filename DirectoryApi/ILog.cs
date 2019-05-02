using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryApi
{
    public interface ILog
    {
        void Add(string message, bool error = false);
    }
}
