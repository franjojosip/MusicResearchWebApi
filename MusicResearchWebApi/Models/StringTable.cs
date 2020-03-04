using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicResearchWebApi.Models
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }

        public string[,] Values { get; set; }
    }
}