using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractApp.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? ConclusionDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string Status { get; set; } = "Черновик";
    }
}
