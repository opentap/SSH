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

namespace OpenTap.Plugins.Ssh
{
    
    [Display("Kill Background SSH Command", "Kills a command running in the background.", Groups: new[] { "SSH", "Background" })]
    public class KillBackgroundSshCommandStep : SshStepBase
    {
        #region Settings
        [Display("Process PID", "Select the Background SSH Command to kill")]

        public Input<string> InputPid { get; set; }

        #endregion

        public KillBackgroundSshCommandStep()
        {
            Name = "Kill Background SSH process PID {Process PID}";
            InputPid = new Input<string>();
        }

        public override void Run()
        {
            if (InputPid == null)
            {
                throw new ArgumentNullException("No PID was set");
            }
            string pid = InputPid.Value;

            SshCommand command = SshResource.SshClient.CreateCommand($"kill {pid}");
            command.Execute();

            if (command.ExitStatus == 0)
            {
                Log.Debug($"Gracefully killed PID={pid}");
                UpgradeVerdict(Verdict.Pass);
                return;
            }

            command = SshResource.SshClient.CreateCommand($"kill -9 {pid}");
            command.Execute();

            if (command.ExitStatus == 0)
            {
                Log.Debug($"SIGKILLed PID={pid}");
                UpgradeVerdict(Verdict.Pass);
                return;
            }

            UpgradeVerdict(Verdict.Fail);

        }
    }
}
