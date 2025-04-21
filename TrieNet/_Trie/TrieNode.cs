// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Microsoft.Win32;

namespace Gma.DataStructures.StringSearch
{
    [Serializable]
    public class TrieNode<TValue> : TrieNodeBase<TValue>
    {
        private readonly Dictionary<char, TrieNode<TValue>> m_Children;
        private readonly Queue<TValue> m_Values;

        protected TrieNode()
        {
            m_Children = new Dictionary<char, TrieNode<TValue>>();
            m_Values = new Queue<TValue>();
        }

        protected override int KeyLength
        {
            get { return 1; }
        }

        protected override IEnumerable<TrieNodeBase<TValue>> Children()
        {
            return m_Children.Values;
        }

        protected override IEnumerable<TValue> Values()
        {
            return m_Values;
        }

        protected override TrieNodeBase<TValue> GetOrCreateChild( char key )
        {
            TrieNode<TValue> result;
            if (! m_Children.TryGetValue( key, out result ))
            {
                result = new TrieNode<TValue>();
                m_Children.Add( key, result );
            }
            return result;
        }

        protected override TrieNodeBase<TValue> GetChildOrNull( string query, int position )
        {
            if (query == null) throw new ArgumentNullException( "query" );
            TrieNode<TValue> childNode;
            return
                m_Children.TryGetValue( query[position], out childNode )
                    ? childNode
                    : null;
        }

        public void Add( string key, int position, TValue value )
        {
            base.Add( key, position, value );
        }

        protected override void AddValue( TValue value )
        {
            m_Values.Enqueue( value );
        }

        /// <summary>
        /// Returns a list of prefixes, each of which cover at most <see cref="maxPartSize"/> words.  All prefixes
        /// taken together will cover the entire trie. The idea is that a wildcard expression with each prefix will
        /// find every file (word).
        /// </summary>
        /// <param name="maxPartSize"></param>
        /// <param name="prefixSoFar">The prefix accumulated so far</param>
        /// <returns></returns>
        public List<string> Partition( int maxPartSize, string prefixSoFar )
        {
            List<string> retval;
            // Depth-first search for a node with <= maxPartSize.
            // TODO: This algorithm is slightly wrong and must be fixed before it can be trusted.  There are two
            // problems:
            // (1) There may be cases where there are a sufficient number of duplicates words (strings, if we're talking
            //      filenames, which can include spaces) to cause the count of all occurrences of a string to exceed
            //      <see cref="maxPartSize"/>.  In this case, the algorithm fails and no such partition can be found.
            //      Throw an exception.
            // (2) There may be a case where the current node corresponds to <see cref="maxPartSize"/> without
            //      considering children.  In such a case, we need to return the current <see cref="prefixSoFar"/>
            //      <em>without</em> wildcard characters, and consider whether to also return <see cref="prefixSoFar"/>
            //      <em>with</em> a wildcard, or continue the partition algorithm with the children.
            // (3) (There's also a 3rd thing.)  We need a 3rd parameter for the wildcard expression to be appended to
            //      the pattern.  Or some placeholder indicator that we can later replace with a wildcard.
            if (DescendantsCount <= maxPartSize)
            {
                retval = new List<string> { prefixSoFar };
            }
            else
            {
                retval = m_Children
                         .Select( c => c.Value.Partition( maxPartSize, prefixSoFar + c.Key ) )
                         .Aggregate( new List<string>(), ( rv, lst ) =>
                         {
                             rv.AddRange( lst );
                             return rv;
                         } );
            }
            return retval;
        }
    }
}
