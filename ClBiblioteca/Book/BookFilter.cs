using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClBiblioteca
{
	public class BookFilter
	{
		public int? ID { get; set; }
		public string? Title { get; set; }
		public string? Author { get; set; }

		public BookFilter() { 
		}
	}
}
