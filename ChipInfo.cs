using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectViewProgram
{
    public class ChipInfo
    {

        public Dictionary<(int, int), List<DefectInfo>> chipDefects = new Dictionary<(int, int), List<DefectInfo>>(); // 디펙 인포 객체가 반복적으로 들어가면서 쌓임


        public void AddDefect(DefectInfo defect)
        {
            var key = (defect.XIndex, defect.YIndex);
            if (!chipDefects.ContainsKey(key))
                chipDefects[key] = new List<DefectInfo>();

            chipDefects[key].Add(defect);
        }


        public string GetAllDefects()
        {
            var sb = new StringBuilder();
            foreach (var list in chipDefects.Values)
            {
                foreach (var defect in list)
                    sb.AppendLine(defect.ToString());
            }
            return sb.ToString();
        }


        public List<DefectInfo> GetDefects(int xIndex, int yIndex)
        {
            var key = (xIndex, yIndex);
            return chipDefects.ContainsKey(key) ? chipDefects[key] : new List<DefectInfo>();
        }

        public void ChipDefectClear()
        {
            chipDefects.Clear();
        }
    }
}
