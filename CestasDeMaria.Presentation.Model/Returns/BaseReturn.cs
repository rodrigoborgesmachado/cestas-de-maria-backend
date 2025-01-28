namespace CestasDeMaria.Presentation.Model.Returns
{
    public class BaseReturn<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T? Object { get; set; }
    }
}
