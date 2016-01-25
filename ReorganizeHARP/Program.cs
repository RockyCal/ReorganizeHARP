using System;
using WinSCP;
using System.Configuration;

namespace ReorganizeHARP
{
    class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                // Setup session
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = ConfigurationManager.AppSettings["hostName"],
                    UserName = ConfigurationManager.AppSettings["userName"],
                    Password = ConfigurationManager.AppSettings["password"],
                    SshHostKeyFingerprint = ConfigurationManager.AppSettings["sshHostKeyFingerprint"]
                };

                using (Session session = new Session())
                {
                    System.Diagnostics.Debug.Write("made it to here");
                    // Connect
                    session.Open(sessionOptions);

                    RemoteDirectoryInfo directory = session.ListDirectory("/home/cetus/shared/HARP^ Deployment^ and^ Recovery^ Files");
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        System.Diagnostics.Debug.Write("{0}", fileInfo.Name);
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write("Error: {0}", e.ToString());
                return 1;
            }
        }
    }
}
