using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OpenTap;
using Renci.SshNet;

namespace OnsDemo
{
    [Display("SshCommandStep")]
    [AllowAsChildIn(typeof(SshSessionStep))]
    public class SshCommandStep : TestStep
    {
        #region Settings
        public string Command { get; set; }
        #endregion
        public SshCommandStep()
        {
            Command = "pwd";
        }
        public override void Run()
        {
            SshClient client = GetParent<SshSessionStep>().Client;
            var command = client.RunCommand(Command);
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
        }

        public override void PostPlanRun()
        {
            // ToDo: Optionally add any cleanup code this step needs to run after the entire testplan has finished
            base.PostPlanRun();
        }
    }
}
