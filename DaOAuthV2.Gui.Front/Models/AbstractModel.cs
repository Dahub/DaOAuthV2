using DaOAuthV2.Service.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaOAuthV2.Gui.Front.Models
{
    public abstract class AbstractModel
    {
        public AbstractModel()
        {
            Errors = new List<string>();
        }

        public IList<string> Errors { get; set; }
        public bool HasErrors
        {
            get
            {
                return Errors != null && Errors.Count > 0;
            }
        }

        public async Task<bool> ValidateAsync(HttpResponseMessage response)
        {
            if(response != null && (int)response.StatusCode >= 300)
            {
                try
                {
                    ErrorApiResultDto e = JsonConvert.DeserializeObject<ErrorApiResultDto>(await response.Content.ReadAsStringAsync());
                    if(e != null)
                    {
                        Errors = e.Message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();                       
                    }
                }
                catch
                {
                    Errors.Add($"{(int)response.StatusCode} {response.ReasonPhrase}");
                    return false;
                }
            }
            return !HasErrors;
        }
    }
}
