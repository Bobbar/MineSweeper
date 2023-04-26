using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace MineSweeper
{
	public static class Extensions
	{

		public static string ToString(this D2DPoint point)
		{
			return $"({point.x.ToString()}, {point.y.ToString()})";
		}
	}
}
