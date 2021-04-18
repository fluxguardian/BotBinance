using System;
using System.Collections.Generic;
using System.Text;

namespace Strategy.Data.DataLastStrategy
{
    public class DataStrategy
    {
        public string Symbol { get; set; }

        #region Periods

        public int LrSlope { get; set; }
        public int RSI { get; set; }
        public int TSIFirstR { get; set; }
        public int TSISecondS { get; set; }

        #endregion

        #region Values Long

        public int RsiValueLong { get; set; }
        public int SlopeValueLong { get; set; }
        public int TSIValueLong { get; set; }

        #endregion

        //#region Values Short

        //public decimal RsiValueShort { get; set; }
        //public decimal SlopeValueShort { get; set; }
        //public decimal TSIValueShort { get; set; }

        //#endregion

        #region Profit

        public decimal ProfitLong { get; set; }
        //public decimal ProfitShort { get; set; }

        #endregion
    }
}
