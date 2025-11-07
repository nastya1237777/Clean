using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean
{
    public class Partners
    {
        public int PartnerId { get; set; }
        public int PartnerTypeId { get; set; }
        public string Name { get; set; }
        public string DirectorFullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Inn { get; set; }
        public decimal Rating { get; set; }
        public bool IsActive { get; set; }

    }
}

