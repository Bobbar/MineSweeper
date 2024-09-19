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
        private D2DRect[] _numberSprites = new D2DRect[10];
        private D2DRect[] _faceSprites = new D2DRect[5];
        private FaceStates _faceState = FaceStates.HappyUp;

        private Cell[,] _cells;

        private Size _boardDims = new Size(30, 30);
        private int _numMines = 20;

        private D2DRect _headerRect;

        private bool _gameOver = false;
        private bool _gameWon = false;
        private bool _firstClick = false;

        private System.Windows.Forms.Timer _gameTimer = new System.Windows.Forms.Timer();
        private int _gameTimeSecs = 0;


        private int RemainingMines
        {
            get
            {
                return _numMines - NumFlags();
            }
        }

        private enum FaceStates
        {
            HappyUp,
            HappyDown,
            Surprised,
            Cool,
            Dead
        }

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

            _gameTimer.Interval = 1000;
            _gameTimer.Tick += GameTimer_Tick;
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            _gameTimeSecs += 1;

            DrawCells();
        }

        private void InitGfx()
        {
            _device?.Dispose();
            _device = D2DDevice.FromHwnd(pictureBox1.Handle);
            _device.Resize();
            _gfx = new D2DGraphics(_device);

            var step = GetStep();
            _headerRect = new D2DRect(0, 0, step * _boardDims.Width, 50);
        }

        private void InitSprites()
        {
            _sprites = _device.CreateBitmapFromFile($@".\minesweeper-sprites.png");

            float x = 0;

            var cellStart = new D2DRect(0, 51, 16, 16);

            x = 0;
            for (int i = 0; i < _cellSprites.Length; i++)
            {
                _cellSprites[i] = new D2DRect(x, cellStart.Y, cellStart.Width, cellStart.Height);
                x += cellStart.Width + 1;
            }

            var mineCountStart = new D2DRect(0, 68, 16, 16);
            x = 0;
            for (int i = 0; i < _mineCountSprites.Length; i++)
            {
                _mineCountSprites[i] = new D2DRect(x, mineCountStart.Y, mineCountStart.Width, mineCountStart.Height);
                x += mineCountStart.Width + 1;
            }

            var numbersStart = new D2DRect(0, 0, 13, 23);
            x = 0;
            for (int i = 0; i < _numberSprites.Length; i++)
            {
                _numberSprites[i] = new D2DRect(x, numbersStart.Y, numbersStart.Width, numbersStart.Height);
                x += numbersStart.Width + 1;
            }

            var faceStart = new D2DRect(0, 24, 26, 26);
            x = 0;
            for (int i = 0; i < _faceSprites.Length; i++)
            {
                _faceSprites[i] = new D2DRect(x, faceStart.Y, faceStart.Width, faceStart.Height);
                x += faceStart.Width + 1;
            }
        }

        private void InitCells()
        {
            _gameOver = false;
            _gameWon = false;
            _firstClick = false;
            _gameTimer.Stop();
            _gameTimeSecs = 0;
            _faceState = FaceStates.HappyUp;

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

            var step = GetStep();
            _headerRect = new D2DRect(0, 0, step * _boardDims.Width, 50);

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
            _gfx.BeginRender(D2DColor.FromGDIColor(pictureBox1.BackColor));

            var step = GetStep();

            _gfx.FillRectangle(_headerRect, D2DColor.Gray);
            DrawNumberBox(_gfx, new D2DPoint(10, 12), 3, _gameWon ? 0 : RemainingMines);
            DrawNumberBox(_gfx, new D2DPoint(_headerRect.Width - (13 * 5), 12), 3, _gameTimeSecs);
            DrawFace(_gfx, new D2DPoint(_headerRect.Width / 2.0f, _headerRect.Height / 2.0f), _faceState);

            for (int x = 0; x < _boardDims.Width; x++)
            {
                for (int y = 0; y < _boardDims.Height; y++)
                {
                    var cell = _cells[x, y];
                    var pos = new D2DPoint(x * step, (y * step) + _headerRect.Height);
                    var destRect = new D2DRect(pos.x, pos.y, step, step);
                    var state = cell.GetState();

                    if (!_gameOver && !_gameWon)
                    {
                        DrawSprite(_gfx, destRect, _cellSprites[(int)state]);
                    }
                    else
                    {
                        if (cell.HasMine)
                        {
                            if (cell.Revealed)
                                DrawSprite(_gfx, destRect, _cellSprites[(int)CellState.MineHit]);
                            else
                                if (!_gameWon)
                                DrawSprite(_gfx, destRect, _cellSprites[(int)CellState.Mine]);
                            else
                                DrawSprite(_gfx, destRect, _cellSprites[(int)CellState.Flagged]);
                        }
                        else
                        {
                            DrawSprite(_gfx, destRect, _cellSprites[(int)state]);
                        }
                    }

                    var nMines = NumMines(cell);
                    if (cell.Revealed && nMines > 0 && !cell.HasMine)
                    {
                        DrawSprite(_gfx, destRect, _mineCountSprites[nMines - 1]);
                    }

                    //if (_gameWon)
                    //	_gfx.DrawTextCenter("You win!!!", D2DColor.Green, "Consolas", 50.0f, new D2DRect(0, 0, pictureBox1.ClientRectangle.Width, pictureBox1.ClientRectangle.Height));
                }
            }

            _gfx.EndRender();
        }

        private void DrawSprite(D2DGraphics gfx, D2DRect dest, D2DRect src)
        {
            gfx.DrawBitmap(_sprites, dest, src, 1f, D2DBitmapInterpolationMode.NearestNeighbor);
        }

        private void DrawNumberBox(D2DGraphics gfx, D2DPoint loc, int nDigits, int value)
        {
            var dest = _numberSprites[0];
            var pos = new D2DPoint(loc.x, loc.y);
            var start = nDigits - value.ToString().Length;
            var destRect = new D2DRect(pos.x, pos.y, dest.Width, dest.Height);

            for (int i = 0; i < start; i++)
            {
                gfx.DrawBitmap(_sprites, destRect, _numberSprites[9]);

                pos.x += dest.Width;
                destRect = new D2DRect(pos.x, pos.y, dest.Width, dest.Height);
            }

            foreach (var digit in GetDigits(value))
            {
                int digitIdx = (digit - 1 >= 0) ? digit - 1 : 9;

                gfx.DrawBitmap(_sprites, destRect, _numberSprites[digitIdx]);

                pos.x += dest.Width;
                destRect = new D2DRect(pos.x, pos.y, dest.Width, dest.Height);
            }
        }

        private void DrawFace(D2DGraphics gfx, D2DPoint loc, FaceStates state)
        {
            var dest = new D2DRect(loc, _faceSprites[0].Size);
            gfx.DrawBitmap(_sprites, dest, _faceSprites[(int)state]);
        }

        private IEnumerable<int> GetDigits(int source)
        {
            int individualFactor = 0;
            int tennerFactor = Convert.ToInt32(Math.Pow(10, source.ToString().Length));
            do
            {
                source -= tennerFactor * individualFactor;
                tennerFactor /= 10;
                individualFactor = source / tennerFactor;

                yield return individualFactor;
            } while (tennerFactor > 1);
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

        private int NumFlags()
        {
            int num = 0;

            for (int x = 0; x < _boardDims.Width; x++)
            {
                for (int y = 0; y < _boardDims.Height; y++)
                {
                    if (_cells[x, y].HasFlag)
                        num++;
                }
            }

            return num;
        }

        private float GetStep()
        {
            var stepX = pictureBox1.ClientRectangle.Width / (float)_boardDims.Width;
            var stepY = (pictureBox1.ClientRectangle.Height - _headerRect.Height) / (float)_boardDims.Height;
            var min = (float)Math.Floor(Math.Min(stepX, stepY));

            return min;
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

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_gameOver && !_gameWon)
                _faceState = FaceStates.Surprised;

            DrawCells();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_gameOver && !_gameWon)
                _faceState = FaceStates.HappyUp;

            float step = GetStep();
            int idxX = (int)Math.Floor(e.X / step);
            int idxY = (int)Math.Floor((e.Y - _headerRect.Height) / step);

            //Debug.WriteLine($"({e.X},{e.Y}) -> ({idxX},{idxY})  Sz: ({pictureBox1.ClientRectangle.ToString()})");

            if (idxX < 0 || idxX >= _boardDims.Width || idxY < 0 || idxY >= _boardDims.Height)
            {
                DrawCells();
                return;
            }

            var targetCell = _cells[idxX, idxY];

            if (e.Button == MouseButtons.Left)
            {
                if (targetCell.HasFlag)
                    return;

                if (targetCell.HasMine)
                {
                    targetCell.Revealed = true;
                    _gameOver = true;
                    _gameTimer.Stop();
                    _faceState = FaceStates.Dead;
                }
                else
                {
                    if (_gameTimer.Enabled == false && !_gameOver && !_gameWon)
                    {
                        _gameTimeSecs = 1;
                        _gameTimer.Start();
                    }

                    RevealEmpty(targetCell);

                    if (HasWon())
                    {
                        _gameWon = true;
                        _gameTimer.Stop();
                        _faceState = FaceStates.Cool;
                    }
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
            if (int.TryParse(widthTextBox.Text, out int width) && int.TryParse(heightTextBox.Text, out int height) && int.TryParse(numMinesTextBox.Text, out int nMines))
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

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            DrawCells();
        }
    }
}