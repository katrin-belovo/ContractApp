using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractApp.Models
{
    public class BilateralContract : Contract
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}