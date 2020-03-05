using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OpenTap;
using Renci.SshNet;

namespace OnsDemo
{
    [Display("SshSessionStep")]
    public class SshSessionStep : TestStep
    {
        internal SshClient Client { get; private set; }

        #region Settings
        [Display("Host Name", "Host name or IP address of the machine to connect to.","Connection")]
        public string Host { get; set; }
        [Display("User Name", Group: "Connection")]
        public string UserName { get; set; }
        [Display("Password",  Group: "Connection")]
        public string Password { get; set; }
        #endregion

        public SshSessionStep()
        {
            
            Host = "localhost";
            UserName = "demo";
            Password = "12345678";
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        public override void Run()
        {
            using(var client = new SshClient(Host,UserName,Password))
            {
                client.Connect();
                Client = client;
                RunChildSteps();
                client.Disconnect();
            }
        }

        public override void PostPlanRun()
        {
            // ToDo: Optionally add any cleanup code this step needs to run after the entire testplan has finished
            base.PostPlanRun();
        }
    }
}
