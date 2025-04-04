using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DefectViewProgram
{
    public class DefectInfo
    {
        public int DefectId { get; set; }
        public double XRel { get; set; }
        public double YRel { get; set; }
        public int XIndex { get; set; }
        public int YIndex { get; set; }
        public int XSize { get; set; }
        public int YSize { get; set; }

        public DefectInfo(string[] content)
        {
            // 디펙 객체 초기화
            DefectId = int.Parse(content[0]);
            XRel = double.Parse(content[1], System.Globalization.NumberStyles.Float);
            YRel = double.Parse(content[2], System.Globalization.NumberStyles.Float);
            XIndex = int.Parse(content[3]);
            YIndex = int.Parse(content[4]);
            XSize = int.Parse(content[5]);
            YSize = int.Parse(content[6]);
        }
        public DefectInfo()
        {
        }

        public override string ToString()
        {
            return $"{DefectId},{XRel},{YRel},{XIndex},{YIndex},{XSize},{YSize}";
        }

        public void WriteDefectInfo(DefectInfo defectInfo)
        {
            ChipInfo chipInfo = new ChipInfo(XIndex, YIndex);
            chipInfo.AddDefect(defectInfo);
        }
    }
}       
