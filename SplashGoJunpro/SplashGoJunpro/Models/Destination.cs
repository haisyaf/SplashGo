using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplashGoJunpro.Models
{
    public class Destination
    {
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public void UpdateInfo(string name, string location, string desc)
        {
            Name = name;
            Location = location;
            Description = desc;
        }
    }
}
