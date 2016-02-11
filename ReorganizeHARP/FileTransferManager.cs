using System;
using WinSCP;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReorganizeHARP
{
    class FileTransferManager
    {
        public String path;
        public RemoteFileInfo expedition;  // the expedition as a file info object
        public Session session;
        public RemoteDirectoryInfo exp;    // the expedition directory
        public String expPath;             // the path to the expedition directory

        public FileTransferManager(String srcPath, Session ftpSession, RemoteFileInfo expeditionFile)
        {
            path = srcPath;
            session = ftpSession;
            expedition = expeditionFile;
            expPath = path + '/' + expedition.Name;
            // Select expedition directory
            exp = session.ListDirectory(expPath);
        }
        
        public void createNewTarget()
        {
            Console.WriteLine("Set target name: ");
            String targetName = Console.ReadLine();
            //Console.WriteLine("Set target directory name (the name of the directory where the files you want are hosted):");
            //String dirName = Console.ReadLine();
            Console.WriteLine("Set target path (where to find your target's files) ex: deploy/dl32: ");
            String targetPath = Console.ReadLine();
            FileTransferTarget target = new FileTransferTarget(targetName, session);
            String fullpath = expPath + '/' + targetPath;
            target.setFullPath(fullpath);
            target.findTargetFiles();
        }
    }
}
