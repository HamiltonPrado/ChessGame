using System;
using System.Collections.Generic;

namespace ChessGame
{
    // Representa uma peça no tabuleiro
    abstract class Piece
    {
        public string Name { get; set; }
        public string Color { get; set; } // "White" ou "Black"
        public abstract bool IsValidMove(int startX, int startY, int endX, int endY, Board board);
    }

    class Pawn : Piece
    {
        public override bool IsValidMove(int startX, int startY, int endX, int endY, Board board)
        {
            int direction = (Color == "White") ? 1 : -1;

            // Movimento para frente
            if (startX + direction == endX && startY == endY && board.GetPiece(endX, endY) == null)
                return true;

            // Captura diagonal
            if (startX + direction == endX && Math.Abs(startY - endY) == 1 && board.GetPiece(endX, endY)?.Color != Color)
                return true;

            // Movimento inicial de dois passos
            if ((Color == "White" && startX == 1 || Color == "Black" && startX == 6) &&
                startX + 2 * direction == endX && startY == endY && board.GetPiece(endX, endY) == null)
                return true;

            return false;
        }
    }

    class Rook : Piece
    {
        public override bool IsValidMove(int startX, int startY, int endX, int endY, Board board)
        {
            if (startX == endX || startY == endY)
            {
                // Verificar caminho livre
                if (board.IsPathClear(startX, startY, endX, endY))
                    return true;
            }
            return false;
        }
    }

    class Knight : Piece
    {
        public override bool IsValidMove(int startX, int startY, int endX, int endY, Board board)
        {
            int dx = Math.Abs(startX - endX);
            int dy = Math.Abs(startY - endY);
            return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);
        }
    }

    class Bishop : Piece
    {
        public override bool IsValidMove(int startX, int startY, int endX, int endY, Board board)
        {
            if (Math.Abs(startX - endX) == Math.Abs(startY - endY))
            {
                // Verificar caminho livre
                if (board.IsPathClear(startX, startY, endX, endY))
                    return true;
            }
            return false;
        }
    }

    class Queen : Piece
    {
        public override bool IsValidMove(int startX, int startY, int endX, int endY, Board board)
        {
            // Combina movimentos de torre e bispo
            return new Rook { Color = Color }.IsValidMove(startX, startY, endX, endY, board) ||
                   new Bishop { Color = Color }.IsValidMove(startX, startY, endX, endY, board);
        }
    }

    class King : Piece
    {
        public override bool IsValidMove(int startX, int startY, int endX, int endY, Board board)
        {
            int dx = Math.Abs(startX - endX);
            int dy = Math.Abs(startY - endY);

            // Movimento normal
            if (dx <= 1 && dy <= 1)
                return true;

            // Adicionar lógica de roque aqui

            return false;
        }
    }

    class Board
    {
        public Piece[,] Grid { get; private set; }

        public Board()
        {
            Grid = new Piece[8, 8];
            Initialize();
        }

        private void Initialize()
        {
            // Inicializar peões
            for (int i = 0; i < 8; i++)
            {
                Grid[1, i] = new Pawn { Name = "Pawn", Color = "White" };
                Grid[6, i] = new Pawn { Name = "Pawn", Color = "Black" };
            }

            // Inicializar outras peças
            Grid[0, 0] = Grid[0, 7] = new Rook { Name = "Rook", Color = "White" };
            Grid[7, 0] = Grid[7, 7] = new Rook { Name = "Rook", Color = "Black" };
            Grid[0, 1] = Grid[0, 6] = new Knight { Name = "Knight", Color = "White" };
            Grid[7, 1] = Grid[7, 6] = new Knight { Name = "Knight", Color = "Black" };
            Grid[0, 2] = Grid[0, 5] = new Bishop { Name = "Bishop", Color = "White" };
            Grid[7, 2] = Grid[7, 5] = new Bishop { Name = "Bishop", Color = "Black" };
            Grid[0, 3] = new Queen { Name = "Queen", Color = "White" };
            Grid[7, 3] = new Queen { Name = "Queen", Color = "Black" };
            Grid[0, 4] = new King { Name = "King", Color = "White" };
            Grid[7, 4] = new King { Name = "King", Color = "Black" };
        }

        public Piece GetPiece(int x, int y)
        {
            return Grid[x, y];
        }

        public void MovePiece(int startX, int startY, int endX, int endY)
        {
            Grid[endX, endY] = Grid[startX, startY];
            Grid[startX, startY] = null;
        }

        public bool IsPathClear(int startX, int startY, int endX, int endY)
        {
            // Verificar caminho reto ou diagonal
            int dx = Math.Sign(endX - startX);
            int dy = Math.Sign(endY - startY);

            int x = startX + dx;
            int y = startY + dy;

            while (x != endX || y != endY)
            {
                if (Grid[x, y] != null)
                    return false;

                x += dx;
                y += dy;
            }

            return true;
        }
    }

    class Game
    {
        private Board board;

        public Game()
        {
            board = new Board();
        }

        public void Play()
        {
            while (true)
            {
                Console.Clear();
                DisplayBoard();
                Console.WriteLine("Enter your move (e.g., e2 e4):");
                string move = Console.ReadLine();

                if (ProcessMove(move))
                {
                    Console.WriteLine("Move successful.");
                }
                else
                {
                    Console.WriteLine("Invalid move. Try again.");
                }
            }
        }

        private void DisplayBoard()
        {
            Console.WriteLine("   a  b  c  d  e  f  g  h"); // Letras das colunas
            for (int i = 0; i < 8; i++)
            {
                Console.Write($"{8 - i} "); // Números das linhas (8 a 1)
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board.GetPiece(i, j);
                    Console.Write(piece == null ? " . " : $" {piece.Name[0]} ");
                }
                Console.WriteLine($" {8 - i}"); // Número da linha no final
            }
            Console.WriteLine("   a  b  c  d  e  f  g  h"); // Letras das colunas novamente
        }

        private bool ProcessMove(string move)
        {
            try
            {
                string[] parts = move.Split(' ');
                int startX = 8 - int.Parse(parts[0][1].ToString());
                int startY = parts[0][0] - 'a';
                int endX = 8 - int.Parse(parts[1][1].ToString());
                int endY = parts[1][0] - 'a';

                Piece piece = board.GetPiece(startX, startY);
                if (piece != null && piece.IsValidMove(startX, startY, endX, endY, board))
                {
                    board.MovePiece(startX, startY, endX, endY);
                    return true;
                }
            }
            catch
            {
                // Ignorar erros de entrada
            }

            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Play();
        }
    }
}
