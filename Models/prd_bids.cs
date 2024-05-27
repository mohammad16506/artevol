using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace artevol.Models
{
	public class prd_bids
	{
		public product Product { get; set; }
		public List<bid> Bids { get; set; }

	}
}