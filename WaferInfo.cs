using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectViewProgram
{
    public class WaferInfo
    {
        private RecipeInfo recipeText; 

        public WaferInfo(RecipeInfo recipeInfo)
        {
            this.recipeText = recipeInfo;
        }

        public Dictionary<(int, int), List<DefectInfo>> chipDefectsInfo = new Dictionary<(int, int), List<DefectInfo>>();
        public WaferInfo(ChipInfo chipInfo)
        {
            this.chipDefectsInfo = chipInfo.chipDefects;
        }

        public Dictionary<(int, int), string> chipInfoList = new Dictionary<(int, int), string>();

        
        /// <summary>
        /// 전체 칩이 뭐가 있는지 입력하는 용도겸, 칩이 불량인지 확인하는 딕셔너리
        /// </summary>

        public void WriteWholeChipGridStatus(int xIndex, int yIndex, ChipInfo chipInfo)
        {
            var key = (xIndex, yIndex);

            if (!chipInfoList.ContainsKey(key))
            {
                chipInfoList[key] = "O"; // 정상
            }
       
            if (chipInfo.chipDefects.ContainsKey(key))
            {
              chipInfoList[key] = "X"; // 불량있음
            }
            Console.WriteLine($"정상불량 판정 메서드 확인용: {key}, {chipInfoList[key]}");
        }
    }
}
