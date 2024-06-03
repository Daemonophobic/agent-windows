using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WINREM.DTO
{
    internal class PostDTO
    {
        [JsonProperty("communicationToken")]
        public string apiKey { get; set; }
        [JsonProperty("output")]
        public string output { get; set; }
    }
}
