namespace src.ViewModel
{
    public class TaskViewModel
    {
        public string Id { get; set; }
        public string CodOperator { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public List<ProductViewModel> ProductList { get; set; }
        public string CodTask { get; set; }
    }

}
