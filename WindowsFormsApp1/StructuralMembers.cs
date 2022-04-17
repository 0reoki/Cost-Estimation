using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class StructuralMembers
    {
        //Structure 1st List -- floor, 2nd List -- str members, 3rd List -- values
        //See AddStructForm for order of values
        //Isolated Footing
        public List<string> footingColumnNames = new List<string>();
        public List<List<List<string>>> footingColumn = new List<List<List<string>>>();
        

        public StructuralMembers()
        {
            //
        }
    }
}
