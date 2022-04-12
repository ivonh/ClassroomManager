using System;

namespace ClassroomManager
{

    class Cell : IComparable<Cell>
    {
        // private data fields
        /* private int row;
         private int col;
         private string cellValue;
         private string cellColour;*/
        // public data fields
        public int Row { get; set; }
        public int Col { get; set; }
        public string CellValue { get; set; }
        public string CellColour { get; set; }

        // public constructor method
        public Cell(int row, int col, string cellValue, string cellColour)
        {
            Row = row;
            Col = col;
            CellValue = cellValue;
            CellColour = cellColour;
        }

        //override ToString() method
        // returns csv format string
        public override string ToString()
        {
            return Col.ToString() + "," + Row.ToString() + "," + CellValue + "," + CellColour;
        }

        //implement CompareTo() method for IComparable interface
        // returns 0 if cell values are the same
        // returns -1 if (this) instance is less than other cell value
        // returns 1 (if this intance) cell value is greater
        public int CompareTo(Cell otherCellObj)
        {
            return this.CellValue.CompareTo(otherCellObj.CellValue);
        }
    }
}

