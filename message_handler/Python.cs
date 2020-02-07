using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using IronPython;
using IronPython.Hosting;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using Microsoft.Scripting.Hosting;
using System.Threading;
using System.IO;

namespace message_handler
{
    namespace UntrustedCode
    {
        public class PythonExecutor
        {
            public static string Execute(string pythonCode)
            {
                var engine = IronPython.Hosting.Python.CreateEngine();
                //var scope = engine.CreateScope();
                //var scriptSource = engine.CreateScriptSourceFromString(pythonCode);
                //await context.PostAsync("計算中......");
                object result = engine.Execute(pythonCode);
                //await context.PostAsync("計算完成");
                return JsonConvert.SerializeObject(result);
            }
        }
    }
    [Serializable]
    class Python:DialogNode
    {
        [Serializable]
        class Sandboxer : MarshalByRefObject
        {
            const string pathToUntrusted = @"Sandbox";
            const string untrustedAssembly = "message_handler.UntrustedCode";
            const string untrustedClass = "message_handler.UntrustedCode.PythonExecutor";
            const string entryPoint = "Execute";
            //private Object[] parameters = { 45 };
            public static string ExecutePython(string pythonCode)
            {
                //Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder   
                //other than the one in which the sandboxer resides.  
                AppDomainSetup adSetup = new AppDomainSetup();
                var fullPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + pathToUntrusted;
                if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
                adSetup.ApplicationBase = fullPath;

                //Setting the permissions for the AppDomain. We give the permission to execute and to   
                //read/discover the location where the untrusted code is loaded.  
                PermissionSet permSet = new PermissionSet(PermissionState.None);
                permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

                //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.  
                StrongName fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();

                //Now we have everything we need to create the AppDomain, so let's create it.  
                AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);

                //Use CreateInstanceFrom to load an instance of the Sandboxer class into the  
                //new AppDomain.   
                ObjectHandle handle = Activator.CreateInstanceFrom(
                    newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
                    typeof(Sandboxer).FullName
                    );
                //Unwrap the new domain instance into a reference in this domain and use it to execute the   
                //untrusted code.  
                Sandboxer newDomainInstance = (Sandboxer)handle.Unwrap();
                return newDomainInstance.ExecuteUntrustedPythonCode(untrustedAssembly, untrustedClass, entryPoint, pythonCode);
            }
            public string ExecuteUntrustedPythonCode(string assemblyName, string typeName, string entryPoint, string pythonCode)
            {
                //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or   
                //you can use Assembly.EntryPoint to get to the main function in an executable.  
                MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);
                try
                {
                    //Now invoke the method.  
                    return (string)target.Invoke(null, new Object[1] { pythonCode });
                }
                catch (Exception ex)
                {
                    // When we print informations from a SecurityException extra information can be printed if we are   
                    //calling it with a full-trust stack.  
                    (new PermissionSet(PermissionState.Unrestricted)).Assert();
                    var ret = $"SecurityException caught:\n{ex}";
                    CodeAccessPermission.RevertAssert();
                    return ret;
                }
            }
        }
        public static async Task<string> ExecutePython(string pythonCode)
        {
            string answer = null;
            bool completed = false;
            var startTime = DateTime.Now;
            Exception e = null;
            Thread thread = new Thread(() =>
            {
                try
                {
                    answer = UntrustedCode.PythonExecutor.Execute($"from math import *\n{pythonCode}");
                    completed = true;
                }
                catch (Exception error)
                {
                    e = error;
                    completed = true;
                }
            });
            thread.IsBackground = true;
            thread.Start();
            while ((DateTime.Now - startTime).TotalSeconds < 3)
            {
                await Task.Delay(100);
                if (completed) break;
            }
            if (!completed)
            {
                thread.Abort();
                throw new TimeoutException();
            }
            else if (e != null) throw e;
            else
            {
                if (answer.Length > 600) answer = answer.Remove(250) + $"...<br/>(中間還有{answer.Length - 500}位數)<br/>..." + answer.Substring(answer.Length - 250);
                return answer;
            }
        }
        public static string ProcessPythonCode(string code, bool allDouble)
        {
            code = code.Trim(' ')
                        .Replace('（', '(').Replace('）', ')').Replace('＋', '+').Replace('－', '-').Replace('＊', '*').Replace('／', '/').Replace('︿', '^')
                        .Replace("^", "**");
            if (allDouble)
            {
                string ans = "";
                for (int i = 0; i < code.Length; i++)
                {
                    if (char.IsNumber(code[i]))
                    {
                        for (; i < code.Length && char.IsNumber(code[i]); i++) ans += code[i];
                        ans += '.';
                        if (i < code.Length && code[i] == '.')
                        {
                            for (i++; i < code.Length && char.IsNumber(code[i]); i++) ans += code[i];
                        }
                        i--;
                    }
                    else ans += code[i];
                }
                code = ans;
            }
            return code;
        }
        public override void Run()
        {
            if (sender_msg.StartsWith("幫我算") || sender_msg.StartsWith("幫算"))
            {
                var pythonCode = sender_msg;
                bool transToDouble = pythonCode.StartsWith("幫我算");
                pythonCode = pythonCode.Substring(transToDouble ? 3 : 2);
                SendMsg($"計算中... {pythonCode.Replace("*", "\\*")}");
                //await context.PostAsync(Sandboxer.ExecutePython(pythonCode));
                try
                {
                    pythonCode = ProcessPythonCode(pythonCode, transToDouble);
                    //await context.PostAsync(pythonCode);
                    //if(pythonCode.Contains("import os") || pythonCode.Contains("import sys") || pythonCode.Contains("import call") || pythonCode.Contains("import socket") ||
                    //    pythonCode.Contains("os import") || pythonCode.Contains("sys import") || pythonCode.Contains("call import") || pythonCode.Contains("socket import"))
                    if (pythonCode.Contains("import"))
                    {
                        SendMsg("你是駭客嗎？拜託教教小莫怎麼用利用這個python功能駭入bot，列出某資料夾底下的檔案之類的，拜託～ ><");
                    }
                    SendMsg($"計算結果：{ExecutePython(pythonCode).Result}");
                }
                catch (TimeoutException)
                {
                    SendMsg("計算太久了，已中斷");
                }
                catch (Exception error)
                {
                    SendMsg($"計算時發生問題：<br/>{error}");
                }
                EndDialog(Program.NextDialog);
            }
        }
    }
}
