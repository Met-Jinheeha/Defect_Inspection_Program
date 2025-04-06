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

        public static Dictionary<Tuple<int, int>, List<DefectInfo>> chipDefects = new Dictionary<Tuple<int, int>, List<DefectInfo>>();


        public void AddDefect(Tuple<int, int> chipIndex, DefectInfo defectData)
        {
            if (!chipDefects.ContainsKey(chipIndex))
            {
                chipDefects[chipIndex] = new List<DefectInfo>();
            }
            chipDefects[chipIndex].Add(defectData);
            Console.WriteLine($"ADD Defect: ({chipIndex}, {chipDefects[chipIndex]})"); // 디버깅 출력 추가
        }

        /// <summary>
        /// StringBuilder 
        /// </summary>
        public string GetAllDefects()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var kvp in chipDefects)
            {
                sb.AppendLine($"{string.Join(",", kvp.Value)}");
            }

            string result = sb.ToString();
            Console.WriteLine(result);
            return result;
        }

        public List<DefectInfo> GetDefects(int xIndex, int yIndex)
        {
            var key = Tuple.Create(xIndex, yIndex);
            return chipDefects.TryGetValue(key, out var defects) ? defects : new List<DefectInfo>();
        }

        public void ChipDefectClear()
        {
            chipDefects.Clear();
        }

        
    }
}
