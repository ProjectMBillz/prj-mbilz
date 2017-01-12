using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// POJO for C1
	/// </summary>
	/// <remark>
	/// This class is autogenerated
	/// </remark>
	[Serializable]
	public class C1 : B
	{
		#region Fields

		/// <summary>
		/// Holder for address
		/// </summary>
		private String _address;

		/// <summary>
		/// Holder for d
		/// </summary>
		private D _d;

		/// <summary>
		/// Holder for c2
		/// </summary>
		private C2 _c2;

		/// <summary>
		/// Holder for c2s
		/// </summary>
		private IList<C2> _c2s = new List<C2>();

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor for class C1
		/// </summary>
		public C1()
		{
		}

		/// <summary>
		/// Constructor for class C1
		/// </summary>
		/// <param name="name">Initial name value</param>
		/// <param name="count">Initial count value</param>
		/// <param name="map">Initial map value</param>
		/// <param name="address">Initial address value</param>
		/// <param name="d">Initial d value</param>
		public C1(String name, Int32 count, IDictionary<string, string> map, String address, D d)
			: base(name, count, map)
		{
			this._address = address;
			this._d = d;
		}

		/// <summary>
		/// Minimal constructor for class C1
		/// </summary>
		/// <param name="map">Initial map value</param>
		public C1(IDictionary<string, string> map)
			: base(map)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get/set for address
		/// </summary>
		public virtual String Address
		{
			get { return _address; }
			set { _address = value; }
		}

		/// <summary>
		/// Get/set for d
		/// </summary>
		public virtual D D
		{
			get { return _d; }
			set { _d = value; }
		}

		public virtual C2 C2
		{
			get { return _c2; }
			set { _c2 = value; }
		}

		public virtual IList<C2> C2s
		{
			get { return _c2s; }
			set { _c2s = value; }
		}

		#endregion
	}
}