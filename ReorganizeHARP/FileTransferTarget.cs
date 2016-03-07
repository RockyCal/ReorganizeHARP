using System;
using System.IO;
using System.Collections.Generic;
using WinSCP;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ReorganizeHARP
{
    class FileTransferTarget
    {
        public String name;                        // target name
        public Session session;                    // session
        public String srcRelPath;                  // relative (to current transfer) path to target files
        public String srcParentPath;               // path to parent directory of target source
        public String srcDirName;                  // name of directory of target files
        public String fullSrcpath;                 // full path to where to find target files
        public String destParentPath;              // path to parent directory of target destination
        public String fullDestPath;                // full path to where to put target files
        public List<FileTransferRule> rules;      // rules for getting target files

        public FileTransferTarget(String name, Session session)
        {
            this.name = name;
            this.session = session;
            fullSrcpath = null;
            rules = new List<FileTransferRule>();
        }

        public FileTransferTarget(String name, Session session, String srcPath, String destPath)
        {
            this.name = name;
            this.session = session;
            fullSrcpath = srcPath;
            fullDestPath = destPath;
            rules = new List<FileTransferRule>();
        }

        public String getDirName()
        {
            return srcDirName;
        }

        public void setDirName(String directoryName)
        {
            srcDirName = directoryName;
        }

        public String getFullSrcPath()
        {
            return (fullSrcpath == null) ? fullSrcpath : srcParentPath + '/' + srcDirName;
        }

        public void setFullSrcPath(String parentPath, String dirName)
        {
            this.srcParentPath = parentPath;
            this.srcDirName = dirName;
            fullSrcpath = parentPath + '/' + dirName;
        }

        public void setFullSrcPath(String path)
        {
            fullSrcpath = path;
        }

        public void addRule()
        {
            Console.WriteLine("Set rule name: ");
            String ruleName = Console.ReadLine();
            Severity ruleSev = 0;
            Boolean ruleSevSet = false;
            while (!ruleSevSet)
            {
                Console.WriteLine("Set rule severity; Low(0), Medium(1), or High(2)?");
                String ruleSeverity = Console.ReadLine();
                try
                {
                    ruleSev = (Severity)Enum.Parse(typeof(Severity), ruleSeverity, true);
                    if (Enum.IsDefined(typeof(Severity), ruleSev))
                    {
                        Console.WriteLine("{0} rule severity is {1}", ruleName, ruleSev);
                        ruleSevSet = true;
                    }
                    else
                    {
                        Console.WriteLine("'{0}' is not a valid severity rating. Please enter 'Low', 'Medium', 'High;, '0', '1', or '2'", ruleSeverity);
                    }
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("'{0}' is not a valid severity rating. Please enter 'Low', 'Medium', 'High;, '0', '1', or '2'", ruleSeverity);
                }
            }
            FileTransferRule rule = new FileTransferRule(ruleName, ruleSev);
            rule.target = this;
            rule.setRulePath();
            rules.Add(rule);
        }

        public void checkTargetDestination(String parentPath, String currentTransfer)
        {
            // Create directory with source name and target name
            destParentPath = parentPath;
            fullDestPath = parentPath + '/' +  currentTransfer + '_' + name;
            if (Directory.Exists(fullDestPath))
            {
                Console.WriteLine("Existing target destination directory found. Will use for target files destination");
                Console.WriteLine(fullDestPath);
                return;
            }
            // Create target destination directory
            Console.WriteLine("Creating target destination directory");
            try
            {
                DirectoryInfo destDir = Directory.CreateDirectory(fullDestPath);
                Console.WriteLine("{0} created succesfully. Will use for transfer destination", destDir.Name);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not create directory: {0}", e.ToString());
                return;
            }
        }

        public int findTargetFiles()
        {
            // Find target files
            try
            {
                RemoteDirectoryInfo target = session.ListDirectory(fullSrcpath);
                Console.WriteLine("Transfer files: ");
                foreach(RemoteFileInfo file in target.Files)
                {
                    Console.WriteLine(file.Name);
                }
                Console.WriteLine("?");
                return 0;
            }
            catch (SessionRemoteException e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: ", e);
                return 1;
            }
        }
    }
}
