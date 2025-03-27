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
            get; set;
        }

        public ChipInfo(int x, int y)
        {
            ChipIndex = new Point(x, y);
        }

        public ChipInfo()
        {

        }

        private static Dictionary<Point, List<DefectInfo>> chipDefects = new Dictionary<Point, List<DefectInfo>>();


        public override string ToString()
        {
            return $"{ChipIndex.x},{ChipIndex.y},{chipDefects.Values}";
        }

        public void AddDefect(DefectInfo defectData)
        {
            if (!chipDefects.ContainsKey(ChipIndex))
            {
                chipDefects[ChipIndex] = new List<DefectInfo>();
            }
            chipDefects[ChipIndex].Add(defectData);
        }


        public List<DefectInfo> GetDefects(int xIndex, int yIndex)
        {
            Point chipPoint = new Point(xIndex, yIndex);

            if (chipDefects.ContainsKey(chipPoint))
                return chipDefects[chipPoint];

            return new List<DefectInfo>();
        }

        public string GetAll()
        {
            return chipDefects.ToString();
        }


        public string GetAllDefects()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var kvp in chipDefects)
            {
                sb.AppendLine($"{kvp.Key},{string.Join(",", kvp.Value)}");
            }

            string result = sb.ToString();
            Console.WriteLine(result);
            return result;
        }
    }
}
