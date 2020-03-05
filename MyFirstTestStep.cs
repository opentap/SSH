using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OpenTap;
using Renci.SshNet;

namespace OnsDemo
{
    [Display("MyFirstTestStep")]
    public class MyFirstTestStep : TestStep
    {
        #region Settings
        [Display("Host Name", "Host name or IP address of the machine to connect to.","Connection")]
        public string Host { get; set; }
        [Display("User Name", Group: "Connection")]
        public string UserName { get; set; }
        [Display("Password",  Group: "Connection")]
        public string Password { get; set; }
        [Display("Command", Description: "Command to execute on remote.", Group: "Command")]
        [Layout(LayoutMode.FullRow, 10)]
        public string Command { get; set; }
        #endregion

        public MyFirstTestStep()
        {
            Host = "localhost";
            UserName = "demo";
            Password = "12345678";
            Command = "pwd";
            Name = "SSH {User Name}@{Host Name}";
        }

        public override void Run()
        {
            // ToDo: Add test case code here
            using(var client = new SshClient(Host,UserName,Password))
            {
                client.Connect();
                var command = client.RunCommand(Command.Replace("\r", "").Replace('\n', ';'));
                if(command.ExitStatus == 0)
                {
                    UpgradeVerdict(Verdict.Pass);
                    //using(var reader = new StreamReader(command.OutputStream))
                    //    Log.Info(reader.ReadToEnd());
                    foreach (var line in command.Result.Trim().Split('\n'))
                    {
                        Log.Info(line);
                    }
                }
                else
                {
                    Log.Warning(command.Error);
                    UpgradeVerdict(Verdict.Pass);
                }
                client.Disconnect();
            }

            UpgradeVerdict(Verdict.Pass);
        }
    }
}
