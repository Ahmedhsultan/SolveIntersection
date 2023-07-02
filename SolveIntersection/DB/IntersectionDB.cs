using SolveIntersection.DB.Entities;

namespace SolveIntersection.DB
{
    public class IntersectionDB
    {
        private static IntersectionDB Instance { get; set; }

        public DataDB data { get; set; }
        public Selection selection { get; set; }
        public Road road_Main { get; set; }
        public Road road_Secondary { get; set; }
        public RightTurn rightTurn_Right { get; set; }
        public RightTurn rightTurn_Left { get; set; }

        private IntersectionDB()
        {
            data = new DataDB();
            selection = new Selection();
            road_Main = new Road();
            road_Secondary = new Road();
            rightTurn_Right = new RightTurn();
            rightTurn_Left = new RightTurn();
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
