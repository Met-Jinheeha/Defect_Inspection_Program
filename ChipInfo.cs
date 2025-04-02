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
        //public Dictionary<Point, List<DefectInfo>> ChipDefect
        //{
        //    get => chipDefects;
        //    set => chipDefects = value;
        //}


        //public override string ToString()
        //{
        //    return $"{ChipIndex.x},{ChipIndex.y},{chipDefects.Values}";
        //}

        public void AddDefect(DefectInfo defectData)
        {
            if (!chipDefects.ContainsKey(ChipIndex))
            {
                chipDefects[ChipIndex] = new List<DefectInfo>(); // 없을 경우 디펙인포 리스트 밸류값 객체
            }
            chipDefects[ChipIndex].Add(defectData); // 키값에 이미 값이 있을 경우 리스트에 객체 추가
        }

        /// <summary>
        /// StringBuilder 
        /// </summary>
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
                if (key.x == xIndex && key.y == yIndex)
                {
                    result.AddRange(entry.Value);
                }
            }
            return result;
        }

        public void ChipDefectClear()
        {
            chipDefects.Clear();
        }
    }
}
