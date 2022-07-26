using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class PdfOrExcelNameIsNotValidException : Exception
    {
        public PdfOrExcelNameIsNotValidException() : base("Pdf or Excel name is not found")
        {

        }
    }
}
