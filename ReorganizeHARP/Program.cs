using System;
using WinSCP;
using System.Configuration;
using System.Collections.Generic;
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
                    List<RemoteFileInfo> files = new List<RemoteFileInfo>(); // to hold file names that will be sorted
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        if (!(Regex.IsMatch(fileInfo.Name, @"^\.")) && !(Regex.IsMatch(fileInfo.Name, @"^\d")))
                        {
                            files.Add(fileInfo);
                        }
                    }
                    // Sort files alphabetically
                    files.Sort(delegate (RemoteFileInfo x, RemoteFileInfo y)
                    {
                        if (x.Name == null && y.Name == null) return 0;
                        else if (x.Name == null) return -1;
                        else if (y.Name == null) return 1;
                        else return x.Name.CompareTo(y.Name);
                    });
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
