using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectViewProgram
{
    class ChipInfo
    {

        public Point ChipIndex
        {
            get; private set;
        }

        public ChipInfo(int x, int y)
        {
            ChipIndex = new Point(x, y);
        }

        private Dictionary<Point, List<DefectInfo>> chipDefects = new Dictionary<Point, List<DefectInfo>>();

        public void AddDefect(int xIndex, int yIndex, string[] defectData)
        {
            Point chipPoint = new Point(xIndex, yIndex);
            DefectInfo defect = new DefectInfo(defectData);

            if (!chipDefects.ContainsKey(chipPoint))
                chipDefects[chipPoint] = new List<DefectInfo>();

            chipDefects[chipPoint].Add(defect);
        }

        public List<DefectInfo> GetDefects(int xIndex, int yIndex)
        {
            Point chipPoint = new Point(xIndex, yIndex);

            if (chipDefects.ContainsKey(chipPoint))
                return chipDefects[chipPoint];

            return new List<DefectInfo>();
        }

        public Dictionary<Point, List<DefectInfo>> GetAllDefects()
        {
            return chipDefects;
        }
    }
}
