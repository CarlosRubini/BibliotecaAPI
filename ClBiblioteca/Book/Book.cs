namespace ClBiblioteca
{
	public class Book
	{
		public bool Exists { get; set; }
		public double Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public List<BookLoan> Loans { get; set; }

		public Book(bool exists, double id, string title, string author, List<BookLoan> loans)
		{
			Exists = exists;
			Id = id;
			Title = title;
			Author = author;
			Loans = loans;
		}
	}
}