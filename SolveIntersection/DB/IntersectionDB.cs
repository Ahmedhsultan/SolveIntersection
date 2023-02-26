using SolveIntersection.DB.Entities;

namespace SolveIntersection.DB
{
    internal class IntersectionDB
    {
        private static IntersectionDB Instance { get; set; }

        public AssemblyDB assembly { get; set; }
        public AlignmentDB alignment { get; set; }
        public CorridorDB corridor { get; set; }
        public DataDB data { get; set; }
        public PolylinesDB polylinesDB { get; set; }
        private IntersectionDB()
        {
            assembly = new AssemblyDB();
            alignment = new AlignmentDB();
            corridor = new CorridorDB();
            data = new DataDB();
            polylinesDB = new PolylinesDB();
        }

        public static IntersectionDB getInstance()
        {
            if (Instance == null)
                Instance = new IntersectionDB();

            return Instance;
        }
    }
}
