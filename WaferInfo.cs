using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectViewProgram
{
    class WaferInfo
    {
        private RecipeInfo recipeText; 

        public WaferInfo(RecipeInfo recipeInfo)
        {
            this.recipeText = recipeInfo;
        }

        public Dictionary<(int, int), string> chipInfoList = new Dictionary<(int, int), string>();

        public void WriteWholeChipGridStatus(int xIndex, int yIndex, ChipInfo chipInfo)
        {
            var key = (xIndex, yIndex);

            if (!chipInfoList.ContainsKey(key))
            {
                chipInfoList[key] = "O";
            }
       
            if (chipInfo.chipDefects.ContainsKey(key))
            {
              chipInfoList[key] = "X";
            }
        }
    }
}
