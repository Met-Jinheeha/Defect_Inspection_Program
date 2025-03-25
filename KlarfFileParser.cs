using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectViewProgram
{
    class KlarfFileParser
    {
        public string ParsedContent { get; private set; }

        public override string ToString()
        {
            return base.ToString();
        }



        public void ParseText(string filePath)
        {
            if(filePath == null)
            {
                return;
            }
            ParsedContent = File.ReadAllText(filePath);

            List<string[]> defects = new List<string[]>();

            string[] lines = ParsedContent.Split('\n'); // 본문 내용을 줄 단위로 배열화 시킴

            // "DefectList"가 있는 줄 찾기
            int defectListLine = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("DefectList"))
                {
                    defectListLine = i;
                    break;
                }
            }

            string[] defectInfo = new string[20];

            // 찾은 줄 다음부터 처리 시작
            if (defectListLine != -1)
            {
                for (int i = defectListLine + 1; i < lines.Length - 4; i = i + 2)
                {
                    //Console.WriteLine(lines[i]);
                    defectInfo = lines[i].Split(' '); // 디펙하나 483개 중에 한줄
                    //defects.Add(defectInfo);

                    int indexX = int.Parse(defectInfo[3]);
                    int indexY = int.Parse(defectInfo[4]);

                    DefectInfo defectDetails = new DefectInfo(defectInfo);

                    defectDetails.AddDefect(indexX, indexY, defectInfo);
                }
            }
            
        }
    }
}
