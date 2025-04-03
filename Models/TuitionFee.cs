using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractApp.Models
{
    public class TuitionFee
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int DirectionId { get; set; }
        public decimal Amount { get; set; }
        public Direction Direction { get; set; }
    }
}
