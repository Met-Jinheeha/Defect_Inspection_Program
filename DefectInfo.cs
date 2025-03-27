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
        public int defectId { get; set; }

        public double xRel { get; set; }

        public double yRel { get; set; }

        public int xIndex { get; set; }

        public int yIndex { get; set; }

        public int xSize { get; set; }

        public int ySize { get; set; }


        public DefectInfo(string[] content)
        {
            // 디펙 객체 초기화
            defectId = int.Parse(content[0]);
            xRel = double.Parse(content[1], System.Globalization.NumberStyles.Float);
            yRel = double.Parse(content[2], System.Globalization.NumberStyles.Float);
            xIndex = int.Parse(content[3]);
            yIndex = int.Parse(content[4]);
            xSize = int.Parse(content[5]);
            ySize = int.Parse(content[6]);
        }
        public DefectInfo()
        { 
        }

        public override string ToString()
        {
            return $"{defectId},{xRel},{yRel},{xIndex},{yIndex},{xSize},{ySize}";
        }

        public void WriteDefectInfo(DefectInfo defectInfo)
        {
            ChipInfo chipInfo = new ChipInfo(xIndex, yIndex);
            chipInfo.AddDefect(defectInfo);
        }
    }
}       
