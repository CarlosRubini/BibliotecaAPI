using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClBiblioteca
{
	public class BookDAO : DAO<BookFilter, Book>
	{
		public BookDAO(SqlConnection connection) : base(connection)
		{
		}

		public override string GetSqlSelect(BookFilter filter)
		{
			Validate(filter);

			StringBuilder strSQL = new($@"SELECT BK.id
												,BK.title
								    			,BK.author
										  FROM BltBook BK");
			if (filter.ID.HasValue)
			{
				strSQL.AppendLine($"AND BK.id={filter.ID}");
			}
			if (!string.IsNullOrEmpty(filter.Title))
			{
				strSQL.AppendLine($"AND BK.title='{filter.Title}'");
			}
			if (!string.IsNullOrEmpty(filter.Author))
			{
				strSQL.AppendLine($"AND BK.author='{filter.Author}'");
			}
			return strSQL.ToString();
		}

		public override List<Book> FindAll(BookFilter filter)
		{
			List<Book> books = base.FindAll(filter);
			List<BookLoan> loans = new BookLoanDAO(Connection).FindAll(filter);
			return BooksWithLoans(books, loans);
		}

		private static List<Book> BooksWithLoans(List<Book> books,List<BookLoan> loans)
		{
			foreach (Book book in books)
			{
				book.Loans = loans.Where(loan => loan.BookId == book.Id).ToList();
			}
			return books;
		}

		internal override Book LoadObject(IDataReader dr)
		{
			return new Book( exists: true
						   , id: dr.GetDouble(0)
						   , title: dr.GetString(1)
						   , author: dr.GetString(2)
						   , loans: new List<BookLoan>());
		}

		public override void Save(Book book, SqlTransaction transaction)
		{
			string strSQL;
			if (!book.Exists)
			{
				strSQL = @"INSERT INTO BltBook(title,author) VALUES(@title,@author)
						   Select Scope_Identity()";
				SqlCommand comando = new(strSQL, Connection,transaction);
				comando.Parameters.Add(new SqlParameter("@title", book.Title));
				comando.Parameters.Add(new SqlParameter("@author", book.Author));
				book.Id = Convert.ToInt32(comando.ExecuteScalar());
				book.Exists = true;
			}
			else
			{
				strSQL = "UPDATE BltBook set title=@title, author=@author WHERE id=@id";
				SqlCommand comando = new(strSQL, Connection, transaction);
				comando.Parameters.Add(new SqlParameter("@id", book.Id));
				comando.Parameters.Add(new SqlParameter("@title", book.Title));
				comando.Parameters.Add(new SqlParameter("@author", book.Author));
				comando.ExecuteScalar();
			}
		}

		public override void Delete(Book book, SqlTransaction transaction)
		{
			string strSQL = "DELETE FROM BltBook WHERE id=@id";
			SqlCommand comando = new(strSQL, Connection, transaction);
			comando.Parameters.Add(new SqlParameter("@id", book.Id));
			comando.ExecuteNonQuery();

			new BookLoanDAO(Connection).Delete(book.Loans, transaction);
		}
	}
}
