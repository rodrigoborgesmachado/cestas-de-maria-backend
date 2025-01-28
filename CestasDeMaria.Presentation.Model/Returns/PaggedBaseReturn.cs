namespace CestasDeMaria.Presentation.Model.Returns
{
    public class PaggedBaseReturn<T> where T : class
    {
        public int Page { get; set; }
        public int Quantity { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Results { get; set; }
    }
}
