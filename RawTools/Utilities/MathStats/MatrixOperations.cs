// Copyright 2018 Kevin Kovalchik & Christopher Hughes
//
// The contents of the MatrixOperations class below are based on the following:
//   https://jamesmccaffrey.wordpress.com/2015/03/06/inverting-a-matrix-using-c/
//   https://stackoverflow.com/questions/46836908/double-inversion-c-sharp
//
// Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// Kevin Kovalchik and Christopher Hughes do not claim copyright of
// any third-party libraries ditributed with RawTools. All third party
// licenses are provided in accompanying files as outline in the NOTICE.using System;

using System;

namespace RawTools.Utilities.MathStats
{
    public static class MatrixOperations
    {
        public static double[][] MatrixCreate(int rows, int cols)
        {
            var result = new double[rows][];
            for (var i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        private static double[][] MatrixIdentity(int n)
        {
            // return an n x n Identity matrix
            var result = MatrixCreate(n, n);
            for (var i = 0; i < n; ++i)
                result[i][i] = 1.0;

            return result;
        }

        public static double[][] MatrixProduct(double[][] matrixA, double[][] matrixB)
        {
            var aRows = matrixA.Length;
            var aCols = matrixA[0].Length;
            var bRows = matrixB.Length;
            var bCols = matrixB[0].Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");

            var result = MatrixCreate(aRows, bCols);

            for (var i = 0; i < aRows; ++i) // each row of A
            for (var j = 0; j < bCols; ++j) // each col of B
            for (var k = 0; k < aCols; ++k) // could use k less-than bRows
                result[i][j] += matrixA[i][k] * matrixB[k][j];

            return result;
        }

        public static double[][] MatrixInverse(double[][] matrix)
        {
            var n = matrix.Length;
            var result = MatrixDuplicate(matrix);

            int[] perm;
            int toggle;
            var lum = MatrixDecompose(matrix, out perm,
                out toggle);
            if (lum == null)
                throw new Exception("Unable to compute inverse");

            var b = new double[n];
            for (var i = 0; i < n; ++i)
            {
                for (var j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                var x = HelperSolve(lum, b);

                for (var j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }

            return result;
        }

        private static double[][] MatrixDuplicate(double[][] matrix)
        {
            // allocates/creates a duplicate of a matrix.
            var result = MatrixCreate(matrix.Length, matrix[0].Length);
            for (var i = 0; i < matrix.Length; ++i) // copy the values
            for (var j = 0; j < matrix[i].Length; ++j)
                result[i][j] = matrix[i][j];
            return result;
        }

        private static double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            var n = luMatrix.Length;
            var x = new double[n];
            b.CopyTo(x, 0);

            for (var i = 1; i < n; ++i)
            {
                var sum = x[i];
                for (var j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (var i = n - 2; i >= 0; --i)
            {
                var sum = x[i];
                for (var j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }

        private static double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // rerturns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            var rows = matrix.Length;
            var cols = matrix[0].Length; // assume square
            if (rows != cols)
                throw new Exception("Attempt to decompose a non-square m");

            var n = rows; // convenience

            var result = MatrixDuplicate(matrix);

            perm = new int[n]; // set up row permutation result
            for (var i = 0; i < n; ++i) perm[i] = i;

            toggle = 1; // toggle tracks row swaps.
            // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

            for (var j = 0; j < n - 1; ++j) // each column
            {
                var colMax = Math.Abs(result[j][j]); // find largest val in col
                var pRow = j;
                //for (int i = j + 1; i less-than n; ++i)
                //{
                //  if (result[i][j] greater-than colMax)
                //  {
                //    colMax = result[i][j];
                //    pRow = i;
                //  }
                //}

                // reader Matt V needed this:
                for (var i = j + 1; i < n; ++i)
                    if (Math.Abs(result[i][j]) > colMax)
                    {
                        colMax = Math.Abs(result[i][j]);
                        pRow = i;
                    }
                // Not sure if this approach is needed always, or not.

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    var rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;

                    var tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                // --------------------------------------------------
                // This part added later (not in original)
                // and replaces the 'return null' below.
                // if there is a 0 on the diagonal, find a good row
                // from i = j+1 down that doesn't have
                // a 0 in column j, and swap that good row with row j
                // --------------------------------------------------

                if (result[j][j] == 0.0)
                {
                    // find a good row to swap
                    var goodRow = -1;
                    for (var row = j + 1; row < n; ++row)
                        if (result[row][j] != 0.0)
                            goodRow = row;

                    if (goodRow == -1)
                        throw new Exception("Cannot use Doolittle's method");

                    // swap rows so 0.0 no longer on diagonal
                    var rowPtr = result[goodRow];
                    result[goodRow] = result[j];
                    result[j] = rowPtr;

                    var tmp = perm[goodRow]; // and swap perm info
                    perm[goodRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }
                // --------------------------------------------------
                // if diagonal after swap is zero . .
                //if (Math.Abs(result[j][j]) less-than 1.0E-20) 
                //  return null; // consider a throw

                for (var i = j + 1; i < n; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (var k = j + 1; k < n; ++k) result[i][k] -= result[i][j] * result[j][k];
                }
            } // main j column loop

            return result;
        }
    }
}