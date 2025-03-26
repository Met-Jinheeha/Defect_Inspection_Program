using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DefectViewProgram
{
    class DefectInfo
    {
        public int DefectId { get; set; }

        public double XRel { get; set; }

        public double YRel { get; set; }

        public int XSize { get; set; }

        public int YSize { get; set; }


        public DefectInfo(string[] content)
        {
            // 디펙 객체 초기화
            DefectId = int.Parse(content[0]);
            XRel = double.Parse(content[1], System.Globalization.NumberStyles.Float);
            YRel = double.Parse(content[2], System.Globalization.NumberStyles.Float);
            XSize = int.Parse(content[5]);
            YSize = int.Parse(content[6]);
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
