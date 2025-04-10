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

        private ChipInfo chipInfo;

        public Dictionary<(int, int), List<DefectInfo>> chipDefectsInfo = new Dictionary<(int, int), List<DefectInfo>>();
        public WaferInfo(RecipeInfo recipeInfo, ChipInfo chipInfo)
        {
            this.recipeText = recipeInfo;
            this.chipDefectsInfo = chipInfo.chipDefects;
            this.chipInfo = chipInfo;
        }

        public Dictionary<(int, int), string> chipInfoList = new Dictionary<(int, int), string>();

        
        /// <summary>
        /// 전체 칩이 뭐가 있는지 입력하는 용도겸, 칩이 불량인지 확인하는 딕셔너리
        /// </summary>

        public void WriteWholeChipGridStatus(int xIndex, int yIndex)
        {
            var key = (xIndex, yIndex);

            if (!chipInfoList.ContainsKey(key))
            {
                chipInfoList[key] = "O"; // 정상
            }
       
            if (chipDefectsInfo.ContainsKey(key))
            {
              chipInfoList[key] = "X"; // 불량있음
            }
            Console.WriteLine($"정상불량 판정 메서드 확인용: {key}, {chipInfoList[key]}");
        }
    }
}
