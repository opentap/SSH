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
    
    public class Pid
    {
        #region Settings
        public uint pid { get; private set; }
        
        #endregion

        public Pid(uint pid)
        {
            this.pid = pid;
        }

        public Pid(string pid)
        {
            this.pid = uint.Parse(pid);
        }

        public override string ToString()
        {
            return pid.ToString();
        }

    }
}
