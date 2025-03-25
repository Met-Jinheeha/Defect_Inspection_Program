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

























        //public List<string[]> GetDefectList(string content)
        //{
        //    List<string[]> defects = new List<string[]>();

        //    string[] lines = content.Split('\n'); // 본문 내용을 줄 단위로 배열화 시킴

        //    // "DefectList"가 있는 줄 찾기
        //    int defectListLine = -1;
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        if (lines[i].Contains("DefectList"))
        //        {
        //            defectListLine = i;
        //            break;
        //        }
        //    }

        //    // 찾은 줄 다음부터 처리 시작
        //    if (defectListLine != -1)
        //    {
        //        for (int i = defectListLine + 1; i < lines.Length-4; i = i+2)
        //        {
        //            Console.WriteLine(lines[i]);
        //            string[] defectInfo = lines[i].Split(' '); // 디펙하나 483개 중에 한줄
        //            defects.Add(defectInfo); // 디펙 정보배열 리스트 483개의 전체 줄
        //        }
        //    }
        //    return defects;
        //}
    }
}       
