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
    
    [Display("Background SSH Command", "Run a command in the background using a session setup by an SSH Session step, SSH Instrument or SSH Dut.", Groups: new[] { "SSH", "Background (Linux only)" })]
    public class BackgroundSshCommandStep : SshStepBase
    {
        #region Settings
        public string Command { get; set; }

        [Output]
        [Display("Process PID", Description:"The PID of the background process", Group: "Response")]
        public Pid pid { get; private set; }
        
        #endregion

        public BackgroundSshCommandStep()
        {
            Name = "Background SSH Command: {Command}";
        }

        public override void Run()
        {
            pid = SshResource.StartBackgroundProcess(Command);
            Log.Info($"Running command `{Command}` in the background");
        }
    }
}
