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
    public abstract class SshStepBase : TestStep
    {

        #region Settings

        [ResourceOpen(ResourceOpenBehavior.Ignore)]
        public IEnumerable<SshResource> sshSessions
        {
            get
            {
                var parentSession = this.GetParent<SshSessionStep>()?.GetSshResource();
                return (parentSession == null ? Array.Empty<SshResource>() : new[] { parentSession })
                    .Concat(InstrumentSettings.Current.OfType<SshResource>()
                    .Concat(DutSettings.Current.OfType<SshResource>()));
            }
        }
        
        private SshResource BackingResource;

        [Display("Connection", "Use SSH session defined by this Instrument, DUT or Parent step.")]
        [AvailableValues(nameof(sshSessions))]
        public SshResource SshResource
        {
            get
            {
                if (BackingResource == null || BackingResource.Invalid)
                    return GetParent<SshSessionStep>()?.GetSshResource();
                return BackingResource;
            }
            set => BackingResource = value;
        }

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
        public string Command { get; set; } = "pwd";

        [Display("Add To Log", Group: "Response", Collapsed: true, Order:0)]
        public bool AddToLog { get; set; } = true;
        [Display("Output Response", "Sets if the output of the ssh command should be saved as an output.", Group: "Response", Collapsed: true, Order:0)]
        public bool OutputResponse { get; set; }

        [Output]
        [Browsable(true)]
        [EnabledIf(nameof(OutputResponse), HideIfDisabled = true)]
        [Display("Response", Description:"The standard output (stdout) of the executed program.", Group: "Response", Collapsed: true, Order:1)]
        public string Response { get; private set; }

        [Output]
        [Browsable(true)]
        [Display("Exit Code", Description:"The exit code of the command.", Group: "Response", Collapsed: true, Order:1)]
        public int ExitCode { get; private set; }

        [Display("Enabled", Group: "Timeout")]
        public bool TimeoutEnabled { get; set; }
        [Display("Timeout", Group: "Timeout")]
        [Unit("s")]
        [EnabledIf(nameof(TimeoutEnabled), HideIfDisabled = true)]
        public double Timeout { get; set; } = 5.0;

        [Display("Check Exit Code", Description: "Sets the test step verdict based on the exit code. Exit code 0 will cause the step to pass, all other exit codes will cause it to fail.", Group: "Response", Collapsed: true, Order:0)]
        public bool CheckExitCode { get; set; }
        #endregion

        public SshCommandStep()
        {
            Name = "SSH Command {Command}";
        }

        public override void Run()
        {
            SshCommand command = SshResource.SshClient.CreateCommand(Command);
            if (TimeoutEnabled)
                command.CommandTimeout = TimeSpan.FromSeconds(Timeout);
            command.Execute();

            ExitCode = command.ExitStatus;
            if (OutputResponse)
            {
                Response = command.Result.TrimEnd('\n');
            }
            if(command.ExitStatus == 0)
            {
                if (AddToLog)
                {
                    foreach (var line in command.Result.Trim().Split('\n'))
                    {
                        Log.Info(line);
                    }
                }
            }
            else
            {
                if (AddToLog)
                    Log.Warning(command.Error);
            }
            if (CheckExitCode)
            {
                if (ExitCode == 0)
                {
                    UpgradeVerdict(Verdict.Pass);
                }
                else
                {
                    UpgradeVerdict(Verdict.Fail);
                }
            }
        }
    }
}
