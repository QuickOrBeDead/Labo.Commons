﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparerReverser.cs" company="Labo">
//   The MIT License (MIT)
//   
//   Copyright (c) 2013 Bora Akgun
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Defines the ComparerReverser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Comparer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ComparerReverser class
    /// </summary>
    /// <typeparam name="TItem">Item type for comparison</typeparam>
    public sealed class ComparerReverser<TItem> : IComparer<TItem>
    {
        /// <summary>
        /// The wrapped comparer.
        /// </summary>
        private readonly IComparer<TItem> m_WrappedComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerReverser{TItem}" /> class.
        /// </summary>
        /// <param name="wrappedComparer">The wrapped comparer.</param>
        /// <exception cref="System.ArgumentNullException">wrappedComparer</exception>
        public ComparerReverser(IComparer<TItem> wrappedComparer)
        {
            if (wrappedComparer == null)
            {
                throw new ArgumentNullException("wrappedComparer");
            }

            m_WrappedComparer = wrappedComparer;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
        public int Compare(TItem x, TItem y)
        {
            return m_WrappedComparer.Compare(y, x);
        }
    }
}
