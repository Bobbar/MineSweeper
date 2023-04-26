using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;
using System.Drawing;

namespace MineSweeper
{
	public class Cell
	{
		public bool HasMine { get; set; } = false;
		public bool HasFlag { get; set; } = false;
		public bool Visited { get; set; } = false;
		public bool Revealed { get; set; } = false;
	

		//public bool HasMine 
		//{ 
		//	get { return State == CellState.Mine || State == CellState.MineHit || State == CellState.MineX; }
		//	set
		//	{
		//		if (value)
		//			State = CellState.Mine;
		//	}
		//}

		//public bool HasFlag 
		//{ 
		//	get { return State == CellState.Flagged; }
		//	set { if (value) State = CellState.Flagged; }
		//}

		//public bool Revealed 
		//{ 
		//	get { return State == CellState.Revealed || State == CellState.QuestionRevealed; }
		//	set { if (value) State |= CellState.Revealed; } 
		//}

		public CellState State { get; set; } = CellState.Hidden;

		//public D2DPoint Idx { get; set; }
		public Point Idx { get; set; }


		public Cell() { }

		public Cell(Point idx)
		{
			Idx = idx;
		}

		//public Cell(D2DPoint idx) 
		//{
		//	Idx = idx;
		//}

		public CellState GetState()
		{
			if (!Revealed && !HasMine && !HasFlag)
				return CellState.Hidden;
			else if (Revealed && !HasMine && !HasFlag)
				return CellState.Revealed;
			else if (Revealed && HasMine && !HasFlag)
				return CellState.MineHit;
			else if (!Revealed && HasFlag)
				return CellState.Flagged;

			//if (HasFlag)
			//	return CellState.Flagged;
			
			return CellState.Hidden;
		}

	}

	
	public enum CellState
	{
		Hidden,
		Revealed,
		Flagged,
		QuestionHidded,
		QuestionRevealed,
		Mine,
		MineHit,
		MineX

	}
}
