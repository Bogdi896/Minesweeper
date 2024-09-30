using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Minesweeper.Models
{
    internal class GameBoard
    {
        public ObservableCollection<Cell> Cells { get; set; }
        private int rows, columns, bombCount;
        private bool bombsPlaced = false;
        public bool BombsPlaced => bombsPlaced;

        public GameBoard(int rows, int columns, int bombCount)
        {
            this.rows = rows;
            this.columns = columns;
            this.bombCount = bombCount;
            Cells = new ObservableCollection<Cell>();
            InitializeCells();
        }

        private void InitializeCells()
        {
            Cells.Clear();
            for (int i = 0; i < rows * columns; i++)
            {
                Cells.Add(new Cell());
            }
        }

        private void PlaceBombs(int excludeRow, int excludeCol)
        {
            var random = new Random();
            int bombsPlaced = 0;
            while (bombsPlaced < bombCount)
            {
                int index = random.Next(0, Cells.Count);
                int row = index / columns;
                int col = index % columns;

                // Exclude the first clicked cell and its adjacent cells
                if (!Cells[index].IsMine && !IsAdjacent(excludeRow, excludeCol, row, col))
                {
                    Cells[index].IsMine = true;
                    bombsPlaced++;
                }
            }
            CalculateAdjacentMines();
        }

        private bool IsAdjacent(int row1, int col1, int row2, int col2)
        {
            return Math.Abs(row1 - row2) <= 1 && Math.Abs(col1 - col2) <= 1;
        }

        private void CalculateAdjacentMines()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (!Cells[i * columns + j].IsMine)
                    {
                        int adjacentMines = CountAdjacentMines(i, j);
                        Cells[i * columns + j].AdjacentMinesCount = adjacentMines;
                    }
                }
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;
                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < columns)
                    {
                        if (Cells[newRow * columns + newCol].IsMine)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public void RevealCell(int row, int col)
        {
            int index = row * columns + col;
            Cell cell = Cells[index];

            // Only reveal the cell if it's not already revealed or flagged
            if (!cell.IsRevealed && !cell.IsFlagged)
            {
                cell.IsRevealed = true;
                if (cell.AdjacentMinesCount == 0 && !cell.IsMine)
                {
                    // Recursively reveal adjacent cells
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0) continue; // Skip the current cell

                            int newRow = row + i;
                            int newCol = col + j;
                            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < columns)
                            {
                                RevealCell(newRow, newCol); // Recursive call
                            }
                        }
                    }
                }
            }
        }

        public void FirstCellClicked(int row, int col)
        {
            if (!bombsPlaced)
            {
                PlaceBombs(row, col);
                bombsPlaced = true;
            }
            RevealCell(row, col);
        }

        public bool CheckWinCondition()
        {
            foreach (var cell in Cells)
            {
                if (!cell.IsMine && !cell.IsRevealed)
                {
                    return false;  // There are still non-mine cells that are not revealed
                }
            }
            return true;  // All non-mine cells are revealed, player wins
        }




        // Additional methods for calculating adjacent mines, revealing cells, etc., will be added here.
    }

}
