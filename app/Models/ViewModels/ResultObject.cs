using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models.ViewModels
{
    public class ResultObject
    {
        [EnumDataType(typeof(ResultType))]
        public ResultType status { get; set; }
        public object Payload { get; set; }
        public string Message { get; set; }

    }
}
