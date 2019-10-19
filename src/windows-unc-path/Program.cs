using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace windows_unc_path
{
    static class Program
    {
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
           [MarshalAs(UnmanagedType.LPTStr)] string localName,
           [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
           ref int length);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            string directoryName = null;

            //Get path from directory
            if (Environment.GetCommandLineArgs().Length > 1)

                for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
                {
                    if (i == 1)
                    {
                        directoryName = Environment.GetCommandLineArgs()[i];
                    }
                    else
                    {
                        directoryName = directoryName + " " + Environment.GetCommandLineArgs()[i];
                    }
                }

            if (string.IsNullOrEmpty(directoryName))
                MessageBox.Show("After installing the application, " +
                    "Right click on a folder an select 'Get UNC Path'");
            else
             
                Clipboard.SetText(GetUNCPath(@directoryName));
        }


        //Get UNC path from directory path
        public static string GetUNCPath(string originalPath)
        {
            StringBuilder sb = new StringBuilder(512);
            int size = sb.Capacity;


            char b = originalPath[1];
            if ((b >= 'a' && b <= 'z') || (b >= 'A' && b <= 'Z'))
            {
                originalPath = originalPath.Substring(1, originalPath.Length-1);
            }         
          
            // look for the {LETTER}: combination ...
            if (originalPath.Length > 2 && originalPath[1] == ':')
            {  
                // the only valid drive letters are a-z && A-Z.
                char c = originalPath[0];


                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    int error = WNetGetConnection(originalPath.Substring(0, 2),
                        sb, ref size);

                    if (error == 0)
                    {
                        string path = Path.GetFullPath(originalPath)
                            .Substring(Path.GetPathRoot(originalPath).Length);
                        return Path.Combine(sb.ToString().TrimEnd(), path);
                    }
                }
            }

            return originalPath;
        }
    }
}
