using System.Collections;
using System.Runtime.Remoting.Channels;         
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting.Channels.Ipc;

namespace CrawlerContracts
{
    public class IpcServices
    {
        private static IpcChannel channel;
        public static bool Registered;
        public static object Mutex = new object();

        public static void RegisterChannel(string portName)
        {
            lock (Mutex)
            {
                if (Registered) return;

                var serverProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };
                var clientProvider = new BinaryClientFormatterSinkProvider();
                var properties = new Hashtable();
                properties["portName"] = portName;

                channel = new IpcChannel(properties, clientProvider, serverProvider);
                if (ChannelServices.GetChannel("ipc") == null)
                {
                    ChannelServices.RegisterChannel(channel, false);
                }
                else
                {
                    channel = ChannelServices.GetChannel("ipc") as IpcChannel;
                    channel.StartListening(new object());
                }
               
                Registered = true;
            }
        }

        public static void UnRegisterChannel()
        {
            lock (Mutex)
            {
                if (!Registered) return;   

                if (channel != null)
                    ChannelServices.UnregisterChannel(channel);

                Registered = false;
            }
        }
    }
}
