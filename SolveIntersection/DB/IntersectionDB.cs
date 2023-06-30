using SolveIntersection.DB.Entities;

namespace SolveIntersection.DB
{
    internal class IntersectionDB
    {
        private static IntersectionDB Instance { get; set; }

        public DataDB data { get; set; }
        public Selection selection { get; set; }
        public Road_Main road_Main { get; set; }
        public Road_Secondary road_Secondary { get; set; }
        public RightTurn1 rightTurn1 { get; set; }
        public RightTurn2 rightTurn2 { get; set; }

        private IntersectionDB()
        {
            data = new DataDB();
            selection = new Selection();
            road_Main = new Road_Main();
            road_Secondary = new Road_Secondary();
            rightTurn1 = new RightTurn1();
            rightTurn2 = new RightTurn2();
        }

        public static IntersectionDB getInstance()
        {
            if (Instance == null)
                Instance = new IntersectionDB();

            return Instance;
        }

        public static void clearInstance()
        {
            Instance = null;
        }
    }
}
