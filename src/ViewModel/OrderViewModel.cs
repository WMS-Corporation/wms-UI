namespace src.ViewModel
{
    public class OrderViewModel
    {
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public List<ProductViewModel> ProductList { get; set; }
        public string CodOrder { get; set; }
    }
}
