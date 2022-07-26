using System.Collections.Generic;

namespace Models.AuthServer
{
    public class ApiResponse<T>
    {
        public MetaDetail Meta { get; set; }
        public T Data { get; set; }
        public List<ErrorDetail> Errors { get; set; }
        public class MetaDetail
        {
            public string Copyright { get; set; }
            public string Email { get; set; }
            public ApiDetail Api { get; set; }
        }
        public class ApiDetail
        {
            public string Version { get; set; }
        }

        public class ErrorDetail
        {
            public string Code { get; set; }
            public string Source { get; set; }
            public string Title { get; set; }
            public string Detail { get; set; }
        }
    }
}
