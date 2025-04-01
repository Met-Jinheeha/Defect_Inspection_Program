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

        public static Dictionary<Point, List<DefectInfo>> chipDefects = new Dictionary<Point, List<DefectInfo>>();
        public Dictionary<Point, List<DefectInfo>> ChipDefect
        {
            get => chipDefects;
            set => chipDefects = value;
        }


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


        public List<DefectInfo> GetDefects(int xIndex, int yIndex)
        {
            List<DefectInfo> result = new List<DefectInfo>();

            foreach (var entry in chipDefects)
            {
                Point key = entry.Key;
                if ((int)key.x == xIndex && (int)key.y == yIndex)
                {
                    result.AddRange(entry.Value);
                }
            }
            return result;
        }


        public string GetAll()
        {
            return chipDefects.ToString();
        }


        public void ChipDefectClear()
        {
            chipDefects.Clear();
        }
    }
}
