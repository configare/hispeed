using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Telerik.WinControls.Keyboard
{
    [ListBindableAttribute(BindableSupport.No)]
    public class InputBindingsCollection : CollectionBase
    {
		private Shortcuts owner;

		internal Shortcuts Owner
		{
			get { return owner; }
			set { owner = value; }
		}

        public InputBindingsCollection()
        {
        }
		public InputBindingsCollection(Shortcuts owner)
		{
			this.owner = owner;
		}

		///// <summary>
		/////     <para>
		/////       Initializes a new instance of <see cref='Telerik.WinControls.InputBindingCollection'/>.
		/////    </para>
		///// </summary>
        //public InputBindingsCollection(InputBinding owner)
        //{
        //    this.owner = owner;
        //}

		/// <summary>
		///     <para>
        ///       Initializes a new instance of <see cref='InputBindingsCollection'/> based on another <see cref='InputBindingsCollection'/>.
		///    </para>
		/// </summary>
		/// <param name='value'>
        ///       A <see cref='InputBindingsCollection'/> from which the contents are copied
		/// </param>
		public InputBindingsCollection(/*InputBinding owner, */InputBindingsCollection value)
		{
			//this.owner = owner;
			this.AddRange(value);
		}

		/// <summary>
		///     <para>
        ///       Initializes a new instance of <see cref='InputBindingsCollection'/> containing any array of <see cref='Telerik.WinControls.Keyboard.InputBinding'/> objects.
		///    </para>
		/// </summary>
		/// <param name='value'>
        ///       A array of <see cref='Telerik.WinControls.Keyboard.InputBinding'/> objects with which to intialize the collection
		/// </param>
        public InputBindingsCollection(/*InputBinding owner, */InputBinding[] value)
		{
			//this.owner = owner;
			this.AddRange(value);
		}

		/// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='Telerik.WinControls.Keyboard.InputBinding'/>.</para>
		/// </summary>
		/// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
		/// <value>
		///    <para> The entry at the specified index of the collection.</para>
		/// </value>
		/// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
		public InputBinding this[int index]
		{
			get
			{
				return ((InputBinding) (List[index]));
			}
			set
			{
				List[index] = value;
			}
		}

		/// <summary>
		///    <para>Adds a <see cref='Telerik.WinControls.Keyboard.InputBinding'/> with the specified value to the 
		///    <see cref='InputBindingsCollection'/> .</para>
		/// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.Keyboard.InputBinding'/> to add.</param>
		/// <returns>
		///    <para>The index at which the new element was inserted.</para>
		/// </returns>
		public int Add(InputBinding value)
		{
			return List.Add(value);
		}

		/// <summary>
		/// <para>Copies the elements of an array to the end of the <see cref='InputBindingsCollection'/>.</para>
		/// </summary>
		/// <param name='value'>
		///    An array of type <see cref='Telerik.WinControls.Keyboard.InputBinding'/> containing the objects to add to the collection.
		/// </param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		public void AddRange(InputBinding[] value)
		{
			for (int i = 0; (i < value.Length); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		/// <summary>
		///     <para>
		///       Adds the contents of another <see cref='InputBindingsCollection'/> to the end of the collection.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///    A <see cref='InputBindingsCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		public void AddRange(InputBindingsCollection value)
		{
			for (int i = 0; (i < value.Count); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		/// <summary>
		/// <para>Gets a value indicating whether the 
		///    <see cref='InputBindingsCollection'/> contains the specified <see cref='Telerik.WinControls.Keyboard.InputBinding'/>.</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.Keyboard.InputBinding'/> to locate.</param>
		/// <returns>
		/// <para><see langword='true'/> if the <see cref='Telerik.WinControls.Keyboard.InputBinding'/> is contained in the collection; 
		///   otherwise, <see langword='false'/>.</para>
		/// </returns>
		public bool Contains(InputBinding value)
		{
			return List.Contains(value);
		}

		/// <summary>
		/// <para>Copies the <see cref='InputBindingsCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
		///    specified index.</para>
		/// </summary>
		/// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='InputBindingsCollection'/> .</para></param>
		/// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='InputBindingsCollection'/> is greater than the available space between <paramref name='index'/> and the end of <paramref name='array'/>.</para></exception>
		/// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is less than <paramref name='array'/>'s lowbound. </exception>
		public void CopyTo(InputBinding[] array, int index)
		{
			List.CopyTo(array, index);
		}

		public InputBindingsCollection GetBindingByComponent(IComponent component)
		{
			InputBindingsCollection tempColection = new InputBindingsCollection();
			for (int i = 0; i < List.Count; i++)
			{
				IComponent tempComponent = ((InputBinding)List[i]).CommandContext as IComponent;
				if (tempComponent != null &&
					(tempComponent == component))
				{
					tempColection.Add((InputBinding)List[i]);
				}
				
			}
			if (tempColection.Count > 0)
			{
				return tempColection;
			}
			return null;
		}

		public void RemoveBindingByComponent(IComponent component) 
		{
			InputBindingsCollection tempColection = this.GetBindingByComponent(component);
			if (tempColection != null)
			{
				for (int i = 0; i < tempColection.Count; i++)
				{
					this.Remove(tempColection[i]);
				}
			}
		}

		/// <summary>
		///    <para>Returns the index of a <see cref='Telerik.WinControls.Keyboard.InputBinding'/> in 
		///       the <see cref='InputBindingsCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.Keyboard.InputBinding'/> to locate.</param>
		/// <returns>
		/// <para>The index of the <see cref='Telerik.WinControls.Keyboard.InputBinding'/> of <paramref name='value'/> in the 
		/// <see cref='InputBindingsCollection'/>, if found; otherwise, -1.</para>
		/// </returns>
		public int IndexOf(InputBinding value)
		{
			return List.IndexOf(value);
		}

		/// <summary>
		/// <para>Inserts a <see cref='Telerik.WinControls.Keyboard.InputBinding'/> into the <see cref='InputBindingsCollection'/> at the specified index.</para>
		/// </summary>
		/// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
		/// <param name=' value'>The <see cref='Telerik.WinControls.Keyboard.InputBinding'/> to insert.</param>
		/// <returns><para>None.</para></returns>
		public void Insert(int index, InputBinding value)
		{
			List.Insert(index, value);
		}

		/// <summary>
		///    <para> Removes a specific <see cref='Telerik.WinControls.Keyboard.InputBinding'/> from the 
		///    <see cref='InputBindingsCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.Keyboard.InputBinding'/> to remove from the <see cref='InputBindingsCollection'/> .</param>
		/// <returns><para>None.</para></returns>
		/// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
		public void Remove(InputBinding value)
		{
			List.Remove(value);
		}
    }
}
