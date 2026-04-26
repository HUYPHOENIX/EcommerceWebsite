namespace SharedViewModel.DTOs
{
    public class PagedItems<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        //Calculating the pages for raw data from Infra
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); 
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}