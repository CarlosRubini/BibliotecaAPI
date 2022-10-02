using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClBiblioteca
{
	public class BookLoan
	{
		public bool Exists { get; set; }
		public double Id { get; set; }
		public double BookId { get; set; }
		public DateTime Borrowed { get; set; }
		public DateTime ReturnDate { get; set; }
		public DateTime Returned { get; set; }

		public BookLoan(bool exists,double id, double bookId, DateTime borrowed,DateTime returnDate, DateTime returned)
		{
			Exists = exists;
			Id = id;
			BookId = bookId;
			Borrowed = borrowed;
			ReturnDate = returnDate;
			Returned = returned;
		}
	}
}
