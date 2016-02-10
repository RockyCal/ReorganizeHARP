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
        public RemoteFileInfo expedition;
        public Session session;

        public FileTransferManager(String ppath, Session psession, RemoteFileInfo pexpedition)
        {
            path = ppath;
            expedition = pexpedition;
            session = psession;
        }
        public void getFiles()
        {
            String expPath = path + '/' + expedition.Name;
            // Select expedition directory
            RemoteDirectoryInfo exp = session.ListDirectory(expPath);
            // Find deployment files
            String deployPath = expPath + "/deploy";
            try
            {
                RemoteDirectoryInfo expDeployment = session.ListDirectory(deployPath);
            }
            catch (SessionRemoteException e)
            {
                try
                {
                    deployPath = expPath + "/Deploy";
                    RemoteDirectoryInfo expDeployment = session.ListDirectory(deployPath);
                }
                catch (SessionRemoteException ex)
                {

                }
            }
            // Find recovery files
            String recoveryPath = expPath + "/recovery";
            RemoteDirectoryInfo expRecovery = session.ListDirectory(recoveryPath);
        }
    }
}
