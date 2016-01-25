using System;
using WinSCP;
using System.Configuration;

namespace ReorganizeHARP
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Setup session
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = ConfigurationManager.AppSettings["hostName"],
                    UserName = ConfigurationManager.AppSettings["userName"],
                    Password = ConfigurationManager.AppSettings["password"]
                };

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    RemoteDirectoryInfo directory = session.ListDirectory("/home/cetus/shared/HARP^ Deployment^ and^ Recovery^ Files");
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        Console.WriteLine("{0}", fileInfo.Name);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
        }
    }
}
