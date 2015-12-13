using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerContracts
{
   public class PluginLoaderBootstrapper : MarshalByRefObject
    {
        public void Run(string name)
        {
            new RemotePlugin().Run(name);
        }
    }
}
