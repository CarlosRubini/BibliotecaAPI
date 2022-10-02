using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using ClBiblioteca;

namespace AppBibliotecaAPI.Controllers
{
	[ApiController]
	[Route("book")]
	public class BookController : ControllerBase
	{
		private readonly IConfiguration Configuration;
		public BookController(IConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		[HttpGet]
		[Route("getBooks")]
		public IEnumerable<Book> GetAll()
		{
			List<Book> books;
			using SqlConnection connection = new(Configuration.GetConnectionString("AzureConnection"));
			{
				connection.Open();
				books = new BookDAO(connection).FindAll(new BookFilter());
				connection.Close();
			}
			return books;
		}

		[HttpPut]
		[Route("saveBook")]
		public Book Save(Book book)
		{
			ValidateBook(book);
			using SqlConnection connection = new(Configuration.GetConnectionString("AzureConnection"));
			{
				connection.Open();
				SqlTransaction tr = connection.BeginTransaction();
				try
				{
					new BookDAO(connection).Save(book, tr);
					tr.Commit();
				}
				catch (Exception ex)
				{
					tr.Rollback();
					throw new ApplicationException(ex.Message);
				}
				finally
				{
					tr.Dispose();
					connection.Close();
				}
			}
			return book;
		}

		[HttpDelete]
		[Route("deleteBook")]
		public bool Delete(Book book)
		{
			ValidateBook(book);
			using SqlConnection connection = new(Configuration.GetConnectionString("AzureConnection"));
			{
				connection.Open();
				SqlTransaction tr = connection.BeginTransaction();
				try
				{
					new BookDAO(connection).Delete(book, tr);
					tr.Commit();
				}
				catch (Exception ex)
				{
					tr.Rollback();
					throw new ApplicationException(ex.Message);
				}
				finally
				{
					tr.Dispose();
					connection.Close();
				}
			}
			return true;
		}

		private static void ValidateBook(Book book)
		{
			if (book == null) throw new ArgumentNullException(book!.ToString());
			if (string.IsNullOrEmpty(book.Title)) throw new ArgumentNullException(book.ToString());
			if (string.IsNullOrEmpty(book.Author)) throw new ArgumentNullException(book.ToString());
		}
	}
}
