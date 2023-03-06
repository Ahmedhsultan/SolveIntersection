using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities.Beans
{
    internal class Meta_Data
    {
        public double width { get; set; }
        public double rightSlop { get; set; }
        public double leftSlop { get; set; }
        public double longitudinalSlop { get; set; }
        public Direction direction { get; set; }
    }
}
