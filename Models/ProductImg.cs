using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace artevol.Models
{
	public class ProductImg
	{
		public product Product { get; set; }
		public List<image> Images { get; set; } = new List<image>();
	}
}