using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities.Beans
{
    public class Meta_Data_Road
    {
        public double width { get; set; }
        public double rightSlop { get; set; }
        public double leftSlop { get; set; }
        public Direction direction { get; set; }
    }
}
