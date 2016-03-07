using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReorganizeHARP
{
    enum Severity : int { Low = 0, Medium = 1, High = 2 }

    /*
     * Severity of rule indicates how the violation of the rule 
     * should affect the transfer.
     *
     * Low: Indicates that breaking the rule is inconsequential.
     *      In other words, the breaking of or the inability
     *      to fulfill this rule's requirements should not stop
     *      the transfer of target files and the rule violation
     *      should not be logged.
     *      For example, say the user is transferring files having
     *      to do with famous singers where, in the source location,
     *      each singer has thier own directory. For each singer, the 
     *      user might have targets like "Performed", "Written", and 
     *      "Awards".
     *      The user could set up one low severity rule for the 
     *      "Awards" target that dictates that we should look for 
     *      a file called Grammy.txt (it's a text file because bare 
     *      with me, I am having a hard time coming up with a good 
     *      example) to put in the "Awards" directory when the files 
     *      are transferred. Not all actors famous singers have 
     *      Grammys though! So if the program is transferring a 
     *      singer's files and it does not find the Grammy.txt file 
     *      for the current actor it is working on that's okay, 
     *      this is perfectly normal so it should just continue with 
     *      the transfer. This rule is mainly here to say that if
     *      it can be followed, it should but it's no problem if it
     *      can't be. 
     * Medium: Indicates that breaking the rule is something the 
     *         user should know about but should not stop the transfer
     *         of the target files.
     *         Resuing the singers example, the user could have a
     *         target named "About" and a medium rule for this target 
     *         could be that the program should look for a file 
     *         called "Biography.txt" to put in the "About" target 
     *         destination directory. If the program doesn't find
     *         the "Biography.txt" file though, the user does not
     *         want the transfer to stop because they still want 
     *         the performed, written and awards files. The user does 
     *         want to know about it though because they consider the 
     *         "Biography.txt" files fairly important and they may want 
     *         to look for the file manually later or to create one.
     *         The breaking of this rule is added to the log that
     *         the program outputs at the end.
     * High: This indicates that breaking the rule or being
     *       unable to fulfill it should prevent the transfer from
     *       completing and the user should be notified.
     *       The user, logically, thinks that for a singer to be 
     *       considered a singer (at least in thsi context) there
     *       should be at least one mp3 file in the source directory
     *       for the singer. If the program cannot find at least one
     *       mp3 file though, there is something wrong with these 
     *       files and the user needs to be notified and the transfer
     *       should not be completed. The program cancels the transfer
     *       and adds the breaking of the rule (along with its severity
     *       and the fact that the transfer was not performed) to the log.
     *       
     */
    class FileTransferRule
    {
        public String name;                 // rule name
        public FileTransferTarget target;   // target rule is associated with
        public Severity severity;           // hardness of rule
        public String path;                 // path to directory/file needed to follow rule, if target source path does not have it
        public Boolean fulfilled;
        public Conditions conditions;

        public struct Conditions
        {
            public Boolean isDir;
            public String nameScheme;
            public Boolean isNameScheme;
            public String extension;
            public int quantity;

            public Conditions(Boolean isDirectory, String nameSchm, Boolean isNScheme, String ext, int qty)
            {
                isDir = isDirectory;
                nameScheme = nameSchm;
                isNameScheme = isNScheme;
                extension = ext;
                quantity = qty;
            }
        }

        public FileTransferRule(String ruleName, Severity ruleSeverity)
        {
            name = ruleName;
            severity = ruleSeverity;
            conditions = new Conditions();
        }

        public FileTransferRule(String name, Severity severity, String path)
        {
            this.name = name;
            this.severity = severity;
            this.path = path;
            conditions = new Conditions();
        }

        public void configure()
        {
            setRulePath();
        }

        public void setRulePath()
        {
            Console.WriteLine("Does this rule require a source path different from the target source path {0} (Y/N)?", target.fullSrcpath);
            String response = Console.ReadLine();
            switch (response)
            {
                case "Y":
                    Console.WriteLine("Set rule path: ");
                    path = Console.ReadLine();
                    break;
                case "N":
                    path = target.fullSrcpath;
                    Console.WriteLine("Rule path sent to {0}", path);
                    break;
                default:
                    break;
            }
        }

        public void setConditions()
        {
            Console.WriteLine("Set conditions for {0}'s rule {1}", target, name);
            Console.WriteLine("Is the goal of this rule to ensure transfer of a (D)irectory or a single (F)ile?");
            String fileOption = Console.ReadLine();
            Boolean isDirectory = false;
            switch (fileOption)
            {
                case "d":
                case "D":
                    isDirectory = true;
                    break;
                case "f":
                case "F":
                    isDirectory = false;
                    break;
                default:
                    break;
            }
            Console.WriteLine("How many of {0} to transfer?", isDirectory ? "directory" : "file");
            // TO DO: convert to int
            String qty = Console.ReadLine();
        }
    }
}
