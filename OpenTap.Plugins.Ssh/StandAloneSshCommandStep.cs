//Copyright 2019-2020 Keysight Technologies
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OpenTap;
using Renci.SshNet;

namespace OpenTap.Plugins.Ssh
{
    [Display("Stand-alone SSH Command.","Runs a one or more commands on a specified host via SSH.", Group: "SSH")]
    public class StandAloneSshCommandStep : TestStep
    {
        #region Settings
        [EmbedProperties]
        public SshConnectionInfo Connection { get; set; }
        [Display("Command", Description: "Command to execute on remote.", Group: "Command")]
        [Layout(LayoutMode.FullRow, 10)]
        public string Command { get; set; }
        #endregion

        public StandAloneSshCommandStep()
        {
            Connection = new SshConnectionInfo();
            Command = "pwd";
            Name = "SSH {User Name}@{Host Name}";
        }

        public override void Run()
        {
            // ToDo: Add test case code here
            using(var client = new SshClient(Connection.GetConnectionInfo()))
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
