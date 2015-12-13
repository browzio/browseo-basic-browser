using CrawlerContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerProcess
{
    class Program
    {
        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting PluginProcess {0}, {1} bit", args);

                var name = args[0];
                int bits = IntPtr.Size * 8;
                Console.WriteLine("Starting PluginProcess {0}, {1} bit", name, bits);

                var assemblyPath = args[1];
                Console.WriteLine("Plugin assembly: {0}", assemblyPath);

                if (CheckFileExists(assemblyPath))
                {
                    var configFile = GetConfigFile(assemblyPath);

                    var appBase = Path.GetDirectoryName(assemblyPath);

                    var appDomain = CreateAppDomain(appBase, configFile);
                    var bootstrapper = CreateInstanceFrom<PluginLoaderBootstrapper>(appDomain);
                    bootstrapper.Run(name);
                }
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("CrawlerProcess " + ex.Message);
                Console.ReadLine();
            }
        }
                                                                                                                                               
        private static T CreateInstanceFrom<T>(AppDomain appDomain)
        {
            return (T)appDomain.CreateInstanceFromAndUnwrap(typeof(T).Assembly.Location, typeof(T).FullName);
        }

        private static bool CheckFileExists(string path)
        {
            // if (!File.Exists(path)) throw new InvalidOperationException("File '" + path + "' does not exist");
            return File.Exists(path);
        }

        private static string GetConfigFile(string assemblyPath)
        {
            var name = assemblyPath + ".config";
            return File.Exists(name) ? name : null;
        }

        private static AppDomain CreateAppDomain(string appBase, string config)
        {
            var setup = new AppDomainSetup
            {
                ApplicationBase = appBase,
                ConfigurationFile = String.IsNullOrEmpty(config) ? null : config
            };

            return AppDomain.CreateDomain("PluginDomain", null, setup);
        }
    }
}
