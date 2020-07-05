using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMatch.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoMatch
{
    class GameField
    {
        private readonly int _cellSizeInPx;
        private readonly int _betweenCellsInPx;
        private readonly int _cellRows;
        private readonly int _cellCols;
        private Random rand = new Random();


        private Cell swapCell1;
        private Cell swapCell2;

        private Queue<Cell> _awaitBonusList;

        public int Points { get; private set; }
        public Cell[,] Field { get; private set; }
        private List<Cell> _lastAnimatedCells;
        public GamePhase _gamePhase;
        public List<Destroyer> Destroyers { get; set; }
        public GameField(GamePhase gamePhase, int cellSizeInPx, int betweenCellsInPx, int cellRows, int cellCols)
        {
            _cellSizeInPx = cellSizeInPx;
            _betweenCellsInPx = betweenCellsInPx;
            _cellRows = cellRows;
            _cellCols = cellCols;

            Field = new Cell[_cellRows, _cellCols];
            _lastAnimatedCells = new List<Cell>();
            Destroyers = new List<Destroyer>();

            Points = 0;

            _gamePhase = gamePhase;

            _awaitBonusList = new Queue<Cell>();

            SeedCells();
        }

        public void Update()
        {
            foreach (Destroyer destroyer in Destroyers)
            {
                destroyer.Update();
            }

            foreach (Cell cell in Field)
            {
                cell.Update();
            }
        }

        private List<List<Cell>> GetXCombos2()
        {
            List<List<Cell>> result = new List<List<Cell>>();

            Cell[,] cells = (Cell[,])Field.Clone();
            for (int x = 0; x < _cellCols; x++)
            {
                for (int y = 0; y < _cellRows; y++)
                {
                    List<Cell> match = XMatch(cells[x, y]);

                    foreach (Cell cell in match)
                    {
                        var (x1, y1) = GetCellPositon(cell);
                        cells[x1, y1] = null;
                    }

                    if (match.Count > 2)
                        result.Add(match);
                }
            }

            return result;
        }

        private List<List<Cell>> GetYCombos2()
        {
            List<List<Cell>> result = new List<List<Cell>>();

            Cell[,] cells = (Cell[,])Field.Clone();
            for (int x = 0; x < _cellCols; x++)
            {
                for (int y = 0; y < _cellRows; y++)
                {
                    List<Cell> match = YMatch(cells[x, y]);

                    foreach (Cell cell in match)
                    {
                        var (x1, y1) = GetCellPositon(cell);
                        cells[x1, y1] = null;
                    }

                    if (match.Count > 2)
                        result.Add(match);
                }
            }

            return result;
        }

        private List<DestroyList> GetDestroyList(List<List<Cell>> xCombos, List<List<Cell>> yCombos)
        {
            List<DestroyList> result = new List<DestroyList>();
            List<List<Cell>> crossCombos = new List<List<Cell>>();

            List<List<Cell>> horCombos = new List<List<Cell>>();
            horCombos.AddRange(xCombos);
            List<List<Cell>> verCombos = new List<List<Cell>>();
            verCombos.AddRange(yCombos);

            foreach (List<Cell> xCombo in xCombos)
            {
                foreach (List<Cell> yCombo in yCombos)
                {
                    if (xCombo.Intersect(yCombo).Count() != 0)
                    {
                        List<Cell> cross = xCombo.Intersect(yCombo).Intersect(_lastAnimatedCells).ToList();
                        if (cross.Count > 0)
                        {
                            DestroyList bombList = new DestroyList();
                            horCombos.Remove(xCombo);
                            verCombos.Remove(yCombo);
                            xCombo.AddRange(yCombo);
                            bombList.AddRange(xCombo.Distinct().ToList());
                            //Match with existent bonus
                            if (bombList.All(x => x.BonusType == BonusType.None))
                            {
                                Cell bCell = cross.First();
                                bombList.Remove(bCell);
                                bCell.BonusType = BonusType.Bomb;
                            }
                            //End match with existent bonus

                            result.Add(bombList);
                        }
                    }
                }
            }

            foreach (List<Cell> xCombo in horCombos)
            {
                if (xCombo.Count > 3)
                {
                    List<Cell> match = xCombo.Intersect(_lastAnimatedCells).ToList();
                    if (match.Count > 0)
                    {
                        DestroyList horDestroyList = new DestroyList();
                        horDestroyList.AddRange(xCombo);

                        //Match with existent bonus
                        if (horDestroyList.All(x => x.BonusType == BonusType.None))
                        {
                            Cell hCell = match.First();
                            horDestroyList.Remove(hCell);
                            hCell.BonusType = xCombo.Count < 5 ? BonusType.HorDestroyer : BonusType.Bomb;
                        }
                        //End match with existent bonus

                        result.Add(horDestroyList);
                    }
                }
                else
                {
                    DestroyList noBonusList = new DestroyList();
                    noBonusList.AddRange(xCombo);
                    result.Add(noBonusList);
                }
            }

            foreach (List<Cell> yCombo in verCombos)
            {
                if (yCombo.Count > 3)
                {
                    List<Cell> match = yCombo.Intersect(_lastAnimatedCells).ToList();
                    if (match.Count > 0)
                    {
                        DestroyList verDestroyList = new DestroyList();
                        verDestroyList.AddRange(yCombo);

                        //Match with existent bonus
                        if (verDestroyList.All(x => x.BonusType == BonusType.None))
                        {
                            Cell vCell = match.First();
                            verDestroyList.Remove(vCell);
                            vCell.BonusType = yCombo.Count < 5 ? BonusType.VerDestroyer : BonusType.Bomb;
                        }
                        //End match with existent bonus

                        result.Add(verDestroyList);
                    }
                }
                else
                {
                    DestroyList noBonusList = new DestroyList();
                    noBonusList.AddRange(yCombo);
                    result.Add(noBonusList);
                }
            }
            return result;
        }


        public GameStatus Click(int x, int y)
        {
            int cellX = GetCellXByRealX(x);
            int cellY = GetCellYByRealY(y);

            Cell clickedCell = GetCellByPosition(cellX, cellY);

            if (_gamePhase == GamePhase.Interaction)
            {
                clickedCell.IsSelected = true;
                _gamePhase = GamePhase.Selection;
            }
            else if (_gamePhase == GamePhase.Selection)
            {
                Cell selectedCell = GetSelected();
                if (IsTheyNeigbors(clickedCell, selectedCell))
                {
                    swapCell1 = clickedCell;
                    swapCell2 = selectedCell;

                    selectedCell.IsSelected = false;

                    _gamePhase = GamePhase.Swap;
                    Swap(clickedCell, selectedCell);
                }
                else
                {
                    selectedCell.IsSelected = false;
                    _gamePhase = GamePhase.Interaction;
                }
            }

            return GameStatus.GameScreen;
        }

        private void Swap(Cell cell1, Cell cell2)
        {
            var (cell1x, cell1y) = GetCellPositon(cell1);
            var (cell2x, cell2y) = GetCellPositon(cell2);
            Cell tmpCell = cell1;
            Field[cell1x, cell1y] = cell2;
            Field[cell2x, cell2y] = tmpCell;
            cell1.MoveTo(cell2.Position);
            cell2.MoveTo(cell1.Position);

        }

        private bool IsTheyNeigbors(Cell cell1, Cell cell2)
        {
            var (cell1x, cell1y) = GetCellPositon(cell1);
            var (cell2x, cell2y) = GetCellPositon(cell2);
            if (Math.Abs(cell1x - cell2x) +
                Math.Abs(cell1y - cell2y) == 1)
                return true;
            return false;
        }
        private int GetCellXByRealX(int x)
        {
            return (x - _betweenCellsInPx) / (_cellSizeInPx + _betweenCellsInPx);
        }

        private int GetCellYByRealY(int y)
        {
            return (y - _betweenCellsInPx) / (_cellSizeInPx + _betweenCellsInPx);
        }

        private CellType GetRandomCellType()
        {
            return (CellType)rand.Next(5);
        }
        private List<Cell> XMatch(Cell cell)
        {
            Cell matchCell = cell;
            List<Cell> result = new List<Cell>();

            if (cell != null)
            {
                while (matchCell != null && !matchCell.IsDestroyed && matchCell.CellType == cell.CellType)
                {
                    result.Add(matchCell);
                    var (x, y) = GetCellPositon(matchCell);
                    matchCell = GetCellByPosition(x - 1, y);
                }

                matchCell = cell;
                while (matchCell != null && !matchCell.IsDestroyed && matchCell.CellType == cell.CellType)
                {
                    result.Add(matchCell);
                    var (x, y) = GetCellPositon(matchCell);
                    matchCell = GetCellByPosition(x + 1, y);
                }
            }
            return result.Distinct().ToList();
        }

        private Cell GetSelected()
        {
            for (int i = 0; i < _cellRows; i++)
            {
                for (int j = 0; j < _cellCols; j++)
                {
                    if (Field[i, j].IsSelected)
                        return Field[i, j];
                }
            }
            return null;
        }
        private List<Cell> YMatch(Cell cell)
        {
            Cell matchCell = cell;
            List<Cell> result = new List<Cell>();

            if (cell != null)
            {
                while (matchCell != null && !matchCell.IsDestroyed && matchCell.CellType == cell.CellType)
                {
                    result.Add(matchCell);
                    var (x, y) = GetCellPositon(matchCell);
                    matchCell = GetCellByPosition(x, y - 1);
                }

                matchCell = cell;
                while (matchCell != null && !matchCell.IsDestroyed && matchCell.CellType == cell.CellType)
                {
                    result.Add(matchCell);
                    var (x, y) = GetCellPositon(matchCell);
                    matchCell = GetCellByPosition(x, y + 1);
                }
            }
            return result.Distinct().ToList();
        }


        public Cell GetCellByPosition(int x, int y)
        {
            if (x >= 0 && x < _cellCols && y >= 0 && y < _cellRows)
                return Field[x, y];
            return null;
        }


        public (int x, int y) GetCellPositon(Cell cell)
        {
            for (int i = 0; i < _cellRows; i++)
            {
                for (int j = 0; j < _cellCols; j++)
                {
                    if (cell == Field[i, j])
                        return (i, j);
                }
            }
            return (-1, -1);
        }
        private int GetRealXbyCellX(int x)
        {
            return x * (_cellSizeInPx + _betweenCellsInPx) + _betweenCellsInPx + _cellSizeInPx / 2;
        }
        private int GetRealYbyCellY(int y)
        {
            return y * (_cellSizeInPx + _betweenCellsInPx) + _betweenCellsInPx + _cellSizeInPx / 2;
        }
        private void SeedCells()
        {
            for (int i = 0; i < _cellRows; i++)
            {
                for (int j = 0; j < _cellCols; j++)
                {
                    int x = GetRealXbyCellX(i);
                    int y = GetRealYbyCellY(j);

                    CellType cellType = GetRandomCellType();
                    Vector2 position = new Vector2(x, y);
                    Cell cell = new Cell(cellType, position);
                    cell.ActionCompleted += Cell_ActionCompleted;
                    Field[i, j] = cell;

                    while (XMatch(cell).Count > 2 || YMatch(cell).Count > 2)
                    {
                        cell.CellType = GetRandomCellType();
                    }
                }
            }
        }

        public bool InProcess
        {
            get
            {
                foreach (Cell cell in Field)
                {
                    if (cell.IsBusy)
                        return true;
                }
                foreach (Destroyer destroyer in Destroyers)
                {
                    if (destroyer.IsBusy)
                        return true;
                }
                if (Destroyers.Any(x => !x.IsDestroyed))
                    return true;
                return false;
            }
        }

        private void UpdatePhase()
        {
            if (!InProcess)
            {
                Destroyers = Destroyers.Where(x => !x.IsDestroyed).ToList();
                switch (_gamePhase)
                {
                    case GamePhase.Interaction:
                        _lastAnimatedCells.Clear();
                        break;
                    case GamePhase.Swap:
                        if (GetXCombos2().Count == 0 && GetYCombos2().Count == 0)
                        {
                            Swap(swapCell2, swapCell1);
                            _gamePhase = GamePhase.SwapBack;
                        }
                        else
                            _gamePhase = GamePhase.Animation;
                            Destroy();
                        break;
                    case GamePhase.SwapBack:
                        _lastAnimatedCells.Clear();
                        _gamePhase = GamePhase.Interaction;
                        break;
                    case GamePhase.Animation:
                        if (GetXCombos2().Count > 0 || GetYCombos2().Count > 0 || _awaitBonusList.Count > 0)
                        {
                            Destroy();
                        }
                        else if (HaveDestroyed())
                        {
                            Fall();
                        }
                        else
                        {
                            _lastAnimatedCells.Clear();
                            _gamePhase = GamePhase.Interaction;
                        }
                        break;
                }
            }

        }

        private bool HaveDestroyed()
        {
            foreach (var item in Field)
            {
                if (item.IsDestroyed)
                    return true;
            }
            return false;
        }


        private void Destroy()
        {
            foreach (var destroyList in GetDestroyList(GetXCombos2(), GetYCombos2()))
            {
                foreach (var item in destroyList)
                {
                    if (!item.IsBusy && item.BonusType != BonusType.None && !_awaitBonusList.Contains(item))
                        _awaitBonusList.Enqueue(item);
                    item.Destroy();
                }
            }

            if (_awaitBonusList.Count > 0)
                ActivateBonuses();
            else
                UpdatePhase();
        }
        private void ActivateBonuses()
        {
            while (_awaitBonusList.Count > 0)
            {
                Cell bonusCell = _awaitBonusList.Dequeue();
                switch (bonusCell.BonusType)
                {
                    case BonusType.Bomb:
                        ActivateBomb(bonusCell);
                        break;
                    case BonusType.HorDestroyer:
                        _gamePhase = GamePhase.Animation;
                        Destroyer horDestroyer = new Destroyer(
                            bonusCell.Position,
                            new Vector2(0 - _cellSizeInPx - _betweenCellsInPx, bonusCell.Position.Y),
                            new Vector2((_cellCols + 2) * (_cellSizeInPx + _betweenCellsInPx), bonusCell.Position.Y),
                            bonusCell.BonusType, _cellSizeInPx + _betweenCellsInPx);
                        horDestroyer.StepCompleted += Destroyer_Moved;
                        horDestroyer.ActionCompleted += Destroyer_ActionCompleted;
                        Destroyers.Add(horDestroyer);
                        bonusCell.Destroy();
                        break;
                    case BonusType.VerDestroyer:
                        _gamePhase = GamePhase.Animation;
                        Destroyer verDestroyer = new Destroyer(
                            bonusCell.Position,
                            new Vector2(bonusCell.Position.X, 0 - _cellSizeInPx - _betweenCellsInPx),
                            new Vector2(bonusCell.Position.X, (_cellCols + 2) * (_cellSizeInPx + _betweenCellsInPx)),
                            bonusCell.BonusType, _cellSizeInPx + _betweenCellsInPx);
                        verDestroyer.StepCompleted += Destroyer_Moved;
                        verDestroyer.ActionCompleted += Destroyer_ActionCompleted;
                        Destroyers.Add(verDestroyer);
                        bonusCell.Destroy();
                        break;
                }

            }
            UpdatePhase();
        }

        private void ActivateBomb(Cell bombCell)
        {
            (int x, int y) = GetCellPositon(bombCell);

            int xMin = (x - 1 < 0) ? 0 : x - 1;
            int xMax = (x + 1 == _cellCols) ? _cellCols : x + 2;
            int yMin = (y - 1 < 0) ? 0 : y - 1;
            int yMax = (y + 1 == _cellRows) ? _cellRows : y + 2;
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    if (Field[i, j] != bombCell && Field[i, j].BonusType != BonusType.None && !_awaitBonusList.Contains(Field[i, j]) && !Field[i, j].IsBusy && !Field[i, j].IsDestroyed)
                        _awaitBonusList.Enqueue(Field[i, j]);
                    if (!Field[i, j].IsBusy && !Field[i,j].IsDestroyed)
                        Field[i, j].DestroyByBomb();
                }
            }
        }

        private void Cell_ActionCompleted(object sender, EventArgs e)
        {
            _lastAnimatedCells.Add((Cell)sender);
            if (((Cell)sender).IsDestroyed)
                Points++;
            UpdatePhase();
        }
        private void Destroyer_ActionCompleted(object sender, EventArgs e)
        {
            UpdatePhase();
        }

        private void Destroyer_Moved(object sender, EventArgs e)
        {
            Destroyer destroyer = (Destroyer)sender;
            int x = GetCellXByRealX((int)destroyer.Position1.X);
            int y = GetCellYByRealY((int)destroyer.Position1.Y);
            if (x >= 0 && x < _cellCols && y >= 0 && y < _cellRows)
            {
                if (!Field[x, y].IsBusy && !Field[x, y].IsDestroyed && Field[x, y].BonusType != BonusType.None && !_awaitBonusList.Contains(Field[x, y]))
                    _awaitBonusList.Enqueue(Field[x, y]);
                Field[x, y].Destroy();
            }


            x = GetCellXByRealX((int)destroyer.Position2.X);
            y = GetCellYByRealY((int)destroyer.Position2.Y);
            if (x >= 0 && x < _cellCols && y >= 0 && y < _cellRows)
            {
                if (!Field[x, y].IsBusy && !Field[x, y].IsDestroyed && Field[x, y].BonusType != BonusType.None && !_awaitBonusList.Contains(Field[x, y]))
                    _awaitBonusList.Enqueue(Field[x, y]);
                Field[x, y].Destroy();
            }
        }

        public void Fall()
        {
            _lastAnimatedCells.Clear();

            _gamePhase = GamePhase.Animation;
            for (int col = 0; col < _cellCols; col++)
            {
                int rowDestroyed = 0;
                for (int row = _cellRows - 1; row >= 0; row--)
                {
                    if (Field[col, row].IsDestroyed)
                    {
                        rowDestroyed++;
                    }
                    if (rowDestroyed > 0 && !Field[col, row].IsDestroyed)
                    {
                        int newRow = row + rowDestroyed;
                        Field[col, row].MoveTo(new Vector2(GetRealXbyCellX(col), GetRealYbyCellY(newRow)));
                        Field[col, newRow] = Field[col, row];
                    }
                }

                for (int row = 0; row < rowDestroyed; row++)
                {
                    Field[col, row] = new Cell(GetRandomCellType(), new Vector2(GetRealXbyCellX(col), (_cellSizeInPx + _betweenCellsInPx) * (rowDestroyed - row) * -1));
                    Field[col, row].ActionCompleted += Cell_ActionCompleted;
                    Field[col, row].MoveTo(new Vector2(GetRealXbyCellX(col), GetRealYbyCellY(row)));
                }
            }
            UpdatePhase();
        }
    }
}


