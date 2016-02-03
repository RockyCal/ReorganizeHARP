using System;
using WinSCP;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ReorganizeHARP
{
    class Program
    {
        public static int Main(string[] args)
        {
            /*
            // Setup data connection (for cross-referencing ftp files with database tuples)
            string dbServer = ConfigurationManager.AppSettings["dbServerName"];
            string dbName = ConfigurationManager.AppSettings["dbName"];
            string dbUsername = ConfigurationManager.AppSettings["dbUserName"];
            string dbPassword = ConfigurationManager.AppSettings["dbPassword"];

            string template = "Data Source={0};Initial Catalog={1};User ID={2};Password=";
            string connectionString = string.Concat(string.Format(template, dbServer, dbName, dbUsername), dbPassword);

            System.Diagnostics.Debug.WriteLine(connectionString);
            SqlConnection cnn = new SqlConnection(connectionString);
            try
            {
                // Connect to DB
                cnn.Open();
                Console.WriteLine("Attempting to connect to SQL server...");
                System.Diagnostics.Debug.WriteLine("Connection Open!");
                cnn.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Can not open connection!");
                System.Diagnostics.Debug.WriteLine("{0}", ex);
            }
            */
            Console.WriteLine("Let's reorganize some files!");
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
                    Console.WriteLine("Connected to session.");
                    RemoteDirectoryInfo directory = session.ListDirectory("/home/cetus/shared/HARP Deployment and Recovery Files");
                    List<RemoteFileInfo> files = new List<RemoteFileInfo>(); // to hold file names that will be sorted
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        if (!(Regex.IsMatch(fileInfo.Name, @"^\.")) && !(Regex.IsMatch(fileInfo.Name, @"^\d")))
                        {
                            files.Add(fileInfo);
                        }
                    }
                    Console.WriteLine("Files found, processing and sorting");

                    // Sort files alphabetically
                    files.Sort(delegate (RemoteFileInfo x, RemoteFileInfo y)
                    {
                        if (x.Name == null && y.Name == null) return 0;
                        else if (x.Name == null) return -1;
                        else if (y.Name == null) return 1;
                        else return y.Name.CompareTo(x.Name);
                    });

                    foreach (RemoteFileInfo fileInfo in files)
                    {
                        Console.WriteLine("Would you like to organize {0}(Y/N)?", fileInfo.Name);
                        String answer = Console.ReadLine();
                        if(answer == "Y")
                        {
                            Console.WriteLine(fileInfo.Name);
                        }
                        else
                        {
                            Console.WriteLine("bye!");
                            break;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: {0}", e);
                return 1;
            }
        }
    }
}
