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
using System.ComponentModel;
using Renci.SshNet;

namespace OpenTap.Plugins.Ssh.Background
{
    
    [Display("Check Background SSH Command", "Checks if a background command is still running.", Groups: new[] { "SSH", "Background" })]
    public class CheckBackgroundSshCommandStep : SshStepBase
    {
        #region Settings
        [Display("Process PID", "Select the Background SSH Command to kill")]

        public Input<Pid> InputPid { get; set; }

        #endregion

        public CheckBackgroundSshCommandStep()
        {
            Name = "Check Background SSH Command: PID={Process PID}";
            InputPid = new Input<Pid>();
        }

        public override void Run()
        {
            if (InputPid == null)
            {
                throw new ArgumentNullException("No PID was set");
            }
            Pid pid = InputPid.Value;

            SshCommand command = SshResource.SshClient.CreateCommand($"ps -p {pid}");
            command.Execute();

            if (command.ExitStatus == 0)
            {
                Log.Info($"Process with PID={pid} is alive");
                UpgradeVerdict(Verdict.Pass);
                return;
            }

            Log.Info($"Process with PID={pid} is not alive");
            UpgradeVerdict(Verdict.Fail);
        }
    }
}
