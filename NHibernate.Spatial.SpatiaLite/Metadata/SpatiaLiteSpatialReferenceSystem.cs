using System;

namespace NHibernate.Spatial.Metadata
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class SpatiaLiteSpatialReferenceSystem : SpatialReferenceSystem
    {
        private string _refSysName;
        private string _proj4Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatiaLiteSpatialReferenceSystem"/> class.
        /// </summary>
        public SpatiaLiteSpatialReferenceSystem()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatiaLiteSpatialReferenceSystem"/> class.
        /// </summary>
        /// <param name="srid">The SRID.</param>
        /// <param name="authorityName">Name of the authority.</param>
        /// <param name="authoritySrid">The authority srid.</param>
        /// <param name="refSysName">The spatial reference system name.</param>
        /// <param name="proj4Text">The PROJ.4 text.</param>
        /// <param name="wellKnownText">The well known text.</param>
        public SpatiaLiteSpatialReferenceSystem(int srid,
                                                string authorityName,
                                                int authoritySrid,
                                                string refSysName,
                                                string proj4Text,
                                                string wellKnownText)
        {
            SRID = srid;
            AuthorityName = authorityName;
            AuthoritySRID = authoritySrid;
            RefSysName = refSysName;
            Proj4Text = proj4Text;
            WellKnownText = wellKnownText;
        }

        /// <summary>
        /// Gets or sets the spatial reference system name.
        /// </summary>
        public virtual string RefSysName
        {
            get => _refSysName;
            set => _refSysName = value;
        }

        /// <summary>
        /// Gets or sets the PROJ.4 text.
        /// </summary>
        public virtual string Proj4Text
        {
            get => _proj4Text;
            set => _proj4Text = value;
        }
    }
}
