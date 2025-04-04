using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractApp.Models
{
    public class ContractSettings
    {

        public ContractSettings()
        {
            ProxyDate = DateTime.Today > new DateTime(2000, 1, 1)
                ? DateTime.Today
                : new DateTime(2000, 1, 1);
        }
        public int Id { get; set; }
        public string Position { get; set; }
        public string FullName { get; set; }
        public string ProxyNumber { get; set; }
        public DateTime ProxyDate { get; set; }
        
        public bool IsActive { get; set; }

        
    }
}
