using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WINREM.DTO
{
    public class JobDTO
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("jobName")]
        public string jobName { get; set; }
        [JsonProperty("jobDescription")]
        public string jobDescription { get; set; }
        [JsonProperty("shellCommand")]
        public bool shellCommand { get; set; }
        [JsonProperty("command")]
        public string command { get; set; }
    }
}
