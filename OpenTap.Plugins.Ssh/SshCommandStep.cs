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
    public abstract class SshStepBase : TestStep
    {
        #region Settings
        [ResourceOpen(ResourceOpenBehavior.Ignore)]
        public IEnumerable<SshResource> sshSessions
        {
            get
            {
                var parentSessions = new List<SshResource>();
                var parent = this.Parent;
                while (parent != null)
                {
                    if (parent is SshSessionStep sessionStep)
                    {
                        parentSessions.Add(sessionStep.SshResource);
                    }
                    parent = parent.Parent;
                }
                return parentSessions.Concat(
                        InstrumentSettings.Current.OfType<SshResource>()
                        .Concat(DutSettings.Current.OfType<SshResource>()));
            }
        }

        [Display("Connection", "Use SSH session defined by this Instrument, DUT or Parent step.")]
        [AvailableValues(nameof(sshSessions))]
        public SshResource SshResource { get; set; }
        #endregion

        public SshStepBase()
        {
            SshResource = sshSessions.FirstOrDefault();
            Rules.Add(() => SshResource != null, "Connection must be set.", nameof(SshResource));
        }
    }

    [Display("SSH Command", "Run a command using a session setup by an SSH Session step, SSH Instrument or SSH Dut.", Group: "SSH")]
    public class SshCommandStep : SshStepBase
    {
        #region Settings
        public string Command { get; set; }
        #endregion

        public SshCommandStep()
        {
            Command = "pwd";
            Name = "SSH Command {Command}";
        }

        public override void Run()
        {
            SshCommand command = SshResource.SshClient.RunCommand(Command);
            if(command.ExitStatus == 0)
            {
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
            }

        }
    }
}
