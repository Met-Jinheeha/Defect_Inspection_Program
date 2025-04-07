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

        public string[] defectInfo = new string[20];

        public string[] sampleInfo = new string[9];

        public string waferInfo = "";

        private ChipInfo chipInfo;

        public KlarfFileParser(ChipInfo chipInfo)
        {
            this.chipInfo = chipInfo;
        }

        public KlarfFileParser()
        {
        }

        public void ParseText(string filePath)
        {
            if(filePath == null)
            {
                return;
            }
            ParsedContent = File.ReadAllText(filePath);

            string[] lines = ParsedContent.Split('\n'); // 본문 내용을 줄 단위로 배열화 시킴


            // Wafer정보 입력
            int defectListLine = -1;
            int gridLineList = -1;
            int gridListEndLine = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                sampleInfo[0] = lines[1]; // 시간
                sampleInfo[1] = lines[4]; // 샘플 타입
                sampleInfo[2] = lines[6]; // Lot 아이디
                sampleInfo[3] = lines[7]; // 샘플 사이즈
                sampleInfo[4] = lines[9]; // 레시피 아이디
                sampleInfo[5] = lines[10];// 노치타입여부
                sampleInfo[6] = lines[11];// 노치위치
                sampleInfo[7] = lines[15];// 웨이퍼아이디
                sampleInfo[8] = lines[16];// Slot 넘버

                if (lines[i].Contains("SampleTestPlan"))
                {
                    gridLineList = i;
                    continue;
                }

                if (lines[i].Contains("AreaPerTest"))
                {
                    gridListEndLine = i;
                    continue;
                }

                // "DefectList"가 있는 줄 찾기
                if (lines[i].Contains("DefectList"))
                {
                    defectListLine = i;
                    break;
                }
            }

            RecipeInfo recipeInfo = new RecipeInfo(sampleInfo);
            waferInfo = recipeInfo.ToString();

            WaferInfo wafer = new WaferInfo(recipeInfo);

            // 그리드 정보 리스트에 할당
            //List<string> gridList = new List<string>();

            if (gridLineList != -1)
            {
                for (int i = gridLineList + 1; i < gridListEndLine; i++)
                {
                    string[] temp = lines[i].Split(' ');
                    if (temp[1].Contains(';')){
                        temp[1] = temp[1].Substring(0, temp[1].Length - 1);
                    }
                    int x = int.Parse(temp[0]);
                    int y = int.Parse(temp[1]);

                    wafer.WriteWholeChipGridStatus(x, y, chipInfo);
                }
            }

            // 찾은 줄 다음부터 처리 시작
            if (defectListLine != -1)
            {
                for (int i = defectListLine + 1; i < lines.Length - 4; i = i + 2)
                {
                    defectInfo = lines[i].Split(' '); // 디펙하나 483개 중에 한줄

                    DefectInfo defectDetails = new DefectInfo(defectInfo); // Defect 정보를 반복적으로 초기화
                    chipInfo.AddDefect(defectDetails);
                }
            } // 여기까지 디펙칩, 디펙정보 갱신. 다음 또 인스턴스를 새로 만들어서 
        }
    }
}
