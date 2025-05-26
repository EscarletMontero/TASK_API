using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTO
{
    public class Response
    {
        public bool ThereIsError => Errors.Any();
        public long EntityId { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public Response()
        {
            Successful = true;
            Message = "Operación exitosa.";
        }

        public Response(bool successful, string message)
        {
            Successful = successful;
            Message = message;
        }

        public Response(bool successful, string message, List<string> errors)
            : this(successful, message)
        {
            Errors = errors;
        }
    }

    public class Response<T> : Response 
    {
        public T? SingleData { get; set; }
        public List<T>? DataList { get; set; }

        public Response() : base() { }

        public Response(bool successful, string message) : base(successful, message) { }

        public Response(bool successful, string message, T singleData)
            : base(successful, message)
        {
            SingleData = singleData;
        }

        public Response(bool successful, string message, List<T> dataList)
            : base(successful, message)
        { 
            DataList = dataList;
        }
    }
}
