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

        string[] defectInfo = new string[20];

        string[] sampleInfo = new string[9];

        public string waferInfo = "";


        public void ParseText(string filePath)
        {
            if(filePath == null)
            {
                return;
            }
            ParsedContent = File.ReadAllText(filePath);

            string[] lines = ParsedContent.Split('\n'); // 본문 내용을 줄 단위로 배열화 시킴


            // "DefectList"가 있는 줄 찾기
            int defectListLine = -1;
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

                if (lines[i].Contains("DefectList"))
                {
                    defectListLine = i;
                    break;
                }
            }

            WaferInfo wafer = new WaferInfo(sampleInfo);
            waferInfo = wafer.ToString();

            // 찾은 줄 다음부터 처리 시작
            if (defectListLine != -1)
            {
                for (int i = defectListLine + 1; i < lines.Length - 4; i = i + 2)
                {
                    defectInfo = lines[i].Split(' '); // 디펙하나 483개 중에 한줄

                    DefectInfo defectDetails = new DefectInfo(defectInfo); // Defect 정보를 반복적으로 초기화
                    defectDetails.WriteDefectInfo(defectDetails);
                }
            }
        }
    }
}
