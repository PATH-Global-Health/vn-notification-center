using System;
using System.Collections.Generic;
using System.Text;

namespace Data.ViewModels
{
    public class ResultModel
    {
        public dynamic Data { get; set; }
        public bool Succeed { get; set; }
        public string ErrorMessages { get; set; }
    }
}
