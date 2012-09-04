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

using NHibernate.UserTypes;

namespace NHibernate.Spatial.Type
{
	/// <summary>
	/// 
	/// </summary>
	public interface IGeometryUserType : IUserType, IParameterizedType
	{
		/// <summary>
		/// Gets the system reference identification
		/// </summary>
		int SRID { get; }

		/// <summary>
		/// Gets the OGC geometry subtype name
		/// </summary>
		string Subtype { get; }

        /// <summary>
        /// [3DIS] Gets the Dimension [2,3]
        /// </summary>
        int Dimension { get; }
	}
}
