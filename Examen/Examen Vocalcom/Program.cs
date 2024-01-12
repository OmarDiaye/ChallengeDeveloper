using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

public class WordFinder
{
    private readonly char[,] matrix;

    public WordFinder(IEnumerable<string> matrix)
    {
        if (matrix == null || !matrix.Any())
            throw new ArgumentException("La matriz no debe ser nullo o estar vacia.");

        // Validaciòn de las dimenciones para la matriz.
        int rowCount = matrix.Count();
        int colCount = matrix.First().Length;

        if (rowCount > 64 || colCount > 64)
            throw new ArgumentException("La matris debe estar limitada a 64x64.");

        // Inicializa la matriz
        this.matrix = new char[rowCount, colCount];

        for (int i = 0; i < rowCount; i++)
        {
            string row = matrix.ElementAt(i);

            if (row.Length != colCount)
                throw new ArgumentException("Todas las filas de la matriz deben tener la misma longitud..");

            for (int j = 0; j < colCount; j++)
            {
                this.matrix[i, j] = row[j];
            }
        }
    }

    public IEnumerable<string> Find(IEnumerable<string> wordstream)
    {
        if (wordstream == null || !wordstream.Any())
            return Enumerable.Empty<string>();

        // Esta funcion aplana la matriz en una sola cadena(ignorando mayúsculas y minúsculas)
        string flatMatrix = new string(Enumerable.Range(0, matrix.GetLength(0))
            .SelectMany(i => Enumerable.Range(0, matrix.GetLength(1))
                .Select(j => matrix[i, j]))
            .ToArray());

        // Extrae todas las posibilidades de palabras de forma horizontal y vertical
        var allWords = wordstream
            .SelectMany(word => ExtractWords(word, flatMatrix, StringComparison.OrdinalIgnoreCase))
            .GroupBy(word => word)
            .Select(group => group.Key)
            .OrderByDescending(word => flatMatrix.Split(new[] { word }, StringSplitOptions.None).Length - 1)
            .Take(10);

        return allWords;
    }

    private IEnumerable<string> ExtractWords(string word, string flatMatrix, StringComparison comparison)
    {
        int rowCount = matrix.GetLength(0);
        int colCount = matrix.GetLength(1);

        // Analisis de palabras horizontales
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j <= colCount - word.Length; j++)
            {
                yield return new string(Enumerable.Range(0, word.Length)
                    .Select(k => matrix[i, j + k])
                    .ToArray());
            }
        }

        // Analisis de palabras verticales
        for (int j = 0; j < colCount; j++)
        {
            for (int i = 0; i <= rowCount - word.Length; i++)
            {
                yield return new string(Enumerable.Range(0, word.Length)
                    .Select(k => matrix[i + k, j])
                    .ToArray());
            }
        }
    }
}

class Program
{
    static void Main()
    {
        // Ejemplo de uso
        var matrix = new List<string>
        {
           "abcdc",
           "fgwio",
           "chill",
           "pqnsd",
           "uvdxy"
        };

        var wordFinder = new WordFinder(matrix);
        

        var wordstream = new List<string>
        {
            "cold", "wind", "snow", "chill"
        };

        var result = wordFinder.Find(wordstream);

        Console.WriteLine("Las 10 palabras mas repetidas son:");
        foreach (var word in result)
        {
            Console.WriteLine(word);
        }
    }
}
