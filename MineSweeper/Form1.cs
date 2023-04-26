using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using unvell.D2DLib;
using unvell.D2DLib.WinForm;

namespace MineSweeper
{
	public partial class Form1 : Form
	{
		private D2DDevice _device;
		private D2DGraphics _gfx;
		private D2DBitmap _sprites;

		private D2DRect[] _cellSprites = new D2DRect[8];
		private D2DRect[] _mineCountSprites = new D2DRect[8];
		private Cell[,] _cells;

		private Size _boardDims = new Size(30, 30);
		private int _numMines = 20;

		private bool _gameOver = false;
		private bool _gameWon = false;

		public Form1()
		{
			InitializeComponent();

			widthTextBox.Text = _boardDims.Width.ToString();
			heightTextBox.Text = _boardDims.Height.ToString();
			numMinesTextBox.Text = _numMines.ToString();

			InitGfx();
			InitSprites();
			InitCells();

			DrawCells();
		}

		private void InitGfx()
		{
			_device?.Dispose();
			_device = D2DDevice.FromHwnd(pictureBox1.Handle);
			_device.Resize();
			_gfx = new D2DGraphics(_device);
		}

		private void InitSprites()
		{
			_sprites = _device.CreateBitmapFromFile($@".\minesweeper-sprites.png");

			var cellStart = new D2DRect(0, 50, 17, 17);

			for (int i = 0; i < _cellSprites.Length; i++)
			{
				_cellSprites[i] = new D2DRect(cellStart.X + cellStart.Width * i, cellStart.Y, cellStart.Width, cellStart.Height);
			}

			var numStart = new D2DRect(0, 67, 17, 17);

			for (int i = 0; i < _mineCountSprites.Length; i++)
			{
				_mineCountSprites[i] = new D2DRect(numStart.X + numStart.Width * i, numStart.Y, numStart.Width, numStart.Height);
			}
		}

		private void InitCells()
		{
			_gameOver = false;
			_gameWon = false;
			_cells = new Cell[_boardDims.Width, _boardDims.Height];

			for (int x = 0; x < _boardDims.Width; x++)
			{
				for (int y = 0; y < _boardDims.Height; y++)
				{
					_cells[x, y] = new Cell(new Point(x, y));
				}
			}

			var rnd = new Random();
			for (int i = 0; i < _numMines; i++)
			{
				int x = rnd.Next(_boardDims.Width);
				int y = rnd.Next(_boardDims.Height);

				while (_cells[x, y].HasMine)
				{
					x = rnd.Next(_boardDims.Width);
					y = rnd.Next(_boardDims.Height);
				}

				_cells[x, y].HasMine = true;
			}

			//DrawCells();
		}

		private void ResizeCells()
		{
			_device.Resize();

			for (int x = 0; x < _boardDims.Width; x++)
			{
				for (int y = 0; y < _boardDims.Height; y++)
				{
					_cells[x, y].Idx = new Point(x, y);
				}
			}

			DrawCells();
		}

		private void DrawCells()
		{
			var timer = new System.Diagnostics.Stopwatch();
			timer.Restart();

			_gfx.BeginRender(D2DColor.FromGDIColor(pictureBox1.BackColor));


			var stepX = pictureBox1.ClientRectangle.Width / (float)_boardDims.Width;
			var stepY = pictureBox1.ClientRectangle.Height / (float)_boardDims.Height;

			var min = Math.Min(stepX, stepY);

			stepX = min;
			stepY = min;

			for (int x = 0; x < _boardDims.Width; x++)
			{
				for (int y = 0; y < _boardDims.Height; y++)
				{
					var cell = _cells[x, y];
					var pos = new D2DPoint(x * stepX, y * stepX);
					var destRect = new D2DRect(pos.x, pos.y, stepX, stepY);
					var state = cell.GetState();


					//_gfx.DrawBitmap(_sprites, new D2DRect(pos.x, pos.y, stepX, stepY), _cellSprites[(int)state]);

					if (!_gameOver && !_gameWon)
					{
						_gfx.DrawBitmap(_sprites, destRect, _cellSprites[(int)state]);
					}
					else
					{
						if (cell.HasMine)
						{
							if (cell.Revealed)
								_gfx.DrawBitmap(_sprites, destRect, _cellSprites[(int)CellState.MineHit]);
							else
								if (!_gameWon)
								_gfx.DrawBitmap(_sprites, destRect, _cellSprites[(int)CellState.Mine]);
							else
								_gfx.DrawBitmap(_sprites, destRect, _cellSprites[(int)CellState.Flagged]);

						}
						else
						{
							_gfx.DrawBitmap(_sprites, destRect, _cellSprites[(int)state]);
						}
					}

					var nMines = NumMines(cell);
					if (cell.Revealed && nMines > 0 && !cell.HasMine)
					{
						_gfx.DrawBitmap(_sprites, destRect, _mineCountSprites[nMines - 1]);
					}

					if (_gameWon)
						_gfx.DrawTextCenter("You win!!!", D2DColor.Green, "Consolas", 50.0f, new D2DRect(0, 0, pictureBox1.ClientRectangle.Width, pictureBox1.ClientRectangle.Height));

				}
			}

			_gfx.EndRender(); 

			timer.Stop();
			System.Diagnostics.Debug.WriteLine(string.Format("Render: {0} ms  {1} ticks", timer.Elapsed.TotalMilliseconds, timer.Elapsed.Ticks));

		}

		private bool HasWon()
		{
			int numRevealed = 0;

			foreach (var cell in _cells)
			{
				if (cell.Revealed && !cell.HasMine)
					numRevealed++;
			}

			return numRevealed == _cells.Length - _numMines;
		}

		private void RevealEmpty(Cell target)
		{
			if (!target.Revealed && !target.HasFlag)
			{
				target.Revealed = true;

				if (NumMines(target) == 0)
				{
					foreach (var n in GetNCells(target))
					{
						if (!n.Revealed)
							RevealEmpty(n);
					}
				}
			}
		}

		private Cell[] GetNCells(Cell target)
		{
			var ns = new List<Cell>();

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0)
						continue;

					int ox = target.Idx.X + x;
					int oy = target.Idx.Y + y;

					if (ox >= 0 && ox < _boardDims.Width && oy >= 0 && oy < _boardDims.Height)
					{
						var nCell = _cells[ox, oy];
						ns.Add(nCell);
					}

				}
			}

			return ns.ToArray();
		}

		private int NumMines(Cell target)
		{
			int num = 0;

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					int ox = target.Idx.X + x;
					int oy = target.Idx.Y + y;

					if (ox >= 0 && ox < _boardDims.Width && oy >= 0 && oy < _boardDims.Height)
					{
						var nCell = _cells[ox, oy];
						if (nCell.HasMine)
							num++;
					}
				}
			}

			return num;
		}

		private void resetButton_Click(object sender, EventArgs e)
		{
			InitCells();
			DrawCells();
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			DrawCells();
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			var stepX = pictureBox1.ClientRectangle.Width / (float)_boardDims.Width;
			var stepY = pictureBox1.ClientRectangle.Height / (float)_boardDims.Height;

			var min = Math.Min(stepX, stepY);

			stepX = min;
			stepY = min;

			int idxX = (int)Math.Floor(e.X / stepX);
			int idxY = (int)Math.Floor(e.Y / stepY);

			//Debug.WriteLine($"({e.X},{e.Y}) -> ({idxX},{idxY})  Sz: ({pictureBox1.ClientRectangle.ToString()})");

			if (idxX < 0 || idxX >= _boardDims.Width || idxY < 0 || idxY >= _boardDims.Height)
				return;

			var targetCell = _cells[idxX, idxY];

			if (e.Button == MouseButtons.Left)
			{
				if (targetCell.HasFlag)
					return;

				if (targetCell.HasMine)
				{
					targetCell.Revealed = true;
					_gameOver = true;
				}
				else
				{
					RevealEmpty(targetCell);

					if (HasWon())
						_gameWon = true;
				}
			}

			if (e.Button == MouseButtons.Right)
			{
				if (!targetCell.Revealed)
					targetCell.HasFlag = !targetCell.HasFlag;
			}

			DrawCells();
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			DrawCells();
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			DrawCells();
		}

		private void Form1_ResizeEnd(object sender, EventArgs e)
		{
			ResizeCells();
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			ResizeCells();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			DrawCells();
		}

		private void pictureBox1_MouseEnter(object sender, EventArgs e)
		{
			DrawCells();
		}

		private void pictureBox1_MouseLeave(object sender, EventArgs e)
		{
			DrawCells();
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			if (int.TryParse(widthTextBox.Text, out int width) && int.TryParse(heightTextBox.Text,out int height) && int.TryParse(numMinesTextBox.Text, out int nMines))
			{
				_boardDims = new Size(width, height);
				_numMines = nMines;

				widthTextBox.Text = _boardDims.Width.ToString();
				heightTextBox.Text = _boardDims.Height.ToString();
				numMinesTextBox.Text = _numMines.ToString();

				InitCells();
				DrawCells();
			}
		}
	}
}