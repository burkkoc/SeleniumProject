using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium2
{
    public class Game
    {
        public string Name { get; set; }
        public string NavigateUrl { get; set; }
        public string LowestUrl { get; set; }
        public string ManageOfferVariableXPATH { get; set; }
        public string SellerNode { get; set; }
        public string PriceNode { get; set; }
        List<Game> Games { get; set;} = new List<Game>();
        public override string ToString()
        {
            return Name;
        }
    }
    
}
