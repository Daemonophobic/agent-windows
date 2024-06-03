using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WINREM
{
    internal class Built_in_function
    {
        public string GetPasswordDate()
        {
            string scriptArgument = "Get-LocalUser |\r\nSelect PasswordLastSet";
            string result = ExecuteScript(scriptArgument);
            return result;
        }

        public string GetSubNet()
        {
            string scriptArgument = "ipconfig";
            string result = ExecuteScript(scriptArgument);
            return result;
        }

        public string CustomScript(string script)
        {
            string result = ExecuteScript(script);
            return result;
        }



        private string ExecuteScript(string script)
        {
            var processStartInfo = new ProcessStartInfo("powershell.exe", script);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            using var process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            if(error != null)
            {
                throw new Exception(error);
            }
            else
            {
                return output;
            }
        }
    }
}
