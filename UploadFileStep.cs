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
        private string fileContentAsBase64 = null;

        #region Settings
        [Display(Name: "Remote Upload Path", Group: "Action")]
        public string UploadPath { get; set; }

        [FilePath()]
        [Display(Name: "Local File To Upload", Group: "Action")]
        public string FileBase64
        {
            get
            {
                return fileContentAsBase64;
            }
            set
            {
                bool fallback = true;
                FileInfo fi = null;
                try
                {
                    fi = new FileInfo(value);
                    if (fi.Exists)
                    {
                        string fileContent = File.ReadAllText(value);
                        fileContentAsBase64 = Base64Encoding.EncodeBase64(fileContent);
                        fallback = false;
                    }
                }
                catch
                {
                }

                if (fallback)
                {
                    //if (Base64Encoding.IsBase64(value))
                    //{
                    fileContentAsBase64 = value;
                    //}
                    //else
                    //{
                    //    hotContentAsBase64 = null;
                    //}
                }
            }
        }
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
            string fileContent = Base64Encoding.DecodeBase64(fileContentAsBase64);
            SshResource.ScpClient.Upload(new MemoryStream(Encoding.ASCII.GetBytes(fileContent)), UploadPath);
        }
    }
}
