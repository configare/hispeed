/*************************************************************************
*                                                                        *
*   ------------------------------ Navl ------------------------------   *
*   File: "Avl.cs"                                                       *
*   threaded Avl-tree in C# to implement an ordered list of items        *
*   which can be accessed by value and by index                          *
*                                                                        *
*************************************************************************/

/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is the `Navl' threaded Avl-tree implementation.
 *
 * The Initial Developer of the Original Code is
 * Richard McGraw.
 * Portions created by the Initial Developer are Copyright (C) 2007
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Telerik.Collections.Generic
{
    #region Avl tree
    public sealed class AvlTree<ValueT> : IEnumerable<ValueT>, IList<ValueT>, IList, IEnumerable, IDisposable
    {
        const int STACK_SIZE = 32;

        #region tree data
        private AvlTreeNode<ValueT> _root;
        private UInt32 _count;
        private int _height;
        private IComparer<ValueT> _comparer;
        private int _version;

        //available stacks:
        private AvlTreeNode<ValueT>[] _nodepath;
        private int[] _comparecache;
        private uint[] _countcache;
        #endregion

        #region stack allocation
        private AvlTreeNode<ValueT>[] NodePath
        {
            get
            {
                if (_nodepath == null)
                    _nodepath = new AvlTreeNode<ValueT>[STACK_SIZE];
                return _nodepath;
            }
        }

        private int[] CompareCache
        {
            get
            {
                if (_comparecache == null)
                    _comparecache = new int[STACK_SIZE];
                return _comparecache;
            }
        }
        #endregion

        public AvlTree() : this(Comparer<ValueT>.Default) { }

        public AvlTree(IComparer<ValueT> comparer)
        {
            _root = new AvlTreeNode<ValueT>();
            _root.mkNil();
            _count = 0;
            _height = 0;
            _comparer = comparer;
            _version = 0;
        }

        public AvlTreeNode<ValueT> Root
        {
            get
            {
                if (!this._root.nil)
                {
                    return this._root.Right;
                }
                return null;
            }
        }

        private void FreeStacks()
        {
            _nodepath = null;
            _comparecache = null;
            _countcache = null;
        }

        #region object contract
        public override string ToString()
        {
            if (_root.nil)
                return "[]";
            return Repr(new StringBuilder()).ToString();
        }
        #endregion

        public void Dispose()
        {
            if (_root != null)
            {
                _root.mkNil(); _root = null;
                _count = 0;
                _height = 0;
                _comparer = null;
                _version = 0;
                FreeStacks();
            }
        }

        public int Size
        {
            get { return (int)_count; }
        }

        public bool IsEmpty
        {
            get { return _root.nil; }
        }

        public void MkEmpty()
        {
            _root.mkNil();
            _count = 0;
            _height = 0;
            _version++;
        }

        #region Find values

        public bool Contains(ValueT value)
        {
            int index = this.IndexOf(value);

            return index >= 0;
        }

        //public bool Find(ref ValueT target)
        //{
        //    if (!_root.nil)
        //    {
        //        AvlTreeNode<ValueT> a = Lookup(_root.right, target, _comparer);
        //        if (a != null)
        //        {
        //            target = a.value;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public ValueT Find(ValueT target)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = Lookup(_root.right, target, _comparer);
                if (a != null)
                {
                    return a.value;
                }
            }

            return default(ValueT);
        }

        public AvlTreeNode<ValueT> FindNode(ValueT target)
        {
            if (!_root.nil)
            {
                return Lookup(this._root.right, target, this._comparer);
            }

            return null;
        }

        public bool FindAtMost(ValueT target, out ValueT value)
        {
            value = default(ValueT);
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                int cmp;
                bool found = false;
                while (true)
                {
                    cmp = _comparer.Compare(target, a.value);
                    if (cmp < 0)
                    {
                        if (isLt(a)) break;
                        a = a.left;
                        continue;
                    }
                    value = a.value;
                    found = true;
                    if (cmp == 0 || isRt(a)) break;
                    a = a.right;
                }
                return found;
            }
            return false;
        }

        public bool FindAtLeast(ValueT target, out ValueT value)
        {
            value = default(ValueT);
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                int cmp;
                bool found = false;
                while (true)
                {
                    cmp = _comparer.Compare(target, a.value);
                    if (cmp > 0)
                    {
                        if (isRt(a)) break;
                        a = a.right;
                        continue;
                    }
                    value = a.value;
                    found = true;
                    if (cmp == 0 || isLt(a)) break;
                    a = a.left;
                }
                return found;
            }
            return false;
        }
        #endregion

        #region Get by index
        //public ValueT this[int index] {
        //    get {
        //        if ((index < 0 && (index += (int)_count) < 0) || index >= _count)
        //            throw new IndexOutOfRangeException();
        //        Node<ValueT> a;
        //        if (index==0)
        //            a = Leftmost(_root.right);
        //        else if (index+1==_count)
        //            a = Rightmost(_root.right);
        //        else
        //            a = Select(_root.right, index+1);
        //        return a.value;
        //    }
        //}

        public ValueT FindByIndex(Int32 index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();
            AvlTreeNode<ValueT> a;
            if (index == 0)
                a = Leftmost(_root.right);
            else if (index + 1 == _count)
                a = Rightmost(_root.right);
            else
                a = Select(_root.right, index + 1);
            return a.value;
        }

        public ValueT First
        {
            get
            {
                if (_root.nil)
                    throw new InvalidOperationException("Avl:empty");
                return Leftmost(_root.right).value;
            }
        }

        public ValueT Last
        {
            get
            {
                if (_root.nil)
                    throw new InvalidOperationException("Avl:empty");
                return Rightmost(_root.right).value;
            }
        }

        private static AvlTreeNode<ValueT> Leftmost(AvlTreeNode<ValueT> a)
        {
            Debug.Assert(a != null);
            while (!isLt(a)) a = a.left;
            return a;
        }

        private static AvlTreeNode<ValueT> Rightmost(AvlTreeNode<ValueT> a)
        {
            Debug.Assert(a != null);
            while (!isRt(a)) a = a.right;
            return a;
        }

        // param k: index in [1,size]
        private static AvlTreeNode<ValueT> Select(AvlTreeNode<ValueT> a, Int32 k)
        {
            UInt32 r = (UInt32)k, s;
            while (r != (s = a.rank >> 4))
            {
                if (r < s)
                    a = a.left;
                else
                {
                    r -= s;
                    a = a.right;
                }
            }
            return a;
        }
        #endregion

        #region Index of
        public int Index(ValueT value)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                UInt32 r = 0, idx = 0, h = _count + 1;
                int cmp;
                while (true)
                {
                    cmp = _comparer.Compare(value, a.value);
                    if (cmp <= 0)
                    {
                        h = r + (a.rank >> 4);
                        if (cmp == 0) idx = h;
                        if (isLt(a)) break;
                        a = a.left;
                    }
                    else
                    {
                        r += (a.rank >> 4);
                        if (isRt(a)) break;
                        a = a.right;
                    }
                }
                if (idx != 0)
                    return (int)(idx - 1);
                return -(int)h;
            }
            return -1;
        }

        public int Index(ValueT value, Int32 lo)
        {
            throw new NotImplementedException();
        }

        public int Index(ValueT value, Int32 lo, Int32 hi)
        {
            throw new NotImplementedException();
        }

        public int LastIndex(ValueT value)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                UInt32 r = 0, idx = 0, h = _count + 1;
                int cmp;
                while (true)
                {
                    cmp = _comparer.Compare(value, a.value);
                    if (cmp >= 0)
                    {
                        r += (a.rank >> 4);
                        if (cmp == 0) idx = r;
                        if (isRt(a)) break;
                        a = a.right;
                    }
                    else
                    {
                        h = r + (a.rank >> 4);
                        if (isLt(a)) break;
                        a = a.left;
                    }
                }
                if (idx != 0)
                    return (int)(idx - 1);
                return -(int)h;
            }
            return -1;
        }

        public void Span(ValueT value, out Int32 loIndex, out Int32 hiIndex)
        {
            UInt32 lo, hi;
            lo = _count + 1;
            hi = 0u;
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                int cmp;
                while (true)
                {
                    cmp = _comparer.Compare(value, a.value);
                    if (cmp == 0) break;
                    if (cmp < 0)
                    {
                        lo = hi + (a.rank >> 4);
                        if (isLt(a)) goto finish;
                        a = a.left;
                    }
                    else
                    {
                        hi += (a.rank >> 4);
                        if (isRt(a)) goto finish;
                        a = a.right;
                    }
                }
                //range not empty:
                AvlTreeNode<ValueT> p = a;
                UInt32 r = hi;
                while (true)
                {
                    if (cmp <= 0)
                    {
                        lo = r + (a.rank >> 4);
                        if (isLt(a)) break;
                        a = a.left;
                    }
                    else
                    {
                        r += (a.rank >> 4);
                        if (isRt(a)) break;
                        a = a.right;
                    }
                    cmp = _comparer.Compare(value, a.value);
                }
                a = p;
                while (true)
                {
                    if (cmp < 0)
                    {
                        if (isLt(a)) break;
                        a = a.left;
                    }
                    else
                    {
                        hi += (a.rank >> 4);
                        if (isRt(a)) break;
                        a = a.right;
                    }
                    cmp = _comparer.Compare(value, a.value);
                }
            }
        finish:
            lo--;
            loIndex = (int)lo; hiIndex = (int)hi;
        }

        public void Span(ValueT loValue, ValueT hiValue, out Int32 loIndex, out Int32 hiIndex)
        {
            UInt32 lo, hi;
            lo = _count + 1;
            hi = 0;
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a;
                UInt32 r = 0;
                int cmp;
                // swap if loValue > hiValue:
                if (_comparer.Compare(loValue, hiValue) > 0)
                {
                    ValueT v = loValue;
                    loValue = hiValue;
                    hiValue = v;
                }
                a = _root.right;
                while (true)
                {
                    cmp = _comparer.Compare(loValue, a.value);
                    if (cmp <= 0)
                    {
                        lo = r + (a.rank >> 4);
                        if (isLt(a)) break;
                        a = a.left;
                    }
                    else
                    {
                        r += (a.rank >> 4);
                        if (isRt(a)) break;
                        a = a.right;
                    }
                }
                a = _root.right;
                while (true)
                {
                    cmp = _comparer.Compare(hiValue, a.value);
                    if (cmp < 0)
                    {
                        if (isLt(a)) break;
                        a = a.left;
                    }
                    else
                    {
                        hi += (a.rank >> 4);
                        if (isRt(a)) break;
                        a = a.right;
                    }
                }
            }
            lo--;
            loIndex = (int)lo; hiIndex = (int)hi;
        }
        #endregion

        public void ForEach(Action<ValueT> act)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                while (true)
                {
                    while (!isLt(a))
                    {
                        a = a.left;
                    }
                    while (true)
                    {
                        act(a.value);
                        if (!isRt(a)) break;
                        if (a.right == _root)
                            return;
                        a = a.right;
                    }
                    a = a.right;
                }
            }
        }

        public void ForEachBackwards(Action<ValueT> act)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                while (true)
                {
                    while (!isRt(a))
                    {
                        a = a.right;
                    }
                    while (true)
                    {
                        act(a.value);
                        if (!isLt(a)) break;
                        if (a.left == _root)
                            return;
                        a = a.left;
                    }
                    a = a.left;
                }
            }
        }

        public void ForEach(Action<ValueT> act, Predicate<ValueT> stopcondition)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                while (true)
                {
                    while (!isLt(a))
                    {
                        a = a.left;
                    }
                    while (!stopcondition(a.value))
                    {
                        act(a.value);
                        if (!isRt(a)) break;
                        if (a.right == _root)
                            return;
                        a = a.right;
                    }
                    a = a.right;
                }
            }
        }

        public void ForEachBackwards(Action<ValueT> act, Predicate<ValueT> stopcondition)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = _root.right;
                while (true)
                {
                    while (!isRt(a))
                    {
                        a = a.right;
                    }
                    while (!stopcondition(a.value))
                    {
                        act(a.value);
                        if (!isLt(a)) break;
                        if (a.left == _root)
                            return;
                        a = a.left;
                    }
                    a = a.left;
                }
            }
        }


        private static AvlTreeNode<ValueT> Lookup(AvlTreeNode<ValueT> a, ValueT v, IComparer<ValueT> c)
        {
            Debug.Assert(a != null);
            Debug.Assert(c != null);
            int cmp;
            do
            {
                cmp = c.Compare(v, a.value);
                if (cmp == 0)
                    return a;
                if (cmp < 0)
                {
                    if (isLt(a)) break;
                    a = a.left;
                }
                else
                {
                    if (isRt(a)) break;
                    a = a.right;
                }
            } while (true);
            return null;
        }

        public string Repr()
        {
            if (_root.nil)
                return "[]";
            return Repr(new StringBuilder()).ToString();
        }

        //0.1r1: use GetForwardEnumerator()
        private StringBuilder Repr(StringBuilder buf)
        {
            Debug.Assert(!IsEmpty);
            IEnumerator<ValueT> e = GetForwardEnumerator();
            uint num = _count;
            buf.Append("[");
            try
            {
                do
                {
                    e.MoveNext();
                    buf.Append(e.Current.ToString());
                    if (0 == --num) break;
                    buf.Append(", ");
                } while (true);
            }
            finally
            {
                e.Dispose();
            }
            buf.Append("]");
            return buf;
        }

        #region Insertions
        // duplicateallowed: no node stack
        //             else: node stack to undo rank++ in case of clash  
        //        overwrite: ignored if duplicateallowed
        public void Insert(ValueT value, bool duplicateallowed, bool overwrite)
        {
            if (_root.nil)
            {
                _root.right = new AvlTreeNode<ValueT>(value);
                _root.right.left = _root.right.right = _root;
                unRt(_root);
                _height = 1;
            }
            else
            {
                AvlTreeNode<ValueT> a, p /*p->a*/, t /*mark*/, u /*u->t*/;
                int[] ad = this.CompareCache;
                int cmp, n = 1 /*ad-size*/;
                p = t = _root;
                a = p.right;
                u = null;
                ad[0] = 1;
                // get to p = leaf:
                if (duplicateallowed)
                {
                    while (true)
                    {
                        cmp = _comparer.Compare(value, a.value);
                        if (skewed(a))
                        {
                            t = a; u = p;
                            n = 0;
                        }
                        p = a;
                        if (cmp <= 0)
                        {
                            ad[n] = 0;
                            p.rank += 0x10;
                            if (isLt(a)) break;
                            a = p.left;
                        }
                        else
                        {
                            ad[n] = 1;
                            if (isRt(a)) break;
                            a = p.right;
                        }
                        ++n;
                    }
                }
                else
                {
                    AvlTreeNode<ValueT>[] ap = this.NodePath;
                    int f = 0;
                    while (true)
                    {
                        cmp = _comparer.Compare(value, a.value);
                        if (cmp == 0)
                        {
                            if (overwrite)
                                a.value = value;
                            else while (f != 0) ap[--f].rank -= 0x10;
                            return;
                        }
                        if (skewed(a))
                        {
                            t = a; u = p;
                            n = 0;
                        }
                        p = a;
                        if (cmp < 0)
                        {
                            ad[n] = 0;
                            p.rank += 0x10;
                            if (isLt(a)) break;
                            a = p.left;
                            ap[f++] = p;
                        }
                        else
                        {
                            ad[n] = 1;
                            if (isRt(a)) break;
                            a = p.right;
                        }
                        ++n;
                    }
                }
                // attach new node to leaf:
                a = new AvlTreeNode<ValueT>(value);
                if (ad[n] == 0)
                {
                    a.left = p.left;
                    unLt(p);
                    p.left = a;
                    a.right = p;
                }
                else
                {
                    a.right = p.right;
                    unRt(p);
                    p.right = a;
                    a.left = p;
                }
                _height += FixInsert(t, u, ad, n);
            }
            _count++;
            _version++;
        }

        /// <summary>
        /// Inserts the with duplicates.
        /// </summary>
        /// <param name="value">The value.</param>
        public void InsertWithDuplicates(ValueT value)
        {
            int index = this.LastIndex(value);
            if (index >= 0)
            {

                this.Insert(index + 1, value);
                return;
            }

            Insert(value, true, false);
        }

        public void InsertUnique(ValueT value, bool overwrite)
        {
            Insert(value, false, overwrite);
        }

        public void InsertUnique(ValueT value)
        {
            Insert(value, false, false);
        }

        public void Insert(Int32 index, ValueT value)
        {
            if (index < 0 || index > _count)
                throw new IndexOutOfRangeException("Avl:Insert");
            AvlTreeNode<ValueT> nu = new AvlTreeNode<ValueT>(value);
            if (index == 0)
            {
                _height += RJoin(nu, AvlTreeNode<ValueT>.NIL, _root, 0, 0, true);
            }
            else if (index == _count)
            {
                _height += LJoin(nu, _root, AvlTreeNode<ValueT>.NIL, 0, _count, true);
            }
            else
            {
                AvlTreeNode<ValueT> a, p /*p->a*/, t /*mark*/, u /*u->t*/;
                int[] ad = this.CompareCache;
                int n = 1 /*ad-size*/;
                uint r, s;
                p = t = _root;
                a = p.right;
                u = null;
                ad[0] = 1;
                //locate node in front of (index-1):
                r = (uint)index;
                while (true)
                {
                    if (skewed(a))
                    {
                        t = a; u = p;
                        n = 0;
                    }
                    if (r == (s = a.rank >> 4))
                        break;
                    p = a;
                    if (r < s)
                    {
                        p.rank += 0x10;
                        a = p.left;
                        ad[n++] = 0;
                    }
                    else
                    {
                        r -= s;
                        a = p.right;
                        ad[n++] = 1;
                    }
                }
                //insert new node as successor of node p:
                nu = new AvlTreeNode<ValueT>(value);
                ad[n] = 1;
                if (isRt(a))
                {
                    nu.right = a.right;
                    unRt(a);
                    a.right = nu;
                    nu.left = a;
                }
                else
                {
                    p = a; a = p.right;
                    n++;
                    while (true)
                    {
                        if (skewed(a))
                        {
                            t = a; u = p;
                            n = 0;
                        }
                        a.rank += 0x10;
                        if (isLt(a)) break;
                        p = a;
                        a = p.left;
                        ad[n++] = 0;
                    }
                    ad[n] = 0;
                    nu.left = a.left;
                    unLt(a);
                    a.left = nu;
                    nu.right = a;
                }
                _height += FixInsert(t, u, ad, n);
            }
            _count++;
            _version++;
        }

        //``cons''
        public void InsertFirst(ValueT value)
        {
            AvlTreeNode<ValueT> nu = new AvlTreeNode<ValueT>(value);
            _height += RJoin(nu, AvlTreeNode<ValueT>.NIL, _root, 0, 0, true);
            _count++;
            _version++;
        }

        //``append''
        public void InsertLast(ValueT value)
        {
            AvlTreeNode<ValueT> nu = new AvlTreeNode<ValueT>(value);
            _height += LJoin(nu, _root, AvlTreeNode<ValueT>.NIL, 0, _count, true);
            _count++;
            _version++;
        }

        //t: mark, u: parent of t
        //ad[0,n): path from t to p=parent(nu) with ad[n] for link p->nu
        //returns: 1 if height increase propagates to the top, 0 otherwise
        private static int FixInsert(AvlTreeNode<ValueT> t, AvlTreeNode<ValueT> u, int[] ad, int n)
        {
            AvlTreeNode<ValueT> a;
            //skew zeros:
            if (n != 0)
            {
                int i = 1, e;
                a = t;
                e = ad[0];
                do
                {
                    a = (e == 0 ? a.left : a.right);
                    if (i == n) break;
                    e = ad[i++];
                    a.rank |= 1u << e;
                } while (true);
                a.rank |= 1u << ad[n];
            }
            //fix skew mark:
            if (u != null)
            {
                //bal(t) != 0
                if (0 == (t.rank >> ad[0] & 1u))
                {
                    //\pm 1 --> 0
                    t.rank &= ~3u;
                }
                else
                {
                    if (ad[0] == 0)
                    {
                        t.rank &= ~1u;
                        if (lSkewed(t.left))
                        {
                            a = rotR(t);
                        }
                        else
                        {
                            a = rotLR(t);
                            a.left.rank &= ~2u;
                            switch (a.rank & 3u)
                            {
                                case 1: t.rank |= 2u; break;
                                case 2: a.left.rank |= 1u; break;
                            }
                        }
                    }
                    else
                    {
                        t.rank &= ~2u;
                        if (rSkewed(t.right))
                        {
                            a = rotL(t);
                        }
                        else
                        {
                            a = rotRL(t);
                            a.right.rank &= ~1u;
                            switch (a.rank & 3u)
                            {
                                case 1: a.right.rank |= 2u; break;
                                case 2: t.rank |= 1u; break;
                            }
                        }
                    }
                    a.rank &= ~3u;
                    if (t == u.left) u.left = a; else u.right = a;
                }
                return 0;
            }
            return 1;
        }
        #endregion

        #region Replacement
        public ValueT Replace(Int32 index, ValueT newValue)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();
            AvlTreeNode<ValueT> a;
            if (index == 0)
                a = Leftmost(_root.right);
            else if (index + 1 == _count)
                a = Rightmost(_root.right);
            else
                a = Select(_root.right, index + 1);
            ValueT oldValue = a.value;
            a.value = newValue;
            return oldValue;
        }

        public ValueT ReplaceFirst(ValueT newValue)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = Leftmost(_root.right);
                ValueT value = a.value;
                a.value = newValue;
                return value;
            }
            return default(ValueT);
        }

        public ValueT ReplaceLast(ValueT newValue)
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = Rightmost(_root.right);
                ValueT value = a.value;
                a.value = newValue;
                return value;
            }
            return default(ValueT);
        }
        #endregion

        #region Concatenation
        public AvlTree<ValueT> Append(AvlTree<ValueT> that)
        {
            if (that.IsEmpty)
            {
                return this;
            }

            if (this.IsEmpty)
            {
                this._root = that._root;
                this._height = that._height;
                that._root = new AvlTreeNode<ValueT>();
            }
            else
            {
                int delta = that._height - this._height;
                AvlTreeNode<ValueT> mid;
                if (delta <= 0)
                {
                    delta -= RemoveLeftmost(that._root, out mid, false, NodePath);
                    this._height += LJoin(mid, this._root, that._root, delta, _count,   /*attach=*/true);
                }
                else
                {
                    delta += RemoveRightmost(this._root, out mid, false, NodePath);
                    //count minus one
                    this._height = RJoin(mid, this._root, that._root, delta, _count - 1, /*attach=*/true)
                        + that._height;
                    this._root.right = that._root.right;
                }
                Rightmost(this._root.right).right = this._root;
            }
            _count += that._count;
            that.MkEmpty();
            _version++;
            return this;
        }

        // nR = size (tree rooted at R.right)
        // param attach = link rightmost of mid.left
        // R.right = R.right + S.right
        // _count updated: no
        // returns: delta height (at R)
        private static int LJoin(AvlTreeNode<ValueT> mid, AvlTreeNode<ValueT> R, AvlTreeNode<ValueT> S, int delta, UInt32 nR, bool attach)
        {
            Debug.Assert(delta <= 0 || S.nil);
            AvlTreeNode<ValueT> a, p, t, u;
            p = t = R;
            a = p.right;
            u = null;
            if (S.nil)
            {
                while (!isRt(p))
                {
                    if (skewed(a))
                    {
                        t = a; u = p;
                    }
                    p = a;
                    a = p.right;
                    nR -= GetRank(p);
                }
                if (attach) mid.right = R; //XXX:max(p).right
                Rt(mid);
            }
            else
            {
                while (delta < -1)
                {
                    if (skewed(a))
                    {
                        t = a; u = p;
                    }
                    p = a;
                    a = p.right;
                    nR -= GetRank(p);
                    delta += 1 + (int)(p.rank & 1);
                }
                mid.right = S.right;
                unRt(mid);
                //::prev(min RHS) = mid
            }
            if (isRt(p))
            {
                Debug.Assert(nR == 0);
                mid.left = p;
                Lt(mid);
                unRt(p);
            }
            else
            {
                mid.left = a;
                unLt(mid);
                //::next(max a) = mid (explicit)
                if (attach) Rightmost(a).right = mid;
            }
            //set rank(mid):
            Rank(mid, nR + 1);
            // skew:
            mid.rank = (mid.rank & ~3u) | (uint)(-delta);
            p.right = mid;
            a = t.right;
            while (a != mid)
            {
                a.rank |= 2u;
                a = a.right;
            }
            if (u != null)
            {
                //bal(t) != 0
                if (0 == (t.rank & 2u))
                {
                    t.rank &= ~3u;
                }
                else
                {
                    t.rank &= ~2u;
                    if (rSkewed(t.right))
                    {
                        a = rotL(t);
                    }
                    else
                    {
                        a = rotRL(t);
                        a.right.rank &= ~1u;
                        switch (a.rank & 3u)
                        {
                            case 1: a.right.rank |= 2u; break;
                            case 2: t.rank |= 1u; break;
                        }
                    }
                    a.rank &= ~3u;
                    u.right = a;
                }
                return 0;
            }
            return 1;
        }

        // param nR = size (tree rooted at R.right)
        // param attach = link leftmost of mid.right
        // S.right = R.right + S.right
        // _count updated: no
        // returns: delta height (at S)
        private static int RJoin(AvlTreeNode<ValueT> mid, AvlTreeNode<ValueT> R, AvlTreeNode<ValueT> S, int delta, UInt32 nR, bool attach)
        {
            Debug.Assert(delta >= 0 || R.nil);
            AvlTreeNode<ValueT> a, p, t, u;
            //temporary change:
            S.left = S.right;
            if (!S.nil) unLt(S);
            //--go
            p = t = S;
            a = p.right;
            u = null;
            nR++;
            if (R.nil)
            {
                while (!isLt(p))
                {
                    if (skewed(a))
                    {
                        t = a; u = p;
                    }
                    p = a;
                    p.rank += nR << 4;
                    a = p.left;
                }
                Debug.Assert(nR == 1);
                if (attach) mid.left = S; //XXX:min(p).left
                Lt(mid);
            }
            else
            {
                while (delta > 1)
                {
                    if (skewed(a))
                    {
                        t = a; u = p;
                    }
                    p = a;
                    p.rank += nR << 4;
                    a = p.left;
                    delta -= 1 + (int)(p.rank >> 1 & 1);
                }
                mid.left = R.right;
                unLt(mid);
                //::next(max LHS) = mid
            }
            if (isLt(p))
            {
                mid.right = p;
                Rt(mid);
                unLt(p);
            }
            else
            {
                mid.right = a;
                unRt(mid);
                //::prev(min a) = mid (explicit)
                if (attach) Leftmost(a).left = mid;
            }
            //set rank(mid):
            Rank(mid, nR);
            //skew:
            mid.rank = (mid.rank & ~3u) | ((uint)delta << 1);
            p.left = mid;
            a = t.left;
            while (a != mid)
            {
                a.rank |= 1u;
                a = a.left;
            }
            int augment = 1;
            if (u != null)
            {
                // bal(t)!=0
                if (0 == (t.rank & 1u))
                {
                    t.rank &= ~3u;
                }
                else
                {
                    t.rank &= ~1u;
                    if (lSkewed(t.left))
                    {
                        a = rotR(t);
                    }
                    else
                    {
                        a = rotLR(t);
                        a.left.rank &= ~2u;
                        switch (a.rank & 3u)
                        {
                            case 1: t.rank |= 2u; break;
                            case 2: a.left.rank |= 1u; break;
                        }
                    }
                    a.rank &= ~3u;
                    u.left = a;
                }
                augment = 0;
            }
            //--finish and undo change:
            S.right = S.left;
            S.left = null;
            Lt(S); unRt(S);
            return augment;
        }
        #endregion

        #region Split
        // converse of Join
        public ValueT SplitAt(Int32 index, out AvlTree<ValueT> avl0, out AvlTree<ValueT> avl1)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException("Avl:Split");
            AvlTreeNode<ValueT> a;
            AvlTreeNode<ValueT>[] ap = this.NodePath;
            int[] ad = this.CompareCache;
            uint[] an = this.CountCache;      //???:stackalloc
            uint r, s, n/*size of tree(a)*/;
            uint k = 0;
            int h;
            //locate split node:
            r = (uint)index + 1;
            a = _root.right;
            n = _count;
            h = _height;
            while (r != (s = a.rank >> 4))
            {
                ap[k] = a;
                an[k] = n;
                if (r < s)
                {
                    ad[k] = 0;
                    n = s - 1;
                    a = a.left;
                    h -= 1 + (int)(a.rank >> 1 & 1);
                }
                else
                {
                    r -= s;
                    n -= s;
                    ad[k] = 1;
                    a = a.right;
                    h -= 1 + (int)(a.rank & 1);
                }
                ++k;
            }
            //do split:
            ValueT value = a.value;
            avl0 = new AvlTree<ValueT>(_comparer);
            avl1 = new AvlTree<ValueT>(_comparer);
            DoSplit(avl0, avl1, a, n, h, ap, ad, an, k);
            a.clear();
            MkEmpty();
            return value;
        }

        public ValueT Split(ValueT value, out AvlTree<ValueT> avl0, out AvlTree<ValueT> avl1)
        {
            throw new NotImplementedException();
        }

        //s: splitnode (removed)
        //ns:  size of tree rooted at s
        //hs:  height of tree rooted at s
        //k: path length (k=0 if s is root)
        //(ap,ad): path to splitnode s with an[i] = size of tree rooted at ap[i]
        //clears splitnode: no
        //(R0,R1): handle result
        //computes tree sizes
        private static void DoSplit(AvlTree<ValueT> avl0, AvlTree<ValueT> avl1, AvlTreeNode<ValueT> s, uint ns, int hs,
                                    AvlTreeNode<ValueT>[] ap, int[] ad, uint[] an, uint k)
        {
            AvlTreeNode<ValueT> R0, R1;
            uint n0, n1, rk;
            R0 = avl0._root;
            R1 = avl1._root;
            if (!isLt(s))
            {
                R0.start(s.left);
            }
            if (!isRt(s))
            {
                R1.start(s.right);
            }
            n0 = GetRank(s) - 1;
            n1 = ns - GetRank(s);
            //enough if k=0
            AvlTreeNode<ValueT> S0, S1;
            AvlTreeNode<ValueT> mid;
            int h0, h1, j0, j1, dd, de;
            h0 = hs - 1 - (int)(s.rank >> 1 & 1);  //h(t0)
            h1 = hs - 1 - (int)(s.rank & 1);     //h(t1)
            //joins:
            S0 = new AvlTreeNode<ValueT>();
            S1 = new AvlTreeNode<ValueT>();
            while (k != 0)
            {
                mid = ap[--k];
                dd = GetDelta(mid);
                rk = GetRank(mid) - 1;
                if (ad[k] == 0)
                {
                    if (isRt(mid)) S1.mkNil(); else S1.start(mid.right);
                    //< t1 = join (mid,t1,r(mid),delta) >
                    j0 = h1;
                    j1 = hs + dd;
                    de = j1 - j0;
                    if (de <= 0)
                    {
                        // R1=R1+S1
                        h1 += LJoin(mid, R1, S1, de, n1, false);
                    }
                    else
                    {
                        // S1=R1+S1
                        h1 = j1 + RJoin(mid, R1, S1, de, n1, false);
                        R1.start(S1.right);
                    }
                    //size rarg = size(mid) - rk(mid)
                    n1 += an[k] - rk;
                    //update hs:
                    hs += (dd <= 0 ? 1 : 2);
                }
                else
                {
                    if (isLt(mid)) S0.mkNil(); else S0.start(mid.left);
                    //< t0 = join (mid,l(mid),t0,delta) >
                    j0 = hs - dd;
                    j1 = h0;
                    de = j1 - j0;
                    if (de <= 0)
                    {
                        // S0=S0+R0
                        h0 = j0 + LJoin(mid, S0, R0, de, rk, false);
                        R0.start(S0.right);
                    }
                    else
                    {
                        // R0=S0+R0
                        h0 += RJoin(mid, S0, R0, de, rk, false);
                    }
                    n0 += rk + 1;
                    //update hs:
                    hs += (dd >= 0 ? 1 : 2);
                }
            }
            if (!R0.nil)
            {
                Leftmost(R0.right).left = R0;
                Rightmost(R0.right).right = R0;
            }
            if (!R1.nil)
            {
                Leftmost(R1.right).left = R1;
                Rightmost(R1.right).right = R1;
            }
            avl0._count = n0; avl0._height = h0;
            avl1._count = n1; avl1._height = h1;
            S0.right = S1.right = null;
        }

#if AVL_DOC
        private static void DoSplit(/*...sLo,sHi,...*/) { }
#endif

        private uint[] CountCache
        {
            get
            {
                if (_countcache == null)
                    _countcache = new uint[STACK_SIZE];
                return _countcache;
            }
        }
        #endregion

        #region Trim
        public ValueT LTrim(Int32 index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException("Avl:LTrim");
            ValueT value;
            if (index + 1 == _count)
            {
                value = Rightmost(_root.right).value;
                MkEmpty();
                return value;
            }
            AvlTreeNode<ValueT> a;
            AvlTreeNode<ValueT>[] ap = this.NodePath;
            int[] ad = this.CompareCache;
            uint[] an = this.CountCache;      //???:stackalloc
            uint r, s, n/*size of tree(a)*/;
            uint k = 0;
            int h;
            //locate split node:
            r = (uint)index + 1;
            a = _root.right;
            n = _count;
            h = _height;
            while (r != (s = a.rank >> 4))
            {
                ap[k] = a;
                an[k] = n;
                if (r < s)
                {
                    ad[k] = 0;
                    n = s - 1;
                    a = a.left;
                    h -= 1 + (int)(a.rank >> 1 & 1);
                }
                else
                {
                    r -= s;
                    n -= s;
                    ad[k] = 1;
                    a = a.right;
                    h -= 1 + (int)(a.rank & 1);
                }
                ++k;
            }
            //do trim:
            value = a.value;
            _height = DoLTrim(_root, a, n, h, ap, ad, an, k);
            a.clear();
            _count -= (uint)index + 1;
            _version++;
            return value;
        }

        public ValueT RTrim(Int32 index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException("Avl:RTrim");
            ValueT value;
            if (index == 0)
            {
                value = Leftmost(_root.right).value;
                MkEmpty();
                return value;
            }
            AvlTreeNode<ValueT> a;
            AvlTreeNode<ValueT>[] ap = this.NodePath;
            int[] ad = this.CompareCache;
            uint[] an = this.CountCache;      //???:stackalloc
            uint r, s, n/*size of tree(a)*/;
            uint k = 0;
            int h;
            //locate split node:
            r = (uint)index + 1;
            a = _root.right;
            n = _count;
            h = _height;
            while (r != (s = a.rank >> 4))
            {
                ap[k] = a;
                an[k] = n;
                if (r < s)
                {
                    ad[k] = 0;
                    n = s - 1;
                    a = a.left;
                    h -= 1 + (int)(a.rank >> 1 & 1);
                }
                else
                {
                    r -= s;
                    n -= s;
                    ad[k] = 1;
                    a = a.right;
                    h -= 1 + (int)(a.rank & 1);
                }
                ++k;
            }
            //do trim:
            value = a.value;
            _height = DoRTrim(_root, a, n, h, ap, ad, an, k);
            a.clear();
            _count = (uint)index;
            _version++;
            return value;
        }

        //DoLTrim: keep right part
        //s: splitnode (removed)
        //ns: size of tree rooted at s
        //hs: height of tree rooted at s
        //k: path length (k=0 if s is root)
        //(ap,ad): path to splitnode s with an[i] = size of tree rooted at ap[i]
        //clears splitnode: no
        //R1: handle result
        //h1: height of tree being built
        //computes tree sizes: no
        private static int DoLTrim(AvlTreeNode<ValueT> R1, AvlTreeNode<ValueT> s, uint ns, int hs,
                                   AvlTreeNode<ValueT>[] ap, int[] ad, uint[] an, uint k)
        {
            R1.mkNil();
            if (!isRt(s))
            {
                R1.start(s.right);
            }
            uint n1;
            n1 = ns - GetRank(s);
            //(enough if k=0)
            AvlTreeNode<ValueT> S1;
            AvlTreeNode<ValueT> mid;
            uint rk;
            int h1, j0, j1, dd, de;
            h1 = hs - 1 - (int)(s.rank & 1);     //h(t1)
            //joins:
            S1 = new AvlTreeNode<ValueT>();
            while (k != 0)
            {
                mid = ap[--k];
                dd = GetDelta(mid);
                rk = GetRank(mid) - 1;
                if (ad[k] == 0)
                {
                    if (isRt(mid)) S1.mkNil(); else S1.start(mid.right);
                    //< t1 = join (mid,t1,r(mid),delta) >
                    j0 = h1;
                    j1 = hs + dd;
                    de = j1 - j0;
                    if (de <= 0)
                    {
                        // R1=R1+S1
                        h1 += LJoin(mid, R1, S1, de, n1, false);
                    }
                    else
                    {
                        // S1=R1+S1
                        h1 = j1 + RJoin(mid, R1, S1, de, n1, false);
                        R1.start(S1.right);
                    }
                    //size rarg = size(mid) - rk(mid)
                    n1 += an[k] - rk;
                    //update hs:
                    hs += (dd <= 0 ? 1 : 2);
                }
                else
                {
                    //unlink right:
                    mid.clear();
                    //update hs:
                    hs += (dd >= 0 ? 1 : 2);
                }
            }
            if (!R1.nil)
            {
                Leftmost(R1.right).left = R1;
                Rightmost(R1.right).right = R1;
            }
            S1.right = null;
            return h1;
        }

        //DoRTrim: keep left part
        //s: splitnode (removed)
        //ns:  size of tree rooted at s
        //hs: height of tree rooted at s
        //k: path length (k=0 if s is root)
        //(ap,ad): path to splitnode s with an[i] = size of tree rooted at ap[i]
        //clears splitnode: no
        //R0: handle result
        //h0: height of tree being built
        //computes tree sizes: no
        private static int DoRTrim(AvlTreeNode<ValueT> R0, AvlTreeNode<ValueT> s, uint ns, int hs,
                                   AvlTreeNode<ValueT>[] ap, int[] ad, uint[] an, uint k)
        {
            R0.mkNil();
            if (!isLt(s))
            {
                R0.start(s.left);
            }
            uint n0;
            n0 = GetRank(s) - 1;
            //(enough if k=0)
            AvlTreeNode<ValueT> S0;
            AvlTreeNode<ValueT> mid;
            uint rk;
            int h0, j0, j1, dd, de;
            h0 = hs - 1 - (int)(s.rank >> 1 & 1);  //h(t0)
            //joins:
            S0 = new AvlTreeNode<ValueT>();
            while (k != 0)
            {
                mid = ap[--k];
                dd = GetDelta(mid);
                rk = GetRank(mid) - 1;
                if (ad[k] != 0)
                {
                    if (isLt(mid)) S0.mkNil(); else S0.start(mid.left);
                    //< t0 = join (mid,l(mid),t0,delta) >
                    j0 = hs - dd;
                    j1 = h0;
                    de = j1 - j0;
                    if (de <= 0)
                    {
                        // S0=S0+R0
                        h0 = j0 + LJoin(mid, S0, R0, de, rk, false);
                        R0.start(S0.right);
                    }
                    else
                    {
                        // R0=S0+R0
                        h0 += RJoin(mid, S0, R0, de, rk, false);
                    }
                    n0 += rk + 1;
                    //update hs:
                    hs += (dd >= 0 ? 1 : 2);
                }
                else
                {
                    //unlink left:
                    mid.clear();
                    //update hs:
                    hs += (dd <= 0 ? 1 : 2);
                }
            }
            if (!R0.nil)
            {
                Leftmost(R0.right).left = R0;
                Rightmost(R0.right).right = R0;
            }
            S0.right = null;
            return h0;
        }
        #endregion

        #region Deletions
        public bool Delete(ValueT value, out ValueT delValue)
        {
            delValue = default(ValueT);
            if (_root.nil)
                return false;
            AvlTreeNode<ValueT> a, p /*p->a*/;
            AvlTreeNode<ValueT>[] ap = this.NodePath;
            int[] ad = this.CompareCache;
            int n = 0 /*stack-size*/, cmp;
            p = _root;
            a = p.right;
            //search node to delete:
            while (true)
            {
                cmp = _comparer.Compare(value, a.value);
                if (cmp == 0)
                    break;
                p = a;
                if (cmp < 0)
                {
                    if (isLt(a))
                        return false;
                    a = p.left;
                    ad[n] = 0;
                }
                else
                {
                    if (isRt(a))
                        return false;
                    a = p.right;
                    ad[n] = 1;
                }
                ap[n++] = p;
            }
            //backup value:
            delValue = a.value;
            _height -= FixDelete(_root, a, p, ap, ad, n);
            _count--;
            _version++;
            return true;
        }

        //``uncons''
        public void DeleteFirst(out ValueT delValue)
        {
            delValue = default(ValueT);
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> delNode;
                _height -= RemoveLeftmost(_root, out delNode, true, NodePath);
                delValue = delNode.value;
                delNode.clear();
                _count--;
                _version++;
            }
        }

        public void DeleteLast(out ValueT delValue)
        {
            delValue = default(ValueT);
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> delNode;
                _height -= RemoveRightmost(_root, out delNode, true, NodePath);
                delValue = delNode.value;
                delNode.clear();
                _count--;
                _version++;
            }
        }

        public void DeleteLast()
        {
            ValueT delValue = default(ValueT);
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> delNode;
                _height -= RemoveRightmost(_root, out delNode, true, NodePath);
                delValue = delNode.value;
                delNode.clear();
                _count--;
                _version++;
            }
        }

        //``del this[index]''
        public void DeleteAt(Int32 index, out ValueT delValue)
        {
            delValue = default(ValueT);
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();
            AvlTreeNode<ValueT> delNode;
            if (index == 0)
            {
                _height -= RemoveLeftmost(_root, out delNode, true, NodePath);
            }
            else if (index + 1 == _count)
            {
                _height -= RemoveRightmost(_root, out delNode, true, NodePath);
            }
            else
            {
                AvlTreeNode<ValueT> a, p;
                AvlTreeNode<ValueT>[] ap = this.NodePath;
                int[] ad = this.CompareCache;
                uint r, s;
                int n = 0;
                //get to item at index:
                p = _root;
                a = p.right;
                r = (uint)index + 1;
                while (r != (s = a.rank >> 4))
                {
                    ap[n] = p = a;
                    if (r < s)
                    {
                        a = p.left;
                        ad[n++] = 0;
                    }
                    else
                    {
                        r -= s;
                        a = p.right;
                        ad[n++] = 1;
                    }
                }
                delNode = a;
                _height -= FixDelete(_root, a, p, ap, ad, n);
            }
            //backup value:
            delValue = delNode.value;
            delNode.clear();
            _count--;
            _version++;
        }

        //remove leftmost node from R.right
        //clears node to delete: no
        //return (height decr ? 1 : 0)
        private static int RemoveLeftmost(AvlTreeNode<ValueT> R, out AvlTreeNode<ValueT> leftmost, bool detach, AvlTreeNode<ValueT>[] ap)
        {
            AvlTreeNode<ValueT> a, p /*p->a*/, t;
            int n = 0;
            p = R;
            a = p.right;
            while (!isLt(a))
            {
                ap[n++] = p = a;
                p.rank -= 0x10;
                a = p.left;
            }
            if (isRt(a))
            {
                if (n == 0)
                {
                    R.mkNil(); //delUnique
                }
                else
                {
                    Lt(p);
                    if (detach) p.left = a.left;
                }
            }
            else
            {
                if (detach) Leftmost(a.right).left = a.left;
                if (n == 0) R.right = a.right; else p.left = a.right;
            }
            leftmost = a;
            a.left = a.right = null;
            uint bal;
            while (n != 0)
            {
                a = ap[--n];
                bal = a.rank & 3u;
                if ((bal & 2u) == 0)
                {
                    bal = (bal ^ 1) << 1;
                    a.rank = (a.rank & ~3u) | bal;
                }
                else
                {
                    if ((a.right.rank & 1u) == 0)
                    {
                        t = rotL(a);
                        bal = (t.rank & 3u);
                        a.rank = (a.rank & ~3u) | (bal ^ 2);
                        t.rank = (t.rank & ~3u) | (bal ^ 2) >> 1;
                    }
                    else
                    {
                        t = rotRL(a);
                        bal = (t.rank & 3u);
                        t.rank &= ~3u;
                        a.rank = (a.rank & ~3u) | (bal >> 1 & 1);
                        t.right.rank = (t.right.rank & ~3u) | (bal & 1) << 1;
                    }
                    if (n == 0) R.right = t; else ap[n - 1].left = t;
                    bal = (t.rank & 3u);
                }
                if (bal != 0)
                    return 0;
            }
            return 1;
        }

        //remove righmost node from S.right
        //clears node to delete: no
        //return (height decr ? 1 : 0)
        private static int RemoveRightmost(AvlTreeNode<ValueT> S, out AvlTreeNode<ValueT> rightmost, bool detach, AvlTreeNode<ValueT>[] ap)
        {
            AvlTreeNode<ValueT> a, p /*p->a*/, t;
            int n = 0;
            p = S;
            a = p.right;
            while (!isRt(a))
            {
                ap[n++] = p = a;
                a = p.right;
            }
            if (isLt(a))
            {
                if (n == 0)
                {
                    S.mkNil(); //delUnique
                }
                else
                {
                    Rt(p);
                    if (detach) p.right = a.right;
                }
            }
            else
            {
                if (detach) Rightmost(a.left).right = a.right;
                if (n == 0) S.right = a.left; else p.right = a.left;
            }
            rightmost = a;
            a.left = a.right = null;
            uint bal;
            while (n != 0)
            {
                a = ap[--n];
                bal = a.rank & 3u;
                if ((bal & 1u) == 0)
                {
                    bal = (bal ^ 2) >> 1;
                    a.rank = (a.rank & ~3u) | bal;
                }
                else
                {
                    if ((a.left.rank & 2u) == 0)
                    {
                        t = rotR(a);
                        bal = (t.rank & 3u);
                        a.rank = (a.rank & ~3u) | (bal ^ 1);
                        t.rank = (t.rank & ~3u) | (bal ^ 1) << 1;
                    }
                    else
                    {
                        t = rotLR(a);
                        bal = (t.rank & 3u);
                        t.rank &= ~3u;
                        a.rank = (a.rank & ~3u) | (bal & 1) << 1;
                        t.left.rank = (t.left.rank & ~3u) | (bal >> 1 & 1);
                    }
                    if (n == 0) S.right = t; else ap[n - 1].right = t;
                    bal = t.rank & 3u;
                }
                if (bal != 0)
                    return 0;
            }
            return 1;
        }

        //a: node to delete
        //p: its parent
        //ap,ad: path to node a (n nodes not including a)
        //clears node to delete: no
        //return (height decr ? 1 : 0)
        private static int FixDelete(AvlTreeNode<ValueT> R, AvlTreeNode<ValueT> a, AvlTreeNode<ValueT> p, AvlTreeNode<ValueT>[] ap, int[] ad, int n)
        {
            AvlTreeNode<ValueT> t;
            switch (a.rank >> 2 & 3u)
            {
                case 3:
                    if (p.right == a)
                    {
                        p.right = a.right;
                        Rt(p);
                    }
                    else
                    {
                        p.left = a.left;
                        Lt(p);
                    }
                    if (n == 0) p.mkNil();//delUnique
                    break;
                case 1:
                    t = a.right;
                    //::prev(min a.r)=prev(a)
                    Leftmost(t).left = a.left;
                    if (p.left == a) p.left = t; else p.right = t;
                    break;
                case 2:
                    t = a.left;
                    //::next(max a.l)=next(a)
                    Rightmost(t).right = a.right;
                    if (p.left == a) p.left = t; else p.right = t;
                    break;
                default:
                    t = a.right;
                    if (isLt(t))
                    {
                        ap[n] = t;
                        ad[n++] = 1;
                    }
                    else
                    {
                        int k = n++;
                        AvlTreeNode<ValueT> u;
                        do
                        {
                            u = t; t = u.left;
                            ap[n] = u;
                            ad[n++] = 0;
                        } while (!isLt(t));
                        if (isRt(t))
                        {
                            Lt(u); unRt(t);
                        }
                        else
                        {
                            u.left = t.right;
                        }
                        t.right = a.right;
                        ap[k] = t;
                        ad[k] = 1;
                    }
                    unLt(t);
                    t.left = a.left;
                    t.rank = (a.rank & ~0xCu) | (t.rank & 0xCu);
                    Rightmost(t.left).right = t;
                    if (p.left == a) p.left = t; else p.right = t;
                    break;
            }
            uint bal;
            while (n != 0)
            {
                a = ap[--n];
                bal = a.rank & 3u;
                if (ad[n] == 0)
                {
                    a.rank -= 0x10;
                    if ((bal & 2u) == 0)
                    {
                        a.rank = (a.rank & ~3u) | (bal ^ 1) << 1;
                    }
                    else
                    {
                        if ((a.right.rank & 1u) == 0)
                        {
                            t = rotL(a);
                            bal = (t.rank & 3u);
                            a.rank = (a.rank & ~3u) | (bal ^ 2);
                            t.rank = (t.rank & ~3u) | (bal ^ 2) >> 1;
                        }
                        else
                        {
                            t = rotRL(a);
                            bal = (t.rank & 3u);
                            t.rank &= ~3u;
                            a.rank = (a.rank & ~3u) | (bal >> 1 & 1);
                            t.right.rank = (t.right.rank & ~3u) | (bal & 1) << 1;
                        }
                        if (n == 0) R.right = t;
                        else if (ad[n - 1] == 0) ap[n - 1].left = t; else ap[n - 1].right = t;
                        a = t;
                    }
                }
                else
                {
                    if ((bal & 1u) == 0)
                    {
                        a.rank = (a.rank & ~3u) | (bal ^ 2) >> 1;
                    }
                    else
                    {
                        if ((a.left.rank & 2u) == 0)
                        {
                            t = rotR(a);
                            bal = (t.rank & 3u);
                            a.rank = (a.rank & ~3u) | (bal ^ 1);
                            t.rank = (t.rank & ~3u) | (bal ^ 1) << 1;
                        }
                        else
                        {
                            t = rotLR(a);
                            bal = (t.rank & 3u);
                            t.rank &= ~3u;
                            a.rank = (a.rank & ~3u) | (bal & 1) << 1;
                            t.left.rank = (t.left.rank & ~3u) | (bal >> 1 & 1);
                        }
                        if (n == 0) R.right = t;
                        else if (ad[n - 1] == 0) ap[n - 1].left = t; else ap[n - 1].right = t;
                        a = t;
                    }
                }
                if (skewed(a))
                {
                    //finish decr ranks:
                    while (n != 0)
                    {
                        a = ap[--n];
                        a.rank -= (uint)(1 ^ ad[n]) << 4;
                    }
                    return 0;
                }
            }
            return 1;
        }

        //``del this[lo,hi)''
        public void DeleteRange(int lo, int hi)
        {
            throw new NotImplementedException("TrimTrim");
        }
        #endregion

        #region Copy and Slice
        public AvlTree<ValueT> this[int lo, int hi]
        {
            get
            {
                int count = (int)_count;
                if (lo < 0)
                {
                    if ((lo += count) < 0) lo = 0;
                }
                else if (lo > count)
                    lo = count;
                if (hi < 0)
                {
                    if ((hi += count) < 0) hi = 0;
                }
                else if (hi > count)
                    hi = count;
                return (hi - lo == _count) ? Copy() : SliceB(lo, hi);
            }
        }

        // ``dup''
        public AvlTree<ValueT> Copy()
        {
            AvlTree<ValueT> dup = new AvlTree<ValueT>(_comparer);
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a, b, p;
                a = _root.right;
                b = new AvlTreeNode<ValueT>(a.value);
                b.rank = a.rank;
                b.left = b.right = dup._root;
                dup._root.right = b;
                while (true)
                {
                    while (!isLt(a))
                    {
                        a = a.left;
                        p = b;
                        b = new AvlTreeNode<ValueT>(a.value);
                        b.left = p.left;
                        p.left = b;
                        b.rank = a.rank;
                        b.right = p;
                    }
                    while (true)
                    {
                        if (!isRt(a))
                            break;
                        if (a.right == _root)
                            goto finish;
                        a = a.right;
                        b = b.right;
                    }
                    a = a.right;
                    p = b;
                    b = new AvlTreeNode<ValueT>(a.value);
                    b.rank = a.rank;
                    b.left = p;
                    b.right = p.right;
                    p.right = b;
                }
            }
        finish:
            dup._count = _count;
            dup._height = _height;
            return dup;
        }

        public AvlTree<ValueT> Slice(int lo, int hi)
        {
            if (lo < 0) lo = 0;
            if (hi > _count) hi = (int)_count;
            return SliceB(lo, hi);
        }

        AvlTree<ValueT> SliceB(int lo, int hi)
        {
            //create empty:
            AvlTree<ValueT> slice = new AvlTree<ValueT>(_comparer);
            if (lo < hi)
            {
                uint slen = (uint)(hi - lo);
                AvlTreeNode<ValueT> cur;
                //1-based:
                cur = Select(_root.right, lo + 1);
                slice._root.right = Slice(ref cur, slen, slice._root, slice._root);
                unRt(slice._root);
                slice._count = slen;
                slice._height = height(slen);
            }
            return slice;
        }

        // ref param cur: Move cur forward by count steps, initially at first item in enumeration
        // no random choice
        // height = ceil (log2(n+1))
        private static AvlTreeNode<ValueT> Slice(ref AvlTreeNode<ValueT> cur, uint count, AvlTreeNode<ValueT> pred, AvlTreeNode<ValueT> succ)
        {
            AvlTreeNode<ValueT> mid = new AvlTreeNode<ValueT>();
            //assumed@mid: getRank=1 delta=0 lt=rt=1
            if (count == 1)
            {
                mid.value = cur.value;
                mid.left = pred;
                mid.right = succ;
                cur = Next(cur);
            }
            else
            {
                uint half = count / 2;
                mid.left = Slice(ref cur, half, pred, mid);
                mid.value = cur.value;
                mid.rank = (half + 1) << 4;
                half = count - (half + 1);
                cur = Next(cur);
                if (half == 0)
                {
                    mid.right = succ;
                    Rt(mid);
                }
                else
                {
                    mid.right = Slice(ref cur, half, mid, succ);
                }
                // pow of 2 <=> lSkew
                mid.rank |= (count & -count) == count ? 1u : 0;
            }
            return mid;
        }

        private static AvlTreeNode<ValueT> Next(AvlTreeNode<ValueT> a)
        {
            return isRt(a) ? a.right : Leftmost(a.right);
        }

        // NOTE: e must be `before first' in slice
        // MoveNext() is called count times
        private static AvlTreeNode<ValueT> Slice(IEnumerator<ValueT> e, uint count, AvlTreeNode<ValueT> pred, AvlTreeNode<ValueT> succ)
        {
            AvlTreeNode<ValueT> mid = new AvlTreeNode<ValueT>();
            //assumed@mid: getRank=1 delta=0 lt=rt=1
            if (count == 1)
            {
                e.MoveNext();
                mid.value = e.Current;
                mid.left = pred;
                mid.right = succ;
            }
            else
            {
                uint half = count / 2;
                mid.left = Slice(e, half, pred, mid);
                e.MoveNext();
                mid.value = e.Current;
                mid.rank = (half + 1) << 4;
                half = count - (half + 1);
                if (half == 0)
                {
                    mid.right = succ;
                    Rt(mid);
                }
                else
                {
                    mid.right = Slice(e, half, mid, succ);
                }
                // pow of 2 <=> lSkew
                mid.rank |= (count & -count) == count ? 1u : 0;
            }
            return mid;
        }

        //glue:
        private static IEnumerator<ValueT> ListEnumerator(IList<ValueT> list, Int32 lo, UInt32 len)
        {
            Debug.Assert(list != null);
            Debug.Assert(len > 0);
            int i = lo;
            do
            {
                yield return list[i++];
            } while (0 != --len);
        }
        #endregion

        #region FromSuchAndSuch methods
        public static AvlTree<ValueT> FromSequence(IEnumerable<ValueT> seq, UInt32 len)
        {
            if (seq == null)
                throw new ArgumentNullException();
            AvlTree<ValueT> slice = new AvlTree<ValueT>();
            if (len > 0)
            {
                slice._root.right = Slice(SeqEnumerator(seq, len), len, slice._root, slice._root);
                unRt(slice._root);
                slice._count = len;
                slice._height = height(len);
            }
            return slice;
        }

        //glue to yield at most len elements:
        private static IEnumerator<ValueT> SeqEnumerator(IEnumerable<ValueT> seq, UInt32 len)
        {
            Debug.Assert(len > 0);
            IEnumerator<ValueT> e = seq.GetEnumerator();
            while (len != 0 && e.MoveNext())
            {
                --len;
                yield return e.Current;
            }
        }

        public static AvlTree<ValueT> FromList(IList<ValueT> sortedList)
        {
            return FromList(sortedList, 0, sortedList.Count);
        }

        public static AvlTree<ValueT> FromList(IList<ValueT> sortedList, Int32 lo, Int32 hi)
        {
            return FromList(sortedList, lo, hi, Comparer<ValueT>.Default);
        }

        public static AvlTree<ValueT> FromList(IList<ValueT> sortedList, Int32 lo, Int32 hi, IComparer<ValueT> comparer)
        {
            if (sortedList == null)
                throw new ArgumentNullException();
            //create empty:
            AvlTree<ValueT> slice = new AvlTree<ValueT>(comparer);
            if (lo < 0) lo = 0;
            if (hi > sortedList.Count) hi = sortedList.Count;
            if (lo < hi)
            {
                uint slen = (uint)(hi - lo);
                slice._root.right = Slice(ListEnumerator(sortedList, lo, slen), slen, slice._root, slice._root);
                unRt(slice._root);
                slice._count = slen;
                slice._height = height(slen);
            }
            return slice;
        }

        public static AvlTree<ValueT> FromSequence(AvlTree<ValueT> avl)
        {
            if (avl == null)
                throw new ArgumentNullException("avl");
            return avl.Copy();
        }

        public static AvlTree<ValueT> FromSequence(AvlTree<ValueT> avl, int lo, int hi)
        {
            if (avl == null)
                throw new ArgumentNullException("avl");
            return avl.Slice(lo, hi);
        }
        #endregion

        /**************************
        *                         *
        *           Enumerators   *
        *                         *
        **************************/
        #region Enumerators
        public IEnumerator<ValueT> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IAvlEnumerator<ValueT> GetAvlEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerator<ValueT> GetForwardEnumerator()
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = Leftmost(_root.right);
                do
                {
                    yield return a.value;
                    a = isRt(a) ? a.right : Leftmost(a.right);
                } while (a != _root);
            }
        }

        public IEnumerator<ValueT> GetForwardEnumerator(Int32 lo, Int32 hi)
        {
            if (lo < 0) lo = 0;
            if (hi > _count) hi = (int)_count;
            if (lo < hi)
            {
                AvlTreeNode<ValueT> a = Select(_root.right, lo + 1);
                uint len = (uint)(hi - lo);
                do
                {
                    yield return a.value;
                    a = isRt(a) ? a.right : Leftmost(a.right);
                } while (0 != --len);
            }
        }

        public IEnumerator<ValueT> GetBackwardEnumerator()
        {
            if (!_root.nil)
            {
                AvlTreeNode<ValueT> a = Rightmost(_root.right);
                do
                {
                    yield return a.value;
                    a = isLt(a) ? a.left : Rightmost(a.left);
                } while (a != _root);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        struct Enumerator : IEnumerator<ValueT>, IAvlEnumerator<ValueT>, IEnumerator
        {
            private AvlTree<ValueT> _tree;
            private AvlTreeNode<ValueT> _current, _next;
            private AvlTreeNode<ValueT> _begin, _end;
            private int _version;

            internal Enumerator(AvlTree<ValueT> tree)
            {
                _tree = tree;
                _version = tree._version;
                _next = null;
                _current = null;
                _begin = _end = null;
                Init(null, null);
            }

            public AvlTree<ValueT> Tree
            {
                get { return _tree; }
            }

            public bool MoveNext()
            {
                CheckVersion();
                if (_next == _end)
                {
                    _current = _end;
                    return false;
                }
                _current = _next;
                if (isRt(_current))
                {
                    _next = _current.right;
                }
                else
                    _next = AvlTree<ValueT>.Leftmost(_current.right);
                return true;
            }

            public bool MovePrevious()
            {
                CheckVersion();
                _next = _current;
                if (_current == _begin)
                {
                    _current = _tree._root;
                    return false;
                }
                if (_current == _tree._root)
                {
                    _current = AvlTree<ValueT>.Rightmost(_current.right);
                }
                else if (isLt(_current))
                {
                    _current = _current.left;
                }
                else
                    _current = AvlTree<ValueT>.Rightmost(_current.left);
                return true;
            }

            // restart:next=first
            public void MoveBegin()
            {
                CheckVersion();
                _current = _tree._root;
                _next = _begin;
            }

            // restart:prev=last
            public void MoveEnd()
            {
                CheckVersion();
                _current = _end;
                _next = _end;
            }

            public ValueT Current
            {
                get { return _current.value; }
                // set: no-op if iter out-of-range
                set
                {
                    if (_current == _tree._root || _current == _end)
                        return;
                    _current.value = value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_current == _tree._root || _current == _end)
                        throw new InvalidOperationException();
                    return _current.value;
                }
            }

            public void Reset()
            {
                MoveBegin();
            }

            public void Dispose()
            {
                _tree = null;
                _current = _next = null;
                _begin = _end = null;
                _version = 0;
            }

            private void Init(AvlTreeNode<ValueT> begin, AvlTreeNode<ValueT> end)
            {
                _current = _tree._root;
                if (_current.nil)
                {
                    _begin = _end = _current;
                }
                else
                {
                    if (begin == null)
                        begin = AvlTree<ValueT>.Leftmost(_current.right);
                    if (end == null)
                        end = _current; //after last
                    _begin = begin;
                    _end = end;
                }
                _next = _begin;
            }

            private void CheckVersion()
            {
                if (_version != _tree._version)
                    throw new InvalidOperationException("AvlEnumerator:version mismatch");
            }
        }
        #endregion

        #region Dump
        public void Dump(TextWriter os)
        {
#if DEBUG
            if (_root.nil)
            {
                Debug.Assert(0 == _count);
                Debug.Assert(0 == _height);
                os.WriteLine("<Empty Avl />");
            }
            else
            {
                os.WriteLine("<AvlDump size={0} height={1}>", _count, _height);
                Dumpr(os, _root.right);
                os.WriteLine("</AvlDump>");
            }
#endif
        }

#if DEBUG
        static void Dumpr(TextWriter os, AvlTreeNode<ValueT> a)
        {
            os.WriteLine("{0} {1} {2} [{3}][{4}]",
                a.value.ToString(),
                isLt(a) ? repr(a.left) : "-",
                isRt(a) ? repr(a.right) : "-",
                GetRank(a), balrepr(a));
            if (!isLt(a)) Dumpr(os, a.left);
            if (!isRt(a)) Dumpr(os, a.right);
        }

        static string repr(AvlTreeNode<ValueT> a) { return a == null ? "#" : a.value != null ? a.value.ToString() : "$"; }
        static char balrepr(AvlTreeNode<ValueT> a)
        {
            switch (a.rank & 3u)
            {
                case 0: return '=';
                case 1: return '<';
                case 2: return '>';
            }
            return '?';
        }
#endif
        #endregion

        #region rotations
        private static AvlTreeNode<ValueT> rotL(AvlTreeNode<ValueT> a)
        {
            AvlTreeNode<ValueT> b = a.right;
            if (isLt(b))
            {
                unLt(b);
                Rt(a);
            }
            else
            {
                a.right = b.left;
                b.left = a;
            }
            b.rank += a.rank & zFlags;
            return b;
        }

        private static AvlTreeNode<ValueT> rotR(AvlTreeNode<ValueT> a)
        {
            AvlTreeNode<ValueT> b = a.left;
            if (isRt(b))
            {
                unRt(b);
                Lt(a);
            }
            else
            {
                a.left = b.right;
                b.right = a;
            }
            a.rank -= b.rank & zFlags;
            return b;
        }

        private static AvlTreeNode<ValueT> rotRL(AvlTreeNode<ValueT> a)
        {
            AvlTreeNode<ValueT> b = a.right.left;
            if (isRt(b))
            {
                unRt(b);
                Lt(a.right);
            }
            else
            {
                a.right.left = b.right;
                b.right = a.right;
            }
            if (isLt(b))
            {
                unLt(b);
                Rt(a);
                a.right = b;
            }
            else
            {
                a.right = b.left;
            }
            b.left = a;
            b.right.rank -= b.rank & zFlags;
            b.rank += a.rank & zFlags;
            return b;
        }

        private static AvlTreeNode<ValueT> rotLR(AvlTreeNode<ValueT> a)
        {
            AvlTreeNode<ValueT> b = a.left.right;
            if (isLt(b))
            {
                unLt(b);
                Rt(a.left);
            }
            else
            {
                a.left.right = b.left;
                b.left = a.left;
            }
            if (isRt(b))
            {
                unRt(b);
                Lt(a);
                a.left = b;
            }
            else
            {
                a.left = b.right;
            }
            b.right = a;
            b.rank += b.left.rank & zFlags;
            a.rank -= b.rank & zFlags;
            return b;
        }
        #endregion

        #region Lt-Rt
        static bool isLt(AvlTreeNode<ValueT> a) { return 0 != (a.rank & 4u); }
        static bool isRt(AvlTreeNode<ValueT> a) { return 0 != (a.rank & 8u); }
        static void Lt(AvlTreeNode<ValueT> a) { a.rank |= 4u; }
        static void Rt(AvlTreeNode<ValueT> a) { a.rank |= 8u; }
        static void unLt(AvlTreeNode<ValueT> a) { a.rank &= ~4u; }
        static void unRt(AvlTreeNode<ValueT> a) { a.rank &= ~8u; }
        #endregion

        #region ranks
        const UInt32 zFlags = ~0xFu;
        static UInt32 GetRank(AvlTreeNode<ValueT> a) { return a.rank >> 4; }
        static void Rank(AvlTreeNode<ValueT> a, UInt32 r) { a.rank = (r << 4) | (a.rank & 0xF); }
        #endregion

        #region skew
        static bool skewed(AvlTreeNode<ValueT> a) { return 0 != (a.rank & 3u); }
        static bool lSkewed(AvlTreeNode<ValueT> a) { return 0 != (a.rank & 1u); }
        static bool rSkewed(AvlTreeNode<ValueT> a) { return 0 != (a.rank & 2u); }

        public static int GetDelta(AvlTreeNode<ValueT> a)
        {
            switch (a.rank & 3)
            {
                case 1: return -1;
                case 2: return +1;
                default: return 0;
            }
        }

        #endregion

#if AVL_DOC
        static void RankUp(Node<ValueT> a)   { a.rank += 0x10; }
        static void RankDown(Node<ValueT> a) { a.rank -= 0x10; }
        static bool noSkew(Node<ValueT> a)   { return 0 == (a.rank & 3u); }
        static void unSkew(Node<ValueT> a)   { a.rank &= ~3u; }
        static void unlSkew(Node<ValueT> a)  { a.rank &= ~1u; }
        static void unrSkew(Node<ValueT> a)  { a.rank &= ~2u; }
#endif

        #region misc
        // height(count) = ceil (log2(count+1))
        private static int height(uint count)
        {
            int h = 0;
            for (int pow = 1; pow <= count; pow <<= 1, ++h) { }
            return h;
        }
        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            if (!(value is ValueT))
            {
                throw new InvalidOperationException("InvalidValueType");
            }

            this.InsertLast((ValueT)value);
            return Index((ValueT)value);
        }

        public void Clear()
        {
            this.MkEmpty();
        }

        bool IList.Contains(object value)
        {
            if (value is ValueT)
            {
                return this.Contains((ValueT)value);
            }

            return false;
        }

        int IList.IndexOf(object value)
        {
            if (value is ValueT)
            {
                return this.IndexOf((ValueT)value);
            }

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (value is ValueT)
            {
                this.InsertLast((ValueT)value);
            }
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            if (!(value is ValueT))
            {
                throw new InvalidOperationException("InvalidValueType");
            }

            ValueT val;
            this.Delete((ValueT)value, out val);
        }

        public void RemoveAt(int index)
        {
            ValueT val;
            this.DeleteAt(index, out val);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (!(value is ValueT))
                {
                    throw new InvalidOperationException("InvalidValueType");
                }

                ValueT val;
                this.DeleteAt(index, out val);
                this.InsertLast((ValueT)value);
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            ArrayList arr = new ArrayList();
            for (int i = index; i < this.Count; i++)
            {
                arr.Add(this[i]);
            }

            arr.CopyTo(array, index);          
        }

        public int Count
        {
            get { return this.Size; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IList<ValueT> Members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(ValueT item)
        {
            return this.Index(item);
        }

        public ValueT this[int index]
        {
            get
            {
                if ((index < 0 && (index += (int)_count) < 0) || index >= _count)
                    throw new IndexOutOfRangeException();
                AvlTreeNode<ValueT> a;
                if (index == 0)
                    a = Leftmost(_root.right);
                else if (index + 1 == _count)
                    a = Rightmost(_root.right);
                else
                    a = Select(_root.right, index + 1);
                return a.value;
            }
            set
            {
                //throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<ValueT> Members

        public void Add(ValueT item)
        {
            this.InsertWithDuplicates(item);
        }

        public void CopyTo(ValueT[] array, int arrayIndex)
        {
            ((IList)this).CopyTo(array, arrayIndex);
        }

        public bool Remove(ValueT item)
        {
            ValueT val;
            return this.Delete(item, out val);
        }

        #endregion

        #region IEnumerable<ValueT> Members

        IEnumerator<ValueT> IEnumerable<ValueT>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion
    }
    #endregion

    #region Nodes
    public class AvlTreeNode<ValueT>
    {
        internal AvlTreeNode<ValueT> left, right;
        internal static readonly AvlTreeNode<ValueT> NIL = new AvlTreeNode<ValueT>();

        static AvlTreeNode()
        {
            NIL.left = NIL.right = NIL;
        }

        public AvlTreeNode<ValueT> Left
        {
            get
            {
                return this.left;
            }
        }

        public AvlTreeNode<ValueT> Right
        {
            get
            {
                return this.right;
            }
        }

        public ValueT Value
        {
            get
            {
                return this.value;
            }
        }

        /*
         * rank: 
         * 2 bits for balance + 2 bits for threading
         * higher bits for rank
         *  bit 2,3: Lt,Rt
         *  bit 0-1:
         * 00 =  0
         * 01 = +1
         * 10 = -1
         * 
         * 1 bit per node is sufficient to record balance information
         * but we find it to be not convenient when combined with threading
         */
        internal UInt32 rank;
        internal ValueT value;

        internal AvlTreeNode() : this(default(ValueT)) { }
        internal AvlTreeNode(ValueT v)
        {
            //rank=1    bal=00  lt,rt=1,1
            rank = 0x1C; value = v;
        }
        internal void clear()
        {
            rank = 0x1C; value = default(ValueT);
            left = right = null;
        }
        internal bool nil
        {
            get { return right == this; }
        }
        /*right link pointing to self*/
        internal void mkNil()
        {
            right = this;
            rank |= 8u;     //Rt
        }
        internal void start(AvlTreeNode<ValueT> t)
        {
            right = t;
            rank &= ~8u;   //unRt
        }
    }
    #endregion

    #region Enumerator interface
    public interface IAvlEnumerator<ValueT>
    {
        ValueT Current { get; }
        bool MoveNext();
        bool MovePrevious();
        void MoveBegin();
        void MoveEnd();
        void Reset();
        void Dispose();
    }
    #endregion

}
