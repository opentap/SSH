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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTap.Plugins.Ssh
{
    [Display("Upload File", Group: "SSH", Description: "Uses SCP to upload a file to a remote host defined by an SSH Session step, SSH Instrument or SSH Dut.")]
    public class ScpUploadFileStep : SshStepBase
    {
        #region Settings
        [Display(Name: "Remote Upload Path", Group: "Action")]
        public string UploadPath { get; set; }

        [FilePath()]
        [Display(Name: "Local File To Upload", Group: "Action")]
        public string FileToUpload { get; set; }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="ScpUploadFileStep">ScpUploadFileStep</see> class.
        /// </summary>
        public ScpUploadFileStep()
        {
            UploadPath = "/home/ubuntu/myFile";
        }

        /// <summary>
        /// Run method is called when the step gets executed.
        /// </summary>
        public override void Run()
        {
            using(FileStream fs = File.OpenRead(FileToUpload))
            {
                SshResource.ScpClient.Upload(fs, UploadPath);
            }
        }
    }
}
