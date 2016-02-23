using System;
using WinSCP;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReorganizeHARP
{
    class FileTransferManager
    {
        public String sourcePath;
        public String destinationPath;
        public RemoteFileInfo currentTransfer;                 // the current source for transfer as a file info object
        public Session session;
        public RemoteDirectoryInfo currentTransferDir;        // the current source as a directory
        public String currentTransferSrcPath;                 // the path to the current transfer source
        public String currentTransferDestPath;                // the path to the current transfer destination
        public List<FileTransferTarget> targets;

        public FileTransferManager(String srcPath, String destPath, Session ftpSession)
        {
            sourcePath = srcPath;
            destinationPath = destPath;
            session = ftpSession;
            targets = new List<FileTransferTarget>();
            //expedition = expeditionFile;
            //expPath = sourcePath + '/' + expedition.Name;
            // Select expedition directory
            //exp = session.ListDirectory(expPath);
        }

        public void setCurrentTransferSrc(RemoteFileInfo srcFile)
        {
            currentTransfer = srcFile;
            currentTransferSrcPath = sourcePath + '/' + srcFile.Name;
            // Select expedition directory
            currentTransferDir = session.ListDirectory(currentTransferSrcPath);
        }

        public void checkDestinationDir()
        {
            if (!(Directory.Exists(destinationPath)))
            {
                Console.WriteLine("Can't find destination directory.");
                Console.WriteLine("Set new destination path");
                destinationPath = Console.ReadLine();
            }
            Console.WriteLine("Name of directory to hold all targets: ");
            String destName = Console.ReadLine();
            currentTransferDestPath = destinationPath + '/' + destName;
            if (Directory.Exists(currentTransferDestPath))
            {
                Console.WriteLine("{0} destination directory exists. Will use for transfer destination.", destName);
                return;
            }

            Console.WriteLine("Creating new directory for current transfer.");
            try
            {
                DirectoryInfo dir = Directory.CreateDirectory(currentTransferDestPath);
                Console.WriteLine("{0} created succesfully. Will use for transfer destination", dir.Name);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not create directory: {0}", e.ToString());
                return;
            }

        }

        public void createNewTarget()
        {
            Console.WriteLine("Set target name: ");
            String targetName = Console.ReadLine();
            FileTransferTarget target = new FileTransferTarget(targetName, session);
            targets.Add(target);
        }

        public FileTransferTarget getTargetByName(String name)
        {
            return targets.Find(x => x.name.Equals(name));
        }

        public void setTargetSrcPath(String name)
        {
            FileTransferTarget target;
            try
            {
                target = getTargetByName(name);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Could not find target with name {0}", name);
                return;
            }
            // TODO: change lines below so that program simply finds "deploy" or "Deploy"
            //       then cross references with DB to find which datalogger folder it is and  gets files from there
            Console.WriteLine("Set target path (where to find your target's files) e.g. deploy/dl32 for {0}: ", target.name);
            String targetPath = Console.ReadLine();
            String fullpath = currentTransferSrcPath + '/' + targetPath;
            target.setFullSrcPath(fullpath);
            return;
        }

        public void setTargetSrcPaths()
        {
            foreach(FileTransferTarget target in targets)
            {
                setTargetSrcPath(target.name);
            }
        }

        public void getTarget(String name)
        {
            Console.WriteLine("Transfering files for {0}", name);
            FileTransferTarget target;
            try
            {
                target = getTargetByName(name);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Could not find target with name {0}", name);
                return;
            }
            target.checkTargetDestination(currentTransferDestPath, currentTransfer.Name);
            target.findTargetFiles();
        }

        public void transferTargets()
        {
            foreach(FileTransferTarget target in targets)
            {
                getTarget(target.name);
            }
        }
    }
}
