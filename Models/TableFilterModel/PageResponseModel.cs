using Models.TestRun;

namespace Models.GridTableProperty
{
    public class PagedResponseProjectMemberModel<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

      
        public PagedResponseProjectMemberModel(T data, 
            int pageNumber, 
            int pageSize,
            int totalRecords,
            int totalPages
           )
        {
            PageNumber = pageNumber;
         
            PageSize = pageSize;
            Data = data;
            TotalPages = totalPages;
            TotalRecords = totalRecords;
        }
        public T Data { get; set; }


      
    }


    public class PagedResponsePersonModel<T>
    {
        public T Data { get; set; }

        public PagedResponsePersonModel(T data)
        {
            this.Data = data;
        }
    }


    public class PagedResponseModel<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

     

        public PagedResponseModel(T data,
            int pageNumber,
            int pageSize,
            int totalRecords,
            int totalPages
           )
        {
            PageNumber = pageNumber;
          
            PageSize = pageSize;
            Data = data;
            TotalPages = totalPages;
            TotalRecords = totalRecords;
        }
        public T Data { get; set; }



    }

 

}
