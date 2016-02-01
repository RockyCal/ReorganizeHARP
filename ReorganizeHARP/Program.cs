using System;
using WinSCP;
using System.Configuration;
using System.Text.RegularExpressions;

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
                    // Connect
                    session.Open(sessionOptions);

                    RemoteDirectoryInfo directory = session.ListDirectory("/home/cetus/shared/HARP Deployment and Recovery Files");
                    int numFiles = directory.Files.Count - 2; // Files.Count includes "." and ".." references
                    System.Diagnostics.Debug.WriteLine(numFiles);
                    RemoteFileInfo[] files = new RemoteFileInfo[numFiles]; // to hold file names that will be sorted
                    int idx = 0;
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        if (!(Regex.IsMatch(fileInfo.Name, @"^\.")))
                        {
                            files[idx] = fileInfo;
                            idx++;
                        }
                    }
                    // Sort files alphabetically
                    Array.Sort(files, (x, y) => String.Compare(x.Name, y.Name));
                    foreach (RemoteFileInfo fileInfo in files)
                    {
                        System.Diagnostics.Debug.WriteLine(fileInfo.Name);
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: {0}", e);
                return 1;
            }
        }
    }
}
