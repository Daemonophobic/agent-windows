using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WINREM.DTO
{
    internal class JobResponseDTO
    {
        [JsonProperty("jobs")]
        public List<JobDTO> jobs { get; set; }
    }
}
