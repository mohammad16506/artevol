using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace artevol.Models
{
	public class cart
	{
		public int id { get; set; }
		public string name { get; set; }
		public Nullable<int> price { get; set; }
		public int quantity { get; set; }
		public decimal total { get; set; }
	}
}