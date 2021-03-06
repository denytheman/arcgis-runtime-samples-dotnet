﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArcGISRuntimeSDKDotNet_DesktopSamples.Samples
{
    /// <summary>
    /// This sample shows how to hit test a graphics layer using the GraphicsLayer.HitTestAsync method. Here, the user may sketch a point on the map to initiate the hit testing process - the results of the hit test are then displayed in the UI.
    /// </summary>
    /// <title>Hit Testing</title>
	/// <category>Layers</category>
	/// <subcategory>Graphics Layers</subcategory>
	public partial class GraphicsHitTesting : UserControl
    {
        private const int MAX_GRAPHICS = 50;

        private Random _random = new Random();

        /// <summary>Construct Graphics Hit Testing sample control</summary>
        public GraphicsHitTesting()
        {
            InitializeComponent();

            CreateGraphics();
        }

        // Hit Test the graphics layer by single point
        private async void mapView_MapViewTapped(object sender, MapViewInputEventArgs e)
        {
            try
            {
                var graphics = await graphicsLayer.HitTestAsync(mapView, e.Position, MAX_GRAPHICS);

                string results = "Hit: ";
                if (graphics == null || graphics.Count() == 0)
                    results += "None";
                else
                    results += string.Join(", ", graphics.Select(g => g.Attributes["ID"].ToString()).ToArray());
                txtResults.Text = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show("HitTest Error: " + ex.Message, "Graphics Layer Hit Testing");
            }
        }

        // Create three List<Graphic> objects with random graphics to serve as layer GraphicsSources
        private async void CreateGraphics()
        {
            await mapView.LayersLoadedAsync();

            for (int n = 1; n <= MAX_GRAPHICS; ++n)
            {
                graphicsLayer.Graphics.Add(CreateRandomGraphic(n));
            }
        }

        // Create a random graphic
        private Graphic CreateRandomGraphic(int id)
        {
            var symbol = new CompositeSymbol();
            symbol.Symbols.Add(new SimpleMarkerSymbol() { Style = SimpleMarkerStyle.Circle, Color = Colors.Red, Size = 16 });
            symbol.Symbols.Add(new TextSymbol()
            {
                Text = id.ToString(),
                Color = Colors.White,
                VerticalTextAlignment = VerticalTextAlignment.Middle,
                HorizontalTextAlignment = HorizontalTextAlignment.Center,
                YOffset = -1
            });

            var graphic = new Graphic()
            {
                Geometry = GetRandomMapPoint(),
                Symbol = symbol
            };

            graphic.Attributes["ID"] = id;

            return graphic;
        }

        // Utility: Generate a random MapPoint within the current extent
        private MapPoint GetRandomMapPoint()
        {
            double x = mapView.Extent.XMin + (_random.NextDouble() * mapView.Extent.Width);
            double y = mapView.Extent.YMin + (_random.NextDouble() * mapView.Extent.Height);
            return new MapPoint(x, y, mapView.SpatialReference);
        }
    }
}
