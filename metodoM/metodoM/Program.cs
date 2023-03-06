// See https://aka.ms/new-console-template for more information
using System;

class GranMMethod
{
    static void Main(string[] args)
    {
        // Pedir al usuario que ingrese el número de variables y restricciones
        Console.Write("Ingrese el número de variables: ");
        int n = int.Parse(Console.ReadLine());
        Console.Write("Ingrese el número de restricciones: ");
        int m = int.Parse(Console.ReadLine());

        // Pedir al usuario que ingrese los coeficientes de la matriz de restricciones
        double[,] a = new double[m, n];
        for (int i = 0; i < m; i++)
        {
            Console.WriteLine("Ingrese los coeficientes de la restricción {0}:", i + 1);
            for (int j = 0; j < n; j++)
            {
                Console.Write("a[{0},{1}] = ", i, j);
                a[i, j] = double.Parse(Console.ReadLine());
            }
        }

        // Pedir al usuario que ingrese los coeficientes de la función objetivo
        double[] c = new double[n];
        Console.WriteLine("Ingrese los coeficientes de la función objetivo:");
        for (int j = 0; j < n; j++)
        {
            Console.Write("c[{0}] = ", j);
            c[j] = double.Parse(Console.ReadLine());
        }

        // Pedir al usuario que ingrese los límites de las restricciones
        double[] b = new double[m];
        Console.WriteLine("Ingrese los límites de las restricciones:");
        for (int i = 0; i < m; i++)
        {
            Console.Write("b[{0}] = ", i);
            b[i] = double.Parse(Console.ReadLine());
        }

        // Pedir al usuario que ingrese el valor de M
        Console.Write("Ingrese el valor de M: ");
        double M = double.Parse(Console.ReadLine());

        // Definir la solución inicial
        double[] x = new double[n];

        // Aplicar el método de la gran M
        int iterations = SolveGranM(a, b, c, x, M);

        // Imprimir la solución y el número de iteraciones necesarias
        Console.WriteLine("Solución encontrada en {0} iteraciones:", iterations);
        Console.WriteLine("x = [{0}]", string.Join(", ", x));

        Console.ReadLine();
    }

    static int SolveGranM(double[,] a, double[] b, double[] c, double[] x, double M)
    {
        int m = a.GetLength(0);
        int n = a.GetLength(1);

        // Crear una matriz auxiliar para almacenar los coeficientes de la tabla simplex
        double[,] tableau = new double[m + 1, n + m + 2];

        // Copiar los coeficientes de la función objetivo a la primera fila de la tabla
        for (int j = 0; j < n; j++)
        {
            tableau[0, j] = c[j];
        }

        // Copiar los coeficientes de las restricciones a
        // las filas restantes de la tabla
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                tableau[i + 1, j] = a[i, j];
            }
            tableau[i + 1, n + i] = 1;
            tableau[i + 1, n + m] = b[i];
        }

        // Agregar la fila de la gran M a la tabla
        for (int j = 0; j < n; j++)
        {
            tableau[m + 1, j] = -M * c[j];
        }
        tableau[m + 1, n + m + 1] = -M;

        // Realizar iteraciones del método simplex hasta que se obtenga una solución óptima
        int iterations = 0;
        while (true)
        {
            // Encontrar la columna de pivote
            int pivotColumn = -1;
            double maxCoefficient = 0;
            for (int j = 0; j < n + m + 1; j++)
            {
                if (tableau[m + 1, j] < 0 && Math.Abs(tableau[m + 1, j]) > maxCoefficient)
                {
                    pivotColumn = j;
                    maxCoefficient = Math.Abs(tableau[m + 1, j]);
                }
            }
            if (pivotColumn == -1)
            {
                // Se ha alcanzado una solución óptima
                break;
            }

            // Encontrar la fila de pivote
            int pivotRow = -1;
            double minRatio = double.MaxValue;
            for (int i = 0; i < m; i++)
            {
                if (tableau[i + 1, pivotColumn] > 0)
                {
                    double ratio = tableau[i + 1, n + m + 1] / tableau[i + 1, pivotColumn];
                    if (ratio < minRatio)
                    {
                        pivotRow = i + 1;
                        minRatio = ratio;
                    }
                }
            }
            if (pivotRow == -1)
            {
                // La solución es ilimitada
                return -1;
            }

            // Realizar la operación de pivote
            double pivotElement = tableau[pivotRow, pivotColumn];
            for (int j = 0; j < n + m + 2; j++)
            {
                tableau[pivotRow, j] /= pivotElement;
            }
            for (int i = 0; i < m + 2; i++)
            {
                if (i != pivotRow)
                {
                    double multiple = tableau[i, pivotColumn];
                    for (int j = 0; j < n + m + 2; j++)
                    {
                        tableau[i, j] -= multiple * tableau[pivotRow, j];
                    }
                }
            }

            iterations++;
        }

        // Copiar la solución a la variable x
        for (int j = 0; j < n; j++)
        {
            x[j] = tableau[m + 1, j];
        }

        return iterations;
    }
}