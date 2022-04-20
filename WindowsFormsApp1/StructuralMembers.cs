﻿using System;
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
        public List<List<List<string>>> footingsColumn = new List<List<List<string>>>();
        public List<string> footingWallNames = new List<string>();
        public List<List<List<string>>> footingsWall = new List<List<List<string>>>();

        //Earthworks solution variables
        public List<List<double>> earthworkSolutions = new List<List<double>>();

        public StructuralMembers()
        {
            //
        }
    }
}
