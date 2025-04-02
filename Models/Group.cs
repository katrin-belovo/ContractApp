using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractApp.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DirectionId { get; set; }
        public Direction Direction { get; set; } // Навигационное свойство
    }
}
