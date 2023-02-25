using SolveIntersection.DB.Entities;

namespace SolveIntersection.DB
{
    internal class IntersectionDB
    {
        public static IntersectionDB Instance { private get; set; }

        public AssemblyDB assembly { get; set; }
        public AlignmentDB alignment { get; set; }
        public CorridorDB corridor { get; set; }
        private IntersectionDB()
        {
            assembly = new AssemblyDB();
            alignment = new AlignmentDB();
            corridor = new CorridorDB();
        }

        public static IntersectionDB getInstance()
        {
            if (Instance == null)
                Instance = new IntersectionDB();

            return Instance;
        }
    }
}
