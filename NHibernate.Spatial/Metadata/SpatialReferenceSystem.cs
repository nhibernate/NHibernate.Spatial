// Copyright 2007 - Ricardo Stuven (rstuven@gmail.com)
//
// This file is part of NHibernate.Spatial.
// NHibernate.Spatial is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NHibernate.Spatial is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with NHibernate.Spatial; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;

namespace NHibernate.Spatial.Metadata
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class SpatialReferenceSystem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialReferenceSystem"/> class.
		/// </summary>
		public SpatialReferenceSystem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialReferenceSystem"/> class.
		/// </summary>
		/// <param name="srid">The SRID.</param>
		/// <param name="authorityName">Name of the authority.</param>
		/// <param name="authoritySRID">The authority srid.</param>
		/// <param name="wellKnownText">The well known text.</param>
		public SpatialReferenceSystem(int srid, string authorityName, int authoritySRID, string wellKnownText)
		{
			this.SRID = srid;
			this.AuthorityName = authorityName;
			this.AuthoritySRID = authoritySRID;
			this.WellKnownText = wellKnownText;
		}

		private int srid;
		/// <summary>
		/// Gets or sets the SRID.
		/// </summary>
		/// <value>The SRID.</value>
		public virtual int SRID
		{
			get { return this.srid; }
			set { this.srid = value; }
		}

		private string authorityName;
		/// <summary>
		/// Gets or sets the name of the authority.
		/// </summary>
		/// <value>The name of the authority.</value>
		public virtual string AuthorityName
		{
			get { return this.authorityName; }
			set { this.authorityName = value; }
		}

		private int authoritySRID;
		/// <summary>
		/// Gets or sets the authority SRID.
		/// </summary>
		/// <value>The authority SRID.</value>
		public virtual int AuthoritySRID
		{
			get { return this.authoritySRID; }
			set { this.authoritySRID = value; }
		}

		private string wellKnownText;
		/// <summary>
		/// Gets or sets the well known text.
		/// </summary>
		/// <value>The well known text.</value>
		public virtual string WellKnownText
		{
			get { return this.wellKnownText; }
			set { this.wellKnownText = value; }
		}

	}
}
