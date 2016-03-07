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
        public static void connectToDb()
        {
            // Setup data connection (for cross-referencing ftp files with database tuples)
            string dbServer = ConfigurationManager.AppSettings["dbServerName"];
            string dbName = ConfigurationManager.AppSettings["dbName"];
            string dbUsername = ConfigurationManager.AppSettings["dbUserName"];
            string dbPassword = ConfigurationManager.AppSettings["dbPassword"];

            string template = "Data Source={0};Initial Catalog={1};User ID={2};Password=";
            string connectionString = string.Concat(string.Format(template, dbServer, dbName, dbUsername), dbPassword);

            System.Diagnostics.Debug.WriteLine(connectionString);
            SqlConnection cnn = new SqlConnection(connectionString);
            // Connect to DB
            try
            {
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
        }
        
        public static int Main(string[] args)
        {
            //connectToDb();
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
                    // TODO: make source path editable
                    String path = "/home/cetus/shared/HARP Deployment and Recovery Files";
                    Console.WriteLine("Source path is {0}", path);
                    RemoteDirectoryInfo directory = session.ListDirectory(path);
                    List<RemoteFileInfo> sourceFiles = new List<RemoteFileInfo>(); // to hold file names that will be sorted
                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        if (!(Regex.IsMatch(fileInfo.Name, @"^\.")) && !(Regex.IsMatch(fileInfo.Name, @"^\d")) && fileInfo.IsDirectory)
                        {
                            sourceFiles.Add(fileInfo);
                        }
                    }
                    Console.WriteLine("Files found, processing and sorting.");

                    // Sort files alphabetically
                    sourceFiles.Sort(delegate (RemoteFileInfo x, RemoteFileInfo y)
                    {
                        if (x.Name == null && y.Name == null) return 0;
                        else if (x.Name == null) return -1;
                        else if (y.Name == null) return 1;
                        else return y.Name.CompareTo(x.Name);
                    });

                    // Destination path of where the directories holding the targets willbe temporarily held until transferred back to session
                    String destPath = "C:/Users/Harp/Desktop/temp";
                    // TODO: make destination path editable
                    Console.WriteLine("Destination path is {0}", destPath);
                    FileTransferManager fileTransfer = new FileTransferManager(path, destPath, session);
                    Boolean doneTargets = false;
                    while (!doneTargets)
                    {
                        Console.WriteLine("Create new target? (Y/N)");
                        String response = Console.ReadLine();
                        switch (response)
                        {
                            case "Y":
                                fileTransfer.createNewTarget();
                                break;
                            case "N":
                                doneTargets = true;
                                break;
                            default:
                                break;
                        }
                    }
                    Boolean done = false;
                    int i = 0;
                    while (!done)
                    {
                        Console.WriteLine("[E]xit [Y]es [Any key to continue]");
                        RemoteFileInfo aSourceFile = sourceFiles[i];
                        Console.WriteLine("Would you like to organize {0}(Y/N)?", aSourceFile.Name);
                        String answer = Console.ReadLine();
                        switch (answer)
                        {
                            case "Y":
                                fileTransfer.setCurrentTransferSrc(aSourceFile); // set the path to the current source 
                                fileTransfer.checkDestinationDir();              // set path to destination of transfer
                                fileTransfer.setTargetSrcPaths();                // set the paths of the targets
                                fileTransfer.configureTargetRules();             // configure target rules
                                fileTransfer.transferTargets();                  // transfer the target files
                                break;
                            case "N":
                                break;
                            case "E":
                                done = true;
                                break;
                            default:
                                continue;
                        }
                        i++;
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
