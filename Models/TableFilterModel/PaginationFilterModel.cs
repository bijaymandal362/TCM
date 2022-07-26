namespace Models.GridTableFilterModel
{
    public class PaginationFilterModel
    {
        public int PageNumber { get; set; }
        public string SearchValue { get; set; }
        public int PageSize { get; set; }
        public PaginationFilterModel()
        {
            this.PageNumber = 1;
            this.PageSize = 50;
        }
        public PaginationFilterModel(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 1 ? 50 : pageSize;
        }
        public string FilterValue { get; set; }
    }
    public class FilterModel
    {
        public string SearchValue { get; set; }
    }


}
