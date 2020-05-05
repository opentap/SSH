//Copyright 2019 Keysight Technologies
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
using System.Text;
using System.Text.RegularExpressions;

namespace OpenTap.Plugins.Ssh
{
    /// <summary>
    /// Add Base64 encoding/decoding functionality to <see cref="System.Text.Encoding"/>
    /// </summary>
    internal static class Base64Encoding
    {
        private static readonly Regex base64Regex = new Regex(@"^[0-9\+/a-zA-Z]*={0,3}$", RegexOptions.Compiled);

        /// <summary>
        /// Encode the given input string as base 64.
        /// This method will assume the enconding is UTF8.
        /// </summary>
        /// <param name="input">The string to be encoded.</param>
        /// <param name="encoding">The enconding to be used. If nothing provided, UTF8 will be used.</param>
        /// <returns>The base 64 encoded string.</returns>
        public static string EncodeBase64(string input, Encoding encoding = null)
        {
            string output = null;
            if (input != null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                byte[] bytes = encoding.GetBytes(input);
                output = System.Convert.ToBase64String(bytes);
            }
            return output;
        }

        /// <summary>
        /// Decode the given input string as base 64. 
        /// This method will assume the enconding is UTF8.
        /// </summary>
        /// <param name="input">The string to be decoded.</param>
        /// <param name="encoding">The enconding to be used. If nothing provided, UTF8 will be used.</param>
        /// <returns>The base 64 decoded string.</returns>
        public static string DecodeBase64(string input, Encoding encoding = null)
        {
            string output = null;
            if (input != null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                byte[] bytes = System.Convert.FromBase64String(input);
                output = encoding.GetString(bytes);
            }
            return output;
        }

        /// <summary>
        /// This method checks if the provided input string is base64 or not.
        /// </summary>
        /// <param name="input">The string to be verified whether it is base64 or not.</param>
        /// <returns>A boolean indicating whether the provided string is in base64 or not.</returns>
        public static bool IsBase64(string input)
        {
            bool result = true;
            if (input != null)
            {
                string i = input.Trim();
                result = (i.Length % 4 == 0) && base64Regex.IsMatch(i);
            }
            return result;
        }
    }
}
