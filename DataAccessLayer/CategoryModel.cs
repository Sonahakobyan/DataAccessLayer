namespace DataAccessLayer
{
    class CategoryModel
    {
        public int ID { get; set; }
        public string Category { get; set; }

        public override string ToString() => $"ID: {ID}  Name: {Category}";
    }
}
