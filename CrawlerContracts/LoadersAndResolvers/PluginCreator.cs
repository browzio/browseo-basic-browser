using System;
using System.Reflection;
using System.Windows;

namespace CrawlerContracts
{
    internal static class PluginCreator
    {
        public static object CreatePlugin(string assemblyName, string typeName, Contract.IHostToPluginContract host)
        {
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeName);

            if (type == null) throw new InvalidOperationException("Could not find type " + typeName + " in assembly " + assemblyName);

            SetupWpfApplication(assembly);
            var hostConstructor = type.GetConstructor(new[] { typeof(Contract.IHostToPluginContract) });
            if (hostConstructor != null)
            {
                return hostConstructor.Invoke(new object[] { host });
            }

            var defaultConstructor = type.GetConstructor(new Type[0]);
            if (defaultConstructor == null)
            {
                var message = String.Format("Cannot create an instance of {0}. Either a public default constructor, or a public constructor taking ISendToHost must be defined", typeName);
                throw new InvalidOperationException(message);
            }

            return defaultConstructor.Invoke(null);
        }

        private static void SetupWpfApplication(Assembly assembly)
        {
            var application = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            Application.ResourceAssembly = assembly;
        }
    }
}