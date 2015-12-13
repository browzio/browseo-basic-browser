using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerContracts
{
    public class AssemblyResolver
    {
        private string _thisAssemblyName;
        private string _interfacesAssemblyName;
        public void Setup()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            _thisAssemblyName = GetType().Assembly.GetName().Name;
            _interfacesAssemblyName = typeof(IHost).Assembly.GetName().Name; 
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            if (name.Name == _thisAssemblyName) return GetType().Assembly;
            if (name.Name == _interfacesAssemblyName) return typeof(IHost).Assembly;

            return null;
        }
    }
}
