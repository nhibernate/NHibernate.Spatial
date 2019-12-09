using NetTopologySuite.Geometries;

namespace Open.Topology.TestRunner.Utility
{
    public class FunctionsUtil
    {
        public static readonly Envelope DefaultEnvelope = new Envelope(0, 100, 0, 100);

        public static Envelope getEnvelopeOrDefault(Geometry g)
        {
            if (g == null) return DefaultEnvelope;
            return g.EnvelopeInternal;
        }

        private static readonly GeometryFactory Factory = new GeometryFactory();

        public static GeometryFactory getFactoryOrDefault(Geometry g)
        {
            if (g == null) return Factory; /*NTSTestBuilder.getGeometryFactory()*/;
            return g.Factory;
        }

        /*
  public static void showIndicator(Geometry geom)
  {
    GeometryEditPanel panel = JTSTestBuilderFrame
    .getInstance().getTestCasePanel()
    .getGeometryEditPanel();
    Graphics2D gr = (Graphics2D) panel.getGraphics();
    GeometryRenderer.paint(geom, panel.getViewport(), gr,
        AppColors.INDICATOR_LINE_COLOR,
        AppColors.INDICATOR_FILL_COLOR);
  }
     */
    }
}