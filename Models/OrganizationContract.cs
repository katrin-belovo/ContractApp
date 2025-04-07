using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractApp.Models
{
    public class OrganizationContract : Contract
    {
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}